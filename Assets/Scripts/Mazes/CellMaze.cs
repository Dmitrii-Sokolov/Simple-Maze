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
}

public abstract class CellMaze
{
    protected static List<IntVector2> shifts = new List<IntVector2>()
    {
        IntVector2.East, IntVector2.North, IntVector2.West, IntVector2.South
    };

    public Texture2D Texture { protected set; get; }
    public abstract int OutTextureWidth { get; }
    public abstract int OutTextureHeight { get; }
    public int Width { private set; get; }
    public int Height { private set; get; }

    protected abstract IntVector2 CurrentCell { set; get; }

    protected IntVector2 currentCell;

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
                Texture.SetPixel(i, n, Color.black);

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

    protected bool GetPass(IntVector2 cell)
    {
        return passes[cell.x, cell.y];
    }
}
