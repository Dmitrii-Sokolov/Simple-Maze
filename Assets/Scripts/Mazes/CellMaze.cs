using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    protected int[,] steps;

    public virtual void Click(Vector2 point)
    {
        var localPoint = new IntVector2(Mathf.FloorToInt(point.x * Width), Mathf.FloorToInt(point.y * Height));
        OnRoomClick(localPoint);
    }

    protected virtual void OnRoomClick(IntVector2 point)
    {
        //Debug.Log("Room: " + point);
        steps = new int[Width, Height];
        for (int i = 0; i < Width; i++)
            for (int n = 0; n < Height; n++)
                steps[i, n] = -1;

        PaveDirections(point, 0);
    }

    protected virtual void PaveDirections(IntVector2 from, int range)
    {
        if (GetPass(from))
        {
            steps[from.x, from.y] = range;
            Texture.SetPixel(from.x, from.y, Color.Lerp(Empty, Full, range / (0.5f * Height * Width)));

            foreach (var item in shifts)
            {
                var adj = from + item;
                if (InMaze(adj))
                    if (steps[adj.x, adj.y] == -1)
                        PaveDirections(adj, range + 1);
            }
        }
    }


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
