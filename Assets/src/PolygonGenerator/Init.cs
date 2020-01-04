using UnityEngine;
using System.Collections.Generic;
using SweepLine;

namespace ConcaveHull
{
	public class Init : MonoBehaviour
	{

		List<Node> dot_list = new List<Node>(); //Used only for the demo
		
		public string seed;
		public int scaleFactor;
		public int number_of_dots;
		public double concavity;
		public Vector2 m_targetPoint;
		public bool m_contains;
		SimplePolygon m_polygon;
		void Start()
		{
			var _time1 = Time.realtimeSinceStartup;
			setDots(number_of_dots); //Used only for the demo
			generateHull();
			var _time2 = Time.realtimeSinceStartup;
			Debug.LogWarning((_time2 - _time1).ToString("f4"));
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
		}


		private void Update()
		{
			var _newPos = transform.position;
			var _pos2D = new Vector2(_newPos.x, _newPos.y);
			if (m_targetPoint != _pos2D)
			{
				m_targetPoint = _pos2D;
				float _t1=Time.realtimeSinceStartup;
				m_contains = m_polygon.ContainsPoint(m_targetPoint);
				float _t2=Time.realtimeSinceStartup;
				// Debug.Log("used time:"+(_t2-_t1));
			}
		}

		public void setDots(int number_of_dots)
		{
			// This method is only used for the demo!
			System.Random pseudorandom = new System.Random(seed.GetHashCode());
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
			Gizmos.color = Color.yellow;
			for(int i = 0; i < Hull.hull_edges.Count; i++)
			{
				Vector2 left = new Vector2((float)Hull.hull_edges[i].nodes[0].x, (float)Hull.hull_edges[i].nodes[0].y);
				Vector2 right = new Vector2((float)Hull.hull_edges[i].nodes[1].x, (float)Hull.hull_edges[i].nodes[1].y);
				Gizmos.DrawLine(left, right);
			}

			// Concave hull
			Gizmos.color = Color.blue;
			for(int i = 0; i < Hull.hull_concave_edges.Count; i++)
			{
				Vector2 left = new Vector2((float)Hull.hull_concave_edges[i].nodes[0].x, (float)Hull.hull_concave_edges[i].nodes[0].y);
				Vector2 right = new Vector2((float)Hull.hull_concave_edges[i].nodes[1].x, (float)Hull.hull_concave_edges[i].nodes[1].y);
				Gizmos.DrawLine(left, right);
			}

			// Dots
			Gizmos.color = Color.red;
			for(int i = 0; i < dot_list.Count; i++)
			{
				Gizmos.DrawSphere(new Vector3((float)dot_list[i].x, (float)dot_list[i].y, 0), 0.5f);
			}
		}
	}

}
