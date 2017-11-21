using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://habrahabr.ru/post/321210/
public class WilsonThin : WalledMaze, MazeGenerator
{
    public void Generate() { while (NextStep()) ; }
    public void Init(Maze TargetMaze) { }

    public WilsonThin(int width, int height)
    {
        SetSize(width, height);
    }

    private List<IntVector2> MazeTrace = new List<IntVector2>();
    private int cells;
    protected int[,] OldCells;

    public override void Clear()
    {
        base.Clear();
        SetPass(CurrentCell, true);
        MazeTrace.Clear();
        cells = Width * Height - 1;

        OldCells = new int[Width, Height];
        for (int i = 0; i < Width; i++)
            for (int n = 0; n < Height; n++)
                OldCells[i, n] = -1;
    }
    
    public bool NextStep()
    {
        if (cells != 0)
        {
            if (GetPass(CurrentCell))
            {
                if (MazeTrace.Count == 0)
                {
                    SetRandomCurrent();
                    MazeTrace.Add(CurrentCell);
                    OldCells[CurrentCell.x, CurrentCell.y] = 0;
                }
                else
                {
                    SetTunnels();
                    MazeTrace.Clear();
                    for (int i = 0; i < Width; i++)
                        for (int n = 0; n < Height; n++)
                            OldCells[i, n] = -1;
                }
            }
            else
            {
                var choices = new List<IntVector2>();
                foreach (var item in shifts)
                {
                    var adj = CurrentCell + item;
                    if (MazeTrace.Count < 2 || MazeTrace[MazeTrace.Count - 2] != adj)
                        if (InMaze(adj))
                            choices.Add(adj);
                }

                CurrentCell = choices[Random.Range(0, choices.Count)];

                if (OldCells[CurrentCell.x, CurrentCell.y] != -1)
                {
                    var cut = OldCells[CurrentCell.x, CurrentCell.y];
                    for (int i = cut; i < MazeTrace.Count; i++)
                    {
                        PaintTemp(MazeTrace[i], false);
                        OldCells[MazeTrace[i].x, MazeTrace[i].y] = -1;
                    }

                    MazeTrace.RemoveRange(cut, MazeTrace.Count - cut);
                }

                if (MazeTrace.Count > 1)
                    PaintTemp(MazeTrace[MazeTrace.Count - 1], true);

                MazeTrace.Add(CurrentCell);
                OldCells[CurrentCell.x, CurrentCell.y] = MazeTrace.Count - 1;
            }
        }
        return (cells != 0);
    }

    private void SetRandomCurrent()
    {
        var index = Random.Range(0, cells);
        for (int i = 0; i < Width; i++)
            for (int n = 0; n < Height; n++)
                if (!GetPass(new IntVector2(i, n)))
                    if (index-- == 0)
                        CurrentCell = new IntVector2(i, n);
    }

    private void SetTunnels()
    {
        for (int i = 0; i < MazeTrace.Count - 1; i++)
        {
            SetPass(MazeTrace[i], true);
            SetTunnel(MazeTrace[i], MazeTrace[i + 1], true);
            cells--;
        }
    }
}

