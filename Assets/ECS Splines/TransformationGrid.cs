using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class TransformationGrid : MonoBehaviour
{
    public Transform prefab;

    public int gridResolution = 10;

    private Transform[] grid;
    
    // Start is called before the first frame update
    private void Awake()
    {
        grid = new Transform[gridResolution * gridResolution * gridResolution];
        for (int i = 0, z = 0; z < gridResolution; z++)
        {
            for (int y = 0; y < gridResolution; y++)
            {
                for (int x = 0; x < gridResolution; x++, i++)
                {
                    grid[i] = CreateGridPoint(x, y, z);
                }
            }
        }
    }

    private Transform CreateGridPoint(int x, int y, int z)
    {
        Transform punt = Instantiate<Transform>(prefab);
        punt.localPosition = GetCoordinates(x, y, z);
        punt.GetComponent<MeshRenderer>().material.color = new Color(
            (float)x / gridResolution,
            (float)y / gridResolution,
            (float)z / gridResolution);
        return punt;
    }

    private Vector3 GetCoordinates(int x, int y, int z)
    {
        return new Vector3(
            x - (gridResolution - 1) * 0.05f,
            y - (gridResolution - 1) * 0.05f,
            z - (gridResolution - 1) * 0.05f);
        
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[InternalBufferCapacity(1000)]
public struct GridBuffer : IBufferElementData
{
    public Entity Value;
    
    // The following implicit conversions are optional, but can be convenient.
    public static implicit operator Entity(GridBuffer e)
    {
        return e.Value;
    }

    public static implicit operator GridBuffer(Entity e)
    {
        return new GridBuffer {Value = e};
    }
}




public struct GridArray : IComponentData
{
    //public F
}

public struct GridPositionTag : IComponentData { }

public class TransformationGridJobSyst : JobComponentSystem
{
   
    public int gridResolution = 10;
    
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    
    //public Material
    
    protected override void OnCreate()
    {
        base.OnCreate();
        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        //if(!HasSingleton<TestMoveojbectTag>())
        if (!HasSingleton<GridArray>())
        {
            Entity pipple = EntityManager.CreateEntity();
            EntityManager.AddComponent(pipple, typeof(GridArray));
            EntityManager.AddBuffer<GridBuffer>(pipple);
            //Have temp disable this for clarity and just in case
            Enabled = false;
        }

    }
    
    

    protected override void OnStartRunning()
    {
        
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        //Todo Need to sort out this stupid EntityCommandBuffer thingy
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();
        
        //grid = new NativeArray<Translation>(gridResolution * gridResolution * gridResolution, Allocator.Temp);
        int gridsize = gridResolution * gridResolution * gridResolution;
        
        //Entity pipple = EntityManager.CreateEntity();

        int gridresolve = gridResolution;
        
        //EntityManager.AddBuffer<GridBuffer>(pipple);

        //var Gridboofer = EntityManager.GetBuffer<GridBuffer>(pipple);
        
//        float3 testval = new float3(0f, 0f, 0f);
//        GridBuffer gridtempboth;
//        gridtempboth.value = testval;
//        Gridboofer.Add(gridtempboth);


        var CreatGrid = new CreateGridPoint();

        //CreatGrid.

        var Dotprebab = GetSingleton<DotPrefabinator>();
        
        
       // Debug.Log(Dotprebab.Dootprefab);
        
        Entities.ForEach((Entity gridEnt, ref DynamicBuffer<GridBuffer> gridbuff) =>
        {
            for (int i = 0, z = 0; z < gridresolve; z++) {
                for (int y = 0; y < gridresolve; y++) {
                    for (int x = 0; x < gridresolve; x++, i++) {
                        //grid[i] = CreateGridPoint(x, y, z);
                        //This code will create the grid point
                        Entity dooch = ecb.Instantiate(Dotprebab.Dootprefab);
                        //EntityManager.SetComponentData(toto, new Translation { Value = new float3(0, 0, 0) });
                        var tempfloat = new float3(x - (gridresolve - 1) * 0.05f, y - (gridresolve - 1) * 0.05f, z - (gridresolve - 1) * 0.05f);
                        ecb.AddComponent(dooch, typeof(GridPositionTag));
                        
                        ecb.SetComponent(dooch, new Translation {Value = tempfloat});
                        
                        ecb.AddComponent(dooch, new MaterialColor { Value = new float4(x / gridresolve,y / gridresolve,z / gridresolve,1) });
                        
//                        var renderMesh = EntityManager.GetSharedComponentData<RenderMesh>(entity);
//                        renderMesh .material = material;
//                        EntityManager.SetSharedComponent<RenderMesh>(entity, renderMesh );
                        
                        //ecb.AddComponent(dooch, new RenderMesh());









                        //Todo I dont understand why this buffer keeps breaking when I click on it in the editor
                        //gridbuff.Add(dooch);

                        //Debug.Log(gridbuff.Length);

                    }
                }
            }




        }).WithoutBurst().Run();
        
        //ecb.Dispose();

        Enabled = false;
        


        return default;
    }
}

public class MoveGridJobSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        //Todo Just disabling this for now
        this.Enabled = false;

        var movepos = new float3(1.7f, 4.3f, -4.34f);
        
        Entities.WithAll<GridPositionTag>().ForEach((ref Translation currpos, ref Rotation rutate) =>
        {
            Matrix4x4 tempmatrix = Matrix4x4.identity;


            Vector3 tempscale = new Vector3(1.0f, 1.0f, 1.0f);
            
            tempmatrix.SetTRS(currpos.Value, rutate.Value, tempscale);
            
            //float3 worldPositon = matrix4x4.MultiplyPoint3x4(localPosition );
            //var bezzyTRS = float4x4.TRS(currpos.Value, rutate.Value, scool.Value);
            
            //Todo Just need to give this a value to test
            float3 worldposs = tempmatrix.MultiplyPoint3x4(movepos);
            
            //Debug.Log("The matrix value is" + worldposs);

            //currpos.Value = worldposs;

            rutate.Value = quaternion.LookRotation(worldposs, math.up());
            //var resultPoint = math.mul(yourTRS, bezzyTRS);

            //var resultdirection = resultPoint.c3.xyz;
            
            //ref var tempblob = ref wappadudes.Waypoints.Value.Waypoints;
//            
//            float3 heading = tempblob[dudud.value] - transit.Value;
////      
//            heading.y = 0f;
//            rutate.Value = quaternion.LookRotation(heading, math.up());


        }).WithoutBurst().Run();


        return default;
    }
}

public struct CreateGridPoint
{
    public int x, y, z;

    public CreateGridPoint(int p1, int p2, int p3)
    {
        x = p1;
        y = p2;
        z = p3;

    }

}

//public struct Coords
//{
//    public int x, y;
//
//    public Coords(int p1, int p2)
//    {
//        x = p1;
//        y = p2;
//    }
//}