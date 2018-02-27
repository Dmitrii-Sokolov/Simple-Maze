using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMazeGenerator
{
    void SetMaze(IMaze TargetMaze);
    void Init();

    void Generate();
    bool NextStep();
}
