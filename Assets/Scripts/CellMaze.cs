using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CellMaze
{
    protected class Cell
    {
        private class Shift
        {
            public readonly int Dx;
            public readonly int Dy;
            public Shift(int dx, int dy)
            {
                Dx = dx;
                Dy = dy;
            }
        }

        private static List<Shift> shifts = new List<Shift>()
        {
            new Shift(-1, 0),
            new Shift(1, 0),
            new Shift(0, -1),
            new Shift(0, 1),
        };

        public readonly int X;
        public readonly int Y;

        public Cell(int x, int y)
        {
            X = x;
            Y = y;
        }

        public List<Cell> AdjQuad
        {
            get
            {
                var outList = new List<Cell>();
                foreach (var item in shifts)
                    outList.Add(new Cell(X + item.Dx, Y + item.Dy));
                return outList;
            }
        }
    }

    public Texture2D Texture { protected set; get; }
    public abstract int OutTextureWidth { get; }
    public abstract int OutTextureHeight { get; }
    public int Width { private set; get; }
    public int Height { private set; get; }

    protected abstract Cell CurrentCell { set; get; }

    protected Cell currentCell = null;
    protected Stack<Cell> MazeTrace = new Stack<Cell>();

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

        MazeTrace.Clear();
        CurrentCell = new Cell(Random.Range(0, Width), Random.Range(0, Height));
    }

    public void Generate()
    {
        while (NextStep());
    }

    public abstract bool NextStep();

    protected bool InMaze(Cell cell)
    {
        return ((cell.X < Width) && (cell.X >= 0) && (cell.Y < Height) && (cell.Y >= 0));
    }

    protected bool GetPass(Cell cell)
    {
        return passes[cell.X, cell.Y];
    }
}
