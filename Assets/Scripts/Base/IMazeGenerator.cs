public interface IMazeGenerator
{
    void SetMaze(IMaze target);

    void Generate();

    bool NextStep();
}
