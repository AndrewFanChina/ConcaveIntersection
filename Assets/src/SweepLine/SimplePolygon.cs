using System;
using System.Collections.Generic;
using UnityEngine;

namespace SweepLine
{
    public class SimplePolygon
    {
        protected List<Point2> m_points = new List<Point2>();
		protected List<Vector2> m_localPoints = new List<Vector2>();
		protected Vector2 m_original = new Vector2();
		protected SweepLineAlg m_sweepLineAlg=new SweepLineAlg();
		protected Point2 m_searchingTarget;
		public List<Point2> Points
		{
			get
			{
				return m_points;
			}
		}
        public void Clear()
        {
			m_localPoints.Clear();
			foreach(var _element in m_points)
			{
				Point2.Pool.GiveBack(_element);
			}
			m_points.Clear();
		}

        public void AddPoint(Vector2 _point)
        {
			m_localPoints.Add(_point);
			m_points.Add(Point2.Pool.Take.SetValue(_point + m_original));
		}

		public SimplePolygon LinkNeighbors()
		{
			int len = m_points.Count;
            for (int i = 0; i < len; i++)
            {
				m_points[i].m_left = m_points[(i - 1 + len) % len];
				m_points[i].m_right = m_points[(i + 1) % len];
            }
			return this;
		}

        public void AddPoints(Vector2[] _points)
        {
            for (int i = 0; i < _points.Length; i++)
            {
                AddPoint(_points[i]);
            }
        }
		public void SetOriginal(Vector2 _original)
		{
			if(m_original == _original)
			{ 
				return;
			}
			m_original = _original;
			for(int i = 0; i < m_localPoints.Count; i++)
			{
				var _point = m_localPoints[i] + m_original;
				m_points[i].SetValue(_point);
			}
		}
		public bool ContainsPolygon(SimplePolygon _other)
        {
			return m_sweepLineAlg.ContainsPolygon(this, _other);
        }


		public bool ContainsPoint(Vector2 _point)
        {
			//Add the target point
			if(m_searchingTarget == null)
			{
				m_searchingTarget = Point2.Pool.Take.SetValue(_point);
			}
			else
			{
				m_searchingTarget.SetValue(_point);
			}
			return m_sweepLineAlg.ContainsPoint(m_points, m_searchingTarget);
        }
    }
}