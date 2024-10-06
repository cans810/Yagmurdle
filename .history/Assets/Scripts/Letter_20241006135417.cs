using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;

public class Letter : MonoBehaviour
{
    public TextMeshProUGUI letterTextComponent;
    public string letterText = "";
    public bool currentlyActive;
    public Vector2 vertices;

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

