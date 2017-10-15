using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WalledMaze : CellMaze
{
    private const int RoomSize = 2;
    private const int WallSize = 1;

    public override int OutTextureWidth { get { return texWidth; } }
    public override int OutTextureHeight { get { return texHeight; } }

    private bool[,] vertPasses;
    private bool[,] horPasses;

    private int texWidth;
    private int texHeight;

    private Color[] WallFull = new Color[WallSize * RoomSize];
    private Color[] WallEmpty = new Color[WallSize * RoomSize];
    private Color[] RoomRig = new Color[RoomSize * RoomSize];
    private Color[] RoomFull = new Color[RoomSize * RoomSize];
    private Color[] RoomEmpty = new Color[RoomSize * RoomSize];

    public WalledMaze() { }

    public WalledMaze(int width, int height)
    {
        SetSize(width, height);
    }

    public override void SetSize(int width, int height)
    {
        texHeight = (WallSize + RoomSize) * height + WallSize;
        texWidth = (WallSize + RoomSize) * width + WallSize;
        base.SetSize(width, height);
    }

    public override void Clear()
    {
        for (int i = 0; i < RoomSize * RoomSize; i++)
        {
            RoomRig[i] = Rig;
            RoomFull[i] = Full;
            RoomEmpty[i] = Empty;
        }

        for (int i = 0; i < WallSize * RoomSize; i++)
        {
            WallFull[i] = Full;
            WallEmpty[i] = Empty;
        }

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
        Texture.SetPixels((WallSize + RoomSize) * cell.x + WallSize, (WallSize + RoomSize) * cell.y + WallSize, RoomSize, RoomSize, GetPass(cell) ? RoomEmpty : RoomFull);
    }

    protected override void PaintRig(IntVector2 cell)
    {
        Texture.SetPixels((WallSize + RoomSize) * cell.x + WallSize, (WallSize + RoomSize) * cell.y + WallSize, RoomSize, RoomSize, RoomRig);
    }

    protected void SetTunnel(IntVector2 cell, IntVector2 to, bool tunnel)
    {
        if ((to.x - cell.x) == 0)
        {
            vertPasses[cell.x, Mathf.Min(cell.y, to.y)] = tunnel;
            Texture.SetPixels((WallSize + RoomSize) * cell.x + WallSize, (WallSize + RoomSize) * (1 + Mathf.Min(cell.y, to.y)), RoomSize, WallSize, tunnel ? WallEmpty : WallFull);
            return;
        }

        if ((to.y - cell.y) == 0)
        {
            horPasses[Mathf.Min(cell.x, to.x), cell.y] = tunnel;
            Texture.SetPixels((WallSize + RoomSize) * (1 + Mathf.Min(cell.x, to.x)), (WallSize + RoomSize) * cell.y + WallSize, WallSize, RoomSize, tunnel ? WallEmpty : WallFull);
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
