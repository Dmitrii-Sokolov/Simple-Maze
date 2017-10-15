using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//http://wiki.roblox.com/index.php/Hunt-and-Kill
public class HuntAndKillThin : WalledMaze
{
    public HuntAndKillThin() { }
    public HuntAndKillThin(int width, int height)
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
                    choices.Add(adj);
        }

        if (choices.Count == 0)
        {
            for (int i = 0; i < Width; i++)
                for (int n = 0; n < Height; n++)
                {
                    var hunter = new IntVector2(i, n);
                    if (!GetPass(hunter))
                        foreach (var item in shifts)
                        {
                            var adj = hunter + item;
                            if (InMaze(adj))
                                if (GetPass(adj))
                                {
                                    SetTunnel(hunter, adj, true);
                                    CurrentCell = hunter;
                                    return true;
                                }
                        }
                }
        }
        else
        {
            var index = Random.Range(0, choices.Count);
            SetTunnel(CurrentCell, choices[index], true);
            CurrentCell = choices[index];
            return true;
        }

        return false;
    }
}
