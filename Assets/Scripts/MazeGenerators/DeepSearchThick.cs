using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeepSearchThick : CellMaze, MazeGenerator
{
    public void Generate() { while (NextStep()) ; }
    public void Init(Maze TargetMaze) { }

    public DeepSearchThick(int width, int height)
    {
        SetSize(width, height);
    }

    private Stack<IntVector2> MazeTrace = new Stack<IntVector2>();
    private sbyte[,] nodeDegrees;

    public override void Clear()
    {
        base.Clear();
        MazeTrace.Clear();

        nodeDegrees = new sbyte[Width, Height];
        for (int i = 0; i < Width; i++)
            for (int n = 0; n < Height; n++)
                nodeDegrees[i, n] = 0;
    }
  
    public bool NextStep()
    {
        if (!GetPass(CurrentCell))
        {
            SetPass(CurrentCell, true);
            foreach (var item in shifts)
            {
                var adj = CurrentCell + item;
                if (InMaze(adj))
                    DegreeIncrease(adj, 1);
            }
        }

        var choices = new List<IntVector2>();

        foreach (var item in shifts)
        {
            var adj = CurrentCell + item;
            if (InMaze(adj))
                if (!GetPass(adj))
                    if (GetDegree(adj) <= 1)
                        choices.Add(adj);
        }

        if (choices.Count == 0)
        {
            if (MazeTrace.Count == 0)
                return false;
            else
                CurrentCell = MazeTrace.Pop();
        }
        else
        {
            var index = Random.Range(0, choices.Count);
            MazeTrace.Push(CurrentCell);
            CurrentCell = choices[index];
        }

        return true;
    }

    private sbyte GetDegree(IntVector2 cell)
    {
        return nodeDegrees[cell.x, cell.y];
    }

    private void DegreeIncrease(IntVector2 cell, sbyte count)
    {
        nodeDegrees[cell.x, cell.y] += count;
    }
}
