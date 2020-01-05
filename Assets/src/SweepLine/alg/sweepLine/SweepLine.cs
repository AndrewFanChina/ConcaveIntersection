
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SweepLine
{
  
    public class SweepLine
    {
        protected SortableList<SlopeRegion> m_sweepRegions = new SortableList<SlopeRegion>();
        public void Clear()
        {
            foreach (var item in m_sweepRegions)
            {
                SlopeRegion.Pool.GiveBack(item);
            }
            m_sweepRegions.Clear();
        }
        public void RemoveEndwith(Point2 _site)
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



        public void SetSweepTo(float x)
        {
            for (int i = 0; i < m_sweepRegions.Count; i++)
            {
                m_sweepRegions[i].SweepTo(x);
            }
            m_sweepRegions.QuickSort();
		}

        public bool CheckByCurrentLine(Point2 _searchingTarget)
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

        public void OpenSite(Point2 _site)
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


        public void TurnSite(Point2 _site)
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

        public void CloseSite(Point2 _site, Vector2 _toLeft, Vector2 _toRight)
        {
            RemoveEndwith(_site);
        }
        public int GetRegionID(float y)
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

        public static float SlopeOf(Vector2 _vector2)
        {
            if (Mathf.Approximately(_vector2.x, 0))
            {
                return float.MaxValue;
            }
            return _vector2.y / _vector2.x;
        }


    }

}
