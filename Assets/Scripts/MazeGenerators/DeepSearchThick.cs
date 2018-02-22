using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeepSearchThick : MazeGenerator
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
        MazeTrace.Clear();

        nodeDegrees = new sbyte[maze.Width, maze.Height];
        for (int i = 0; i < maze.Width; i++)
            for (int n = 0; n < maze.Height; n++)
                nodeDegrees[i, n] = 0;
    }

    private Stack<Vector2Int> MazeTrace = new Stack<Vector2Int>();
    private sbyte[,] nodeDegrees;

    public bool NextStep()
    {
        if (!maze.GetPass(maze.CurrentCell))
        {
            maze.SetPass(maze.CurrentCell, true);
            foreach (var item in CellMaze.Shifts)
            {
                var adj = maze.CurrentCell + item;
                if (maze.InMaze(adj))
                    DegreeIncrease(adj, 1);
            }
        }

        var choices = new List<Vector2Int>();

        foreach (var item in CellMaze.Shifts)
        {
            var adj = maze.CurrentCell + item;
            if (maze.InMaze(adj))
                if (!maze.GetPass(adj))
                    if (GetDegree(adj) <= 1)
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
            maze.CurrentCell = choices[index];
        }

        return true;
    }

    private sbyte GetDegree(Vector2Int cell)
    {
        return nodeDegrees[cell.x, cell.y];
    }

    private void DegreeIncrease(Vector2Int cell, sbyte count)
    {
        nodeDegrees[cell.x, cell.y] += count;
    }
}
