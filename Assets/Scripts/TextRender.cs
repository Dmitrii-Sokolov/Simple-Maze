using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextRender : MonoBehaviour {

    [SerializeField]
    private string Format = "0.00";
    private Text text;

	void Start ()
    {
        text = GetComponent<Text>();
	}

    public void SetText(float newText)
    {
        text.text = newText.ToString(Format);
    }
}
