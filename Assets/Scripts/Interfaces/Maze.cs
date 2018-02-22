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
    Vector2Int CurrentCell { get; set; }

    void PaintCell(Vector2Int cell);
    void PaintRig(Vector2Int cell);
    void PaintTemp(Vector2Int cell, bool temp);

    bool GetPass(Vector2Int cell);
    void SetPass(Vector2Int cell, bool pass);

    void SetTunnel(Vector2Int cell, Vector2Int to, bool tunnel);
    bool GetTunnel(Vector2Int cell, Vector2Int to);

    bool InMaze(Vector2Int cell);

    void Click(Vector2 point);
}
