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