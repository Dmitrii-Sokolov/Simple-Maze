using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
[ExecuteInEditMode]
public class TextRender : MonoBehaviour {

    [SerializeField]
    private string Format = "0.00";
    private Text text;

    private void OnEnable()
    {
        if (null == text)
            text = GetComponent<Text>();
    }
    
    public void SetText(float newText)
    {
        text.text = newText.ToString(Format);
    }
}
