using System;
using System.Runtime.CompilerServices;
using UnityEngine;
namespace SweepLine
{
	public class Point2 : IComparable<Point2>, IEquatable<Point2>
	{
		private static int IncID = 0;
		private int m_id;
		public float x;
		public float y;
		public Point2 m_left;
		public Point2 m_right;
		public Point2(float _x, float _y)
		{
			m_id = IncID++;
			x = _x;
			y = _y;
		}
		public Point2(Vector2 _point)
		{
			m_id = IncID++;
			x = _point.x;
			y = _point.y;
		}
		public void SetValue(Vector2 _point)
		{
			if(m_id == 0)
			{
				m_id = IncID++;
			}
			x = _point.x;
			y = _point.y;
		}
		public Vector2 getValue()
		{
			return new Vector2(x, y);
		}
		public int CompareTo(Point2 other)
		{
			if(x != other.x)
			{
				return x > other.x ? 1 : -1;
			}
			if(y != other.y)
			{
				return y > other.y ? 1 : -1;
			}
			return 0;
		}
		public static bool operator ==(Point2 left, Point2 right)
		{
			if (left is null || right is null)
			{
				return left is null && right is null;
			}
			return left.Equals(right);
		}

		public static bool operator !=(Point2 left, Point2 right)
		{
			return !(left == right);
		}
		public bool Equals(Point2 other)
		{
			if (CompareTo(other) != 0)
			{
				return false;
			}
			return m_id == other.m_id;
		}
		public override bool Equals(object obj)
		{
			return obj is Point2 other && Equals(other);
		}
		public override int GetHashCode()
		{
			//int hash = 13;
			//hash = (hash * 7) + X.GetHashCode();
			//hash = (hash * 7) + Y.GetHashCode();
			return RuntimeHelpers.GetHashCode(this);
		}
		public override String ToString()
		{
			return "(" + x + "," + y + ")";
		}


	}
}