using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class DisplaySpline : MonoBehaviour
{
    // Start is called before the first frame update
    public LineRenderer linny;
    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 1);
        //Gizmos.DrawLine();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class DisplaySplineECS : JobComponentSystem
{
    public LineRenderer linny;
    
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        //float3 point1 = new float3(0, 0, 0);
        //float3 point2 = new float3(10, 0, 0);

        //var lanny = linny;

        //var anty = EntityManager.CreateEntity(typeof(RenderMesh));
        //EntityManager.AddComponent(anty, typeof(Translation));
        //EntityManager.AddComponent(anty, typeof(Render));
        //var t = Time.time;
       // EntityManager.SetComponentData(anty, new LineRenderer{});

        //Entities.ForEach((ref Rendermesh du) =>
        {
            
            //for (int i = 0; i < 5; i++)
            {
                //lanny.SetPosition(i, new Vector3(i * 0.5f, Mathf.Sin(i + t), 0.0f));
            }
        }//).WithoutBurst().Run();
        
        //Debug.DrawLine(point1, point2);
        //Debug.DrawLine();
        
        //linny.SetPositions(point1, point2);

        return default;
    }
}

public struct LineComponent : IComponentData
{
    
}