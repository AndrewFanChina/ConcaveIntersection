using System;
using System.Collections.Generic;
using UnityEngine;
namespace SweepLine
{
    public struct Point2:IComparable<Point2>
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
            if (m_id == 0)
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

        public override String ToString()
        {
	        return "(" + X + "," + Y + ")";
        }
        public int CompareTo(Point2 other)
        {
	        if (X != other.X)
	        {
		        return X > other.X ? 1 : -1;
			}
	        if(Y != other.Y)
	        {
		        return Y > other.Y ? 1 : -1;
	        }
	        return 0;
        }
		public struct Comparer : IEqualityComparer<Point2>
        {
            bool IEqualityComparer<Point2>.Equals(Point2 x, Point2 y)
            {
                return x.Equals(y);
            }

            int IEqualityComparer<Point2>.GetHashCode(Point2 obj)
            {
                return obj.GetHashCode();
            }
        }


    }
}