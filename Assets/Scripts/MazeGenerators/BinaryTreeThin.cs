using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://habrahabr.ru/post/320140/
public class BinaryTreeThin : BaseMazeGenerator
{
    private static List<Vector2Int> shiftsSimple = new List<Vector2Int>()
    {
        Vector2Int.right, Vector2Int.up
    };

    public override void Init()
    {
        maze.CurrentCell = new Vector2Int(0, 0);
    }

    public override bool NextStep()
    {
        maze.SetPass(maze.CurrentCell, true);
        var choices = new List<Vector2Int>();

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
                maze.CurrentCell = new Vector2Int(0, maze.CurrentCell.y + 1);
        }

        return true;
    }
}
