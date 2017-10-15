using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeepSearchThick : ThickWalledMaze
{
    public DeepSearchThick() { }
    public DeepSearchThick(int width, int height)
    {
        SetSize(width, height);
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
}
