using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalledMaze : RectangleMaze
{
    private bool[,] vertPasses;
    private bool[,] horPasses;

    public Vector2Int currentCell;
    public override Vector2Int CurrentCell
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

    public override void Click(Vector2 point, float wallWidth)
    {
        var localPoint = new Vector2(point.x * (Width + wallWidth), point.y * (Height + wallWidth));
        var mark = new Vector2(localPoint.x - Mathf.Floor(localPoint.x), localPoint.y - Mathf.Floor(localPoint.y));
        var position = new Vector2Int(Mathf.FloorToInt(localPoint.x), Mathf.FloorToInt(localPoint.y));

        if (mark.x < wallWidth && mark.y < wallWidth)
            OnWallClick();
        else if (mark.x >= wallWidth && mark.y >= wallWidth)
            OnRoomClick(position);
        else if (mark.x >= wallWidth && mark.y < wallWidth)
            OnTunnelClick(position + Vector2Int.down, position);
        else if (mark.x < wallWidth && mark.y >= wallWidth)
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

    private void OnRoomClick(Vector2Int point)
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

    private void PaveInit()
    {
        steps = new int[Width, Height];
        for (int i = 0; i < Width; i++)
            for (int n = 0; n < Height; n++)
                steps[i, n] = -1;

        stepsQueue.Clear();
        maxRange = 0;
    }

    private void PaveDirections()
    {
        while (stepsQueue.Count > 0)
        {
            var from = stepsQueue.Dequeue();
            foreach (var item in Shifts)
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

    private void PavePaint()
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

    public override void Clear()
    {
        Texture = new Texture2D(2 * Width + 1, 2 * Height + 1, TextureFormat.ARGB32, false)
        {
            filterMode = FilterMode.Point
        };

        for (int i = 0; i < 2 * Width + 1; i++)
            for (int n = 0; n < 2 * Height + 1; n++)
                Texture.SetPixel(i, n, Full);

        passes = new bool[Width, Height];
        for (int i = 0; i < Width; i++)
            for (int n = 0; n < Height; n++)
                passes[i, n] = false;

        vertPasses = new bool[Width, Height - 1];
        for (int i = 0; i < Width; i++)
            for (int n = 0; n < Height - 1; n++)
                vertPasses[i, n] = false;

        horPasses = new bool[Width - 1, Height];
        for (int i = 0; i < Width - 1; i++)
            for (int n = 0; n < Height; n++)
                horPasses[i, n] = false;

        CurrentCell = new Vector2Int(Random.Range(0, Width), Random.Range(0, Height));
    }

    public void SetPass(Vector2Int cell, bool pass)
    {
        if (passes[cell.x, cell.y] != pass)
        {
            passes[cell.x, cell.y] = pass;
            PaintCell(cell);
        }
    }

    public bool GetPass(Vector2Int cell)
    {
        return passes[cell.x, cell.y];
    }

    public void PaintTemp(Vector2Int cell, bool temp)
    {
        Texture.SetPixel(2  * cell.x + 1, 2  * cell.y + 1, temp ? Temprorary : GetPass(cell) ? Empty : Full);
    }

    protected void PaintCell(Vector2Int cell, Color color)
    {
        Texture.SetPixel(2  * cell.x + 1, 2  * cell.y + 1, color);
    }

    public void PaintCell(Vector2Int cell)
    {
        Texture.SetPixel(2 * cell.x + 1, 2 * cell.y + 1, GetPass(cell) ? Empty : Full);
    }

    public void PaintRig(Vector2Int cell)
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
    public void SetTunnel(Vector2Int cell, Vector2Int to, bool tunnel)
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

    public bool GetTunnel(Vector2Int cell, Vector2Int to)
    {
        if ((to.x - cell.x) == 0)
            return vertPasses[cell.x, Mathf.Min(cell.y, to.y)];

        if ((to.y - cell.y) == 0)
            return horPasses[Mathf.Min(cell.x, to.x), cell.y];

        return false;
    }
}
