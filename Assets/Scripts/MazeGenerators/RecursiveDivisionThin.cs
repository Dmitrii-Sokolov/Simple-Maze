using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//http://progressor-blog.ru/qt/generatsiya-labirinta-i-ego-prohozhdenie/
public class RecursiveDivisionThin : WalledMaze
{
    private struct Rect
    {
        public readonly IntVector2 from;
        public readonly IntVector2 to;
        public readonly Type type;

        public enum Type
        {
            VerticalLine, Vertical, Square, Horizontal, HorizontalLine, Undefined
        }

        public Rect(IntVector2 from, IntVector2 to)
        {
            this.from = from;
            this.to = to;

            if (from.x == to.x)
                type = Type.VerticalLine;
            else if (from.y == to.y)
                type = Type.HorizontalLine;
            else
            {
                var aspect = (to.y - from.y) - (to.x - from.x);
                if (aspect > 0)
                    type = Type.Vertical;
                else if (aspect < 0)
                    type = Type.Horizontal;
                else if (aspect == 0)
                    type = Type.Square;
                else
                    type = Type.Undefined;
            }
        }

        public int Height
        {
            get
            {
                return to.y - from.y;
            }
        }

        public int Width
        {
            get
            {
                return to.x - from.x;
            }
        }
    }

    public RecursiveDivisionThin(int width, int height)
    {
        SetSize(width, height);
    }

    Queue<Rect> rects = new Queue<Rect>();
    int stepNumber;

    public override void Clear()
    {
        base.Clear();
        CurrentCell = new IntVector2(-1, -1);
        stepNumber = 0;
        rects.Clear();
        rects.Enqueue(new Rect(new IntVector2(0, 0), new IntVector2(Width - 1, Height - 1)));
    }

    public override bool NextStep()
    {
        if (rects.Count == 0)
            return false;

        var rect = rects.Dequeue();
        stepNumber++;

        if (rect.type == Rect.Type.VerticalLine)
        {
            for (int i = rect.from.y; i < rect.to.y; i++)
                SetTunnelNorth(rect.from.x, i);
            return true;
        }

        if (rect.type == Rect.Type.HorizontalLine)
        {
            for (int i = rect.from.x; i < rect.to.x; i++)
                SetTunnelEast(i, rect.from.y);
            return true;
        }

        if (rect.type == Rect.Type.Horizontal || (rect.type == Rect.Type.Square && Random.value > 0.5f))
        {
            var newX = CustomRandom(rect.from.x, rect.to.x);
            rects.Enqueue(new Rect(rect.from, new IntVector2(newX, rect.to.y)));
            rects.Enqueue(new Rect(new IntVector2(newX + 1, rect.from.y), rect.to));

            var tunnelY = Random.Range(rect.from.y, rect.to.y);
            SetTunnelEast(newX, tunnelY);

            return true;
        }

        if (rect.type == Rect.Type.Vertical || rect.type == Rect.Type.Square)
        {
            var newY = CustomRandom(rect.from.y, rect.to.y);
            rects.Enqueue(new Rect(rect.from, new IntVector2(rect.to.x, newY)));
            rects.Enqueue(new Rect(new IntVector2(rect.from.x, newY + 1), rect.to));

            var tunnelX = Random.Range(rect.from.x, rect.to.x);
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
