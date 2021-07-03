using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class BezierCurve : MonoBehaviour//, IConvertGameObjectToEntity
{

	public Vector3[] points;
	//Todo not sure if I should do it this way or have a blob conversion system will edit this out for now and try it with the gameobjectconversionsystem
//	public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
//	{
//
//		dstManager.AddComponentData(entity, new BezeirpointsTag());
//		
//		dstManager.AddBuffer<BezierpointsBuffer>(entity);
//		DynamicBuffer<BezierpointsBuffer> danbuffer = dstManager.GetBuffer<BezierpointsBuffer>(entity);
//
//		for (int i = 0; i < 4; i++)
//		{
//			var teman = points[i];
//			var tempvar = new Translation {Value = new float3(teman)};
//			danbuffer.Add(tempvar);
//		}
//
//	}
	
	
	public Vector3 GetPoint (float t) {
		return transform.TransformPoint(Bezier.GetPoint(points[0], points[1], points[2], points[3], t));
	}
	
	public Vector3 GetVelocity (float t) {
		return transform.TransformPoint(Bezier.GetFirstDerivative(points[0], points[1], points[2], points[3], t)) - transform.position;
	}
	
	public Vector3 GetDirection (float t) {
		return GetVelocity(t).normalized;
	}
	
	public void Reset () {
		points = new Vector3[] {
			new Vector3(1f, 0f, 0f),
			new Vector3(2f, 0f, 0f),
			new Vector3(3f, 0f, 0f),
			new Vector3(4f, 0f, 0f)
		};
	}
}

//
//[DisallowMultipleComponent]
//[RequiresEntityConversion]
//public class CameraTagAuthoring : MonoBehaviour, IConvertGameObjectToEntity
//{
//	public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
//	{
//
//		//dstManager.AddComponentData(entity, new CameraTag());
//		dstManager.AddComponent(entity, typeof(CopyTransformToGameObject));
//	}
//}
