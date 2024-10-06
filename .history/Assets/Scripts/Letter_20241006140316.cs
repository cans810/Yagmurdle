using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;

public class Letter : MonoBehaviour
{
    public TMP_InputField letterTextComponent;
    public string letterText = "";
    public bool currentlyActive;
    public UnityEngine.Vector2 point;

    public void SetLetter(string letter)
    {
        letterText = letter;
        letterTextComponent.text = letter;
    }

    public void SetColor(Color color)
    {
        letterTextComponent.color = color;
    }
}
