using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;

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
        Enabled = false;
        BezierGraph_Query = GetEntityQuery(typeof(BezierGraphSpawner));

    }
    protected override void OnStartRunning()
    {
        //The spline positions to check the distance at, if you want to check the distance between objects on the spline then you could get the tsplineposition of that object and apply it to 
        BeginSplinepos = 0f;
        EndSplinepos = 1f;

        //TODO This value is the minimum distance value that it checks, the lower it goes the less performant it is but the more accurate it is, I think the best value is 0.01 currently
        //But you may need to go lower if you want more accuracy for objects that are closer distance, then again maybe this value will be fine for all cases
        MinLengthDist = 0.01f;        
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

        if (begintpos > endtpos)
        {
            Debug.Log("The beginSplinepos length check is larger than the Endsplinepos, thats an error you doofus");
            return;

        }

        Entities.ForEach((ref SplineLengthData length) =>
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

                length.FloatLength += math.distance(currentposition, previousposition);

                if (j >= endtpos)
                {
                    return;
                }

                previousposition = currentposition;

            }            
        }).Schedule();

        tempgraphs.Dispose();

    }
}