using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TextureGenerator : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private Slider SizeInput;

    [SerializeField]
    private Slider StepInput;

    [SerializeField]
    private Image targetImage;

    RectTransform rectTransform;

    public enum GenerateType
    {
        RecursiveDivision, RecursiveUnion, Prim, Kruskal, Wilson, AldousBroder, HuntAndKill, Sidewinder, BinaryTree, DeepSearch, DeepSearchT, RandomString
    }

    public enum CommandType
    {
        AutoStep, OneStep, Stop, Clear, Full
    }

    private float currentTime = 0;
    private float nextTime = 0;
    private float timeStep = 0.008f;
    private CommandType lastCommand = CommandType.AutoStep;

    private bool isAutoMaze = true;
    private bool NeedRedraw = true;
    private CellMaze Maze;

    private int size = 30;
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
        rectTransform = gameObject.GetComponent<RectTransform>();

        if (null != StepInput)
            StepInput.onValueChanged.AddListener(c => timeStep = Mathf.Pow(c, 4));

        if (null != SizeInput)
            SizeInput.onValueChanged.AddListener(c => Size = (int)c);
    }

    public void SetType(GenerateType type)
    {
        NeedRedraw = true;
        switch (type)
        {
            default:
            case GenerateType.RecursiveDivision:
                Maze = new RecursiveDivisionThin(Size, Size);
                break;
            case GenerateType.RecursiveUnion:
                Maze = new RecursiveUnionThin(Size, Size);
                break;
            case GenerateType.Prim:
                Maze = new PrimThin(Size, Size);
                break;
            case GenerateType.Kruskal:
                Maze = new KruskalThin(Size, Size);
                break;
            case GenerateType.Wilson:
                Maze = new WilsonThin(Size, Size);
                break;
            case GenerateType.AldousBroder:
                Maze = new AldousBroderThin(Size, Size);
                break;
            case GenerateType.HuntAndKill:
                Maze = new HuntAndKillThin(Size, Size);
                break;
            case GenerateType.Sidewinder:
                Maze = new SidewinderThin(Size, Size);
                break;
            case GenerateType.BinaryTree:
                Maze = new BinaryTreeThin(Size, Size);
                break;
            case GenerateType.DeepSearch:
                Maze = new DeepSearchThin(Size, Size);
                break;
            case GenerateType.DeepSearchT:
                Maze = new DeepSearchThick(Size, Size);
                break;
            case GenerateType.RandomString:
                Maze = new RandomStringThin(Size, Size);
                break;
        }
    }

    public void Command(CommandType command)
    {
        lastCommand = command;
        NeedRedraw = true;

        switch (command)
        {
            default:
            case CommandType.Full:
                Maze.Generate();
                break;
            case CommandType.OneStep:
                Maze.NextStep();
                break;
            case CommandType.AutoStep:
                isAutoMaze = true;
                nextTime = 0;
                currentTime = 0;
                break;
            case CommandType.Stop:
                isAutoMaze = false;
                break;
            case CommandType.Clear:
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

    public void OnPointerClick(PointerEventData eventData)
    {
        var localPos = (eventData.pressPosition - (Vector2)rectTransform.position) - rectTransform.rect.position;
        var relativePos = new Vector2(localPos.x / rectTransform.rect.width, localPos.y / rectTransform.rect.height);
        Maze.Click(relativePos);
        Visualize();
    }
}
