
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SweepLine
{
   
    public class SweepLine
    {
        protected SortableList<Point2> m_rowPoints = new SortableList<Point2>();
        protected SortableList<SlopeRegion> m_sweepRegions = new SortableList<SlopeRegion>();
        protected void RemoveEndwith(Point2 _site)
        {
            for (int i = 0; i < m_sweepRegions.Count; i++)
            {
				var elementI = m_sweepRegions[i];
				if (elementI.m_end == _site)
                {
                    m_sweepRegions.RemoveAt(i);
					SlopeRegion.Pool.GiveBack(elementI);
					i--;
				}
            }
        }
        protected void ShowPoints(string _polygonInfor)
        {
            _polygonInfor += "\n";
            foreach (var item in m_rowPoints)
            {
                _polygonInfor += item + ",";
            }
            Debug.Log(_polygonInfor);
        }

        public bool ContainsPoint(List<Point2> _polygon, Point2 _target)
        {
            int len = _polygon.Count;
            for (int i = 0; i < len; i++)
            {
				m_rowPoints.Add(_polygon[i]);
            }
            //ShowPoints("-----polygon points-----");
            m_rowPoints.Add(_target);
			//sort from left to right
			m_rowPoints.QuickSort();
            //ShowPoints("-----sorted points-----");
            //sweep from left to right
            int _bufLen = m_rowPoints.Count;
			bool _result = false;
            for (int i = 0; i < _bufLen; i++)
            {
                Point2 _pI = m_rowPoints[i];
                if (_pI == _target)
                {
					_result = CheckByCurrentLine(_target);
					break;
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

			//Add polygon points
			m_rowPoints.Clear();
			foreach(var item in m_sweepRegions)
			{
				SlopeRegion.Pool.GiveBack(item);
			}
			m_sweepRegions.Clear();
			return _result;
        }

        public bool ContainsPolygon(List<Point2> _polygon1, List<Point2> _polygon2)
        {
            return false;
        }

        protected void SetSweepTo(float x)
        {
            for (int i = 0; i < m_sweepRegions.Count; i++)
            {
                m_sweepRegions[i].SweepTo(x);
            }
            m_sweepRegions.QuickSort();
		}

        protected bool CheckByCurrentLine(Point2 _searchingTarget)
        {
            for (int i = m_sweepRegions.Count - 1; i >= 0; i--)
            {
                if (m_sweepRegions[i].Contains(_searchingTarget))
                {
                    return m_sweepRegions[i].m_inOut == InOut.In;
                }
            }
            return false;
        }

        protected void OpenSite(Point2 _site)
        {
            SlopeRegion _regionHi = SlopeRegion.Pool.Take.SetValue(_site, _site.m_left);
            var _toLeft = _site.m_left.getValue() - _site.getValue();
            _regionHi.m_slop = SlopeOf(_toLeft);

            SlopeRegion _regionLow = SlopeRegion.Pool.Take.SetValue(_site, _site.m_right);
            var _toRight = _site.m_right.getValue() - _site.getValue();
            _regionLow.m_slop = SlopeOf(_toRight);

            if (_toLeft.y < _toRight.y)
            {
                var _temp = _regionHi;
                _regionHi = _regionLow;
                _regionLow = _temp;
            }
            m_sweepRegions.Add(_regionHi);
            m_sweepRegions.Add(_regionLow);
            SetSweepTo(_site.x);
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
            SlopeRegion _region = SlopeRegion.Pool.Take.SetValue(_site, _right);
            var _toRight = _right.getValue() - _site.getValue();
            _region.m_slop = SlopeOf(_toRight);
            m_sweepRegions.Add(_region);
            SetSweepTo(_site.x);
        }

        protected void CloseSite(Point2 _site, Vector2 _toLeft, Vector2 _toRight)
        {
            RemoveEndwith(_site);
        }
        protected int GetRegionID(float y)
        {
            int _count = m_sweepRegions.Count;
            int _up = 0;
            int _down = _count - 1;
            int _center = (_up + _down) >> 1;
            int _id = _up;
            while (_up < _down)
            {
                if (y >= m_sweepRegions[_center].m_start.y)
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
