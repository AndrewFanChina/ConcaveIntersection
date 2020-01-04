
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
        public Point2 m_site;
        public float m_slop;
        public InOut m_inOut;
        public int m_regionID;

        public InOut InOutOf(Vector2 pos)
        {
            Debug.Assert(pos.x >= m_site.x);
            float _slopY = m_slop * (pos.x - m_site.x);
            float _relativeY = pos.y - m_site.y;
            InOut _result;
            if (_relativeY >= _slopY)
            {
                _result = m_inOut;
            }
            else
            {
                _result = Reverse(m_inOut);
            }
            return _result;
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
            if (m_site.y != other.m_site.y)
            {
                return m_site.y > other.m_site.y ? -1 : 1;
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
        protected MultiSortedSet<Point2, SlopeRegion> m_lineSet = new MultiSortedSet<Point2, SlopeRegion>();
        protected Point2 m_searchingTarget;
        protected void InseartRegion(Point2 _atSite, int _regionID, SlopeRegion _region)
        {
            _region.m_site = _atSite;
            if (_regionID >= m_lineList.Count)
            {
                _region.m_regionID = m_lineList.Count;
                m_lineList.Add(_region);
            }
            else
            {
                _region.m_regionID = _regionID;
                m_lineList.Insert(_regionID, _region);
                for (int i = _regionID; i < m_lineList.Count; i++)
                {
                    m_lineList[i].m_regionID = i;
                }
            }
            m_lineSet.Add(_atSite, _region);
        }
        protected void RemoveRegion(SlopeRegion _region)
        {
            var _atSite = _region.m_site;
            int _regionID = _region.m_regionID;
            m_lineList.RemoveAt(_regionID);
            for (int i = _regionID; i < m_lineList.Count; i++)
            {
                m_lineList[i].m_regionID = i;
            }
            m_lineSet.Remove(_atSite, _region);
        }
        private void RemoveEndwith(Point2 _site)
        {

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
            ////	int _count = m_lineList.Count;
            ////	if(_count == 0)
            ////	{
            ////		_regionID = 0;
            ////		return InOut.Out;
            ////	}
            ////	_regionID = GetRegionID(_pos.y);
            ////	return m_lineList[_regionID].InOutOf(_pos);

            InOut _inOut = InOutOf(_searchingTarget.getValue(), out _);
            return _inOut == InOut.In;
        }

        protected void OpenSite(Point2 _site)
        {
            SlopeRegion _regionHi = new SlopeRegion();
            var _toLeft = _site.m_left.getValue() - _site.getValue();
            _regionHi.m_slop = SlopeOf(_toLeft);
            _regionHi.m_inOut = InOutOf(_site, _site.m_left);

            SlopeRegion _regionLow = new SlopeRegion();
            var _toRight = _site.m_right.getValue() - _site.getValue();
            _regionLow.m_slop = SlopeOf(_toRight);
            _regionLow.m_inOut = InOutOf(_site, _site.m_right);

            if (_toLeft.y < _toRight.y)
            {
                var _temp = _regionHi;
                _regionHi = _regionLow;
                _regionLow = _temp;
            }
            int _regionID = GetRegionID(_site.y);
            InseartRegion(_site, _regionID, _regionHi);
            InseartRegion(_site, _regionID + 1, _regionLow);
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
            //remove left neighbor region
            RemoveEndwith(_site);
            SlopeRegion _region = new SlopeRegion();
            var _toRight = _right.getValue() - _site.getValue();
            _region.m_slop = SlopeOf(_toRight);
            _region.m_inOut = InOutOf(_site, _right);
            int _regionID = GetRegionID(_site.y);
            InseartRegion(_site, _regionID, _region);

        }



        protected void CloseSite(Point2 _site, Vector2 _toLeft, Vector2 _toRight)
        {
            Point2 _left, _right;
            if (_toLeft.x < _toRight.x)
            {
                var _temp = _toRight;
                _toRight = _toLeft;
                _toLeft = _temp;
                _left = _site.m_right;
                _right = _site.m_left;
            }
            else
            {
                _left = _site.m_left;
                _right = _site.m_right;
            }

            SlopeRegion _HRegion = m_lineSet[_left].Min;
            SlopeRegion _LRegion = m_lineSet[_right].Max;
            if (_LRegion.m_site.y > _HRegion.m_site.y)
            {
                var _temp = _HRegion;
                _HRegion = _LRegion;
                _LRegion = _temp;
            }

            for (int i = m_lineList.Count - 1; i >= 0; i--)
            {
                var regionI = m_lineList[i];
                var _rID = regionI.m_regionID;
                if (regionI.m_site.x < _right.x
                    && _rID < _HRegion.m_regionID
                    && _rID > _LRegion.m_regionID)
                {
                    RemoveRegion(regionI);
                }
            }
            //int _regionID = _leftSiteRegion.m_regionID;
            //var _inOut = _leftSiteRegion.m_inOut;

            //SlopeRegion _region = new SlopeRegion();
            //_region.m_slop = SlopeOf(_toRight);
            //_region.m_inOut = _inOut;

            //AddRegion(_site, _regionID, ref _region);
        }

        protected InOut InOutOf(Point2 _pointStart, Point2 _pointEnd)
        {
            if (_pointStart.m_right == _pointEnd)
            {
                return InOut.Out;
            }
            return InOut.In;
        }

        //protected InOut InOutOf(Vector2 _pos, out int _regionID)
        //{
        //	int _count = m_lineList.Count;
        //	if(_count == 0)
        //	{
        //		_regionID = 0;
        //		return InOut.Out;
        //	}
        //	_regionID = GetRegionID(_pos.y);
        //	return m_lineList[_regionID].InOutOf(_pos);
        //}

        protected int GetRegionID(float y)
        {
            int _count = m_lineList.Count;
            int _up = 0;
            int _down = _count - 1;
            int _center = (_up + _down) >> 1;
            int _id = _up;
            while (_up < _down)
            {
                if (y >= m_lineList[_center].m_site.y)
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
