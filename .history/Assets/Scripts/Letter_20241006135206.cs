using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Letter : MonoBehaviour
{
    public TextMeshProUGUI letterTextComponent;
    public string letterText = "";
    public bool filled;
    public int rowBelongedTo;

    public bool currentlyActive;

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

