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
        protected static List<Point2> BufferedList = new List<Point2>();
        // protected static Dictionary<Point2, SimplePolygon> BufferedTable = new Dictionary<Point2, SimplePolygon>();
        public bool ContainsPoint(Vector2 _point)
        {
            BufferedList.Clear();
            int len = m_points.Count;
            for (int i = 0; i < len; i++)
            {
                BufferedList.Add(m_points[i]);
                // BufferedTable.Add(m_points[i], this);
            }
            Point2 _pointTarget = new Point2(_point);
            BufferedList.Add(_pointTarget);
            // BufferedTable.Add(_pointTarget, this);
            BufferedList.Sort();
            string _polygonInfor = "-----sorted points-----\n";
            foreach (var item in BufferedList)
            {
	            _polygonInfor+=item.ToString()+",";
            }
            Debug.Log(_polygonInfor);

            int _bufLen = BufferedList.Count;
            for (int i = 0; i < _bufLen; i++)
            {
	            Point2 _pI = BufferedList[i];
	            Point2 _pIL = BufferedList[(i + _bufLen - 1)% _bufLen];
	            Point2 _pIR = BufferedList[(i + 1) % _bufLen];
	            Vector2 _pIV = _pI.getValue();
	            Vector2 _toLV = _pIL.getValue() - _pIV;
	            Vector2 _toRV = _pIR.getValue() - _pIV;
				//ÅÐ¶ÏÊÂ¼þ

            }

			return false;

        }
    }
}