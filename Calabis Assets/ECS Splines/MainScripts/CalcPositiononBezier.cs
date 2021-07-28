using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public struct CalcPositiononBezier
{

    public float3 Execute(float placeonSpline, BezierBlobsRef currentBezier)
    {
        float3 resultPlaceonSpline = new float3();

        ref var booked = ref currentBezier.BlobrefVal.Value;


        var Curvecount = (booked.Bezzypoint.Length - 1) / 3;

        float3 Bezpoint1 = new float3();
        float3 Bezpoint2 = new float3();
        float3 Bezpoint3 = new float3();
        float3 Bezpoint4 = new float3();

        float tempval = placeonSpline;

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

        resultPlaceonSpline = p;

        return resultPlaceonSpline;

    }

}