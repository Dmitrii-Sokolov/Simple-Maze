using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//https://ru.wikipedia.org/wiki/Алгоритм_Прима
public class PrimThin : BaseMazeGenerator
{
    private LinkedList<Edge> Ratios = new LinkedList<Edge>();
    private float[,] vertRatios;
    private float[,] horRatios;
    private int cells;

    public override void Init()
    {

        Ratios.Clear();
        cells = maze.Width * maze.Height - 1;

        vertRatios = new float[maze.Width, maze.Height - 1];
        for (int i = 0; i < maze.Width; i++)
            for (int n = 0; n < maze.Height - 1; n++)
                vertRatios[i, n] = Random.value;

        horRatios = new float[maze.Width - 1, maze.Height];
        for (int i = 0; i < maze.Width - 1; i++)
            for (int n = 0; n < maze.Height; n++)
                horRatios[i, n] = Random.value;
    }

    public override bool NextStep()
    {
        maze.SetPass(maze.CurrentCell, true);
        if (cells == 0)
            return false;

        foreach (var item in CelledMaze.Shifts)
        {
            var adj = maze.CurrentCell + item;
            if (maze.InMaze(adj))
                if (!maze.GetPass(adj))
                    Ratios.AddLast(new Edge(maze.CurrentCell, adj, GetRatio(maze.CurrentCell, adj)));
        }

        var ration = Ratios.Min();

        maze.SetTunnel(ration.from, ration.to, true);
        maze.CurrentCell = ration.to;
        cells--;

        var trash = Ratios.Where(c => c.to == maze.CurrentCell).ToArray();
        foreach (var item in trash)
            Ratios.Remove(item);

        return true;
    }

    protected float GetRatio(Vector2Int cell, Vector2Int to)
    {
        if ((to.x - cell.x) == 0)
            return vertRatios[cell.x, Mathf.Min(cell.y, to.y)];

        if ((to.y - cell.y) == 0)
            return horRatios[Mathf.Min(cell.x, to.x), cell.y];

        return Mathf.Infinity;
    }
}
