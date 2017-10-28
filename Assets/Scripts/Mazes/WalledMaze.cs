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
    private Color[] RoomTemp = new Color[RoomSize * RoomSize];

    public WalledMaze() { }

    public WalledMaze(int width, int height)
    {
        SetSize(width, height);
    }

    public override void Click(Vector2 point)
    {
        var localPoint = new IntVector2(Mathf.FloorToInt(point.x * OutTextureWidth), Mathf.FloorToInt(point.y * OutTextureHeight));
        var mark = new IntVector2(localPoint.x % (RoomSize + WallSize), localPoint.y % (RoomSize + WallSize));
        var position = new IntVector2(localPoint.x / (RoomSize + WallSize), localPoint.y / (RoomSize + WallSize));

        if (mark.x < WallSize && mark.y < WallSize)
            OnWallClick();
        else if (mark.x >= WallSize && mark.y >= WallSize)
            OnRoomClick(position);
        else if (mark.x >= WallSize && mark.y < WallSize)
            OnTunnelClick(position + IntVector2.South, position);
        else if (mark.x < WallSize && mark.y >= WallSize)
            OnTunnelClick(position + IntVector2.West, position);
    }
    
    protected void OnTunnelClick(IntVector2 from, IntVector2 to)
    {
        if (InMaze(from) && InMaze(to) && GetTunnel(from, to))
        {
            PaveInit();

            steps[from.x, from.y] = 0;
            steps[to.x, to.y] = 0;
            stepsQueue.Enqueue(from);
            stepsQueue.Enqueue(to);

            PaveDirections();
            PavePaint();
        }
    }

    protected void OnWallClick()
    {
        //Debug.Log("Wall");
    }

    protected override void PaveDirections()
    {
        while (stepsQueue.Count > 0)
        {
            var from = stepsQueue.Dequeue();
            foreach (var item in shifts)
            {
                var adj = from + item;
                if (InMaze(adj))
                    if (steps[adj.x, adj.y] == -1 || steps[adj.x, adj.y] > steps[from.x, from.y] + 1)
                        if (GetTunnel(from, adj))
                            if (GetPass(adj))
                            {
                                steps[adj.x, adj.y] = steps[from.x, from.y] + 1;
                                maxRange = Mathf.Max(steps[adj.x, adj.y], maxRange);
                                stepsQueue.Enqueue(adj);
                            }
            }
        }
    }

    protected override void PavePaint()
    {
        for (int i = 0; i < Width; i++)
            for (int n = 0; n < Height; n++)
            {
                var current = new IntVector2(i, n);
                var up = current + IntVector2.North;
                var right = current + IntVector2.East;

                if (steps[i, n] != -1)
                    PaintCell(current, Color.Lerp(MinRangeColor, MaxRangeColor, steps[i, n] / ((float)maxRange)));

                if (InMaze(up))
                    if (steps[i, n + 1] != -1)
                        if (GetTunnel(current, up))
                            PaintTunnel(current, up, Color.Lerp(MinRangeColor, MaxRangeColor, 0.5f * (steps[i, n] + steps[i, n + 1]) / ((float)maxRange)));

                if (InMaze(right))
                    if (steps[i + 1, n] != -1)
                        if (GetTunnel(current, right))
                            PaintTunnel(current, right, Color.Lerp(MinRangeColor, MaxRangeColor, 0.5f * (steps[i, n] + steps[i + 1, n]) / ((float)maxRange)));
            }
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
            RoomTemp[i] = Temprorary;
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

    protected void PaintTemp(IntVector2 cell, bool temp)
    {
        Texture.SetPixels((WallSize + RoomSize) * cell.x + WallSize, (WallSize + RoomSize) * cell.y + WallSize, RoomSize, RoomSize, temp ? RoomTemp : GetPass(cell) ? RoomEmpty : RoomFull);
    }

    protected override void PaintCell(IntVector2 cell, Color color)
    {
        for (int i = 0; i < RoomSize; i++)
            for (int n = 0; n < RoomSize; n++)
                Texture.SetPixel((WallSize + RoomSize) * cell.x + WallSize + i, (WallSize + RoomSize) * cell.y + WallSize + n, color);
    }

    protected override void PaintCell(IntVector2 cell)
    {
        Texture.SetPixels((WallSize + RoomSize) * cell.x + WallSize, (WallSize + RoomSize) * cell.y + WallSize, RoomSize, RoomSize, GetPass(cell) ? RoomEmpty : RoomFull);
    }

    protected override void PaintRig(IntVector2 cell)
    {
        Texture.SetPixels((WallSize + RoomSize) * cell.x + WallSize, (WallSize + RoomSize) * cell.y + WallSize, RoomSize, RoomSize, RoomRig);
    }

    protected void PaintTunnel(IntVector2 cell, IntVector2 to, Color color)
    {
        if ((to.x - cell.x) == 0)
        {
            for (int i = 0; i < RoomSize; i++)
                for (int n = 0; n < WallSize; n++)
                    Texture.SetPixel((WallSize + RoomSize) * cell.x + WallSize + i, (WallSize + RoomSize) * (1 + Mathf.Min(cell.y, to.y)) + n, color);
            return;
        }

        if ((to.y - cell.y) == 0)
        {
            for (int i = 0; i < WallSize; i++) 
                for (int n = 0; n < RoomSize; n++)
                    Texture.SetPixel((WallSize + RoomSize) * (1 + Mathf.Min(cell.x, to.x)) + i, (WallSize + RoomSize) * cell.y + WallSize + n, color);
        }
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
