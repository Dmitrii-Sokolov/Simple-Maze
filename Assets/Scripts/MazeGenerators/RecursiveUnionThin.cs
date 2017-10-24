using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//http://progressor-blog.ru/qt/generatsiya-labirinta-i-ego-prohozhdenie/
public class RecursiveUnionThin : WalledMaze
{
    public RecursiveUnionThin(int width, int height)
    {
        SetSize(width, height);
    }

    Queue<IntRect> rects = new Queue<IntRect>();

    public override void Clear()
    {
        base.Clear();
        CurrentCell = new IntVector2(-1, -1);
        rects.Clear();
        rects.Enqueue(new IntRect(new IntVector2(0, 0), new IntVector2(Width - 1, Height - 1)));
    }

    public override bool NextStep()
    {
        if (rects.Count == 0)
            return false;

        var rect = rects.Dequeue();

        if (rect.type == IntRect.Type.VerticalLine)
        {
            for (int i = rect.from.y; i < rect.to.y; i++)
                SetTunnelNorth(rect.from.x, i);
            return true;
        }

        if (rect.type == IntRect.Type.HorizontalLine)
        {
            for (int i = rect.from.x; i < rect.to.x; i++)
                SetTunnelEast(i, rect.from.y);
            return true;
        }

        if (rect.type == IntRect.Type.Horizontal || (rect.type == IntRect.Type.Square && Random.value > 0.5f))
        {
            var newX = CustomRandom(rect.from.x, rect.to.x);
            rects.Enqueue(new IntRect(rect.from, new IntVector2(newX, rect.to.y)));
            rects.Enqueue(new IntRect(new IntVector2(newX + 1, rect.from.y), rect.to));

            var tunnelY = Random.Range(rect.from.y, rect.to.y + 1);
            SetTunnelEast(newX, tunnelY);

            return true;
        }

        if (rect.type == IntRect.Type.Vertical || rect.type == IntRect.Type.Square)
        {
            var newY = CustomRandom(rect.from.y, rect.to.y);
            rects.Enqueue(new IntRect(rect.from, new IntVector2(rect.to.x, newY)));
            rects.Enqueue(new IntRect(new IntVector2(rect.from.x, newY + 1), rect.to));

            var tunnelX = Random.Range(rect.from.x, rect.to.x + 1);
            SetTunnelNorth(tunnelX, newY);

            return true;
        }

        return false;
    }

    private void SetTunnelEast(int x, int y)
    {
        SetPass(new IntVector2(x, y), true);
        SetPass(new IntVector2(x + 1, y), true);
        SetTunnel(new IntVector2(x, y), new IntVector2(x + 1, y), true);
    }

    private void SetTunnelNorth(int x, int y)
    {
        SetPass(new IntVector2(x, y), true);
        SetPass(new IntVector2(x, y + 1), true);
        SetTunnel(new IntVector2(x, y), new IntVector2(x, y + 1), true);
    }

    //Borders are included
    private int CustomRandom(int from, int to)
    {
        var sample = 2f * Random.value - 1;
        //sample = -1f .. 1f
        sample = (to - from) * (Mathf.Pow(sample, 3) + 1f) / 2f + from;
        //(Mathf.Pow(sample, 3) + 1f) / 2f = 0 .. 1f
        //sample = from .. to
        return Mathf.FloorToInt(sample);
    }
}
