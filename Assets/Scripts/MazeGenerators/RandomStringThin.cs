using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://habrahabr.ru/post/251631/
public class RandomStringThin : WalledMaze
{
    public RandomStringThin(int width, int height)
    {
        SetSize(width, height);
    }

    public override void Clear()
    {
        base.Clear();
        CurrentCell = new IntVector2(0, 0);
    }

    public override bool NextStep()
    {
        if (Random.value > 0.5f)
        {
            SetQuad(CurrentCell, CurrentCell + IntVector2.East);
            SetQuad(CurrentCell + IntVector2.North, CurrentCell + IntVector2.East + IntVector2.North);
        }
        else
        {
            SetQuad(CurrentCell, CurrentCell + IntVector2.North);
            SetQuad(CurrentCell + IntVector2.East, CurrentCell + IntVector2.East + IntVector2.North);
        }

        CurrentCell = new IntVector2(CurrentCell.x + 2, CurrentCell.y);
        if (!InMaze(CurrentCell))
            CurrentCell = new IntVector2((CurrentCell.y + 1) % 2, CurrentCell.y + 1);

        return InMaze(CurrentCell);
    }


    public void SetQuad(IntVector2 cell1, IntVector2 cell2)
    {
        if (InMaze(cell1) && InMaze(cell2))
        {
            SetPass(cell1, true);
            SetPass(cell2, true);
            SetTunnel(cell1, cell2, true);
        }
    }
}
