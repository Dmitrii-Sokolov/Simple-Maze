using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//http://progressor-blog.ru/qt/generatsiya-labirinta-i-ego-prohozhdenie/
public class RecursiveUnionThin : MazeGenerator
{
    public void Generate() { while (NextStep()); }

    Queue<IntRect> rects = new Queue<IntRect>();
    private Maze maze;

    public void Init(Maze TargetMaze)
    {
        maze = TargetMaze;
        Init();
    }

    public void Init()
    {
        rects.Clear();
        rects.Enqueue(new IntRect(new Vector2Int(0, 0), new Vector2Int(maze.Width - 1, maze.Height - 1)));
    }

    public bool NextStep()
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
            rects.Enqueue(new IntRect(rect.from, new Vector2Int(newX, rect.to.y)));
            rects.Enqueue(new IntRect(new Vector2Int(newX + 1, rect.from.y), rect.to));

            var tunnelY = Random.Range(rect.from.y, rect.to.y + 1);
            SetTunnelEast(newX, tunnelY);

            return true;
        }

        if (rect.type == IntRect.Type.Vertical || rect.type == IntRect.Type.Square)
        {
            var newY = CustomRandom(rect.from.y, rect.to.y);
            rects.Enqueue(new IntRect(rect.from, new Vector2Int(rect.to.x, newY)));
            rects.Enqueue(new IntRect(new Vector2Int(rect.from.x, newY + 1), rect.to));

            var tunnelX = Random.Range(rect.from.x, rect.to.x + 1);
            SetTunnelNorth(tunnelX, newY);

            return true;
        }

        return false;
    }

    private void SetTunnelEast(int x, int y)
    {
        maze.SetPass(new Vector2Int(x, y), true);
        maze.SetPass(new Vector2Int(x + 1, y), true);
        maze.SetTunnel(new Vector2Int(x, y), new Vector2Int(x + 1, y), true);
    }

    private void SetTunnelNorth(int x, int y)
    {
        maze.SetPass(new Vector2Int(x, y), true);
        maze.SetPass(new Vector2Int(x, y + 1), true);
        maze.SetTunnel(new Vector2Int(x, y), new Vector2Int(x, y + 1), true);
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
