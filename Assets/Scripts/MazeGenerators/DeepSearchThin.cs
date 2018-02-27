using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeepSearchThin : BaseMazeGenerator
{
    private Stack<Vector2Int> MazeTrace = new Stack<Vector2Int>();

    public override void Init()
    {
        MazeTrace.Clear();
    }

    public override bool NextStep()
    {
        maze.SetPass(maze.CurrentCell, true);
        var choices = new List<Vector2Int>();

        foreach (var item in CelledMaze.Shifts)
        {
            var adj = maze.CurrentCell + item;
            if (maze.InMaze(adj))
                if (!maze.GetPass(adj))
                    choices.Add(adj);
        }

        if (choices.Count == 0)
        {
            if (MazeTrace.Count == 0)
                return false;
            else
                maze.CurrentCell = MazeTrace.Pop();
        }
        else
        {
            var index = Random.Range(0, choices.Count);
            MazeTrace.Push(maze.CurrentCell);
            maze.SetTunnel(maze.CurrentCell, choices[index], true);
            maze.CurrentCell = choices[index];
        }

        return true;
    }
}
