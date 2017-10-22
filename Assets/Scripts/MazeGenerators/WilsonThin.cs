using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://habrahabr.ru/post/321210/
public class WilsonThin : WalledMaze
{
    public WilsonThin() { }
    public WilsonThin(int width, int height)
    {
        SetSize(width, height);
    }

    private List<IntVector2> MazeTrace = new List<IntVector2>();
    private int cells;
    private Dictionary<IntVector2, int> OldCells = new Dictionary<IntVector2, int>();

    public override void Clear()
    {
        base.Clear();
        SetPass(CurrentCell, true);
        MazeTrace.Clear();
        OldCells.Clear();
        cells = Width * Height - 1;
    }
    
    public override bool NextStep()
    {
        if (cells != 0)
        {
            if (GetPass(CurrentCell))
            {
                if (MazeTrace.Count == 0)
                {
                    SetRandomCurrent();
                    MazeTrace.Add(CurrentCell);
                    OldCells.Add(CurrentCell, 0);
                }
                else
                {
                    SetTunnels();
                    MazeTrace.Clear();
                    OldCells.Clear();
                }
            }
            else
            {
                var choices = new List<IntVector2>();
                foreach (var item in shifts)
                {
                    var adj = CurrentCell + item;
                    if (InMaze(adj))
                        if (MazeTrace.Count < 2 || MazeTrace[MazeTrace.Count - 2] != adj)
                            choices.Add(adj);
                }

                CurrentCell = choices[Random.Range(0, choices.Count)];

                if (OldCells.ContainsKey(CurrentCell))
                {
                    var cut = OldCells[CurrentCell];
                    for (int i = cut; i < MazeTrace.Count; i++)
                    {
                        PaintTemp(MazeTrace[i], false);
                        OldCells.Remove(MazeTrace[i]);
                    }

                    MazeTrace.RemoveRange(cut, MazeTrace.Count - cut);
                }

                if (MazeTrace.Count > 1)
                    PaintTemp(MazeTrace[MazeTrace.Count - 1], true);

                MazeTrace.Add(CurrentCell);
                OldCells.Add(CurrentCell, MazeTrace.Count - 1);
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

