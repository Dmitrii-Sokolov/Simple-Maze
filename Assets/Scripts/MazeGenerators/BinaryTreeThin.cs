using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinaryTreeThin : ThinWalledMaze
{
    private static List<IntVector2> shiftsSimple = new List<IntVector2>()
    {
        new IntVector2(1, 0),
        new IntVector2(0, 1),
    };

    public BinaryTreeThin() { }
    public BinaryTreeThin(int width, int height)
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
        SetPass(CurrentCell, true);
        var choices = new List<IntVector2>();

        foreach (var item in shiftsSimple)
        {
            var adj = CurrentCell + item;
            if (InMaze(adj))
                if (!GetPass(adj))
                    choices.Add(adj);
        }

        if (choices.Count == 0)
            return false;
        else
        {
            var index = Random.Range(0, choices.Count);
            SetTunnel(CurrentCell, choices[index], true);
            CurrentCell += shiftsSimple[0];
            if (!InMaze(CurrentCell))
                CurrentCell = new IntVector2(0, CurrentCell.y + 1);
        }

        return true;
    }
}
