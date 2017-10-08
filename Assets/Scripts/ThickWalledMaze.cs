using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThickWalledMaze
{
    private readonly int Width;
    private readonly int Height;

    private bool[,] bits;
    private byte[,] nears;

    private Cell CurrentCell = null;
    private Stack<Cell> MazeTrace = new Stack<Cell>();

    public bool[,] Maze
    {
        get
        {
            return bits;
        }
    }

    public bool[] LinearMaze
    {
        get
        {
            var outMaze = new bool[Width * Height];
            for (int i = 0; i < Width; i++)
                for (int n = 0; n < Height; n++)
                    outMaze[i + n * Width] = bits[i, n];
            return outMaze;
        }
    }

    public byte[] LinearMazeBytes
    {
        get
        {
            var outMaze = new byte[Width * Height];
            for (int i = 0; i < Width; i++)
                for (int n = 0; n < Height; n++)
                    outMaze[i + n * Width] = nears[i, n];
            return outMaze;
        }
    }

    public bool[] LinearMazeCurrentCell
    {
        get
        {
            var outMaze = new bool[Width * Height];
            for (int i = 0; i < Width; i++)
                for (int n = 0; n < Height; n++)
                    outMaze[i + n * Width] = (null != CurrentCell) && (CurrentCell.X == i) && (CurrentCell.Y == n);
            return outMaze;
        }
    }

    private Cell this[int x, int y]
    {
        get
        {
            if (InMaze(x, y))
                return new Cell(x, y, this);
            else
                return null;
        }
    }

    public ThickWalledMaze(int width, int height)
    {
        Width = width;
        Height = height;
        bits = new bool[Width, Height];
        nears = new byte[Width, Height];
        Clear();
    }

    public void Clear()
    {
        CurrentCell = this[Random.Range(0, Width), Random.Range(0, Height)];
        //CurrentCell = this[Width / 2, Height / 2];
        MazeTrace.Clear();
        for (int i = 0; i < Width; i++)
            for (int n = 0; n < Height; n++)
            {
                bits[i, n] = false;
                nears[i, n] = 0;
            }
    }

    public void Generate()
    {
        while (NextStep()) ;
    }

    public bool NextStep()
    {
        CurrentCell.Pass = true;
        var choices = CurrentCell.Neighbours(false, 1);
        if (choices.Count == 0)
        {
            if (MazeTrace.Count == 0)
                return false;
            else
                CurrentCell = MazeTrace.Pop();
        }
        else
        {
            var index = Random.Range(0, choices.Count);
            MazeTrace.Push(CurrentCell);
            CurrentCell = choices[index];
        }

        return true;
    }

    private bool InMaze(int x, int y)
    {
        return ((x < Width) && (x >= 0) && (y < Height) && (y >= 0));
    }

    public class Cell
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

        private readonly List<Shift> shifts = new List<Shift>()
        {
            new Shift(-1, 0),
            new Shift(1, 0),
            new Shift(0, -1),
            new Shift(0, 1),
        };

        private ThickWalledMaze Parent;
        public int X;
        public int Y;
        public byte Near
        {
            set
            {
                Parent.nears[X, Y] = value;
            }
            get
            {
                return Parent.nears[X, Y];
            }
        }

        public bool Pass
        {
            set
            {
                if (Parent.bits[X, Y] != value)
                {
                    foreach (var item in Neighbours())
                        if (value)
                            item.Near++;
                        else
                            item.Near--;
                    Parent.bits[X, Y] = value;
                }
            }
            get
            {
                return Parent.bits[X, Y];
            }
        }

        public Cell(int x, int y, ThickWalledMaze parent)
        {
            X = x;
            Y = y;
            Parent = parent;
        }

        public List<Cell> Neighbours()
        {
            var outList = new List<Cell>();
            foreach (var item in shifts)
                if (Parent.InMaze(X + item.Dx, Y + item.Dy))
                    outList.Add(new Cell(X + item.Dx, Y + item.Dy, Parent));
            return outList;
        }

        public List<Cell> Neighbours(bool mask)
        {
            var outList = new List<Cell>();
            foreach (var item in shifts)
                if (Parent.InMaze(X + item.Dx, Y + item.Dy) && (Parent.bits[X + item.Dx, Y + item.Dy] == mask))
                    outList.Add(new Cell(X + item.Dx, Y + item.Dy, Parent));
            return outList;
        }

        public List<Cell> Neighbours(bool mask, byte maxNear)
        {
            var outList = new List<Cell>();
            foreach (var item in shifts)
                if (Parent.InMaze(X + item.Dx, Y + item.Dy)
                    && (Parent.bits[X + item.Dx, Y + item.Dy] == mask)
                    && (Parent.nears[X + item.Dx, Y + item.Dy] <= maxNear))
                    outList.Add(new Cell(X + item.Dx, Y + item.Dy, Parent));
            return outList;
        }
    }
}