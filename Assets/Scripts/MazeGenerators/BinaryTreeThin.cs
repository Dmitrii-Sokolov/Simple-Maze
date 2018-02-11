using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://habrahabr.ru/post/320140/
public class BinaryTreeThin : MazeGenerator
{
    public void Generate() { while (NextStep()) ; }
    private Maze maze;

    public void Init(Maze TargetMaze)
    {
        maze = TargetMaze;
        Init();
    }

    public void Init()
    {
        maze.CurrentCell = new IntVector2(0, 0);
    }

    private static List<IntVector2> shiftsSimple = new List<IntVector2>()
    {
        IntVector2.East, IntVector2.North
    };

    public bool NextStep()
    {
        maze.SetPass(maze.CurrentCell, true);
        var choices = new List<IntVector2>();

        foreach (var item in shiftsSimple)
        {
            var adj = maze.CurrentCell + item;
            if (maze.InMaze(adj))
                if (!maze.GetPass(adj))
                    choices.Add(adj);
        }

        if (choices.Count == 0)
            return false;
        else
        {
            var index = Random.Range(0, choices.Count);
            maze.SetTunnel(maze.CurrentCell, choices[index], true);
            maze.CurrentCell += shiftsSimple[0];
            if (!maze.InMaze(maze.CurrentCell))
                maze.CurrentCell = new IntVector2(0, maze.CurrentCell.y + 1);
        }

        return true;
    }
}
