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
    private RawImage targetRawImage;

    IMazeGenerator mazeGenerator;
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
    private IMaze Maze;

    private int size = 30;
    private int Size
    {
        set
        {
            size = value;
            Maze.SetSize(new Vector2Int(Size, Size));
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

    public void SetMazeType(Type type)
    {
        var probe = Activator.CreateInstance(type) as IMaze;
        if (probe != null)
        {
            Maze = probe;
            targetRawImage.material = Resources.Load<Material>(type.ToString()) ?? Graphic.defaultGraphicMaterial;
            Maze.SetSize(new Vector2Int(Size, Size));
        }
        else
            Debug.LogError("Invalid maze type match.");
    }

    public void SetGeneratorType(Type type)
    {
        var probe = Activator.CreateInstance(type) as IMazeGenerator;
        if (probe != null)
        {
            //Maze = new WalledMaze();
            //Maze.SetSize(new Vector2Int(Size, Size));
            //Вместо пересозданиия лучше просто очистить
            Maze.Clear();
            mazeGenerator = probe;
            mazeGenerator.SetMaze(Maze);
            NeedRedraw = true;
        }
        else
            Debug.LogError("Invalid maze type match.");
    }

    public void Command(CommandType command)
    {
        lastCommand = command;
        NeedRedraw = true;

        switch (command)
        {
            default:
            case CommandType.Full:
                mazeGenerator.Generate();
                break;
            case CommandType.OneStep:
                mazeGenerator.NextStep();
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
                mazeGenerator.SetMaze(Maze);
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
                if (!mazeGenerator.NextStep())
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
        targetRawImage.texture = Maze.GetTexture();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var localPos = (eventData.pressPosition - (Vector2)rectTransform.position) - rectTransform.rect.position;
        var relativePos = new Vector2(localPos.x / rectTransform.rect.width, localPos.y / rectTransform.rect.height);
        Maze.Click(relativePos);
        Visualize();
    }
}
