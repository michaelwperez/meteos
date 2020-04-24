using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point
{
    public int x;
    public int y;

    public Point(int _x, int _y)
    {
        x = _x;
        y = _y;
    }

    public Vector2 ToVector()
    {
        return new Vector2(x, y);
    }

    public bool Equals(Point p)
    {
        return x == p.x && y == p.y;
    }

    public static Point FromVector(Vector2 v)
    {
        return new Point((int)v.x, (int)v.y);
    }

    public static Point FromVector(Vector3 v)
    {
        return new Point((int)v.x, (int)v.y);
    }

    public static Point Mult(Point p, int mult)
    {
        return new Point(p.x * mult, p.y * mult);
    }

    public void Mult(int mult)
    {
        x = x * mult;
        y = y * mult;
    }

    public static Point Add(Point p, Point o)
    {
        return new Point(p.x + o.x, p.y + o.y);
    }

    public void Add(Point p)
    {
        x = x + p.x;
        y = y + p.y;
    }

    public static Point Clone(Point p)
    {
        return new Point(p.x, p.y);
    }

    public static Point Zero
    {
        get { return new Point(0, 0); }
    }

    public static Point One
    {
        get { return new Point(1, 1); }
    }

    public static Point Up
    {
        get { return new Point(0, 1); }
    }

    public static Point Down
    {
        get { return new Point(0, -1); }
    }

    public static Point Right
    {
        get { return new Point(1, 0); }
    }

    public static Point Left
    {
        get { return new Point(-1, 0); }
    }
}
