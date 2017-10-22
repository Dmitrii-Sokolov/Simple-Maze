using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//http://progressor-blog.ru/qt/generatsiya-labirinta-i-ego-prohozhdenie/
public class RecursiveDivisionThin : WalledMaze
{
    private struct Rect
    {
        public IntVector2 from;
        public IntVector2 to;

        public Rect(IntVector2 from, IntVector2 to)
        {
            this.from = from;
            this.to = to;
        }
    }

    public RecursiveDivisionThin(int width, int height)
    {
        SetSize(width, height);
    }

    Stack<Rect> rects = new Stack<Rect>();
    int stepNumber;

    public override void Clear()
    {
        base.Clear();
        CurrentCell = new IntVector2(-1, -1);
        stepNumber = 0;
        rects.Clear();
        rects.Push(new Rect(new IntVector2(0, 0), new IntVector2(Width - 1, Height - 1)));
    }

    public override bool NextStep()
    {
        if (rects.Count == 0)
            return false;

        var rect = rects.Pop();
        stepNumber++;

        if (rect.from.x == rect.to.x)
        {
            for (int i = rect.from.y; i < rect.to.y; i++)
            {
                SetPass(new IntVector2(rect.from.x, i), true);
                SetPass(new IntVector2(rect.from.x, i + 1), true);
                SetTunnel(new IntVector2(rect.from.x, i), new IntVector2(rect.from.x, i + 1), true);
            }
            return true;
        }

        if (rect.from.y == rect.to.y)
        {
            for (int i = rect.from.x; i < rect.to.x; i++)
            {
                SetPass(new IntVector2(i, rect.from.y), true);
                SetPass(new IntVector2(i + 1, rect.from.y), true);
                SetTunnel(new IntVector2(i, rect.from.y), new IntVector2(i + 1, rect.from.y), true);
            }
            return true;
        }

        if (stepNumber % 2 == 0)
        {
            var newX = CustomRandom(rect.from.x, rect.to.x);
            rects.Push(new Rect(new IntVector2(rect.from.x, rect.from.y), new IntVector2(newX, rect.to.y)));
            rects.Push(new Rect(new IntVector2(newX + 1, rect.from.y), new IntVector2(rect.to.x, rect.to.y)));

            var tunnelY = CustomRandom(rect.from.y, rect.to.y + 1);

            SetPass(new IntVector2(newX, tunnelY), true);
            SetPass(new IntVector2(newX + 1, tunnelY), true);
            SetTunnel(new IntVector2(newX, tunnelY), new IntVector2(newX + 1, tunnelY), true);
        }
        else
        {
            var newY = CustomRandom(rect.from.y, rect.to.y);
            rects.Push(new Rect(new IntVector2(rect.from.x, rect.from.y), new IntVector2(rect.to.x, newY)));
            rects.Push(new Rect(new IntVector2(rect.from.x, newY + 1), new IntVector2(rect.to.x, rect.to.y)));

            var tunnelX = CustomRandom(rect.from.x, rect.to.x + 1);

            SetPass(new IntVector2(tunnelX, newY), true);
            SetPass(new IntVector2(tunnelX, newY + 1), true);
            SetTunnel(new IntVector2(tunnelX, newY), new IntVector2(tunnelX, newY + 1), true);
        }

        return true;
    }

    private int CustomRandom(int from, int to)
    {
        var sample = 1.9999f * Random.value - 1;
        sample = (to - from) * (Mathf.Pow(sample, 3) + 1f) / 2f + from;
        return Mathf.FloorToInt(sample);
    }
}
