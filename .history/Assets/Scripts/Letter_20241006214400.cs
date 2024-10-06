using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Letter : MonoBehaviour
{
    public TMP_InputField inputField;
    public string letterText = " ";
    public bool currentlyActive = false;
    public UnityEngine.Vector2 point;
    public bool isAnEdge;

    public Image bg;

    public void Start(){
        inputField.caretWidth = 0;
    }

    public void SetLetter(string letter)
    {
        letterText = letter;
        inputField.text = letter;
    }

    public void SetColor(Color color){
        bg.color = color;
    }
}

