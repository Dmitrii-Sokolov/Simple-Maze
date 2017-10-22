using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public abstract class CellMaze
{
    protected static List<IntVector2> shifts = new List<IntVector2>()
    {
        IntVector2.East, IntVector2.North, IntVector2.West, IntVector2.South
    };

    protected static Color Temprorary = Color.yellow;
    protected static Color Rig = Color.red;
    protected static Color Full = Color.black;
    protected static Color Empty = Color.blue;

    protected IntVector2 currentCell;
    protected virtual IntVector2 CurrentCell
    {
        set
        {
            if (InMaze(currentCell))
                PaintCell(currentCell);

            currentCell = value;

            if (InMaze(currentCell))
                PaintRig(currentCell);
        }
        get
        {
            return currentCell;
        }
    }

    public Texture2D Texture { protected set; get; }
    public int Width { private set; get; }
    public int Height { private set; get; }
    public virtual int OutTextureWidth { get { return Width; } }
    public virtual int OutTextureHeight { get { return Height; } }

    protected bool[,] passes;

    public virtual void SetSize(int width, int height)
    {
        Width = width;
        Height = height;
        Clear();
    }

    public virtual void Clear()
    {
        Texture = new Texture2D(OutTextureWidth, OutTextureHeight, TextureFormat.ARGB32, false);
        Texture.filterMode = FilterMode.Point;
        for (int i = 0; i < OutTextureWidth; i++)
            for (int n = 0; n < OutTextureHeight; n++)
                Texture.SetPixel(i, n, Full);

        passes = new bool[Width, Height];
        for (int i = 0; i < Width; i++)
            for (int n = 0; n < Height; n++)
                passes[i, n] = false;

        CurrentCell = new IntVector2(Random.Range(0, Width), Random.Range(0, Height));
    }

    public void Generate()
    {
        while (NextStep());
    }

    public abstract bool NextStep();

    protected bool InMaze(IntVector2 cell)
    {
        return ((cell.x < Width) && (cell.x >= 0) && (cell.y < Height) && (cell.y >= 0));
    }

    protected virtual void SetPass(IntVector2 cell, bool pass)
    {
        if (passes[cell.x, cell.y] != pass)
        {
            passes[cell.x, cell.y] = pass;
            PaintCell(cell);
        }
    }

    protected virtual void PaintCell(IntVector2 cell)
    {
        Texture.SetPixel(cell.x, cell.y, GetPass(cell) ? Empty : Full);
    }

    protected virtual void PaintRig(IntVector2 cell)
    {
        Texture.SetPixel(cell.x, cell.y, Rig);
    }

    protected bool GetPass(IntVector2 cell)
    {
        return passes[cell.x, cell.y];
    }
}
