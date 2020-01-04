using System;
using System.Collections.Generic;
using UnityEngine;

namespace SweepLine
{
    public class SimplePolygon
    {
        protected List<Point2> m_points = new List<Point2>();
        public void Clear()
        {
	        m_points.Clear();
        }

        public void AddPoint(Vector2 _point)
        {
            m_points.Add(Point2.Pool.Take.SetValue(_point));
        }

        public void AddPoints(Vector2[] _points)
        {
            for (int i = 0; i < _points.Length; i++)
            {
                AddPoint(_points[i]);
            }
        }

        public bool ContainsPolygon(SimplePolygon _other)
        {
            bool _contains = false;

            return _contains;
        }

        // protected static Dictionary<Point2, SimplePolygon> BufferedTable = new Dictionary<Point2, SimplePolygon>();
		protected SweepLine m_sweepLine=new SweepLine();
		protected Point2 m_searchingTarget;
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
			return m_sweepLine.ContainsPoint(m_points, m_searchingTarget);
        }
    }
}