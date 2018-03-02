using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseThick : MazeGenerator<CelledMaze>
{
    private Cell[] Ratios;
    private int[,] groups;
    private int index;

    protected override void Init()
    {
        //cells = maze.Width * maze.Height - 1;
        index = 0;

        groups = new int[maze.Width, maze.Height];
        for (int i = 0; i < maze.Width; i++)
            for (int n = 0; n < maze.Height; n++)
                groups[i, n] = i + n * maze.Width;

        Ratios = new Cell[maze.Width * maze.Height];

        for (int i = 0; i < maze.Width; i++)
            for (int n = 0; n < maze.Height; n++)
                Ratios[i + n * maze.Width] = new Cell(new Vector2Int(i, n), Random.value);

        System.Array.Sort(Ratios);
    }

    public override bool NextStep()
    {
        if (base.NextStep())
            return false;

        var nears = new HashSet<int>();
        var passThrough = true;

        foreach (var item in CelledMaze.Shifts)
        {
            var adj = maze.CurrentCell + item;
            if (maze.InMaze(adj))
            {

                if (nears.Contains(GetGroup(adj)))
                {
                    passThrough = false;
                    break;
                }
                else
                    nears.Add(GetGroup(adj));
            }
        }

        if (passThrough)
        {
            maze.SetPass(maze.CurrentCell, true);
            SetGroupRecursively(maze.CurrentCell, GetGroup(maze.CurrentCell));
        }

        index++;
        if (Ratios.Length == index)
            return false;

        maze.CurrentCell = Ratios[index].place;

        return true;
    }

    private void SetGroupRecursively(Vector2Int cell, int group)
    {
        SetGroup(cell, group);
        foreach (var item in CelledMaze.Shifts)
        {
            var adj = cell + item;
            if (maze.InMaze(adj) && maze.GetPass(adj) && GetGroup(adj) != group)
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
