using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GenType
{
    HS, HV, Gray, Random, RandomGradient, Maze, MazeStep, AutoMaze, AutoMazeStop, MazeClear
}

public class TextureGenerator : MonoBehaviour {

    [SerializeField]
    private Slider SizeInput;

    [SerializeField]
    private Slider StepInput;

    private int size;
    private int Size
    {
        set
        {
            size = value;
            Maze = new ThickWalledMaze(Size, Size);
            ColorMap = new Color[Size * Size];
            Command(GenType.AutoMaze);
        }
        get
        {
            return size;
        }
    }

    [SerializeField]
    private Image targetImage;

    [SerializeField]
    private Texture2D outTexture;

    private ThickWalledMaze Maze;
    private bool NeedRedraw = false;
    private Color[] ColorMap;

    void Start ()
    {
        outTexture = new Texture2D(Size, Size, TextureFormat.ARGB32, false);
        outTexture.filterMode = FilterMode.Point;
        Size = 30;
        Command(GenType.AutoMaze);

        if (null != StepInput)
            StepInput.onValueChanged.AddListener(c => timeStep = Mathf.Pow(c, 4));

        if (null != SizeInput)
            SizeInput.onValueChanged.AddListener(c => Size = (int) c);
    }

    public void Command(GenType type)
    {
        NeedRedraw = true;

        switch (type)
        {
            default:
            case GenType.HS:
                for (int i = 0; i < Size * Size; i++)
                    ColorMap[i] = Color.HSVToRGB((i / Size) / (float)Size, (i % Size) / (float)Size, 1f);
                break;
            case GenType.HV:
                for (int i = 0; i < Size * Size; i++)
                    ColorMap[i] = Color.HSVToRGB((i / Size) / (float)Size, 1f, (i % Size) / (float)Size);
                break;
            case GenType.Random:
                for (int i = 0; i < Size * Size; i++)
                    ColorMap[i] = UnityEngine.Random.ColorHSV();
                break;
            case GenType.Gray:
                for (int i = 0; i < Size * Size; i++)
                    ColorMap[i] = UnityEngine.Random.ColorHSV(0f, 0f, 0f, 0f);
                break;
            case GenType.RandomGradient:
                var hue = UnityEngine.Random.value;
                for (int i = 0; i < Size * Size; i++)
                    ColorMap[i] = Color.HSVToRGB(hue, (i / Size) / (float)Size, (i % Size) / (float)Size);
                break;
            case GenType.Maze:
                Maze.Generate();
                MazeToColor();
                break;
            case GenType.MazeStep:
                Maze.NextStep();
                MazeToColor();
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
                MazeToColor();
                break;
        }
    }

    private float currentTime = 0;
    private float nextTime = 0;
    private bool isAutoMaze = false;
    private float timeStep = 0.1f;

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
                    MazeToColor();
                    isAutoMaze = false;
                    Debug.Log("Finished in " + currentTime + " s");
                    break;
                }
            }
        }

        if (NeedRedraw)
        {
            if (isAutoMaze)
                MazeToColor();

            Visualize(ColorMap);
        }
        NeedRedraw = false;
    }

    private void MazeToColor()
    {
        var passes = Maze.LinearMaze;
        var bytes = Maze.LinearMazeBytes;
        var current = Maze.LinearMazeCurrentCell;

        for (int i = 0; i < passes.Length; i++)
        {
            ColorMap[i] = Color.HSVToRGB(current[i] ? 0f : 0.6f, bytes[i] / 5f, passes[i] ? 1f : 0f);
            if (current[i])
                ColorMap[i] = Color.red;
        }
    }

    private void Visualize(Color[] newColors)
    {
        outTexture.Resize(Size, Size);
        outTexture.SetPixels(newColors);
        outTexture.Apply();
        targetImage.sprite = Sprite.Create(outTexture, new Rect(0, 0, Size, Size), new Vector2(0.5f, 0.5f));
    }
}
