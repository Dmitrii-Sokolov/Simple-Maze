using System;
using UnityEngine;

public class MazeGenerator<T> : IMazeGenerator where T: IMaze
{
    protected T maze = (T)Activator.CreateInstance(typeof(T));
    public IMaze Maze { get { return maze; } }

    public void SetSize(int size)
    {
        maze.SetSize(size);
        Init();
    }
    public void SetSize(Vector2Int size)
    {
        maze.SetSize(size);
        Init();
    }

    public void Clear()
    {
        if (maze.IsInitialized)
        {
            maze.Clear();
            Init();
        }
        else
            Debug.LogError("Maze isn't set.");
    }

    protected virtual void Init()
    {
    }

    public void Generate()
    {
        if (maze.IsInitialized)
            while (NextStep());
        else
            Debug.LogError("Maze isn't set.");
    }

    public virtual bool NextStep()
    {
        if (maze.IsInitialized)
            return false;
        else
        {
            Debug.LogError("Maze isn't set.");
            return true;
        }
    }
}
