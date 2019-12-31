
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
				_result = Reverse(m_inOut);
			}
			return _result;
		}
		public static InOut Reverse(InOut _inout)
		{
			_inout = _inout == InOut.Out ? InOut.In : InOut.Out;
			return _inout;
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
				if(_neighbor0.x > 0 && _neighbor1.x > 0)      //Open Site,add double slope
				{
					OpenSite(_pIV, _neighbor0, _neighbor1);
				}
				else if(_neighbor0.x < 0 && _neighbor1.x < 0) //Close Site,remove double slopes
				{
					TurnSite(_pIV, _neighbor0, _neighbor1);
				}
				else //Turn Site,add single slope
				{

				}
			}

			return false;

		}

		protected void OpenSite(Vector2 _sitePos, Vector2 _siteSlope1, Vector2 _siteSlope2)
		{
			if (_siteSlope1.y < _siteSlope2.y)
			{
				var _temp = _siteSlope2;
				_siteSlope2 = _siteSlope1;
				_siteSlope1 = _temp;
			}
			int _regionID;
			var _inOut =  InOutOf(_sitePos, out _regionID);
			SlopeRegion _region1 = new SlopeRegion();
			_region1.m_sitePos = _sitePos;
			_region1.m_slop = SlopeOf(_siteSlope1);
			_region1.m_inOut = _inOut;

			SlopeRegion _region2 = new SlopeRegion();
			_region2.m_sitePos = _sitePos;
			_region2.m_slop = SlopeOf(_siteSlope2);
			_region2.m_inOut = SlopeRegion.Reverse(_inOut);

			if (_regionID >= m_sweepLine.Count)
			{
				m_sweepLine.Add(_region1);
				m_sweepLine.Add(_region2);
			}
			else
			{
				m_sweepLine.Insert(_regionID, _region2);
				m_sweepLine.Insert(_regionID, _region1);
			}

		}

		protected void TurnSite(Vector2 _sitePos, Vector2 _siteSlope1, Vector2 _siteSlope2)
		{
			if(_siteSlope1.x > _siteSlope2.x)
			{
				var _temp = _siteSlope2;
				_siteSlope2 = _siteSlope1;
				_siteSlope1 = _temp;
			}

			Vector2 _sitePosLeft = _sitePos + _siteSlope1;
			_sitePosLeft.y += float.Epsilon;
			int _regionID;

			var _inOut = InOutOf(_sitePosLeft, out _regionID);
			SlopeRegion _region = new SlopeRegion();
			_region.m_sitePos = _sitePos;
			_region.m_slop = SlopeOf(_siteSlope2);
			_region.m_inOut = _inOut;

			if(_regionID >= m_sweepLine.Count)
			{
				m_sweepLine.Add(_region);
			}
			else
			{
				m_sweepLine.Insert(_regionID, _region);
			}
		}

		protected InOut InOutOf(Vector2 _pos, out int _regionID)
		{
			int _count = m_sweepLine.Count;
			if(_count == 0)
			{
				_regionID = 0;
				return InOut.Out;
			}

			float y = _pos.y;
			if(y < m_sweepLine[0].m_sitePos.y)
			{
				_regionID = 0;
				return InOut.Out;
			}
			if(y >= m_sweepLine[_count-1].m_sitePos.y)
			{
				_regionID = _count;
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
			_regionID = _id;
			return m_sweepLine[_id].InOutOf(_pos);
		}
		protected static float SlopeOf(Vector2 _vector2)
		{
			if(Mathf.Approximately(_vector2.x, 0))
			{
				return float.MaxValue;
			}
			return _vector2.y / _vector2.x;
		}


	}

}
