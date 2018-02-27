using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://ru.wikipedia.org/wiki/Алгоритм_Краскала
public class KruskalThin : BaseMazeGenerator
{
    private const bool ShowOnlyPossibleSteps = true;
    private Edge[] Ratios;
    private int[,] groups;
    private int cells;
    private int index;

    public override void Init()
    {
        cells = maze.Width * maze.Height - 1;
        index = 0;

        groups = new int[maze.Width, maze.Height];
        for (int i = 0; i < maze.Width; i++)
            for (int n = 0; n < maze.Height; n++)
                groups[i, n] = i + n * maze.Width;

        Ratios = new Edge[2 * maze.Width * maze.Height - maze.Height - maze.Width];

        for (int i = 0; i < maze.Width; i++)
            for (int n = 0; n < maze.Height - 1; n++)
                Ratios[i + n * maze.Width] = new Edge(new Vector2Int(i, n), new Vector2Int(i, n + 1), Random.value);

        for (int i = 0; i < maze.Width - 1; i++)
            for (int n = 0; n < maze.Height; n++)
                Ratios[i * maze.Height + n + maze.Width * maze.Height - maze.Width] = new Edge(new Vector2Int(i, n), new Vector2Int(i + 1, n), Random.value);

        System.Array.Sort(Ratios);
    }

    public override bool NextStep()
    {
        if (GetGroup(Ratios[index].from) != GetGroup(Ratios[index].to))
        {
            SetGroupRecursively(Ratios[index].to, GetGroup(Ratios[index].from));
            maze.SetPass(Ratios[index].from, true);
            maze.SetPass(Ratios[index].to, true);
            maze.SetTunnel(Ratios[index].from, Ratios[index].to, true);
            cells--;
        }

        maze.PaintCell(Ratios[index].to);
        if (cells == 0)
            return false;

        if (ShowOnlyPossibleSteps)
            do
                index++;
            while (GetGroup(Ratios[index].from) == GetGroup(Ratios[index].to));
        else
            index++;

        maze.CurrentCell = Ratios[index].from;
        maze.PaintRig(Ratios[index].to);

        return true;
    }

    private void SetGroupRecursively(Vector2Int cell, int group)
    {
        var current = GetGroup(cell);
        SetGroup(cell, group);

        foreach (var item in CelledMaze.Shifts)
        {
            var adj = cell + item;
            if (maze.InMaze(adj) && GetGroup(adj) == current)
                SetGroupRecursively(adj, group);
        }
    }

    private void SetGroup(Vector2Int cell, int group)
    {
        groups[cell.x, cell.y] = group;
    }

    private int GetGroup(Vector2Int cell)
    {
        return groups[cell.x, cell.y];
    }
}
