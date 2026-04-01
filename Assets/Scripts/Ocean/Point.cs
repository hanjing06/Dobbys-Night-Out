using System;

[Serializable]
public struct Point
{
    public int x;
    public int y;

    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static Point up => new Point(0, 1);
    public static Point down => new Point(0, -1);
    public static Point left => new Point(-1, 0);
    public static Point right => new Point(1, 0);

    public static Point add(Point a, Point b)
    {
        return new Point(a.x + b.x, a.y + b.y);
    }

    public static Point mult(Point p, int value)
    {
        return new Point(p.x * value, p.y * value);
    }

    public override bool Equals(object obj)
    {
        if (!(obj is Point)) return false;
        Point other = (Point)obj;
        return x == other.x && y == other.y;
    }

    public override int GetHashCode()
    {
        return x * 31 + y;
    }

    public static bool operator ==(Point a, Point b)
    {
        return a.x == b.x && a.y == b.y;
    }

    public static bool operator !=(Point a, Point b)
    {
        return !(a == b);
    }
}