using UnityEngine;

public interface IMazeGenerator
{
    void SetSize(int size);

    void SetSize(Vector2Int size);

    IMaze Maze { get;}

    void Clear();

    void Generate();

    bool NextStep();
}
