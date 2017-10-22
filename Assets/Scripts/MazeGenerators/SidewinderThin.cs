using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://habrahabr.ru/post/320140/
public class SidewinderThin : WalledMaze
{
    private const float horChance = 0.6f;

    private static List<IntVector2> shiftsSimple = new List<IntVector2>()
    {
        IntVector2.East, IntVector2.North
    };

    private int rowStart;

    public SidewinderThin(int width, int height)
    {
        SetSize(width, height);
    }

    public override void Clear()
    {
        base.Clear();
        CurrentCell = new IntVector2(0, 0);
        rowStart = 0;
    }

    public override bool NextStep()
    {
        SetPass(CurrentCell, true);
        var next = CurrentCell + IntVector2.East;

        if (CurrentCell.y == Height - 1)
        {
            if (InMaze(next))
            {
                SetTunnel(CurrentCell, next, true);
                CurrentCell = next;
                return true;
            }
            else
                return false;
        }

        if (InMaze(next) && (Random.value < horChance))
        {
            SetTunnel(CurrentCell, next, true);
            CurrentCell = next;
            return true;
        }
        else
        {
            var index = Random.Range(rowStart, CurrentCell.x);
            SetTunnel(new IntVector2(index, CurrentCell.y), new IntVector2(index, CurrentCell.y + 1), true);
            CurrentCell = next;
            if (!InMaze(CurrentCell))
                CurrentCell = new IntVector2(0, CurrentCell.y + 1);
            rowStart = CurrentCell.x;
            return true;
        }
    }
}
