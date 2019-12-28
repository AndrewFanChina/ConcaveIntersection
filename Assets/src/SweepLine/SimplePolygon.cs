using System;
using System.Collections.Generic;
using UnityEngine;

namespace SweepLine
{
    public class SimplePolygon
    {
        protected IList<Point2> m_points = new List<Point2>();
        public void AddPoint(Vector2 _point)
        {
            m_points.Add(new Point2(_point));
        }

        public void AddPoints(Vector2[] _points)
        {
            for (int i = 0; i < _points.Length; i++)
            {
                m_points.Add(new Point2(_points[i]));
            }
        }

        public void AddPoints(float[] _points)
        {
            int _len=_points.Length-_points.Length%2;
            for (int i = 0; i < _len; i+=2)
            {
                m_points.Add(new Point2(_points[i],_points[i+1]));
            }
        }

        public bool ContainsPolygon(SimplePolygon _other)
        {
            bool _contains = false;

            return _contains;
        }

        // protected static Dictionary<Point2, SimplePolygon> BufferedTable = new Dictionary<Point2, SimplePolygon>();
		protected SweepLine m_sweepLine=new SweepLine();
		public bool ContainsPoint(Vector2 _point)
        {
           return m_sweepLine.ContainsPoint(m_points, _point);
        }
    }
}