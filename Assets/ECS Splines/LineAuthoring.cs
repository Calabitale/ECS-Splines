using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using JetBrains.Annotations;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;


[RequiresEntityConversion]
public class LineAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public Vector3 p0, p1;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new LineTag());
        //todo: I dont think I need this I need to implicitly convert the Vector3 po, p1 
        //dstManager.AddComponent(entity, typeof(CopyTransformToGameObject));
        
    }
}

public struct LineTag : IComponentData
{
    
}
