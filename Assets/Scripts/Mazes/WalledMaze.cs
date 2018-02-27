using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalledMaze : CelledMaze
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

    public override void Click(Vector2 point)
    {
        Debug.LogError("Sorry! Need to replace some code.");
        //var localPoint = new Vector2Int(Mathf.FloorToInt(point.x * OutTextureWidth), Mathf.FloorToInt(point.y * OutTextureHeight));
        //var mark = new Vector2Int(localPoint.x % (RoomSize + WallSize), localPoint.y % (RoomSize + WallSize));
        //var position = new Vector2Int(localPoint.x / (RoomSize + WallSize), localPoint.y / (RoomSize + WallSize));

        //if (mark.x < WallSize && mark.y < WallSize)
        //    OnWallClick();
        //else if (mark.x >= WallSize && mark.y >= WallSize)
        //    OnRoomClick(position);
        //else if (mark.x >= WallSize && mark.y < WallSize)
        //    OnTunnelClick(position + Vector2Int.down, position);
        //else if (mark.x < WallSize && mark.y >= WallSize)
        //    OnTunnelClick(position + Vector2Int.left, position);
    }
    
    protected void OnTunnelClick(Vector2Int from, Vector2Int to)
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
            foreach (var item in CelledMaze.Shifts)
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
                var current = new Vector2Int(i, n);
                var up = current + Vector2Int.up;
                var right = current + Vector2Int.right;

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

    public override void SetSize(Vector2Int size)
    {
        texWidth = (WallSize + RoomSize) * size.x + WallSize;
        texHeight = (WallSize + RoomSize) * size.y + WallSize;
        base.SetSize(size);
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

    public override void PaintTemp(Vector2Int cell, bool temp)
    {
        Texture.SetPixels((WallSize + RoomSize) * cell.x + WallSize, (WallSize + RoomSize) * cell.y + WallSize, RoomSize, RoomSize, temp ? RoomTemp : GetPass(cell) ? RoomEmpty : RoomFull);
    }

    protected override void PaintCell(Vector2Int cell, Color color)
    {
        for (int i = 0; i < RoomSize; i++)
            for (int n = 0; n < RoomSize; n++)
                Texture.SetPixel((WallSize + RoomSize) * cell.x + WallSize + i, (WallSize + RoomSize) * cell.y + WallSize + n, color);
    }

    public override void PaintCell(Vector2Int cell)
    {
        Texture.SetPixels((WallSize + RoomSize) * cell.x + WallSize, (WallSize + RoomSize) * cell.y + WallSize, RoomSize, RoomSize, GetPass(cell) ? RoomEmpty : RoomFull);
    }

    public override void PaintRig(Vector2Int cell)
    {
        Texture.SetPixels((WallSize + RoomSize) * cell.x + WallSize, (WallSize + RoomSize) * cell.y + WallSize, RoomSize, RoomSize, RoomRig);
    }

    protected void PaintTunnel(Vector2Int cell, Vector2Int to, Color color)
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
    public override void SetTunnel(Vector2Int cell, Vector2Int to, bool tunnel)
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

    public override bool GetTunnel(Vector2Int cell, Vector2Int to)
    {
        if ((to.x - cell.x) == 0)
            return vertPasses[cell.x, Mathf.Min(cell.y, to.y)];

        if ((to.y - cell.y) == 0)
            return horPasses[Mathf.Min(cell.x, to.x), cell.y];

        return false;
    }
}
