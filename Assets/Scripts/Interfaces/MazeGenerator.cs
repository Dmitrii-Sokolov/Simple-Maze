using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface MazeGenerator
{
    void Init(Maze TargetMaze);
    void Init();
    void Generate();
    bool NextStep();
}
