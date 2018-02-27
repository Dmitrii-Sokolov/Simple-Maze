using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://habrahabr.ru/post/320140/
public class SidewinderThin : IMazeGenerator
{
    public void Generate() { while (NextStep()) ; }
    private IMaze maze;

    public void Init(IMaze TargetMaze)
    {
        maze = TargetMaze;
        Init();
    }

    public void Init()
    {
        maze.CurrentCell = new Vector2Int(0, 0);
        rowStart = 0;
    }

    private const float horChance = 0.6f;

    private int rowStart;

    public bool NextStep()
    {
        maze.SetPass(maze.CurrentCell, true);
        var next = maze.CurrentCell + Vector2Int.right;

        if (maze.CurrentCell.y == maze.Height - 1)
        {
            if (maze.InMaze(next))
            {
                maze.SetTunnel(maze.CurrentCell, next, true);
                maze.CurrentCell = next;
                return true;
            }
            else
                return false;
        }

        if (maze.InMaze(next) && (Random.value < horChance))
        {
            maze.SetTunnel(maze.CurrentCell, next, true);
            maze.CurrentCell = next;
            return true;
        }
        else
        {
            var index = Random.Range(rowStart, maze.CurrentCell.x);
            maze.SetTunnel(new Vector2Int(index, maze.CurrentCell.y), new Vector2Int(index, maze.CurrentCell.y + 1), true);
            maze.CurrentCell = next;
            if (!maze.InMaze(maze.CurrentCell))
                maze.CurrentCell = new Vector2Int(0, maze.CurrentCell.y + 1);
            rowStart = maze.CurrentCell.x;
            return true;
        }
    }
}
