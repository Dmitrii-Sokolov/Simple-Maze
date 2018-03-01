using System;
using UnityEngine;

public class MazeGenerator<T> : IMazeGenerator where T: IMaze
{
    protected T maze;

    public void SetMaze(IMaze target)
    {
        if (target is T)
            SetMaze((T) target);
    }

    public void SetMaze(T target)
    {
        maze = target;
        Init();
    }

    protected virtual void Init()
    {
    }

    public void Generate()
    {
        if (maze == null)
        {
            Debug.LogError("Maze isn't set.");
            return;
        }

        while (NextStep()) ;
    }

    public virtual bool NextStep()
    {
        if (maze != null)
            return false;

        Debug.LogError("Maze isn't set.");
        return true;
    }
}
