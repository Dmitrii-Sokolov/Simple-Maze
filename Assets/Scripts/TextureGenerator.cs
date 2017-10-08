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
        HS, HV, Gray, Random, RandomGradient, Maze, MazeStep, AutoMaze, AutoMazeStop, MazeClear
    }

    private float currentTime = 0;
    private float nextTime = 0;
    private float timeStep = 0.008f;

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
        Command(GenType.AutoMaze);

        if (null != StepInput)
            StepInput.onValueChanged.AddListener(c => timeStep = Mathf.Pow(c, 4));

        if (null != SizeInput)
            SizeInput.onValueChanged.AddListener(c => Size = (int)c);
    }

    public void Command(GenType type)
    {
        switch (type)
        {
            default:
            case GenType.HS:
                for (int i = 0; i < Size * Size; i++)
                    ColorMap[i] = Color.HSVToRGB((i / Size) / (float)Size, (i % Size) / (float)Size, 1f);
                Visualize();
                isAutoMaze = false;
                break;
            case GenType.HV:
                for (int i = 0; i < Size * Size; i++)
                    ColorMap[i] = Color.HSVToRGB((i / Size) / (float)Size, 1f, (i % Size) / (float)Size);
                Visualize();
                isAutoMaze = false;
                break;
            case GenType.Random:
                for (int i = 0; i < Size * Size; i++)
                    ColorMap[i] = UnityEngine.Random.ColorHSV();
                Visualize();
                isAutoMaze = false;
                break;
            case GenType.Gray:
                for (int i = 0; i < Size * Size; i++)
                    ColorMap[i] = UnityEngine.Random.ColorHSV(0f, 0f, 0f, 0f);
                Visualize();
                isAutoMaze = false;
                break;
            case GenType.RandomGradient:
                var hue = UnityEngine.Random.value;
                for (int i = 0; i < Size * Size; i++)
                    ColorMap[i] = Color.HSVToRGB(hue, (i / Size) / (float)Size, (i % Size) / (float)Size);
                Visualize();
                isAutoMaze = false;
                break;
            case GenType.Maze:
                Maze.Generate();
                NeedRedraw = true;
                break;
            case GenType.MazeStep:
                Maze.NextStep();
                NeedRedraw = true;
                break;
            case GenType.AutoMaze:
                isAutoMaze = true;
                nextTime = 0;
                currentTime = 0;
                break;
            case GenType.AutoMazeStop:
                isAutoMaze = false;
                NeedRedraw = true;
                break;
            case GenType.MazeClear:
                Maze.Clear();
                NeedRedraw = true;
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

    private void Visualize()
    {
        outTexture.SetPixels(ColorMap);
        outTexture.Apply();
        targetImage.sprite = Sprite.Create(outTexture, new Rect(0, 0, Size, Size), new Vector2(0.5f, 0.5f));
    }
}
