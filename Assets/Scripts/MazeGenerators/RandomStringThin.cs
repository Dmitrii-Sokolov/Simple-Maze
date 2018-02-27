using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://habrahabr.ru/post/251631/
public class RandomStringThin : IMazeGenerator
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
    }

    public bool NextStep()
    {
        if (Random.value > 0.5f)
        {
            SetQuad(maze.CurrentCell, maze.CurrentCell + Vector2Int.right);
            SetQuad(maze.CurrentCell + Vector2Int.up, maze.CurrentCell + Vector2Int.right + Vector2Int.up);
        }
        else
        {
            SetQuad(maze.CurrentCell, maze.CurrentCell + Vector2Int.up);
            SetQuad(maze.CurrentCell + Vector2Int.right, maze.CurrentCell + Vector2Int.right + Vector2Int.up);
        }

        maze.CurrentCell = new Vector2Int(maze.CurrentCell.x + 2, maze.CurrentCell.y);
        if (!maze.InMaze(maze.CurrentCell))
            maze.CurrentCell = new Vector2Int((maze.CurrentCell.y + 1) % 2, maze.CurrentCell.y + 1);

        return maze.InMaze(maze.CurrentCell);
    }


    public void SetQuad(Vector2Int cell1, Vector2Int cell2)
    {
        if (maze.InMaze(cell1) && maze.InMaze(cell2))
        {
            maze.SetPass(cell1, true);
            maze.SetPass(cell2, true);
            maze.SetTunnel(cell1, cell2, true);
        }
    }
}
