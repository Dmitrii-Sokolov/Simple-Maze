using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMaze
{
    void Clear();
    void SetSize(int size);
    void SetSize(Vector2Int size);

    Material Material { set; }
    float WallWidth { set; get; }

    int OutTextureWidth { get; }
    int OutTextureHeight { get; }

    int Width { get; }
    int Height { get; }

    Texture2D Texture {get;}
    Vector2Int CurrentCell { get; set; }

    Texture2D GetTexture();

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
