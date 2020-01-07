using System;
using System.Collections.Generic;
using UnityEngine;

namespace SweepLine
{
    public enum LoopDir
    {
        Clockwise,
        Counterclockwise,
    }
    public class SimplePolygon
    {
        protected List<Point2> m_points = new List<Point2>();
        protected List<Vector2> m_localPoints = new List<Vector2>();
        protected Vector2 m_original = new Vector2();
        protected SweepLineAlg m_sweepLineAlg = new SweepLineAlg();
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
            foreach (var _element in m_points)
            {
                Point2.Pool.GiveBack(_element);
            }
            m_points.Clear();
        }
        public void RemovePointAt(int _id)
        {
            m_localPoints.RemoveAt(_id);
            Point2.Pool.GiveBack(m_points[_id]);
            m_points.RemoveAt(_id);
        }
        public void AddPoint(Vector2 _point)
        {
            m_localPoints.Add(_point);
            m_points.Add(Point2.Pool.Take.SetValue(_point + m_original));
        }
        public SimplePolygon Reverse()
        {
            int len = m_points.Count;
            int _halfLen = (len-1) / 2;
            for (int i = 1; i < _halfLen; i++)
            {
                var _back=len - 1 - i;
                
                var _temp = m_points[_back];
                m_points[_back] = m_points[i];
                m_points[i] = _temp;

                var _temp2 = m_localPoints[_back];
                m_localPoints[_back] = m_localPoints[i];
                m_localPoints[i] = _temp2;
            }
            return this;
        }
        private static float Cross(Vector2 _left, Vector2 _right)
        {
            return _left.x * _right.y - _right.x * _left.y;
        }
        public LoopDir CalculateLoopDir()
        {
            int _pointCount = m_points.Count;
            int _dirNow = 0;
            int _current = 0;
            float _positiveDis = 0;
            float _negativeDis = 0;
            float _totalDis = 0;
            for (int i = 0; i < _pointCount; i++, _current++)
            {
                int _currentID = _current % _pointCount;
                int _preID = (_current - 1 + _pointCount) % _pointCount;
                int _nextID = (_current + 1) % _pointCount;
                var _preToCurrent = m_points[_currentID].getValue() - m_points[_preID].getValue();
                var _currentToNext = m_points[_nextID].getValue() - m_points[_currentID].getValue();
                float _crossV = Cross(_preToCurrent,_currentToNext);
                if (Mathf.Abs(_crossV) > 0.001f)
                {
                    _dirNow = _crossV > 0 ? 1 : -1;
                    _current = i;
                    break;
                }
            }
            for (int i = 0; i < _pointCount; i++, _current++)
            {
                int _currentID = _current % _pointCount;
                int _preID = (_current - 1 + _pointCount) % _pointCount;
                int _nextID = (_current + 1) % _pointCount;
                var _preToCurrent = m_points[_currentID].getValue() - m_points[_preID].getValue();
                var _currentToNext = m_points[_nextID].getValue() - m_points[_currentID].getValue();
                float _crossV = Cross(_preToCurrent,_currentToNext);
                if (Mathf.Abs(_crossV) > 0.001f)
                {
                    _dirNow = _crossV > 0 ? 1 : -1;
                }
                float _disCurrent = _currentToNext.magnitude;
                if (_dirNow > 0)
                {
                    _positiveDis += _disCurrent;
                }
                else
                {
                    _negativeDis += _disCurrent;
                }

                _totalDis += _disCurrent;
            }
            var _loopDir = _positiveDis > _negativeDis ? LoopDir.Counterclockwise : LoopDir.Clockwise;
            return _loopDir;
        }

        public SimplePolygon MakeSimple()
        {
            int _pointCount = m_points.Count;
            for (int i = 3; i < _pointCount; i++)
            {
                for (int j = 0; j < i-2; j++)
                {
                    int _p0ID = (i - 1 + _pointCount) % _pointCount;
                    int _p1ID = i;
                    int _p2ID = (j) % _pointCount;;
                    var _p3ID = (j+1) % _pointCount;;
                    var _p0=m_points[_p0ID].getValue();
                    var _p1=m_points[_p1ID].getValue();
                    var _p2=m_points[_p2ID].getValue();
                    var _p3=m_points[_p3ID].getValue();
                    if(SegmenetCross(_p0,_p1,_p2,_p3))
                    {
                        RemovePointAt(i);
                        i--;
                        _pointCount--;
                        break;
                    }
                }
            }
            return this;
        }
        public static bool SegmenetCross(Vector2 _p0,Vector2 _p1,Vector2 _p2,Vector2 _p3)
        {
            var _p01=_p1-_p0;
            var _p02=_p2-_p0;
            var _p03=_p3-_p0;

            var _p23=_p3-_p2;
            var _p20=_p0-_p2;
            var _p21=_p1-_p2;
            float _value1 = Cross(_p01,_p02)*Cross(_p01,_p03);
            float _value2 =  Cross(_p23,_p20)*Cross(_p23,_p21);
            return _value1<=0 && _value2<=0;
        }

        public void ValidateLoop()
        {
            var _loopDir = CalculateLoopDir();
            ShowPoints("----polygon ["+_loopDir+"] ----");
        }

        protected void ShowPoints(string _polygonInfor)
        {
            _polygonInfor += "\n";
            foreach (var item in m_points)
            {
                _polygonInfor += item + ",";
            }
            _polygonInfor += "\n";
            int _count=m_points.Count;
            if(_count>0)
            {
                var _start=m_points[0];
                var _current=_start;
                do
                {
                    _polygonInfor += _current + ",";
                    _current=_current.m_right;
                }
                while(_current!=_start);
            }
            Debug.Log(_polygonInfor);
        }
        public bool IsClockwise()
        {
            return CalculateLoopDir() == LoopDir.Clockwise;
        }
        public SimplePolygon MakeCounterClockwise()
        {
            if (IsClockwise())
            {
                Reverse();
            }
            return this;
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
        public SimplePolygon SetOriginal(Vector2 _original)
        {
            if (m_original == _original)
            {
                return this;
            }
            m_original = _original;
            for (int i = 0; i < m_localPoints.Count; i++)
            {
                var _point = m_localPoints[i] + m_original;
                m_points[i].SetValue(_point);
            }
            return this;
        }
        public bool CrossWith(SimplePolygon _other)
        {
            return m_sweepLineAlg.CrossWith(this, _other);
        }

        public bool ContainsPoint(Vector2 _point)
        {
            //Add the target point
            if (m_searchingTarget == null)
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