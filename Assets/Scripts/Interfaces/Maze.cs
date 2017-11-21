using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Maze
{
    void Clear();
    void SetSize(int width, int height);

    int OutTextureWidth { get; }
    int OutTextureHeight { get; }

    int Width { get; }
    int Height { get; }

    MazeGenerator Generator { set; get; }
    Texture2D Texture {get;}

    bool GetPass(IntVector2 cell);
    void SetPass(IntVector2 cell, bool pass);

    void SetTunnel(IntVector2 cell, IntVector2 to, bool tunnel);
    bool GetTunnel(IntVector2 cell, IntVector2 to);

    void Click(Vector2 point);
}
