using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct IntRect
{
    public readonly Vector2Int from;
    public readonly Vector2Int to;
    public readonly Type type;

    public enum Type
    {
        VerticalLine, Vertical, Square, Horizontal, HorizontalLine, Undefined
    }

    public IntRect(Vector2Int from, Vector2Int to)
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
    public Vector2Int from;
    public Vector2Int to;
    public float ratio;

    public Edge(Vector2Int from, Vector2Int to, float ratio)
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

//public struct Vector2Int
//{
//    public int x, y;
//    public static readonly Vector2Int East = new Vector2Int(1, 0);
//    public static readonly Vector2Int West = new Vector2Int(-1, 0);
//    public static readonly Vector2Int North = new Vector2Int(0, 1);
//    public static readonly Vector2Int South = new Vector2Int(0, -1);


//    public Vector2Int(int x, int y)
//    {
//        this.x = x;
//        this.y = y;
//    }

//    public override string ToString()
//    {
//        return "(" + x + ", " + y + ")";
//    }

//    public static bool operator ==(Vector2Int a, Vector2Int b)
//    {
//        return (a.x == b.x) && (a.y == b.y);
//    }

//    public static bool operator !=(Vector2Int a, Vector2Int b)
//    {
//        return (a.x != b.x) || (a.y != b.y);
//    }

//    public static Vector2Int operator +(Vector2Int a, Vector2Int b)
//    {
//        return new Vector2Int(a.x + b.x, a.y + b.y);
//    }

//    public static Vector2Int operator +(Vector2Int a, Vector2 b)
//    {
//        return new Vector2Int(a.x + (int)b.x, a.y + (int)b.y);
//    }

//    public static Vector2Int operator -(Vector2Int a, Vector2Int b)
//    {
//        return new Vector2Int(a.x - b.x, a.y - b.y);
//    }

//    public static Vector2Int operator -(Vector2Int a, Vector2 b)
//    {
//        return new Vector2Int(a.x - (int)b.x, a.y - (int)b.y);
//    }

//    public static Vector2Int operator *(Vector2Int a, int b)
//    {
//        return new Vector2Int(a.x * b, a.y * b);
//    }

//    public override bool Equals(object obj)
//    {
//        if (!(obj is Vector2Int))
//        {
//            return false;
//        }

//        var vector = (Vector2Int)obj;
//        return x == vector.x &&
//               y == vector.y;
//    }

//    public override int GetHashCode()
//    {
//        var hashCode = 1502939027;
//        hashCode = hashCode * -1521134295 + base.GetHashCode();
//        hashCode = hashCode * -1521134295 + x.GetHashCode();
//        hashCode = hashCode * -1521134295 + y.GetHashCode();
//        return hashCode;
//    }
//}
