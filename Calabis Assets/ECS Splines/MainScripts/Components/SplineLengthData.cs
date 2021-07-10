using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct SplineLengthData : IComponentData
{
    public float FloatLength;
    public float BeginSplinepos;
    public float EndSplinepos;
}