﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBezier : MonoBehaviour
{
	
    

	public class BezierPath
	{
		public List<Vector3> pathPoints;
		private int segments;
		public int pointCount;
		
		public BezierPath()
		{
			pathPoints = new List<Vector3>();
			pointCount = 100;
		}

		public void DeletePath()
		{
			pathPoints.Clear ();
		}
		
		Vector3 BezierPathCalculation(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
		{	
			float tt = t * t;
			float ttt = t * tt;
			float u = 1.0f - t;
			float uu = u * u;
			float uuu = u * uu;
			
			Vector3 B = new Vector3();
			B = uuu * p0;
			B += 3.0f * uu * t * p1;
			B += 3.0f * u * tt * p2;
			B += ttt * p3;
			
			return B;
		}
		
		public void CreateCurve(List<Vector3> controlPoints)
		{
			segments = controlPoints.Count / 3;

			for (int s = 0; s < controlPoints.Count -3; s+=3) 
			{
				Vector3 p0 = controlPoints[s];
				Vector3 p1 = controlPoints[s+1];
				Vector3 p2 = controlPoints[s+2]; 
				Vector3 p3 = controlPoints[s+3];

				if(s == 0)
				{
					pathPoints.Add(BezierPathCalculation(p0, p1, p2, p3, 0.0f));
				}    

				for (int p = 0; p < (pointCount/segments); p++) 
				{
					float t = (1.0f / (pointCount/segments)) * p;
					Vector3 point = new Vector3 ();
					point = BezierPathCalculation (p0, p1, p2, p3, t);
					pathPoints.Add (point);
				}
			}
		}
	}
}

//	private void createLine(Vector3 start, Vector3 end, float lineSize, Color c) 
//	{
//		GameObject canvas = new GameObject("canvas" + canvasIndex); 
//		canvas.transform.parent = transform;
//		canvas.transform.rotation = transform.rotation;
//		LineRenderer lines = (LineRenderer) canvas.AddComponent<LineRenderer>();
//		lines.material = new Material(shader);
//		lines.material.color = c;
//		lines.useWorldSpace = false;
//		lines.SetWidth(lineSize, lineSize);
//		lines.SetVertexCount(2);
//		lines.SetPosition(0, start);
//		lines.SetPosition(1, end);
//		canvasIndex++;
//	}
//
//	private void UpdatePath()
//	{
//		List<Vector3> c = new List<Vector3>();
//		for(int o = 0; o < objects.Length; o++)
//		{
//			if(objects[o] != null)
//			{
//				Vector3 p = objects[o].transform.position;
//				c.Add (p);
//			}
//		}
//		path.DeletePath ();
//		path.CreateCurve(c);
//	}
//	
//	private int canvasIndex = 0;
//	public Shader shader;
//	BezierPath path = new BezierPath();
//	public GameObject[] objects;
//
//	// Use this for initialization
//	void Start () {
//		UpdatePath ();
//	}
//	
//	// Update is called once per frame 
//	void Update () 
//	{
//		UpdatePath ();
//		for (int i = 1; i < (path.pointCount); i++)
//		{
//			Vector3 startv = path.pathPoints[i-1];
//			Vector3 endv = path.pathPoints[i];
//			createLine (startv, endv, 0.25f, Color.blue);
//		}
//	}
//
//	void OnDrawGizmos() 
//	{
//		UpdatePath ();
//		for (int i = 1; i < (path.pointCount); i++)
//		{
//			Vector3 startv = path.pathPoints[i-1];
//			Vector3 endv = path.pathPoints[i];
//			Gizmos.color = Color.blue;
//			Gizmos.DrawLine(startv, endv);
//		}
//	}
//}
