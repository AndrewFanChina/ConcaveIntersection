
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


	public struct SlopeRegion
	{
		public Vector2 m_sitePos;
		public float m_slop;
		public InOut m_inOut;

		public InOut InOutOf(Vector2 pos)
		{
			Debug.Assert(pos.x>= m_sitePos.x);
			float _y = m_slop * pos.x;
			InOut _result;
			if (pos.y >= _y)
			{
				_result = m_inOut;
			}
			else
			{
				_result = m_inOut== InOut.Out? InOut.In: InOut.Out;
			}
			return _result;

		}
	}

	public class SweepLine
	{
		protected static List<Point2> BufferedPoints = new List<Point2>();

		protected List<SlopeRegion> m_sweepLine=new List<SlopeRegion>();
		public bool ContainsPoint(IList<Point2> _polygon,Vector2 _point)
		{
			//Add polygon points
			BufferedPoints.Clear();
			int len = _polygon.Count;
			for(int i = 0; i < len; i++)
			{
				BufferedPoints.Add(_polygon[i]);
			}
			//Add the target point
			Point2 _pointTarget = new Point2(_point);
			BufferedPoints.Add(_pointTarget);
			//sort from left to right
			BufferedPoints.Sort();
			string _polygonInfor = "-----sorted points-----\n";
			foreach(var item in BufferedPoints)
			{
				_polygonInfor += item.ToString() + ",";
			}
			Debug.Log(_polygonInfor);
			//sweep from left to right
			int _bufLen = BufferedPoints.Count;
			for(int i = 0; i < _bufLen; i++)
			{
				Point2 _pI = BufferedPoints[i];
				Point2 _pIL = BufferedPoints[(i + _bufLen - 1) % _bufLen];
				Point2 _pIR = BufferedPoints[(i + 1) % _bufLen];
				Vector2 _pIV = _pI.getValue();
				Vector2 _neighbor0 = _pIL.getValue() - _pIV;
				Vector2 _neighbor1 = _pIR.getValue() - _pIV;
				if(_neighbor0.x > 0 && _neighbor1.x > 0)      //Open point,add double slope
				{
					var _inOut = InOutOf(_pIV);
					SlopeRegion _region =new SlopeRegion();
					_region.m_y = _pIV.y;
					_region.m_slop = SlopeOf(_neighbor0);


				}
				else if(_neighbor0.x < 0 && _neighbor1.x < 0) //Close point,remove double slopes
				{

				}
				else //Turn point,add single slope
				{

				}
			}

			return false;

		}

		private InOut InOutOf(Vector2 _pos)
		{
			int _count = m_sweepLine.Count;
			if(_count == 0)
			{
				return InOut.Out;
			}

			float y = _pos.y;
			if(y < m_sweepLine[0].m_sitePos.y)
			{
				return InOut.Out;
			}
			if(y >= m_sweepLine[_count-1].m_sitePos.y)
			{
				return InOut.Out;
			}

			int _left = 0;
			int _right = _count;
			int _center = (_left + _right) >>1;
			int _id = _left;
			while (_left < _center)
			{
				if(y <= m_sweepLine[_center].m_sitePos.y)
				{
					_id = _left;
					_right = _center;
				}
				else
				{
					_id = _right;
					_left = _center;
				}
				_center = (_left + _right) >>1;
				if (_center==_left|| _center == _right)
				{
					break;
				}
			}

			return m_sweepLine[_id].InOutOf(_pos);
		}

		protected static float SlopeOf(Vector2 _vector2)
		{
			if (Mathf.Approximately(_vector2.x, 0))
			{
				return float.MaxValue;
			}
			return _vector2.y / _vector2.x;
		}

		public void AddPoints(float y, Vector2 _neighbor0, Vector2 _neighbor1)
		{

		}
	}

}
