using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TextureGenerator : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private Slider SpeedInput;

    [SerializeField]
    private Slider SizeInput;

    [SerializeField]
    private Slider WallWidthInput;

    [SerializeField]
    private RawImage targetRawImage;

    [SerializeField]
    private GeneratorToggles generatorToggles;

    IMazeGenerator mazeGenerator;
    RectTransform rectTransform;

    public enum CommandType
    {
        AutoStep, OneStep, Stop, Clear, Full
    }

    private float currentTime = 0;
    private float currentStep = 0;
    private float stepsCount = 0;

    private CommandType lastCommand = CommandType.AutoStep;

    private bool isAutoMaze = true;
    private bool NeedRedraw = true;
    private IMaze Maze;

    private float StepsPerSecond
    {
        get
        {
            return SpeedInput.value;
        }
    }

    private int Size
    {
        set
        {
            if (null != Maze)
            {
                Maze.SetSize(value);
                if (mazeGenerator != null)
                    mazeGenerator.SetMaze(Maze);
            }
            Command(lastCommand);
        }
        get
        {
            return (int) SizeInput.value;
        }
    }

    private float WallWidth
    {
        set
        {
            if (null != Maze)
                Maze.WallWidth = value;
        }
        get
        {
            return WallWidthInput.value;
        }
    }

    void Start()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();

        if (null != SizeInput)
            SizeInput.onValueChanged.AddListener(c => Size = (int)c);

        if (null != WallWidthInput)
            WallWidthInput.onValueChanged.AddListener(c => WallWidth = c);
    }

    public void SetMazeType(Type type)
    {
        var probe = Activator.CreateInstance(type) as IMaze;
        if (probe != null)
        {
            Maze = probe;
            targetRawImage.material = Resources.Load<Material>(type.ToString()) ?? Graphic.defaultGraphicMaterial;
            Maze.SetSize(Size);
            Maze.Material = targetRawImage.material;
            Maze.WallWidth = WallWidth;
            NeedRedraw = true;

            generatorToggles.Generate(type);
        }
        else
            Debug.LogError("Invalid maze type match.");
    }

    public void SetGeneratorType(Type type)
    {
        var probe = Activator.CreateInstance(type) as IMazeGenerator;
        if (probe != null)
        {
            mazeGenerator = probe;
            NeedRedraw = true;

            if (Maze != null)
            {
                Maze.Clear();
                mazeGenerator.SetMaze(Maze);
            }
        }
        else
            Debug.LogError("Invalid maze generator type match.");
    }

    public void Command(CommandType command)
    {
        if (mazeGenerator == null || Maze == null)
            return;

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
                stepsCount = 0f;
                currentStep = 0f;
                currentTime = 0f;
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
        stepsCount += Time.deltaTime * StepsPerSecond;

        if (isAutoMaze)
        {
            UnityEngine.Profiling.Profiler.BeginSample(string.Format("Profiling Generator: {0}", mazeGenerator.GetType().ToString()));
            while (currentStep < stepsCount)
            {
                NeedRedraw = true;
                currentStep++;
                if (!mazeGenerator.NextStep())
                {
                    isAutoMaze = false;
                    Debug.Log("Finished in " + currentTime.ToString("0.000") + "s and " + currentStep.ToString("### ###") + " steps.");
                    break;
                }
            }
            UnityEngine.Profiling.Profiler.EndSample();
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
