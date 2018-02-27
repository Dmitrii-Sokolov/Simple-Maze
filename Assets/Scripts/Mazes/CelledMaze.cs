using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelledMaze : IMaze
{
    public static Vector2Int[] Shifts = new Vector2Int[4]
    {
        Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down
    };

    public CelledMaze()
    {
        Width = 2;
        Height = 2;
    }

    private IMazeGenerator generator { set; get; }

    protected static Color Temprorary = Color.yellow;
    protected static Color Rig = Color.red;
    protected static Color Full = Color.black;
    protected static Color Empty = Color.blue;
    protected static Color MinRangeColor = Color.green;
    protected static Color MaxRangeColor = 0.1f * Color.green;

    public Vector2Int currentCell;
    public virtual Vector2Int CurrentCell
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

    public Texture2D Texture
    {
        protected set;
        get;
    }

    public Texture2D GetTexture()
    {
        Texture.Apply();
        return Texture;
    }

    public int Width { private set; get; }
    public int Height { private set; get; }
    public virtual int OutTextureWidth { get { return Width; } }
    public virtual int OutTextureHeight { get { return Height; } }

    protected bool[,] passes;
    protected int[,] steps;
    protected Queue<Vector2Int> stepsQueue = new Queue<Vector2Int>();
    protected int maxRange;

    public virtual void Click(Vector2 point)
    {
        var localPoint = new Vector2Int(Mathf.FloorToInt(point.x * Width), Mathf.FloorToInt(point.y * Height));
        OnRoomClick(localPoint);
    }

    protected virtual void OnRoomClick(Vector2Int point)
    {
        if (InMaze(point) && GetPass(point))
        {
            PaveInit();

            steps[point.x, point.y] = 0;
            stepsQueue.Enqueue(point);

            PaveDirections();
            PavePaint();
        }
    }

    protected virtual void PaveInit()
    {
        steps = new int[Width, Height];
        for (int i = 0; i < Width; i++)
            for (int n = 0; n < Height; n++)
                steps[i, n] = -1;

        stepsQueue.Clear();
        maxRange = 0;
    }

    protected virtual void PaveDirections()
    {
        while (stepsQueue.Count > 0)
        {
            var from = stepsQueue.Dequeue();
            foreach (var item in CelledMaze.Shifts)
            {
                var adj = from + item;
                if (InMaze(adj))
                    if (steps[adj.x, adj.y] == -1 || steps[adj.x, adj.y] > steps[from.x, from.y] + 1)
                        if (GetPass(adj))
                        {
                            steps[adj.x, adj.y] = steps[from.x, from.y] + 1;
                            maxRange = Mathf.Max(steps[adj.x, adj.y], maxRange);
                            stepsQueue.Enqueue(adj);
                        }
            }
        }
    }

    protected virtual void PavePaint()
    {
        for (int i = 0; i < Width; i++)
            for (int n = 0; n < Height; n++)
                if (steps[i, n] != -1)
                    PaintCell(new Vector2Int(i, n), Color.Lerp(MinRangeColor, MaxRangeColor, steps[i, n] / ((float)maxRange)));
    }

    public virtual void SetSize(Vector2Int size)
    {
        Width = size.x;
        Height = size.y;
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

        CurrentCell = new Vector2Int(Random.Range(0, Width), Random.Range(0, Height));
    }

    public bool InMaze(Vector2Int cell)
    {
        return ((cell.x < Width) && (cell.x >= 0) && (cell.y < Height) && (cell.y >= 0));
    }

    public virtual void SetPass(Vector2Int cell, bool pass)
    {
        if (passes[cell.x, cell.y] != pass)
        {
            passes[cell.x, cell.y] = pass;
            PaintCell(cell);
        }
    }

    public virtual void PaintTemp(Vector2Int cell, bool temp)
    {
        PaintCell(cell, temp ? Temprorary : GetPass(cell) ? Empty : Full);
    }

    protected virtual void PaintCell(Vector2Int cell, Color color)
    {
        Texture.SetPixel(cell.x, cell.y, color);
    }

    public virtual void PaintCell(Vector2Int cell)
    {
        Texture.SetPixel(cell.x, cell.y, GetPass(cell) ? Empty : Full);
    }

    public virtual void PaintRig(Vector2Int cell)
    {
        Texture.SetPixel(cell.x, cell.y, Rig);
    }

    public bool GetPass(Vector2Int cell)
    {
        return passes[cell.x, cell.y];
    }

    public virtual void SetTunnel(Vector2Int cell, Vector2Int to, bool tunnel)
    {
        throw new System.NotImplementedException();
    }

    public virtual bool GetTunnel(Vector2Int cell, Vector2Int to)
    {
        throw new System.NotImplementedException();
    }
}
