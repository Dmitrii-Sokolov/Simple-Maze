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
    IntVector2 CurrentCell { get; set; }

    void PaintCell(IntVector2 cell);
    void PaintRig(IntVector2 cell);
    void PaintTemp(IntVector2 cell, bool temp);

    bool GetPass(IntVector2 cell);
    void SetPass(IntVector2 cell, bool pass);

    void SetTunnel(IntVector2 cell, IntVector2 to, bool tunnel);
    bool GetTunnel(IntVector2 cell, IntVector2 to);

    bool InMaze(IntVector2 cell);

    void Click(Vector2 point);
}
