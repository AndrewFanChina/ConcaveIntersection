
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SweepLine
{
    public enum SiteType
    {
        Open,
        Turn,
        Close
    }
    public enum InOut
    {
        In,
        Out,
    }


    public class SlopeRegion : IComparable<SlopeRegion>
    {
        public Point2 m_start;
        public Point2 m_end;
        public float m_slop;
        public InOut m_inOut;
        public SlopeRegion(Point2 _start, Point2 _end)
        {
            m_start = _start;
            m_end = _end;
            m_inOut = Point2.InOutOf(_start, _end);
        }

        public bool Contains(Point2 pos)
        {
            Debug.Assert(pos.x >= m_start.x);
            float _slopY = m_slop * (pos.x - m_start.x);
            float _relativeY = pos.y - m_start.y;
            return _relativeY <= _slopY;
        }
        public static InOut Reverse(InOut _inout)
        {
            _inout = _inout == InOut.Out ? InOut.In : InOut.Out;
            return _inout;
        }


        public int CompareTo(SlopeRegion other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            if (m_start.y != other.m_start.y)
            {
                return m_start.y > other.m_start.y ? -1 : 1;
            }
            if (m_end.y != other.m_end.y)
            {
                return m_end.y > other.m_end.y ? -1 : 1;
            }
            if (m_slop != other.m_slop)
            {
                return m_slop > other.m_slop ? -1 : 1;
            }
            return 0;
        }


    }

    public class SweepLine
    {
        protected List<Point2> m_allPoints = new List<Point2>();
        protected List<SlopeRegion> m_lineList = new List<SlopeRegion>();
        protected Point2 m_searchingTarget;
        protected void RemoveEndwith(Point2 _site)
        {
            for (int i = 0; i < m_lineList.Count; i++)
            {
                if (m_lineList[i].m_end == _site)
                {
                    m_lineList.RemoveAt(i);
                    i--;
                }
            }
        }
        protected void ShowPoints(string _polygonInfor)
        {
            _polygonInfor += "\n";
            foreach (var item in m_allPoints)
            {
                _polygonInfor += item + ",";
            }
            Debug.Log(_polygonInfor);
        }

        public bool ContainsPoint(IList<Point2> _polygon, Vector2 _point)
        {
            //Add polygon points
            m_allPoints.Clear();
            m_lineList.Clear();
            int len = _polygon.Count;
            for (int i = 0; i < len; i++)
            {
                m_allPoints.Add(_polygon[i]);
            }
            ShowPoints("-----polygon points-----");
            for (int i = 0; i < len; i++)
            {
                _polygon[i].m_left = _polygon[(i - 1 + len) % len];
                _polygon[i].m_right = _polygon[(i + 1) % len];
            }
            //Add the target point
            if (m_searchingTarget == null)
            {
                m_searchingTarget = new Point2(_point);
            }
            else
            {
                m_searchingTarget.SetValue(_point);
            }
            m_allPoints.Add(m_searchingTarget);
            //sort from left to right
            m_allPoints.Sort();
            ShowPoints("-----sorted points-----");
            //sweep from left to right
            int _bufLen = m_allPoints.Count;
            for (int i = 0; i < _bufLen; i++)
            {
                Point2 _pI = m_allPoints[i];
                if (_pI == m_searchingTarget)
                {
                    return CheckByCurrentLine(m_searchingTarget);
                }

                Point2 _pIL = _pI.m_left;
                Point2 _pIR = _pI.m_right;
                Vector2 _pIV = _pI.getValue();
                Vector2 _toLeft = _pIL.getValue() - _pIV;
                Vector2 _toRight = _pIR.getValue() - _pIV;
                if (_toLeft.x > 0 && _toRight.x > 0)      //Open Site,add double slope
                {
                    OpenSite(_pI);
                }
                else if (_toLeft.x < 0 && _toRight.x < 0) //Close Site,remove double slopes
                {
                    CloseSite(_pI, _toLeft, _toRight);
                }
                else //Turn Site,add single slope
                {
                    TurnSite(_pI);
                }
            }

            return false;

        }

        protected bool CheckByCurrentLine(Point2 _searchingTarget)
        {
            for (int i = m_lineList.Count - 1; i >= 0; i--)
            {
                if (m_lineList[i].Contains(_searchingTarget))
                {
                    return m_lineList[i].m_inOut == InOut.In;
                }
            }
            return false;
        }

        protected void OpenSite(Point2 _site)
        {
            SlopeRegion _regionHi = new SlopeRegion(_site, _site.m_left);
            var _toLeft = _site.m_left.getValue() - _site.getValue();
            _regionHi.m_slop = SlopeOf(_toLeft);

            SlopeRegion _regionLow = new SlopeRegion(_site, _site.m_right);
            var _toRight = _site.m_right.getValue() - _site.getValue();
            _regionLow.m_slop = SlopeOf(_toRight);

            if (_toLeft.y < _toRight.y)
            {
                var _temp = _regionHi;
                _regionHi = _regionLow;
                _regionLow = _temp;
            }
            int _regionID = GetRegionID(_site.y);
            m_lineList.Add(_regionHi);
            m_lineList.Add(_regionLow);
            m_lineList.Sort();
        }


        protected void TurnSite(Point2 _site)
        {
            Point2 _right;
            if (_site.m_right.x > _site.m_left.x)
            {
                _right = _site.m_right;
            }
            else
            {
                _right = _site.m_left;
            }
            RemoveEndwith(_site);
            SlopeRegion _region = new SlopeRegion(_site, _right);
            var _toRight = _right.getValue() - _site.getValue();
            _region.m_slop = SlopeOf(_toRight);
            m_lineList.Add(_region);
            m_lineList.Sort();
        }

        protected void CloseSite(Point2 _site, Vector2 _toLeft, Vector2 _toRight)
        {
            RemoveEndwith(_site);
        }
        protected int GetRegionID(float y)
        {
            int _count = m_lineList.Count;
            int _up = 0;
            int _down = _count - 1;
            int _center = (_up + _down) >> 1;
            int _id = _up;
            while (_up < _down)
            {
                if (y >= m_lineList[_center].m_start.y)
                {
                    _id = _up;
                    _down = _center;
                }
                else
                {
                    _id = _down;
                    _up = _center;
                }
                _center = (_up + _down) >> 1;
                if (_center == _up || _center == _down)
                {
                    break;
                }
            }
            return _id;
        }

        protected static float SlopeOf(Vector2 _vector2)
        {
            if (Mathf.Approximately(_vector2.x, 0))
            {
                return float.MaxValue;
            }
            return _vector2.y / _vector2.x;
        }


    }

}
