using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Transforms;

public struct CalcDirectionAlongBezier
{

    public Rotation Execute(float placeonSpline, BezierGraphSpawner currentBezier, float direction = 1f)//These only work in one direction but keep this for just in case I even get around to making it work in both directions
    {
        var rutate = new Rotation();


        ref var booked = ref currentBezier.BezzyGraphcomp.Value;

        var Curvecount = (booked.Bezzypoint.Length - 1) / 3;

        //placeonSpline.floatVal;// = timeaccumalator;

        float tempval = placeonSpline;

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


        return rutate;
    }

}
