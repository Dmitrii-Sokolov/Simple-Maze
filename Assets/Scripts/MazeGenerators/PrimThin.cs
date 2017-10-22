using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//https://ru.wikipedia.org/wiki/Алгоритм_Прима
public class PrimThin : WalledMaze
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

    private LinkedList<Edge> Ratios = new LinkedList<Edge>();
    private float[,] vertRatios;
    private float[,] horRatios;
    private int cells;

    public PrimThin(int width, int height)
    {
        SetSize(width, height);
    }

    public override void Clear()
    {
        base.Clear();
        Ratios.Clear();
        cells = Width * Height - 1;

        vertRatios = new float[Width, Height - 1];
        for (int i = 0; i < Width; i++)
            for (int n = 0; n < Height - 1; n++)
                vertRatios[i, n] = Random.value;

        horRatios = new float[Width - 1, Height];
        for (int i = 0; i < Width - 1; i++)
            for (int n = 0; n < Height; n++)
                horRatios[i, n] = Random.value;
    }

    public override bool NextStep()
    {
        SetPass(CurrentCell, true);
        if (cells == 0)
            return false;

        foreach (var item in shifts)
        {
            var adj = CurrentCell + item;
            if (InMaze(adj))
                if (!GetPass(adj))
                    Ratios.AddLast(new Edge(CurrentCell, adj, GetRatio(CurrentCell, adj)));
        }

        var ration = Ratios.Min();

        SetTunnel(ration.from, ration.to, true);
        CurrentCell = ration.to;
        cells--;

        var trash = Ratios.Where(c => c.to == CurrentCell).ToArray();
        foreach (var item in trash)
            Ratios.Remove(item);

        return true;
    }

    protected float GetRatio(IntVector2 cell, IntVector2 to)
    {
        if ((to.x - cell.x) == 0)
            return vertRatios[cell.x, Mathf.Min(cell.y, to.y)];

        if ((to.y - cell.y) == 0)
            return horRatios[Mathf.Min(cell.x, to.x), cell.y];

        return Mathf.Infinity;
    }
}
