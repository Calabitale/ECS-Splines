using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public struct Beziernode
{
    public BlobArray<float3> Bezzypoint;
}

public struct BezierGraph
{
    public BlobArray<Beziernode> Bezzygraph;
}

public struct BezierGraphSpawner : IComponentData
{
    public BlobAssetReference<Beziernode> BezzyGraphcomp;
}

public class BezierblobConvert : GameObjectConversionSystem
{
    private EntityQuery main_Query;
    
   // BlobAssetReference<NodeGraph> BuildNodeGraph(NodeAuthoring[] authoringNodes)
//    {
//        using (var builder = new BlobBuilder(Allocator.Temp))
//        {
//            ref var root = ref builder.ConstructRoot<NodeGraph>();
//            var nodeArray = builder.Allocate(ref root.Nodes, authoringNodes.Length);
//            for (int i = 0; i < nodeArray.Length; i++)
//            {
//                nodeArray[i].Position = authoringNodes[i].transform.position;
//                var links = builder.Allocate(ref nodeArray[i].Links, authoringNodes[i].links.Length);
//                for (int j = 0; j < authoringNodes[i].links.Length; j++)
//                {
//                    links[j] = Array.IndexOf(authoringNodes, authoringNodes[i].links[j]);
//                }
//            }
//            return builder.CreateBlobAssetReference<NodeGraph>(Allocator.Persistent);
//        }
//    }

//    public int CurveCount {
//        get {
//            return (points.Length - 1) / 3;
//        }
//    }
    
//todo Use BezierNode first then if that works change it to graph if I can
    BlobAssetReference<Beziernode> BuildBezeirspline(Vector3[] beezyspline, Transform temptransform)
    {
        using (var Builder = new BlobBuilder(Allocator.Temp))
        {
            var NumofBezeeys = (beezyspline.Length - 1) / 3;
            ref var root = ref Builder.ConstructRoot<Beziernode>();
            //todo I'll try and just do this with Beziernode first instead of the graph its confusing and difficult to get a list within a list see I can even get the damn thing to work at all first
            var bezeirnode = Builder.Allocate(ref root.Bezzypoint, beezyspline.Length);
            //Debug.Log("Beezy nodes" + bezeirnode.Length);
            
            for (int i = 0; i < beezyspline.Length; i++)
            {
               
                var temppoint = temptransform.TransformPoint(beezyspline[i]);

                bezeirnode[i] = temppoint;
                //bezeirnode[i] = beezyspline[i];

            }
            return Builder.CreateBlobAssetReference<Beziernode>(Allocator.Persistent);
        }
    }

    protected override void OnCreate()
    {
        base.OnCreate();
        Enabled = false;  //TODO It seems you are unable to disable GameObjectConversionSystems
        //Debug.Log("This should not be displaying");
        //m_Query = GetEntityQuery(typeof(BezierSpline), ComponentType.ReadOnly<MainQueueNavPointTag>());
    }
      

    // var authoringNodes = m_Query.ToComponentArray<NodeAuthoring>();
    //        var nodes = Array.FindAll(authoringNodes, node => node.links.Length > 0);
    //
    //        var nodeGraph = BuildNodeGraph(nodes);

    protected override void OnUpdate()
    {

        main_Query = GetEntityQuery(typeof(BezierSpline), ComponentType.Exclude<MainQueueTestTag>());  

        var BezzyMainQueueQuery = GetEntityQuery(typeof(BezierSpline), ComponentType.ReadOnly<MainQueueTestTag>());        

        var bezeirnodes = main_Query.ToComponentArray<BezierSpline>();

        var mainQueueNodes = BezzyMainQueueQuery.ToComponentArray<BezierSpline>();

        if (bezeirnodes.Length == 0) return;
        
        for(int i = 0; i < bezeirnodes.Length; i++)
        {
            var beezypoints = bezeirnodes[i].points;
            var BezeirMainTransform = bezeirnodes[i].transform;

            var nodeGraph = BuildBezeirspline(beezypoints, BezeirMainTransform);

            Entity tempent = DstEntityManager.CreateEntity();
            //TODO This does not seem to be working
            //EntityManager.SetName(tempent, "BezierGraphPoints");
            DstEntityManager.AddComponent(tempent, typeof(BezierBlobsRef));

            DstEntityManager.AddComponentData(tempent, new BezierBlobsRef()
            {
                BlobrefVal = nodeGraph

            });

        }

        if (mainQueueNodes.Length == 0) return;

        for (int i = 0; i < mainQueueNodes.Length; i++)
        {
            var beezypoints = mainQueueNodes[i].points;
            var BezeirMainTransform = mainQueueNodes[i].transform;

            var nodeGraph = BuildBezeirspline(beezypoints, BezeirMainTransform);

            Entity tempent = DstEntityManager.CreateEntity();                  

            //DstEntityManager.AddComponentData(tempent, new BezierGraphSpawner()
            //{
            //    BezzyGraphcomp = nodeGraph

            //});

            
            DstEntityManager.AddComponent(tempent, typeof(MainQueueNavPointTag));
            DstEntityManager.AddComponent(tempent, typeof(BezierBlobsRef));

            DstEntityManager.AddComponentData(tempent, new BezierBlobsRef()
            {
                BlobrefVal = nodeGraph
            });

        }

        //Debug.Log("It is actually going into here when it shouldn't ");
        //var beezypoints = bezeirnodes[0].points;
        //var BezeirMainTransform = bezeirnodes[0].transform;
        //The size is 16 that is correct

        //Debug.Log("Beezey points" + beezypoints.Length);

        //var beezys = Array.FindAll(bezeirnodes, BezierCurve => GeneratePoints.length > 0); //node => node.links.Length > 0);

        //var nodeGraph = BuildBezeirspline(beezypoints, BezeirMainTransform);

        //Debug.Log("This thing is currently" + nodeGraph.Value.Bezzypoint.Length);


        //if (HasSingleton<BezeirpointsTag>())
        //{
        //    Debug.Log("This fucking thing exists");
        //}

        //Entity tempent = DstEntityManager.CreateEntity();

        //DstEntityManager.AddComponentData(tempent, new BezierGraphSpawner()
        //{
        //    BezzyGraphcomp = nodeGraph

        //});


    }
}

public struct BezierBlobsRef : IComponentData
{
    public BlobAssetReference<Beziernode> BlobrefVal;
}

//public class TestViewBezeyBlob : JobComponentSystem
//{
//    protected override JobHandle OnUpdate(JobHandle inputDeps)
//    {
        //var baked = GetSingleton<BezierGraphSpawner>();
//        if (!HasSingleton<BezierGraphSpawner>()) return default;
//            
//        var baked = GetSingleton<BezierGraphSpawner>();
//        ref var booked = ref baked.BezzyGraphcomp.Value;
//        
//        Debug.Log("The thing is here" + booked.Bezzypoint[3]);



//        return default;
//    }
//}

//public struct Node
//{
//    public BlobArray<int> Links;
//    public float3 Position;
//}
//
//public struct NodeGraph
//{
//    public BlobArray<Node> Nodes;
//}
//
//public struct NodeGraphSpawner : IComponentData
//{
//    public BlobAssetReference<NodeGraph> Graph;
//   
//}
//
//public class NodeConversion : GameObjectConversionSystem
//{
//    BlobAssetReference<NodeGraph> BuildNodeGraph(NodeAuthoring[] authoringNodes)
//    {
//        using (var builder = new BlobBuilder(Allocator.Temp))
//        {
//            ref var root = ref builder.ConstructRoot<NodeGraph>();
//            var nodeArray = builder.Allocate(ref root.Nodes, authoringNodes.Length);
//            for (int i = 0; i < nodeArray.Length; i++)
//            {
//                nodeArray[i].Position = authoringNodes[i].transform.position;
//                var links = builder.Allocate(ref nodeArray[i].Links, authoringNodes[i].links.Length);
//                for (int j = 0; j < authoringNodes[i].links.Length; j++)
//                {
//                    links[j] = Array.IndexOf(authoringNodes, authoringNodes[i].links[j]);
//                }
//            }
//            return builder.CreateBlobAssetReference<NodeGraph>(Allocator.Persistent);
//        }
//    }
//
//    private EntityQuery m_Query;
//    
//    protected override void OnCreate()
//    {
//        
//        m_Query = GetEntityQuery(typeof(NodeAuthoring));
//    }
//
//    protected override void OnUpdate()
//    {
//        var authoringNodes = m_Query.ToComponentArray<NodeAuthoring>();
//        var nodes = Array.FindAll(authoringNodes, node => node.links.Length > 0);
//
//        var nodeGraph = BuildNodeGraph(nodes);
//
//        
//        
//
//        Entities.ForEach((GraphAuthoring network) =>
//        {
//            
//            DstEntityManager.AddComponentData(GetPrimaryEntity(network), new NodeGraphSpawner
//            {
//                
//                Graph = nodeGraph,
//                
//            });
//        });
//    }
//}


//namespace Systems
//{
//    class NeedDestinationSystem : JobComponentSystem
//    {
// 
//        public static NativeQueue<Holder> toCalculate = new NativeQueue<Holder>(Allocator.Persistent);
// 
//        protected override void OnDestroy()
//        {
//            toCalculate.Dispose();
//        }
// 
//        protected override JobHandle OnUpdate(JobHandle inputDeps)
//        {
//            var jobGather = new jobGather()
//            {
//                ret = NeedDestinationSystem.toCalculate.AsParallelWriter()
//            };
// 
//            var jobGatherHandle = jobGather.Schedule(this, inputDeps);
// 
//            jobGatherHandle.Complete();
// 
//            while (NeedDestinationSystem.toCalculate.TryDequeue(out Holder element))
//            {
//                DestinationManager.threadQueue.Enqueue(element);
//            }
// 
//            // dequeue results
//            var archetype = World.DefaultGameObjectInjectionWorld.EntityManager.CreateArchetype(new ComponentType[] { typeof(TempTileMapCellToBeRenderedComponent) });
// 
//            NativeHashMap<int, Holder> results = new NativeHashMap<int, Holder>(10000, Allocator.TempJob);
//            while (threadResultQueue.TryDequeue(out Holder element))
//            {
//                // we are here 100% sure that it is a unique place, ensured by .FindClosestFieldCell()
//                results.Add(element.HasActionQueueComponentId, element);
//            }
//            var h = new JobApply()
//            {
//                results = results
//            }.Schedule(this, inputDeps);
// 
//            results.Dispose(h);
// 
// 
//            return h;
//        }
// 
//        [BurstCompile]
//        struct JobApply : IJobForEachWithEntity<NeedDestinationComponent, HasActionQueueComponent>
//        {
//            [ReadOnly]
//            public NativeHashMap<int, Holder> results;
// 
//            public void Execute(Entity entity, int unused, ref NeedDestinationComponent NeedDestinationComponent, ref HasActionQueueComponent HasActionQueueComponent)
//            {
//                if (results.ContainsKey(HasActionQueueComponent.id))
//                {
//                    NeedDestinationComponent.found = 1;
//                    NeedDestinationComponent.active = 0;
//                    NeedDestinationComponent.foundX = results[HasActionQueueComponent.id].foundX;
//                    NeedDestinationComponent.foundY = results[HasActionQueueComponent.id].foundY;
// 
//                    HasActionQueueComponent.actionCurrentHasFinished = (byte)1;
//                }
//            }
//        }
// 
//        [BurstCompile]
//        struct jobGather : IJobForEachWithEntity<NeedDestinationComponent, HasActionQueueComponent, Translation>
//        {
//            public NativeQueue<Holder>.ParallelWriter ret; // ParallelWriter is MANDATORY
// 
//            public void Execute(Entity entity, int index, ref NeedDestinationComponent NeedDestinationComponent, ref HasActionQueueComponent HasActionQueueComponent, ref Translation Translation)
//            {
//                if (NeedDestinationComponent.active == 1 && NeedDestinationComponent.found == 0)
//                {
//                    NeedDestinationComponent.found = 2;
//                    ret.Enqueue(new Holder()
//                    {
//                        x = (int)Translation.Value.x,
//                        y = (int)Translation.Value.y,
//                        type = NeedDestinationComponent.type,
//                        tried = 0,
//                        HasActionQueueComponentId = HasActionQueueComponent.id
//                    });
//                }
//            }
//        }
//    }
//}
//*/