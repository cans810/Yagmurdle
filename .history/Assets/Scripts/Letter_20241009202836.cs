using System.Collections;
using System.Collections.Generic;
using TMPro; // Make sure you're using TextMeshPro namespace
using UnityEngine;
using UnityEngine.UI;

public class Letter : MonoBehaviour
{
    public TMP_InputField inputField;
    public string letterText = "";
    public bool currentlyActive = false;
    public Vector2 point; // Changed to UnityEngine.Vector2 to avoid confusion
    public bool isAnEdge;
    public Image bg;

    void Start()
    {
        // Restrict input to one character
        inputField.characterLimit = 1;

        // Hide the caret if it's not needed (set caret width to zero)
        inputField.caretWidth = 0;

        // Optionally, disable the caret entirely if not required
        inputField.onValueChanged.AddListener(delegate { OnInputChanged(); });
    }

    // Called when the user types into the input field
    void OnInputChanged()
    {
        // Ensure only one character is entered
        if (inputField.text.Length > 1)
        {
            inputField.text = inputField.text.Substring(0, 1);
        }
    }

    public void SetLetter(string letter)
    {
        letterText = letter;
        inputField.text = letter;
    }

    public void SetColor(Color color)
    {
        bg.color = color;
    }
}
