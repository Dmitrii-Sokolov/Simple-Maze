using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://habrahabr.ru/post/320140/
public class SidewinderThin : MazeGenerator
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
        rowStart = 0;
    }

    private const float horChance = 0.6f;

    private int rowStart;

    public bool NextStep()
    {
        maze.SetPass(maze.CurrentCell, true);
        var next = maze.CurrentCell + IntVector2.East;

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
            maze.SetTunnel(new IntVector2(index, maze.CurrentCell.y), new IntVector2(index, maze.CurrentCell.y + 1), true);
            maze.CurrentCell = next;
            if (!maze.InMaze(maze.CurrentCell))
                maze.CurrentCell = new IntVector2(0, maze.CurrentCell.y + 1);
            rowStart = maze.CurrentCell.x;
            return true;
        }
    }
}
