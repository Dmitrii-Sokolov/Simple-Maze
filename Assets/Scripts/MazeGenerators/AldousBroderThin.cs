using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://habrahabr.ru/post/321210/
public class AldousBroderThin : WalledMaze
{
    public AldousBroderThin(int width, int height)
    {
        SetSize(width, height);
    }

    private int cells;

    public override void Clear()
    {
        base.Clear();
        cells = Width * Height - 1;
    }

    public override bool NextStep()
    {
        SetPass(CurrentCell, true);
        var choices = new List<IntVector2>();

        foreach (var item in shifts)
        {
            var adj = CurrentCell + item;
            if (InMaze(adj))
                choices.Add(adj);
        }

        var index = Random.Range(0, choices.Count);
        if (!GetPass(choices[index]))
        {
            SetTunnel(CurrentCell, choices[index], true);
            cells--;
        }

        CurrentCell = choices[index];
        return (cells != 0);
    }
}
