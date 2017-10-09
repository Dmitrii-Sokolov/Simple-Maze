using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextureGenerator : MonoBehaviour
{
    [SerializeField]
    private Slider SizeInput;

    [SerializeField]
    private Slider StepInput;

    [SerializeField]
    private Image targetImage;

    [SerializeField]
    private Texture2D outTexture;


    public enum GenType
    {
        Maze, MazeStep, AutoMaze, AutoMazeStop, MazeClear
    }

    private float currentTime = 0;
    private float nextTime = 0;
    private float timeStep = 0.008f;
    private GenType lastCommand = GenType.AutoMaze;

    private bool isAutoMaze = false;
    private bool NeedRedraw = false;
    private ThickWalledMaze Maze;
    private Color[] ColorMap;

    private int size;
    private int Size
    {
        set
        {
            size = value;
            Maze = new ThickWalledMaze(Size, Size);
            ColorMap = new Color[Size * Size];
            outTexture.Resize(Size, Size);
            Command(lastCommand);
        }
        get
        {
            return size;
        }
    }

    void Start()
    {
        outTexture = new Texture2D(Size, Size, TextureFormat.ARGB32, false);
        outTexture.filterMode = FilterMode.Point;
        Size = 30;

        if (null != StepInput)
            StepInput.onValueChanged.AddListener(c => timeStep = Mathf.Pow(c, 4));

        if (null != SizeInput)
            SizeInput.onValueChanged.AddListener(c => Size = (int)c);
    }

    public void Command(GenType type)
    {
        lastCommand = type;
        NeedRedraw = true;

        switch (type)
        {
            default:
            case GenType.Maze:
                Maze.Generate();
                break;
            case GenType.MazeStep:
                Maze.NextStep();
                break;
            case GenType.AutoMaze:
                isAutoMaze = true;
                nextTime = 0;
                currentTime = 0;
                break;
            case GenType.AutoMazeStop:
                isAutoMaze = false;
                break;
            case GenType.MazeClear:
                Maze.Clear();
                break;
        }
    }

    private void Update()
    {
        currentTime += Time.deltaTime;

        if (isAutoMaze)
        {
            while (nextTime < currentTime)
            {
                NeedRedraw = true;
                nextTime += timeStep;
                if (!Maze.NextStep())
                {
                    isAutoMaze = false;
                    Debug.Log("Finished in " + currentTime + " s");
                    break;
                }
            }
        }

        if (NeedRedraw)
        {
            MazeToColor();
            Visualize();
            NeedRedraw = false;
        }
    }

    private void MazeToColor()
    {
        var maze = Maze.Maze;

        for (int i = 0; i < Size; i++)
            for (int n = 0; n < Size; n++)
                ColorMap[i + n*Size] = MazeStateToColor(maze[i, n]);
    }

    private Color MazeStateToColor(ThickWalledMaze.State state)
    {
        switch (state)
        {
            default:
            case ThickWalledMaze.State.Empty:
                return Color.blue;
            case ThickWalledMaze.State.Full:
                return Color.black;
            case ThickWalledMaze.State.Rig:
                return Color.red;
        }
    }


    private void Visualize()
    {
        //for (int i = 0; i < Size * Size; i++)
        //    ColorMap[i] = Color.HSVToRGB((i / Size) / (float)Size, (i % Size) / (float)Size, 1f);
        //for (int i = 0; i < Size * Size; i++)
        //    ColorMap[i] = Color.HSVToRGB((i / Size) / (float)Size, 1f, (i % Size) / (float)Size);
        //for (int i = 0; i < Size * Size; i++)
        //    ColorMap[i] = UnityEngine.Random.ColorHSV();
        //for (int i = 0; i < Size * Size; i++)
        //    ColorMap[i] = UnityEngine.Random.ColorHSV(0f, 0f, 0f, 0f);
        //var hue = UnityEngine.Random.value;
        //for (int i = 0; i < Size * Size; i++)
        //    ColorMap[i] = Color.HSVToRGB(hue, (i / Size) / (float)Size, (i % Size) / (float)Size);

        outTexture.SetPixels(ColorMap);
        outTexture.Apply();
        targetImage.sprite = Sprite.Create(outTexture, new Rect(0, 0, Size, Size), new Vector2(0.5f, 0.5f));
    }
}
