using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateButton : MonoBehaviour {

    [SerializeField]
    private Transform listRoot;

    [SerializeField]
    private GameObject buttonPrefab;

    [SerializeField]
    private TextureGenerator generator;

    void Start ()
    {
        if (generator == null)
            Debug.LogError("GenerateButton : TextureGenerator isn't set");

        if (listRoot == null)
            Debug.LogError("GenerateButton : listRoot isn't set");

        if (buttonPrefab == null)
            Debug.LogError("GenerateButton : buttonPrefab isn't set");

        foreach (TextureGenerator.GenType element in Enum.GetValues(typeof(TextureGenerator.GenType)))
        {
            var newButton = Instantiate(buttonPrefab, listRoot);
            newButton.GetComponentInChildren<Text>().text = element.ToString();
            newButton.GetComponent<Button>().onClick.AddListener(() => generator.Command(element));
        }
    }
}
