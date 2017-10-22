using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://ru.wikipedia.org/wiki/Алгоритм_Краскала
public class KruskalThin : WalledMaze
{
    private struct Edge : System.IComparable<Edge>
    {
        public IntVector2 from;
        public IntVector2 to;
        public float ratio;

        public Edge(IntVector2 from, IntVector2 to, float ratio)
        {
            this.from = from;
            this.to = to;
            this.ratio = ratio;
        }

        public int CompareTo(Edge obj)
        {
            if (ratio > obj.ratio)
                return 1;
            if (ratio < obj.ratio)
                return -1;
            else
                return 0;
        }
    }

    public KruskalThin(int width, int height)
    {
        SetSize(width, height);
    }

    private const bool ShowOnlyPossibleSteps = true;
    private Edge[] Ratios;
    private int[,] groups;
    private int cells;
    private int index;
    
    public override void Clear()
    {
        base.Clear();

        cells = Width * Height - 1;
        index = 0;

        groups = new int[Width, Height];
        for (int i = 0; i < Width; i++)
            for (int n = 0; n < Height; n++)
                groups[i, n] = i + n * Width;

        Ratios = new Edge[2 * Width * Height - Height - Width];

        for (int i = 0; i < Width; i++)
            for (int n = 0; n < Height - 1; n++)
                Ratios[i + n * Width] = new Edge(new IntVector2(i, n), new IntVector2(i, n + 1), Random.value);

        for (int i = 0; i < Width - 1; i++)
            for (int n = 0; n < Height; n++)
                Ratios[i * Height + n + Width * Height - Width] = new Edge(new IntVector2(i, n), new IntVector2(i + 1, n), Random.value);

        System.Array.Sort(Ratios);
    }

    public override bool NextStep()
    {
        if (GetGroup(Ratios[index].from) != GetGroup(Ratios[index].to))
        {
            SetGroupRecursively(Ratios[index].to, GetGroup(Ratios[index].from));
            SetPass(Ratios[index].from, true);
            SetPass(Ratios[index].to, true);
            SetTunnel(Ratios[index].from, Ratios[index].to, true);
            cells--;
        }

        PaintCell(Ratios[index].to);
        if (cells == 0)
            return false;

        if (ShowOnlyPossibleSteps)
            do
                index++;
            while (GetGroup(Ratios[index].from) == GetGroup(Ratios[index].to));
        else
            index++;

        CurrentCell = Ratios[index].from;
        PaintRig(Ratios[index].to);

        return true;
    }

    private void SetGroupRecursively(IntVector2 cell, int group)
    {
        var current = GetGroup(cell);
        SetGroup(cell, group);

        foreach (var item in shifts)
        {
            var adj = cell + item;
            if (InMaze(adj) && GetGroup(adj) == current)
                SetGroupRecursively(adj, group);
        }
    }

    private void SetGroup(IntVector2 cell, int group)
    {
        groups[cell.x, cell.y] = group;
    }

    private int GetGroup(IntVector2 cell)
    {
        return groups[cell.x, cell.y];
    }
}
