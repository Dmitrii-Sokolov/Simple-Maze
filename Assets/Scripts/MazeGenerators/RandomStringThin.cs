using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://habrahabr.ru/post/251631/
public class RandomStringThin : MazeGenerator
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

    public bool NextStep()
    {
        if (Random.value > 0.5f)
        {
            SetQuad(maze.CurrentCell, maze.CurrentCell + IntVector2.East);
            SetQuad(maze.CurrentCell + IntVector2.North, maze.CurrentCell + IntVector2.East + IntVector2.North);
        }
        else
        {
            SetQuad(maze.CurrentCell, maze.CurrentCell + IntVector2.North);
            SetQuad(maze.CurrentCell + IntVector2.East, maze.CurrentCell + IntVector2.East + IntVector2.North);
        }

        maze.CurrentCell = new IntVector2(maze.CurrentCell.x + 2, maze.CurrentCell.y);
        if (!maze.InMaze(maze.CurrentCell))
            maze.CurrentCell = new IntVector2((maze.CurrentCell.y + 1) % 2, maze.CurrentCell.y + 1);

        return maze.InMaze(maze.CurrentCell);
    }


    public void SetQuad(IntVector2 cell1, IntVector2 cell2)
    {
        if (maze.InMaze(cell1) && maze.InMaze(cell2))
        {
            maze.SetPass(cell1, true);
            maze.SetPass(cell2, true);
            maze.SetTunnel(cell1, cell2, true);
        }
    }
}
