using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://habrahabr.ru/post/321210/
public class WilsonThin : MazeGenerator
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
        maze.SetPass(maze.CurrentCell, true);
        MazeTrace.Clear();
        cells = maze.Width * maze.Height - 1;

        OldCells = new int[maze.Width, maze.Height];
        for (int i = 0; i < maze.Width; i++)
            for (int n = 0; n < maze.Height; n++)
                OldCells[i, n] = -1;
    }

    private List<Vector2Int> MazeTrace = new List<Vector2Int>();
    private int cells;
    protected int[,] OldCells;

    public bool NextStep()
    {
        if (cells != 0)
        {
            if (maze.GetPass(maze.CurrentCell))
            {
                if (MazeTrace.Count == 0)
                {
                    SetRandomCurrent();
                    MazeTrace.Add(maze.CurrentCell);
                    OldCells[maze.CurrentCell.x, maze.CurrentCell.y] = 0;
                }
                else
                {
                    SetTunnels();
                    MazeTrace.Clear();
                    for (int i = 0; i < maze.Width; i++)
                        for (int n = 0; n < maze.Height; n++)
                            OldCells[i, n] = -1;
                }
            }
            else
            {
                var choices = new List<Vector2Int>();
                foreach (var item in CellMaze.Shifts)
                {
                    var adj = maze.CurrentCell + item;
                    if (MazeTrace.Count < 2 || MazeTrace[MazeTrace.Count - 2] != adj)
                        if (maze.InMaze(adj))
                            choices.Add(adj);
                }

                maze.CurrentCell = choices[Random.Range(0, choices.Count)];

                if (OldCells[maze.CurrentCell.x, maze.CurrentCell.y] != -1)
                {
                    var cut = OldCells[maze.CurrentCell.x, maze.CurrentCell.y];
                    for (int i = cut; i < MazeTrace.Count; i++)
                    {
                        maze.PaintTemp(MazeTrace[i], false);
                        OldCells[MazeTrace[i].x, MazeTrace[i].y] = -1;
                    }

                    MazeTrace.RemoveRange(cut, MazeTrace.Count - cut);
                }

                if (MazeTrace.Count > 1)
                    maze.PaintTemp(MazeTrace[MazeTrace.Count - 1], true);

                MazeTrace.Add(maze.CurrentCell);
                OldCells[maze.CurrentCell.x, maze.CurrentCell.y] = MazeTrace.Count - 1;
            }
        }
        return (cells != 0);
    }

    private void SetRandomCurrent()
    {
        var index = Random.Range(0, cells);
        for (int i = 0; i < maze.Width; i++)
            for (int n = 0; n < maze.Height; n++)
                if (!maze.GetPass(new Vector2Int(i, n)))
                    if (index-- == 0)
                        maze.CurrentCell = new Vector2Int(i, n);
    }

    private void SetTunnels()
    {
        for (int i = 0; i < MazeTrace.Count - 1; i++)
        {
            maze.SetPass(MazeTrace[i], true);
            maze.SetTunnel(MazeTrace[i], MazeTrace[i + 1], true);
            cells--;
        }
    }
}

