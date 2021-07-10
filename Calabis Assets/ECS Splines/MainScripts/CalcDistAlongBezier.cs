using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;

public struct CalcDistAlongBezier //Calculalates and returns the distance along the bezier
{

    public float3 Execute(float begintpos, float distance, BezierGraphSpawner currentBezier, out float placeonSpline, float direction = 1f)
    {
        float3 resultantPlaceonSpline = new float3();
        float minDist = 0.001f;

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
        while (splineLength < distance)    //TOTO Need to figure out if I can get spline length a bit closer to the distance maybe it doesn't but I might as well try         
        {
            
            if (infiniteloopstopper > 1000)
            {
                Debug.Log("The infiniteloopstopper has been activated when it never should have");
                return new float3(0, -200, 0);
            }

            ref var booked = ref currentBezier.BezzyGraphcomp.Value;
            var Curvecount = (booked.Bezzypoint.Length - 1) / 3;
                      
            float tempval = placeonSpline;

            float3 Bezpoint1 = new float3();
            float3 Bezpoint2 = new float3();
            float3 Bezpoint3 = new float3();
            float3 Bezpoint4 = new float3();

            int i;

            if (tempval >= 1f)
            {                
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
            //    return new float3(0, -200, 0);
            //}
            #endregion

            #region new supposedly more accurate code
            currentposition = p;

            if (infiniteloopstopper == 0)//TODO Is there perhaps a better way to do this
            {
                previousposition = currentposition;
            }

            Debug.DrawLine(currentposition, previousposition, Color.red);

            splineLength += math.distance(currentposition, previousposition);
            
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