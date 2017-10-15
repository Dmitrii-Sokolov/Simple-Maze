using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThickWalledMaze : CellMaze
{
    private sbyte[,] nodeDegrees;

    public override int OutTextureWidth { get { return Width; } }
    public override int OutTextureHeight { get { return Height; } }

    public override CellMaze SetSize(int width, int height)
    {
        return new ThickWalledMaze(width, height);
    }


    protected override Cell CurrentCell
    {
        set
        {
            if (null != currentCell)
                if (InMaze(currentCell))
                    colorMap[currentCell.X + currentCell.Y * OutTextureWidth] = GetPass(currentCell) ? Color.blue : Color.black;

            currentCell = value;

            if (null != currentCell)
                if (InMaze(currentCell))
                    colorMap[currentCell.X + currentCell.Y * OutTextureWidth] = Color.red;
        }
        get
        {
            return currentCell;
        }
    }

    public ThickWalledMaze(int width, int height) : base(width, height)
    {
        nodeDegrees = new sbyte[Width, Height];
        Clear();
    }

    public override void Clear()
    {
        base.Clear();
        for (int i = 0; i < Width; i++)
            for (int n = 0; n < Height; n++)
                nodeDegrees[i, n] = 0;
    }

    public override bool NextStep()
    {
        SetPass(CurrentCell, true);
        var choices = new List<Cell>();

        foreach (var item in CurrentCell.AdjQuad)
            if (InMaze(item))
                if (!GetPass(item))
                    if (GetDegree(item) <= 1)
                        choices.Add(item);

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

    private sbyte GetDegree(Cell cell)
    {
        return nodeDegrees[cell.X, cell.Y];
    }

    private void DegreeIncrease(Cell cell, sbyte count)
    {
        nodeDegrees[cell.X, cell.Y] += count;
    }

    private void SetPass(Cell cell, bool pass)
    {
        if (InMaze(cell))
            if (passes[cell.X, cell.Y] != pass)
            {
                passes[cell.X, cell.Y] = pass;
                colorMap[cell.X + cell.Y * OutTextureWidth] = pass ? Color.blue : Color.black;
                foreach (var item in cell.AdjQuad)
                    if (InMaze(item))
                        DegreeIncrease(item, pass ? (sbyte) 1 : (sbyte) -1);
            }
    }

    private bool GetPass(Cell cell)
    {
        return passes[cell.X, cell.Y];
    }
}