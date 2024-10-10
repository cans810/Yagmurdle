using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Letter : MonoBehaviour
{
    public TMP_InputField inputField;
    public string letterText = "";
    public bool currentlyActive = false;
    public Vector2 point;
    public bool isAnEdge;
    public Image bg;

    void Start()
    {
        // Restrict input to one character
        inputField.characterLimit = 1;

        // Hide the caret if it's not needed (set caret width to zero)
        inputField.caretWidth = 0;

        // Add input validation
        inputField.onValidateInput += ValidateInput;

        // Optionally, disable the caret entirely if not required
        inputField.onValueChanged.AddListener(delegate { OnInputChanged(); });
    }

    // Called when the user types into the input field
    void OnInputChanged()
    {
        // Ensure only one character is entered
        if (inputField.text.Length > 0)
        {
            inputField.text = inputField.text.Substring(0, 1).ToUpper();
            letterText = inputField.text;
        }
        else
        {
            letterText = "";
        }
    }

    public void SetLetter(string letter)
    {
        letterText = letter.ToUpper();
        inputField.text = letterText;
    }

    public void SetColor(Color color)
    {
        bg.color = color;
    }

    public char ValidateInput(string text, int charIndex, char addedChar)
    {
        // Allow only letters (both lowercase and uppercase)
        if (char.IsLetter(addedChar))
        {
            return char.ToUpper(addedChar); // Convert to uppercase
        }
        return '\0'; // Return null character for any non-letter input
    }
}