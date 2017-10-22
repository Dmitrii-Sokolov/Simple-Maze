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


    public enum GenType
    {
        SetWilsonThin, SetAldousBroderThin, SetHuntAndKill, SetSidewinder, SetBinaryTree, SetDeepSearchThin, SetDeepSearchThick, Maze, MazeStep, AutoMaze, AutoMazeStop, MazeClear
    }

    private float currentTime = 0;
    private float nextTime = 0;
    private float timeStep = 0.008f;
    private GenType lastCommand = GenType.AutoMaze;

    private bool isAutoMaze = false;
    private bool NeedRedraw = false;
    private CellMaze Maze;

    private int size;
    private int Size
    {
        set
        {
            size = value;
            Maze.SetSize(Size, Size);
            Command(lastCommand);
        }
        get
        {
            return size;
        }
    }

    void Start()
    {
        Maze = new AldousBroderThin();
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
            case GenType.SetWilsonThin:
                Maze = new WilsonThin(Size, Size);
                break;
            case GenType.SetAldousBroderThin:
                Maze = new AldousBroderThin(Size, Size);
                break;
            case GenType.SetHuntAndKill:
                Maze = new HuntAndKillThin(Size, Size);
                break;
            case GenType.SetSidewinder:
                Maze = new SidewinderThin(Size, Size);
                break;
            case GenType.SetBinaryTree:
                Maze = new BinaryTreeThin(Size, Size);
                break;
            case GenType.SetDeepSearchThin:
                Maze = new DeepSearchThin(Size, Size);
                break;
            case GenType.SetDeepSearchThick:
                Maze = new DeepSearchThick(Size, Size);
                break;
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
            Visualize();
            NeedRedraw = false;
        }
    }


    private void Visualize()
    {
        //var ColorMap = new Color[Size * Size];
        //var outTexture = new Texture2D(Size, Size, TextureFormat.ARGB32, false);
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
        //outTexture.SetPixels(ColorMap);
        //outTexture.Apply();

        Maze.Texture.Apply();
        targetImage.sprite = Sprite.Create(Maze.Texture, new Rect(0, 0, Maze.OutTextureWidth, Maze.OutTextureHeight), new Vector2(0.5f, 0.5f));
    }
}
