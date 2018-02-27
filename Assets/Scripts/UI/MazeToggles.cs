using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Reflection;

public class MazeToggles : MonoBehaviour
{
    [SerializeField]
    private Transform listRoot;

    [SerializeField]
    private GameObject togglePrefab;

    [SerializeField]
    private TextureGenerator generator;

    [SerializeField]
    private ToggleGroup group;

    [SerializeField]
    private string defaultToggle;

    void Start()
    {
        if (generator == null)
            Debug.LogError("GenerateToggle : TextureGenerator isn't set");

        if (listRoot == null)
            Debug.LogError("GenerateToggle : listRoot isn't set");

        if (togglePrefab == null)
            Debug.LogError("GenerateToggle : buttonPrefab isn't set");

        if (group == null)
            Debug.LogError("GenerateToggle : group isn't set");

        var mazeGeneratorTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IMaze))).ToArray();
        foreach (var element in mazeGeneratorTypes)
        {
            var newButton = Instantiate(togglePrefab, listRoot);
            newButton.GetComponentInChildren<Text>().text = element.ToString();
            newButton.GetComponent<Toggle>().onValueChanged.AddListener(c => { if (c) generator.SetMazeType(element); });
            newButton.GetComponent<Toggle>().group = group;

            if (element.ToString() == defaultToggle)
                newButton.GetComponent<Toggle>().isOn = true;
        }

        //listRoot.GetComponentInChildren<Toggle>().isOn = true;
    }
}
