using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeepSearchThin : MazeGenerator
{
    public void Generate() { while (NextStep()) ; }
    private Maze maze;

    private Stack<IntVector2> MazeTrace = new Stack<IntVector2>();

    public void Init(Maze TargetMaze)
    {
        maze = TargetMaze;
        Init();
    }

    public void Init()
    {
        MazeTrace.Clear();
    }

    public bool NextStep()
    {
        maze.SetPass(maze.CurrentCell, true);
        var choices = new List<IntVector2>();

        foreach (var item in IntVector2.Shifts)
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
