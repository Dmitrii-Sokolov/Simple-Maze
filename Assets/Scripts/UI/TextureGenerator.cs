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
    private Maze Maze;

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

    public void SetType(MazeGeneratorType type)
    {
        Maze = new WalledMaze(Size, Size);

        NeedRedraw = true;
        Maze.Generator = MazeGeneratorExtensions.CreateGenerator(type, Size, Size);
    }

    public void Command(CommandType command)
    {
        lastCommand = command;
        NeedRedraw = true;

        switch (command)
        {
            default:
            case CommandType.Full:
                Maze.Generator.Generate();
                break;
            case CommandType.OneStep:
                Maze.Generator.NextStep();
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
                if (!Maze.Generator.NextStep())
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
