using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

public struct tPlaceonSpline : IComponentData
{
    public float floatVal;
}

public class BezierECSsystem : SystemBase
{    
    private EntityQuery BezierGraph_Query;

    public float timedeelta;

    protected override void OnCreate()
    {
        BezierGraph_Query = GetEntityQuery(typeof(BezierGraphSpawner));       

        //RequireSingletonForUpdate<BezierGraphSpawner>();
        base.OnCreate();
        //Enabled = false;
    }

    protected override void OnStartRunning()
    {
        //timeaccumalator = 0f;

    }

    protected override void OnUpdate()
    {
        //TODO Use this if there is only one spline
        //var baked = BezierGraph_Query.GetSingleton<BezierGraphSpawner>();
        //TODO Use these if there are multiple, Maybe I should use these in OnStartRunning method for efficiency but will stick to this way for now
        var tempgraphs = BezierGraph_Query.ToEntityArray(Allocator.TempJob);
        
        var tempdoomps = EntityManager.GetComponentData<BezierGraphSpawner>(tempgraphs[0]);        

        var baked = tempdoomps;

        float timeaccumalator = Time.DeltaTime / 20f;
    
        
        //Note Moves entities along the spline
        Entities.WithAll<TestMoveojbectTag>().ForEach((ref Translation moventy, ref tPlaceonSpline placeonSpline) =>
        {
            ref var booked = ref baked.BezzyGraphcomp.Value;
            

            var Curvecount = (booked.Bezzypoint.Length - 1) / 3;

            float3 Bezpoint1 = new float3();
            float3 Bezpoint2 = new float3();
            float3 Bezpoint3 = new float3();
            float3 Bezpoint4 = new float3();
          
            placeonSpline.floatVal += timeaccumalator;

            float tempval = placeonSpline.floatVal;   

            int i;
            if (tempval >= 1f)
            {
                tempval = 1f;
                i = booked.Bezzypoint.Length - 4;
            }
            else
            {
                tempval = math.saturate(tempval) * Curvecount;
                i = (int)tempval;
                tempval -= i;
                i *= 3;
            }

            Bezpoint1 = booked.Bezzypoint[i];
            Bezpoint2 = booked.Bezzypoint[i + 1];
            Bezpoint3 = booked.Bezzypoint[i + 2];
            Bezpoint4 = booked.Bezzypoint[i + 3];
          
            //float oneminust = 1f - tempval;

            float u = 1.0f - tempval;
            var tt = tempval * tempval;
            var uu = u * u;
            var uuu = uu * u;
            var ttt = tt * tempval;
            float3 p = uuu * Bezpoint1; //first term
            p += 3 * uu * tempval * Bezpoint2; //second term
            p += 3 * u * tt * Bezpoint3; //third term
            p += ttt * Bezpoint4; //fourth term
                                  //
         //   float u = 1 - t;
           // float tt = t * t;
          //  float uu = u * u;
          //  float uuu = uu * u;
           // float ttt = tt * t;

          //  Vector3 p = uuu * p0;
          //  p += 3 * uu * t * p1;
          //  p += 3 * u * tt * p2;
           // p += ttt * p3;

            moventy.Value = p;

        }).ScheduleParallel();

        //Todo Rotates the Entities to look along the spline
        Entities.WithAll<TestMoveojbectTag>().ForEach((ref Rotation rutate, ref tPlaceonSpline placeonSpline) =>
        {

            ref var booked = ref baked.BezzyGraphcomp.Value;

            var Curvecount = (booked.Bezzypoint.Length - 1) / 3;

            placeonSpline.floatVal += timeaccumalator;

            float tempval = placeonSpline.floatVal;

            float3 Bezpoint0 = new float3();
            float3 Bezpoint1 = new float3();
            float3 Bezpoint2 = new float3();
            float3 Bezpoint3 = new float3();

            int i;
            if (tempval >= 1f)
            {
                tempval = 1f;
                i = booked.Bezzypoint.Length - 4;
            }
            else
            {
                tempval = math.saturate(tempval) * Curvecount;
                i = (int)tempval;
                tempval -= i;
                i *= 3;
            }

            Bezpoint0 = booked.Bezzypoint[i];
            Bezpoint1 = booked.Bezzypoint[i + 1];
            Bezpoint2 = booked.Bezzypoint[i + 2];
            Bezpoint3 = booked.Bezzypoint[i + 3];

            //tempval = math.saturate(tempval);

            float oneMinusT = 1f - tempval;

            float3 BezzyposVelocity = 3f * oneMinusT * oneMinusT * (Bezpoint1 - Bezpoint0) + 6f * oneMinusT * tempval * (Bezpoint2 - Bezpoint1) + 3f * tempval * tempval * (Bezpoint3 - Bezpoint2);

            var tompval = math.normalize(BezzyposVelocity);


            rutate.Value = quaternion.LookRotation(tompval, math.up());

        }).ScheduleParallel();
         

        tempgraphs.Dispose();
    }
    
}

//TODO I create this whenever I want to check the distance of things on the spline
public struct SplineCheckLengthData : IComponentData
{
    public float FloatLength;
    public float BeginSplinepos;
    public float EndSplinepos;
}

//TODO Checks the length of course 
public class CheckBezierLengthSystem : SystemBase
{
    //TODO The positions to calculate the distance from
    public float BeginSplinepos;
    public float EndSplinepos;

    public EntityQuery BezierGraph_Query;

    //The smallest distance which is used to calculate the total distance from
    public float MinLengthDist;

    protected override void OnCreate()
    {
        base.OnCreate();
        //Enabled = false;

        BezierGraph_Query = GetEntityQuery(typeof(BezierGraphSpawner));

        //RequireForUpdate<BezierGraphSpawner>();
    }



    protected override void OnStartRunning()
    {       
        //The spline positions to check the distance at, if you want to check the distance between objects on the spline then you could get the tsplineposition of that object and apply it to 
        BeginSplinepos = 0.49f;
        EndSplinepos = 0.53f;

        //TODO This value is the minimum distance value that it checks, the lower it goes the less performant it is but the more accurate it is, I think the best value is 0.01 currently
        //But you may need to go lower if you want more accuracy for objects that are closer distance, then again maybe this value will be fine for all cases
        MinLengthDist = 0.01f;

        //Enabled = false;
    }

    protected override void OnStopRunning()
    {
        
    }



    protected override void OnUpdate()
    {
        //Gets all the splines
        var tempgraphs = BezierGraph_Query.ToEntityArray(Allocator.TempJob);
        //Gets the specific spline
        var tempdoomps = EntityManager.GetComponentData<BezierGraphSpawner>(tempgraphs[0]);

        var currentBezier = tempdoomps; 

        var begintpos = BeginSplinepos;
        var endtpos = EndSplinepos;

        var minDist = MinLengthDist;

        if(begintpos > endtpos)
        {
            Debug.Log("The beginSplinepos length check is larger than the Endsplinepos, thats an error you doofus");
            return;

        }

        Entities.ForEach((ref SplineCheckLengthData length) =>
        {
            ref var tempbez = ref currentBezier.BezzyGraphcomp.Value;

            length.FloatLength = 0f;

            float3 previousposition = new float3();

            float3 currentposition = new float3();

            for (float j = begintpos; j <= (endtpos + minDist); j += minDist)
            {
                ref var booked = ref currentBezier.BezzyGraphcomp.Value;

                var Curvecount = (booked.Bezzypoint.Length - 1) / 3;

                if (j > endtpos)
                {

                    j = endtpos;
                }

                float tempval = j;

                float3 Bezpoint1 = new float3();
                float3 Bezpoint2 = new float3();
                float3 Bezpoint3 = new float3();
                float3 Bezpoint4 = new float3();

                int i;
                if (tempval >= 1f)
                {
                    tempval = 1f;
                    i = booked.Bezzypoint.Length - 4;
                }
                else
                {
                    tempval = math.saturate(tempval) * Curvecount;
                    i = (int)tempval;
                    tempval -= i;
                    i *= 3;
                }

                Bezpoint1 = booked.Bezzypoint[i];
                Bezpoint2 = booked.Bezzypoint[i + 1];
                Bezpoint3 = booked.Bezzypoint[i + 2];
                Bezpoint4 = booked.Bezzypoint[i + 3];

                //placeonSpline.floatVal = math.saturate(placeonSpline.floatVal);
                //float oneminust = 1f - tempval;

                float u = 1.0f - tempval;
                var tt = tempval * tempval;
                var uu = u * u;
                var uuu = uu * u;
                var ttt = tt * tempval;
                float3 p = uuu * Bezpoint1; //first term
                p += 3 * uu * tempval * Bezpoint2; //second term
                p += 3 * u * tt * Bezpoint3; //third term
                p += ttt * Bezpoint4; //fourth term         

                currentposition = p;

                if (j == begintpos)
                {
                    previousposition = currentposition;
                    //Debug.Log("Is it even going into here");
                }

                Debug.DrawLine(currentposition, previousposition, Color.red);
                //EntityManager.Instantiate()

                length.FloatLength += math.distance(currentposition, previousposition);

                if (j >= endtpos)
                {

                    return;
                }

                previousposition = currentposition;

                //j += minDist;                

            }
            //TODO This would probably work best just scheduled on a single background thread
        }).Schedule();


        tempgraphs.Dispose();
        
    }
}

public struct MoveAlongBezierConstantDistance //TODO The idea behind this is sort of opposite of the check distance system, you give this a position on the spline then give it a distance and it gives you the place on the spline at that distance
{//This should make it easier to place objects at equidistand positions on the spline and also move along the spline at a constant speed perhaps

    public float3 Execute(float begintpos, float distance, BezierGraphSpawner currentBezier, out float placeonSpline, float direction = 1f)
    {
        float3 resultantPlaceonSpline = new float3();
        float minDist = 0.001f; //This has to be this mindist for it to work 

        //for (float j = begintpos; j <= (endtpos + minDist); j += minDist)
        float calcdistance = 0f;
        //Debug.Log("The begintpos is " + begintpos);
        placeonSpline = begintpos;

        float3 previousposition = new float3();
        float3 currentposition = new float3();
        float3 beginfloat3pos = new float3();

        float splineLength = 0f;

        int infiniteloopstopper = 0;
        bool endbloodyloop = false;
        while (splineLength <= distance)    //TOTO Need to figure out if I can get spline length a bit closer to the distance maybe it doesn't but I might as well try         
        {
            //Debug.Log("The spline length is " + splineLength); //+ " " + distance);
            //infiniteloopstopper++;
            //Debug.Log("The number of loops is " + infiniteloopstopper);
            if (infiniteloopstopper > 1000)
            {
                Debug.Log("The infiniteloopstopper has been activated when it never should have");
                return new float3() ;
            }

            ref var booked = ref currentBezier.BezzyGraphcomp.Value;
            var Curvecount = (booked.Bezzypoint.Length - 1) / 3;
                //if (j > endtpos)
                //{

                   // j = endtpos;
                //}                
            float tempval = placeonSpline;
            
            float3 Bezpoint1 = new float3();                
            float3 Bezpoint2 = new float3();                
            float3 Bezpoint3 = new float3();               
            float3 Bezpoint4 = new float3();
                
            int i;
               
            if (tempval >= 1f)                
            {
                //Debug.Log("The tempval is equalling One");
                endbloodyloop = true;
                tempval = 1f;                    
                i = booked.Bezzypoint.Length - 4;                
            }                
            else                
            {                    
                tempval = math.saturate(tempval) * Curvecount;                    
                i = (int)tempval;                    
                tempval -= i;                    
                i *= 3;               
            }               
            Bezpoint1 = booked.Bezzypoint[i];               
            Bezpoint2 = booked.Bezzypoint[i + 1];               
            Bezpoint3 = booked.Bezzypoint[i + 2];               
            Bezpoint4 = booked.Bezzypoint[i + 3];
                
            //float oneminust = 1f - tempval;                
            float u = 1.0f - tempval;               
            var tt = tempval * tempval;               
            var uu = u * u;              
            var uuu = uu * u;                
            var ttt = tt * tempval;               
            float3 p = uuu * Bezpoint1; //first term               
            p += 3 * uu * tempval * Bezpoint2; //second term              
            p += 3 * u * tt * Bezpoint3; //third term              
            p += ttt * Bezpoint4; //fourth term 

            #region Old Code
            //currentposition = p;

            //if (infiniteloopstopper == 0)
            //{
            //    beginfloat3pos = currentposition;
            //}

            //Debug.DrawLine(currentposition, beginfloat3pos, Color.red);

            //splineLength = math.distance(currentposition, beginfloat3pos);

            //placeonSpline += minDist;

            //resultantPlaceonSpline = currentposition;

            //infiniteloopstopper++;

            //if (endbloodyloop)
            //{
            //    return resultantPlaceonSpline;
            //}
            #endregion

            #region new supposedly more accurate code
            currentposition = p;

            if (infiniteloopstopper == 0)//TODO Is there perhaps a better way to do this
            {
                previousposition = currentposition;
            }

            //Debug.DrawLine(currentposition, previousposition, Color.red);

            splineLength += math.distance(currentposition, previousposition);

            //Debug.Log("The lower splinelength is " + splineLength);
            placeonSpline += minDist;

            previousposition = currentposition;

            resultantPlaceonSpline = currentposition;

            infiniteloopstopper++;

            if (endbloodyloop)
            {
                return new float3(0, -200, 0);//TODO I think I need to check the values in this when I use it I need like a null value or something so that I can know it has reached the limit but I don't want to actually return a value on the spline
            }

            #endregion
        }

        return resultantPlaceonSpline;
    }
}


public class BezeirCreateEntitys : SystemBase
{
    private const float TICK_TIMER_MAX = 1f;

    private int tick;
    private float tickTimer;

    public EntityQuery BezierGraph_Query;

    protected override void OnCreate()
    {
        base.OnCreate();
        //Enabled = false;
    }

    protected override void OnStartRunning()
    {
        BezierGraph_Query = GetEntityQuery(typeof(BezierGraphSpawner));

        var boo = GetSingleton<DotPrefabinator>();

        tick = 0;
        tickTimer = 0.0f;
        Entity toto = EntityManager.Instantiate(boo.MovePrefab);
        EntityManager.AddComponent<TestMoveojbectTag>(toto);
        EntityManager.AddComponent<tPlaceonSpline>(toto);
        // float tempfloat = new float();
        EntityManager.SetComponentData<tPlaceonSpline>(toto, new tPlaceonSpline { floatVal = new float() });
        EntityManager.SetComponentData(toto, new Translation { Value = new float3(0, 0, 0) });
        EntityManager.SetName(toto, "TestMoveObject");
        //Enabled = false;
    }

    protected override void OnUpdate()
    {
        Enabled = false;
        var tempgraphs = BezierGraph_Query.ToEntityArray(Allocator.Temp);
        //Gets the specific spline
        var tempdoomps = EntityManager.GetComponentData<BezierGraphSpawner>(tempgraphs[0]);

        //var currentBezier = tempdoomps;


        var boo = GetSingleton<DotPrefabinator>();

        var constdiststruct = new MoveAlongBezierConstantDistance();

        tickTimer += Time.DeltaTime;
        if (tickTimer >= TICK_TIMER_MAX)
        {
            //if(EntityQueries.)
            tickTimer -= TICK_TIMER_MAX;
            tick++;
            Entity toto = EntityManager.Instantiate(boo.MovePrefab);
            EntityManager.AddComponent<TestMoveojbectTag>(toto);
            EntityManager.AddComponent<tPlaceonSpline>(toto);
            EntityManager.SetComponentData<tPlaceonSpline>(toto, new tPlaceonSpline { floatVal = new float() });
            EntityManager.SetComponentData(toto, new Translation { Value = new float3(0, 0, 0) });
            EntityManager.SetName(toto, "TestMoveObject");
        }

        if (HasSingleton<SplineCheckLengthData>())
        {
            var doodly = GetSingleton<SplineCheckLengthData>();
            //Debug.Log("The length is currently" + doodly.FloatLength);
            return;
        }

        float thedistance = 1.0f;
        float tempinto = 0f;

        int numbofdudes = 50;
        //float otherpos = 0f;
        //for (int y = 0; y < 30; y++)
        {
            //Entity duedbruv = EntityManager.Instantiate(boo.MovePrefab);
        //EntityManager.AddComponent<tPlaceonSpline>(duedbruv);
        
            NativeArray<float3> positsonSpline = new NativeArray<float3>(numbofdudes, Allocator.TempJob);
            //NativeArray<Entity> numbtospawn = new NativeArray<Entity>(numbofdudes, Allocator.Temp);

            Job.WithCode(() =>            
            {
                for(int i = 0; i < positsonSpline.Length; i++)
                {

                    positsonSpline[i] = constdiststruct.Execute(tempinto, thedistance, tempdoomps, out tempinto);
                };

            }).Schedule();

            this.CompleteDependency();
            //var templpos = constdiststruct.Execute(tempinto, thedistance, tempdoomps, out tempinto);
            //EntityManager.Instantiate(boo.MovePrefab, numbtospawn);

            //EntityManager.SetComponentData<tPlaceonSpline>(duedbruv, new tPlaceonSpline { floatVal = new float() });

            for (int j = 0; j < numbofdudes; j++)
            {
                //Debug.Log("The valuees in the nativearray are" + positsonSpline[j]);
                if (positsonSpline[j].Equals(new float3(0, -200, 0)))//TODO Not sure if this entirely works
                    return;
                var dudebruv = EntityManager.Instantiate(boo.MovePrefab);
                EntityManager.AddComponent<TheJustCreatedDubesTag>(dudebruv);
                EntityManager.SetComponentData(dudebruv, new Translation { Value = positsonSpline[j] });

            }

            positsonSpline.Dispose();

        }

        //Enabled = false;
            
      
        EntityManager.CreateEntity(typeof(SplineCheckLengthData));

        tempgraphs.Dispose();

    }

    //return default;
}

public struct TheJustCreatedDubesTag : IComponentData { }