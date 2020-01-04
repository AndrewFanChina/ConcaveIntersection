using System;

namespace SweepLine
{
	public enum InOut
	{
		In,
		Out,
	}

	public class SlopeRegion : IComparable<SlopeRegion>
	{
		public static LittlePool<SlopeRegion> Pool = new LittlePool<SlopeRegion>();
		public Point2 m_start;
		public Point2 m_end;
		public float m_slop;
		public float m_swY;
		public InOut m_inOut;

		public SlopeRegion()
		{
		}
		public SlopeRegion SetValue(Point2 _start, Point2 _end)
		{
			m_start = _start;
			m_end = _end;
			m_inOut = Point2.InOutOf(_start, _end);
			return this;
		}

		public void SweepTo(float x)
		{
			m_swY = m_start.y + m_slop * (x - m_start.x);
		}

		public bool Contains(Point2 pos)
		{
			//Debug.Assert(pos.x >= m_start.x);
			float _slopY = m_slop * (pos.x - m_start.x);
			float _relativeY = pos.y - m_start.y;
			return _relativeY <= _slopY;
		}

		public int CompareTo(SlopeRegion other)
		{
			if(ReferenceEquals(this, other)) return 0;
			if(ReferenceEquals(null, other)) return 1;
			if(m_swY != other.m_swY)
			{
				return m_swY > other.m_swY ? -1 : 1;
			}
			return 0;
		}


	}

}
