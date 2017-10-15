using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThinWalledMaze : CellMaze
{
    protected override Cell CurrentCell
    {
        set
        {
            if (null != currentCell)
                if (InMaze(currentCell))
                    Texture.SetPixel(2 * currentCell.X + 1, 2 * currentCell.Y + 1, GetPass(currentCell) ? Color.blue : Color.black);

            currentCell = value;

            if (null != currentCell)
                if (InMaze(currentCell))
                    Texture.SetPixel(2 * currentCell.X + 1, 2 * currentCell.Y + 1, Color.red);
        }
        get
        {
            return currentCell;
        }
    }

    public override int OutTextureWidth { get { return texWidth; } }
    public override int OutTextureHeight { get { return texHeight; } }

    private bool[,] vertPasses;
    private bool[,] horPasses;

    private int texWidth;
    private int texHeight;

    public ThinWalledMaze() { }

    public ThinWalledMaze(int width, int height)
    {
        SetSize(width, height);
    }

    public override void SetSize(int width, int height)
    {
        texHeight = 2 * height + 1;
        texWidth = 2 * width + 1;
        base.SetSize(width, height);
    }

    public override void Clear()
    {
        base.Clear();

        vertPasses = new bool[Width, Height - 1];
        for (int i = 0; i < Width; i++)
            for (int n = 0; n < Height - 1; n++)
                vertPasses[i, n] = false;

        horPasses = new bool[Width - 1, Height];
        for (int i = 0; i < Width - 1; i++)
            for (int n = 0; n < Height; n++)
                horPasses[i, n] = false;
    }

    public override bool NextStep()
    {
        SetPass(CurrentCell, true);
        var choices = new List<Cell>();

        foreach (var item in CurrentCell.AdjQuad)
            if (InMaze(item))
                if (!GetPass(item))
                    choices.Add(item);

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
            SetTunnel(CurrentCell, choices[index], true);
            CurrentCell = choices[index];
        }

        return true;
    }

    private void SetPass(Cell cell, bool pass)
    {
        if (InMaze(cell))
            if (passes[cell.X, cell.Y] != pass)
            {
                passes[cell.X, cell.Y] = pass;
                Texture.SetPixel(2 * cell.X + 1, 2 * cell.Y + 1, pass ? Color.blue : Color.black);
            }
    }

    private void SetTunnel(Cell cell, Cell to, bool tunnel)
    {
        if ((to.X - cell.X) == 0)
        {
            vertPasses[cell.X, Mathf.Min(cell.Y, to.Y)] = tunnel;
            Texture.SetPixel(2 * cell.X + 1, 2 * Mathf.Min(cell.Y, to.Y) + 2, tunnel ? Color.blue : Color.black);
            return;
        }

        if ((to.Y - cell.Y) == 0)
        {
            horPasses[Mathf.Min(cell.X, to.X), cell.Y] = tunnel;
            Texture.SetPixel(2 * Mathf.Min(cell.X, to.X) + 2, 2 * cell.Y + 1, tunnel ? Color.blue : Color.black);
        }
    }

    //private bool GetTunnel(Cell cell, Cell to)
    //{
    //    if ((to.X - cell.X) == 0)
    //        return vertPasses[cell.X, Mathf.Min(cell.Y, to.Y)];

    //    if ((to.Y - cell.Y) == 0)
    //        return horPasses[Mathf.Min(cell.X, to.X), cell.Y];

    //    return false;
    //}
}
