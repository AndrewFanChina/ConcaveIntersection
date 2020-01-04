using System;
using UnityEngine;
namespace SweepLine
{
    public class Point2 : IComparable<Point2>
    {
        public static LittlePool<Point2> Pool=new LittlePool<Point2>();
        public float x;
        public float y;
        public Point2 m_left;
        public Point2 m_right;

        public Point2()
        {
        }
        public Point2 SetValue(Vector2 _point)
        {
            x = _point.x;
            y = _point.y;
            return this;
        }
        public Vector2 getValue()
        {
            return new Vector2(x, y);
        }
        public int CompareTo(Point2 other)
        {
            if (x != other.x)
            {
                return x > other.x ? 1 : -1;
            }
            if (y != other.y)
            {
                return y > other.y ? 1 : -1;
            }
            return 0;
        }
        
        public override String ToString()
        {
            return "(" + x + "," + y + ")";
        }

        public static InOut InOutOf(Point2 _pointStart, Point2 _pointEnd)
        {
            if (_pointStart.m_right == _pointEnd)
            {
                return InOut.Out;
            }
            return InOut.In;
        }

    }
}