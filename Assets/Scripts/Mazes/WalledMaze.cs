using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WalledMaze : CellMaze
{
    public override int OutTextureWidth { get { return texWidth; } }
    public override int OutTextureHeight { get { return texHeight; } }

    private bool[,] vertPasses;
    private bool[,] horPasses;

    private int texWidth;
    private int texHeight;

    public WalledMaze() { }

    public WalledMaze(int width, int height)
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

    protected override void PaintCell(IntVector2 cell)
    {
        Texture.SetPixel(2 * cell.x + 1, 2 * cell.y + 1, GetPass(cell) ? Empty : Full);
    }

    protected override void PaintRig(IntVector2 cell)
    {
        Texture.SetPixel(2 * cell.x + 1, 2 * cell.y + 1, Rig);
    }

    protected void SetTunnel(IntVector2 cell, IntVector2 to, bool tunnel)
    {
        if ((to.x - cell.x) == 0)
        {
            vertPasses[cell.x, Mathf.Min(cell.y, to.y)] = tunnel;
            Texture.SetPixel(2 * cell.x + 1, 2 * Mathf.Min(cell.y, to.y) + 2, tunnel ? Empty : Full);
            return;
        }

        if ((to.y - cell.y) == 0)
        {
            horPasses[Mathf.Min(cell.x, to.x), cell.y] = tunnel;
            Texture.SetPixel(2 * Mathf.Min(cell.x, to.x) + 2, 2 * cell.y + 1, tunnel ? Empty : Full);
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
