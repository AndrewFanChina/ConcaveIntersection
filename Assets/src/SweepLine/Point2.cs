using System;
using System.Collections.Generic;
using UnityEngine;
namespace SweepLine
{
	public struct Point2 : IComparable<Point2>, IEquatable<Point2>
	{
		private static int IncID = 0;
		private int m_id;
		public float X;
		public float Y;
		public Point2(float _x, float _y)
		{
			m_id = IncID++;
			X = _x;
			Y = _y;
		}
		public Point2(Vector2 _point)
		{
			m_id = IncID++;
			X = _point.x;
			Y = _point.y;
		}
		public void SetValue(Vector2 _point)
		{
			if(m_id == 0)
			{
				m_id = IncID++;
			}
			X = _point.x;
			Y = _point.y;
		}
		public Vector2 getValue()
		{
			return new Vector2(X, Y);
		}
		public int CompareTo(Point2 other)
		{
			if(X != other.X)
			{
				return X > other.X ? 1 : -1;
			}
			if(Y != other.Y)
			{
				return Y > other.Y ? 1 : -1;
			}
			return 0;
		}
		public static bool operator ==(Point2 left, Point2 right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Point2 left, Point2 right)
		{
			return !(left == right);
		}
		public bool Equals(Point2 other)
		{
			return CompareTo(other) == 0;
		}
		public override bool Equals(object obj)
		{
			return obj is Point2 other && Equals(other);
		}
		public override int GetHashCode()
		{
			int hash = 13;
			hash = (hash * 7) + X.GetHashCode();
			hash = (hash * 7) + Y.GetHashCode();
			return hash;
		}
		public override String ToString()
		{
			return "(" + X + "," + Y + ")";
		}


	}
}