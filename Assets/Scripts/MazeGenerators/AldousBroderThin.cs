using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://habrahabr.ru/post/321210/
public class AldousBroderThin : MazeGenerator<WalledMaze>
{
    private int cells;

    protected override void Init()
    {
        cells = maze.Width * maze.Height - 1;
    }

    public override bool NextStep()
    {
        if (base.NextStep())
            return false;

        maze.SetPass(maze.CurrentCell, true);
        var choices = new List<Vector2Int>();

        foreach (var item in RectangleMaze.Shifts)
        {
            var adj = maze.CurrentCell + item;
            if (maze.InMaze(adj))
                choices.Add(adj);
        }

        var index = Random.Range(0, choices.Count);
        if (!maze.GetPass(choices[index]))
        {
            maze.SetTunnel(maze.CurrentCell, choices[index], true);
            cells--;
        }

        maze.CurrentCell = choices[index];
        return (cells != 0);
    }
}
