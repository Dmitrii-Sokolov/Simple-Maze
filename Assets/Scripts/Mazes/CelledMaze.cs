using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelledMaze : RectangleMaze
{
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

    public override void Clear()
    {
        Texture = new Texture2D(Width, Height, TextureFormat.ARGB32, false)
        {
            filterMode = FilterMode.Point
        };

        Texture.filterMode = FilterMode.Point;
        for (int i = 0; i < Width; i++)
            for (int n = 0; n < Height; n++)
                Texture.SetPixel(i, n, Full);

        passes = new bool[Width, Height];
        for (int i = 0; i < Width; i++)
            for (int n = 0; n < Height; n++)
                passes[i, n] = false;
    }

    public override void Click(Vector2 point, float wallWidth)
    {
        var localPoint = new Vector2Int(Mathf.FloorToInt(point.x * Width), Mathf.FloorToInt(point.y * Height));
        OnRoomClick(localPoint);
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

    protected void PaveInit()
    {
        steps = new int[Width, Height];
        for (int i = 0; i < Width; i++)
            for (int n = 0; n < Height; n++)
                steps[i, n] = -1;

        stepsQueue.Clear();
        maxRange = 0;
    }

    protected void PaveDirections()
    {
        while (stepsQueue.Count > 0)
        {
            var from = stepsQueue.Dequeue();
            foreach (var item in Shifts)
            {
                var adj = from + item;
                if (InMaze(adj))
                    if (steps[adj.x, adj.y] == -1 || steps[adj.x, adj.y] > steps[from.x, from.y] + 1)
                        if (GetPass(adj))
                        {
                            steps[adj.x, adj.y] = steps[from.x, from.y] + 1;
                            maxRange = Mathf.Max(steps[adj.x, adj.y], maxRange);
                            stepsQueue.Enqueue(adj);
                        }
            }
        }
    }

    protected void PavePaint()
    {
        for (int i = 0; i < Width; i++)
            for (int n = 0; n < Height; n++)
                if (steps[i, n] != -1)
                    PaintCell(new Vector2Int(i, n), Color.Lerp(MinRangeColor, MaxRangeColor, steps[i, n] / ((float)maxRange)));
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
        PaintCell(cell, temp ? Temprorary : GetPass(cell) ? Empty : Full);
    }

    protected void PaintCell(Vector2Int cell, Color color)
    {
        Texture.SetPixel(cell.x, cell.y, color);
    }

    public void PaintCell(Vector2Int cell)
    {
        Texture.SetPixel(cell.x, cell.y, GetPass(cell) ? Empty : Full);
    }

    public void PaintRig(Vector2Int cell)
    {
        Texture.SetPixel(cell.x, cell.y, Rig);
    }
}
