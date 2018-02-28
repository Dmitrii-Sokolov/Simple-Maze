using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalledMaze : CelledMaze
{
    public override int OutTextureWidth { get { return texWidth; } }
    public override int OutTextureHeight { get { return texHeight; } }

    private bool[,] vertPasses;
    private bool[,] horPasses;

    private int texWidth;
    private int texHeight;

    private float wallWidth = 0f;
    public override float WallWidth
    {
        set
        {
            wallWidth = value;
            if (null != mazeMaterial)
                mazeMaterial.SetFloat("_WallWidth", Mathf.Clamp01(value));
        }
        get
        {
            return wallWidth;
        }
    }

    private Material mazeMaterial;
    public override Material Material
    {
        set
        {
            mazeMaterial = value;
        }
    }

    public override void Click(Vector2 point)
    {
        var localPoint = new Vector2(point.x * (Width + WallWidth), point.y * (Height + WallWidth));
        var mark = new Vector2(localPoint.x - Mathf.Floor(localPoint.x), localPoint.y - Mathf.Floor(localPoint.y));
        var position = new Vector2Int(Mathf.FloorToInt(localPoint.x), Mathf.FloorToInt(localPoint.y));

        if (mark.x < WallWidth && mark.y < WallWidth)
            OnWallClick();
        else if (mark.x >= WallWidth && mark.y >= WallWidth)
            OnRoomClick(position);
        else if (mark.x >= WallWidth && mark.y < WallWidth)
            OnTunnelClick(position + Vector2Int.down, position);
        else if (mark.x < WallWidth && mark.y >= WallWidth)
            OnTunnelClick(position + Vector2Int.left, position);
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
        texWidth = 2 * size.x + 1;
        texHeight = 2 * size.y + 1;
        base.SetSize(size);
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

    public override void PaintTemp(Vector2Int cell, bool temp)
    {
        Texture.SetPixel(2  * cell.x + 1, 2  * cell.y + 1, temp ? Temprorary : GetPass(cell) ? Empty : Full);
    }

    protected override void PaintCell(Vector2Int cell, Color color)
    {
        Texture.SetPixel(2  * cell.x + 1, 2  * cell.y + 1, color);
    }

    public override void PaintCell(Vector2Int cell)
    {
        Texture.SetPixel(2 * cell.x + 1, 2 * cell.y + 1, GetPass(cell) ? Empty : Full);
    }

    public override void PaintRig(Vector2Int cell)
    {
        Texture.SetPixel(2  * cell.x + 1, 2  * cell.y + 1, Rig);
    }

    protected void PaintTunnel(Vector2Int cell, Vector2Int to, Color color)
    {
        if ((to.x - cell.x) == 0)
        {
            Texture.SetPixel(2 * cell.x + 1, 2  * (1 + Mathf.Min(cell.y, to.y)), color);
            return;
        }

        if ((to.y - cell.y) == 0)
        {
            Texture.SetPixel(2  * (1 + Mathf.Min(cell.x, to.x)), 2  * cell.y + 1, color);
        }
    }
    public override void SetTunnel(Vector2Int cell, Vector2Int to, bool tunnel)
    {
        if ((to.x - cell.x) == 0)
        {
            vertPasses[cell.x, Mathf.Min(cell.y, to.y)] = tunnel;
            Texture.SetPixel(2  * cell.x + 1, 2  * (1 + Mathf.Min(cell.y, to.y)), tunnel ? Empty : Full);
            return;
        }

        if ((to.y - cell.y) == 0)
        {
            horPasses[Mathf.Min(cell.x, to.x), cell.y] = tunnel;
            Texture.SetPixel(2  * (1 + Mathf.Min(cell.x, to.x)), 2  * cell.y + 1, tunnel ? Empty : Full);
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
