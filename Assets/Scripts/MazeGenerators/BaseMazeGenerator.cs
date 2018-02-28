using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMazeGenerator : IMazeGenerator
{
    protected IMaze maze;

    public void Generate() { while (NextStep()); }

    public void SetMaze(IMaze target)
    {
        maze = target;
        Init();
    }

    public virtual void Init()
    {
    }

    public virtual bool NextStep()
    {
        return false;
    }
}
