using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://habrahabr.ru/post/320140/
public class BinaryTreeThin : WalledMaze, MazeGenerator
{
    public void Generate() { while (NextStep()) ; }
    public void Init(Maze TargetMaze) { }

    private static List<IntVector2> shiftsSimple = new List<IntVector2>()
    {
        IntVector2.East, IntVector2.North
    };

    public BinaryTreeThin(int width, int height)
    {
        SetSize(width, height);
    }

    public override void Clear()
    {
        base.Clear();
        CurrentCell = new IntVector2(0, 0);
    }

    public bool NextStep()
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
