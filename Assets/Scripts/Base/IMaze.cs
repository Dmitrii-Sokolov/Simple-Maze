using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMaze
{
    void Clear();
    void SetSize(int size);
    void SetSize(Vector2Int size);

    int Width { get; }
    int Height { get; }

    Vector2Int CurrentCell { get; set; }

    bool IsInitialized { get; }

    Texture2D GetTexture();

    void Click(Vector2 point, float wallWidth);
}
