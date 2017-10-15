using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ThinWalledMaze : CellMaze
{
    protected override IntVector2 CurrentCell
    {
        set
        {
            if (InMaze(currentCell))
                Texture.SetPixel(2 * currentCell.x + 1, 2 * currentCell.y + 1, GetPass(currentCell) ? Color.blue : Color.black);

            currentCell = value;

            if (InMaze(currentCell))
                Texture.SetPixel(2 * currentCell.x + 1, 2 * currentCell.y + 1, Color.red);
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

    public abstract override bool NextStep();

    protected void SetPass(IntVector2 cell, bool pass)
    {
        if (InMaze(cell))
            if (passes[cell.x, cell.y] != pass)
            {
                passes[cell.x, cell.y] = pass;
                Texture.SetPixel(2 * cell.x + 1, 2 * cell.y + 1, pass ? Color.blue : Color.black);
            }
    }

    protected void SetTunnel(IntVector2 cell, IntVector2 to, bool tunnel)
    {
        if ((to.x - cell.x) == 0)
        {
            vertPasses[cell.x, Mathf.Min(cell.y, to.y)] = tunnel;
            Texture.SetPixel(2 * cell.x + 1, 2 * Mathf.Min(cell.y, to.y) + 2, tunnel ? Color.blue : Color.black);
            return;
        }

        if ((to.y - cell.y) == 0)
        {
            horPasses[Mathf.Min(cell.x, to.x), cell.y] = tunnel;
            Texture.SetPixel(2 * Mathf.Min(cell.x, to.x) + 2, 2 * cell.y + 1, tunnel ? Color.blue : Color.black);
        }
    }

    protected bool GetTunnel(IntVector2 cell, IntVector2 to)
    {
        if ((to.x - cell.x) == 0)
            return vertPasses[cell.x, Mathf.Min(cell.y, to.y)];

        if ((to.y - cell.y) == 0)
            return horPasses[Mathf.Min(cell.x, to.x), cell.y];

        return false;
    }
}
