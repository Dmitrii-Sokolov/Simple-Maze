using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMazeGenerator
{
    void Init(IMaze TargetMaze);
    void Init();
    void Generate();
    bool NextStep();
}
