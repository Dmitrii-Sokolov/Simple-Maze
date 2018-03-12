using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectangleMaze : IMaze
{
    protected static Color Temprorary = Color.yellow;
    protected static Color Rig = Color.red;
    protected static Color Full = Color.black;
    protected static Color Empty = Color.blue;
    protected static Color MinRangeColor = Color.green;
    protected static Color MaxRangeColor = 0.1f * Color.green;
    private bool initialized = false;

    public static Vector2Int[] Shifts = new Vector2Int[4]
    {
        Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down
    };

    protected Texture2D Texture { set; get; }

    public Texture2D GetTexture()
    {
        Texture.Apply();
        return Texture;
    }

    public bool IsInitialized
    {
        get
        {
            return initialized;
        }
    }

    public virtual Vector2Int CurrentCell { set; get; }

    public int Width { private set; get; }
    public int Height { private set; get; }

    protected bool[,] passes;
    protected int[,] steps;
    protected Queue<Vector2Int> stepsQueue = new Queue<Vector2Int>();
    protected int maxRange;

    public virtual void Click(Vector2 point, float wallWidth)
    {
    }

    public virtual void SetSize(int size)
    {
        SetSize(new Vector2Int(size, size));
    }

    public void SetSize(Vector2Int size)
    {
        Width = size.x;
        Height = size.y;
        Clear();
        initialized = true;
    }

    public virtual void Clear()
    {
    }

    public bool InMaze(Vector2Int cell)
    {
        return ((cell.x < Width) && (cell.x >= 0) && (cell.y < Height) && (cell.y >= 0));
    }
}
