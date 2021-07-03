using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


[DisallowMultipleComponent]
[RequiresEntityConversion]
public class GetBezeierpoints : MonoBehaviour, IConvertGameObjectToEntity
{
    public Transform[] points;
    //public Transform p1;
    //public Transform p2;
    //public Transform p3;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddBuffer<BezierpointsBuffer>(entity);
        DynamicBuffer<BezierpointsBuffer> danbuffer = dstManager.GetBuffer<BezierpointsBuffer>(entity);

        for (int i = 0; i < 4; i++)
        {
            var teman = points[i].position;
            var tempvar = new Translation {Value = new float3(teman)};
            danbuffer.Add(tempvar);
        }

        dstManager.AddComponent(entity, typeof(BezeirpointsTag) );

        //dstManager.AddComponentData(entity, new CameraTag());
        //dstManager.AddComponent(entity, typeof(CopyTransformToGameObject));
    }
}

public struct BezeirpointsTag : IComponentData {}

[InternalBufferCapacity(4)]
public struct BezierpointsBuffer : IBufferElementData
{
    // Actual value each buffer element will store.
    public Translation Value;

    // The following implicit conversions are optional, but can be convenient.
    public static implicit operator Translation(BezierpointsBuffer e)
    {
        return e.Value;
    }

    public static implicit operator BezierpointsBuffer(Translation e)
    {
        return new BezierpointsBuffer {Value = e};
    }
}



