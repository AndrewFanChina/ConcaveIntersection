using UnityEngine;
using System.Collections.Generic;
using SweepLine;

namespace ConcaveHull
{
	public class PolygonGen : MonoBehaviour
	{

		List<Node> dot_list = new List<Node>(); //Used only for the demo
		
		public int seed;
		public int scaleFactor;
		public int number_of_dots;
		public double concavity;
		public Color m_color=Color.blue;

		public SimplePolygon m_polygon;
		void Start()
		{
			var _time1 = Time.realtimeSinceStartup;
			setDots(number_of_dots); //Used only for the demo
			generateHull();
			var _time2 = Time.realtimeSinceStartup;
			//Debug.LogWarning((_time2 - _time1).ToString("f4"));
		}

		public void generateHull()
		{
			Hull.setConvexHull(dot_list);
			Hull.setConcaveHull(concavity, scaleFactor);
			SegementsLoop _loop = new SegementsLoop();
			for(int i = 0; i < Hull.hull_concave_edges.Count; i++)
			{
                Vector2 left = new Vector2((float)Hull.hull_concave_edges[i].nodes[0].x, (float)Hull.hull_concave_edges[i].nodes[0].y);
                Vector2 right = new Vector2((float)Hull.hull_concave_edges[i].nodes[1].x, (float)Hull.hull_concave_edges[i].nodes[1].y);
				_loop.AddSegment(left, right);
			}
			m_polygon = _loop.ToPolygon();
			Hull.Clear();
		}

		protected void Update()
		{
			var _pos3=transform.position;
			m_polygon.SetOriginal(new Vector2(_pos3.x,_pos3.y));
		}


		public void setDots(int number_of_dots)
		{
			// This method is only used for the demo!
			System.Random pseudorandom = new System.Random(seed);
			for(int x = 0; x < number_of_dots; x++)
			{
				dot_list.Add(new Node(pseudorandom.Next(0, 100), pseudorandom.Next(0, 100), x));
			}
			//Delete nodes that share same position
			for(int pivot_position = 0; pivot_position < dot_list.Count; pivot_position++)
			{
				for(int position = 0; position < dot_list.Count; position++)
				{
					if(dot_list[pivot_position].x == dot_list[position].x && dot_list[pivot_position].y == dot_list[position].y
						&& pivot_position != position)
					{
						dot_list.RemoveAt(position);
						position--;
					}
				}
			}
		}

		// Unity demo visualization
		void OnDrawGizmos()
		{
			// Convex hull
			//Gizmos.color = Color.yellow;
			//for(int i = 0; i < m_hull.hull_edges.Count; i++)
			//{
			//	Vector2 left = new Vector2((float)m_hull.hull_edges[i].nodes[0].x, (float)m_hull.hull_edges[i].nodes[0].y);
			//	Vector2 right = new Vector2((float)m_hull.hull_edges[i].nodes[1].x, (float)m_hull.hull_edges[i].nodes[1].y);
			//	Gizmos.DrawLine(left, right);
			//}

			// Concave hull
			// Gizmos.color = m_color;
			// for(int i = 0; i < Hull.hull_concave_edges.Count; i++)
			// {
            //     Vector2 left = new Vector2((float)Hull.hull_concave_edges[i].nodes[0].x, (float)Hull.hull_concave_edges[i].nodes[0].y);
            //     Vector2 right = new Vector2((float)Hull.hull_concave_edges[i].nodes[1].x, (float)Hull.hull_concave_edges[i].nodes[1].y);
            //     Gizmos.DrawLine(left, right);
            //     Gizmos.DrawSphere(new Vector3(left.x, left.y, 0), 0.5f);
			// }

			// Dots
			//Gizmos.color = Color.red;
			//for(int i = 0; i < dot_list.Count; i++)
			//{
			//	Gizmos.DrawSphere(new Vector3((float)dot_list[i].x, (float)dot_list[i].y, 0), 0.5f);
			//}
			if(m_polygon==null||m_polygon.Points==null)
			{
				return;
			}
			Gizmos.color = m_color;
			var _points = m_polygon.Points;
			int _count=_points.Count;
			for(int i = 0; i < _count; i++)
			{
                Vector2 left = _points[i].getValue();
                Vector2 right = _points[(i+1)%_count].getValue();
                Gizmos.DrawLine(left, right);
                Gizmos.DrawSphere(new Vector3(left.x, left.y, 0), 0.5f);
			}
		}
	}

}
