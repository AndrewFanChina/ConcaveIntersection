using System.Collections.Generic;
using UnityEngine;

namespace SweepLine
{
    public class SweepLineAlg
    {
        protected SortableList<Point2> m_rowPoints = new SortableList<Point2>();
        protected SweepLine m_sweepLine0 = new SweepLine();
        protected SweepLine m_sweepLine1 = new SweepLine();
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
                    _result = m_sweepLine0.InoutOf(_target) == InOut.In;
                    break;
                }

                Point2 _pIL = _pI.m_left;
                Point2 _pIR = _pI.m_right;
                Vector2 _pIV = _pI.getValue();
                Vector2 _toLeft = _pIL.getValue() - _pIV;
                Vector2 _toRight = _pIR.getValue() - _pIV;
                if (_toLeft.x > 0 && _toRight.x > 0)      //Open Site,add double slope
                {
                    m_sweepLine0.OpenSite(_pI);
                }
                else if (_toLeft.x < 0 && _toRight.x < 0) //Close Site,remove double slopes
                {
                    m_sweepLine0.CloseSite(_pI, _toLeft, _toRight);
                }
                else //Turn Site,add single slope
                {
                    m_sweepLine0.TurnSite(_pI);
                }
            }

            m_rowPoints.Clear();
            m_sweepLine0.Clear();
            return _result;
        }

        public bool CrossWith(SimplePolygon _polygon1, SimplePolygon _polygon2)
        {
            // _polygon1.ValidateLoop();
            // _polygon2.ValidateLoop();
            m_rowPoints.Clear();
            List<Point2> _polygon1Points = _polygon1.Points;
            int len1 = _polygon1Points.Count;
            for (int i = 0; i < len1; i++)
            {
                var _pI = _polygon1Points[i];
                _pI.m_polygonFlag = 0;
                m_rowPoints.Add(_pI);
            }
            List<Point2> _polygon2Points = _polygon2.Points;
            int len2 = _polygon2Points.Count;
            for (int i = 0; i < len2; i++)
            {
                var _pI = _polygon2Points[i];
                _pI.m_polygonFlag = 1;
                m_rowPoints.Add(_pI);
            }
            //sort from left to right
            m_rowPoints.QuickSort();
            //ShowPoints("-----sorted points-----");
            //sweep from left to right
            int _bufLen = m_rowPoints.Count;
            bool _result = false;
            for (int i = 0; i < _bufLen; i++)
            {
                Point2 _pI = m_rowPoints[i];
                var _sweepLineCheck = _pI.m_polygonFlag == 0 ? m_sweepLine1 : m_sweepLine0;
                if (_sweepLineCheck.InoutOf(_pI) == InOut.In)//TODO:polygon's loop order
                {
                    _sweepLineCheck.InoutOf(_pI);
                    _result = true;
                    break;
                }
                var _sweepLineStep = _pI.m_polygonFlag == 0 ? m_sweepLine0 : m_sweepLine1;
                Point2 _pIL = _pI.m_left;
                Point2 _pIR = _pI.m_right;
                Vector2 _pIV = _pI.getValue();
                Vector2 _toLeft = _pIL.getValue() - _pIV;
                Vector2 _toRight = _pIR.getValue() - _pIV;
                if (_toLeft.x > 0 && _toRight.x > 0)      //Open Site,add double slope
                {
                    _sweepLineStep.OpenSite(_pI);
                }
                else if (_toLeft.x < 0 && _toRight.x < 0) //Close Site,remove double slopes
                {
                    _sweepLineStep.CloseSite(_pI, _toLeft, _toRight);
                }
                else //Turn Site,add single slope
                {
                    _sweepLineStep.TurnSite(_pI);
                }
            }

            m_rowPoints.Clear();
            m_sweepLine0.Clear();
            m_sweepLine1.Clear();
            return _result;

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
    }
}