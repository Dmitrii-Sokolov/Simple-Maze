using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct IntRect
{
    public readonly IntVector2 from;
    public readonly IntVector2 to;
    public readonly Type type;

    public enum Type
    {
        VerticalLine, Vertical, Square, Horizontal, HorizontalLine, Undefined
    }

    public IntRect(IntVector2 from, IntVector2 to)
    {
        this.from = from;
        this.to = to;

        if (from.x == to.x)
            type = Type.VerticalLine;
        else if (from.y == to.y)
            type = Type.HorizontalLine;
        else
        {
            var aspect = (to.y - from.y) - (to.x - from.x);
            if (aspect > 0)
                type = Type.Vertical;
            else if (aspect < 0)
                type = Type.Horizontal;
            else if (aspect == 0)
                type = Type.Square;
            else
                type = Type.Undefined;
        }
    }

    public int Height
    {
        get
        {
            return to.y - from.y;
        }
    }

    public int Width
    {
        get
        {
            return to.x - from.x;
        }
    }
}

public struct Edge : System.IComparable<Edge>
{
    public IntVector2 from;
    public IntVector2 to;
    public float ratio;

    public Edge(IntVector2 from, IntVector2 to, float ratio)
    {
        this.from = from;
        this.to = to;
        this.ratio = ratio;
    }

    public int CompareTo(Edge obj)
    {
        if (ratio > obj.ratio)
            return 1;
        if (ratio < obj.ratio)
            return -1;
        else
            return 0;
    }
}

public struct IntVector2
{
    public int x, y;
    public static readonly IntVector2 East = new IntVector2(1, 0);
    public static readonly IntVector2 West = new IntVector2(-1, 0);
    public static readonly IntVector2 North = new IntVector2(0, 1);
    public static readonly IntVector2 South = new IntVector2(0, -1);

    public IntVector2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override string ToString()
    {
        return "(" + x + ", " + y + ")";
    }

    public static bool operator ==(IntVector2 a, IntVector2 b)
    {
        return (a.x == b.x) && (a.y == b.y);
    }

    public static bool operator !=(IntVector2 a, IntVector2 b)
    {
        return (a.x != b.x) || (a.y != b.y);
    }

    public static IntVector2 operator +(IntVector2 a, IntVector2 b)
    {
        return new IntVector2(a.x + b.x, a.y + b.y);
    }

    public static IntVector2 operator +(IntVector2 a, Vector2 b)
    {
        return new IntVector2(a.x + (int)b.x, a.y + (int)b.y);
    }

    public static IntVector2 operator -(IntVector2 a, IntVector2 b)
    {
        return new IntVector2(a.x - b.x, a.y - b.y);
    }

    public static IntVector2 operator -(IntVector2 a, Vector2 b)
    {
        return new IntVector2(a.x - (int)b.x, a.y - (int)b.y);
    }

    public static IntVector2 operator *(IntVector2 a, int b)
    {
        return new IntVector2(a.x * b, a.y * b);
    }

    public override bool Equals(object obj)
    {
        if (!(obj is IntVector2))
        {
            return false;
        }

        var vector = (IntVector2)obj;
        return x == vector.x &&
               y == vector.y;
    }

    public override int GetHashCode()
    {
        var hashCode = 1502939027;
        hashCode = hashCode * -1521134295 + base.GetHashCode();
        hashCode = hashCode * -1521134295 + x.GetHashCode();
        hashCode = hashCode * -1521134295 + y.GetHashCode();
        return hashCode;
    }
}
