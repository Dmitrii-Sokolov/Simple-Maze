using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThickWalledMaze : CellMaze
{
    protected override IntVector2 CurrentCell
    {
        set
        {
            if (InMaze(currentCell))
                Texture.SetPixel(currentCell.x, currentCell.y, GetPass(currentCell) ? Color.blue : Color.black);

            currentCell = value;

            if (InMaze(currentCell))
                Texture.SetPixel(currentCell.x, currentCell.y, Color.red);
        }
        get
        {
            return currentCell;
        }
    }

    public override int OutTextureWidth { get { return Width; } }
    public override int OutTextureHeight { get { return Height; } }

    private sbyte[,] nodeDegrees;

    public ThickWalledMaze() { }

    public ThickWalledMaze(int width, int height)
    {
        SetSize(width, height);
    }

    public override void Clear()
    {
        base.Clear();

        nodeDegrees = new sbyte[Width, Height];
        for (int i = 0; i < Width; i++)
            for (int n = 0; n < Height; n++)
                nodeDegrees[i, n] = 0;
    }

    public override bool NextStep()
    {
        SetPass(CurrentCell, true);
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

    private void SetPass(IntVector2 cell, bool pass)
    {
        if (InMaze(cell))
            if (passes[cell.x, cell.y] != pass)
            {
                passes[cell.x, cell.y] = pass;
                Texture.SetPixel(cell.x, cell.y, pass ? Color.blue : Color.black);
                foreach (var item in shifts)
                {
                    var adj = cell + item;
                    if (InMaze(adj))
                        DegreeIncrease(adj, pass ? (sbyte)1 : (sbyte)-1);
                }
            }
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