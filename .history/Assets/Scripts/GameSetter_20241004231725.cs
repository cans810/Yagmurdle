using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSetter : MonoBehaviour
{
    public GameObject canvas;
    public GameObject LetterObjectPrefab;
    public string targetWord;  // The word that players have to guess
    public Transform originalPos;
    public float letterXOffset;
    public float letterYOffset;

    public bool gameGenerated;
    public bool deleteGame;
    public int currentRowFilling;
    public GameObject currentLetterObjectFilling;
    public List<GameObject> letterObjects;
    public List<GameObject> currentRowObjects;

    private int maxGuesses = 6;  // Like Wordle, give players 6 attempts
    private int currentGuess = 0; // Track the number of guesses made so far

    // Start is called before the first frame update
    void Start()
    {
        letterXOffset = 1;
        letterYOffset = 1;
        gameGenerated = false;
        deleteGame = false;
        currentRowFilling = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameGenerated)
        {
            GenerateGrid(targetWord.Length);  // Create the grid for the word size
            gameGenerated = true;
        }

        if (deleteGame)
        {
            DeleteGrid();
            deleteGame = false;
        }

        if (gameGenerated)
        {
            // Example: if Enter is pressed, evaluate the current row's guess
            if (Input.GetKeyDown(KeyCode.Return))
            {
                CheckGuess();
            }

            // Example: Input letters for the current guess (in a real game, use an input field)
            if (Input.anyKeyDown)
            {
                HandleLetterInput();
            }
        }
    }

    void GenerateGrid(int gridSize)
    {
        float totalGridWidth = (gridSize - 1) * letterXOffset;
        float totalGridHeight = (maxGuesses - 1) * letterYOffset;

        Vector3 gridStartPos = new Vector3(
            originalPos.position.x - totalGridWidth / 2f,
            originalPos.position.y - totalGridHeight / 2f,
            originalPos.position.z
        );

        for (int i = 0; i < maxGuesses; i++)  // 6 guesses like Wordle
        {
            for (int j = 0; j < gridSize; j++)  // One row for each guess
            {
                GameObject letterObject = Instantiate(LetterObjectPrefab, canvas.transform);

                letterObject.transform.position = new Vector3(
                    gridStartPos.x + (j * letterXOffset),
                    gridStartPos.y + (i * letterYOffset),
                    gridStartPos.z
                );

                letterObject.GetComponent<Letter>().rowBelongedTo = i;  // Track the row of the letter
                letterObjects.Add(letterObject);
            }
        }

        currentRowFilling = 0;
        currentRowObjects = GetCurrentRowObjects();
    }

    List<GameObject> GetCurrentRowObjects()
    {
        List<GameObject> rowObjects = new List<GameObject>();

        foreach (GameObject letterObj in letterObjects)
        {
            if (letterObj.GetComponent<Letter>().rowBelongedTo == currentGuess)
            {
                rowObjects.Add(letterObj);
            }
        }

        return rowObjects;
    }

    void CheckGuess()
    {
        if (currentRowFilling < targetWord.Length) return;  // Ensure the player has input all letters

        string playerGuess = "";

        // Build the player's guess string from the current row's letters
        foreach (GameObject letterObject in currentRowObjects)
        {
            playerGuess += letterObject.GetComponent<Letter>().letterText;
        }

        // Compare the player's guess with the target word
        for (int i = 0; i < targetWord.Length; i++)
        {
            GameObject letterObject = currentRowObjects[i];
            Letter letterComponent = letterObject.GetComponent<Letter>();
            char guessedLetter = playerGuess[i];

            if (guessedLetter == targetWord[i])
            {
                // Correct letter and correct position (green)
                letterComponent.SetColor(Color.green);
            }
            else if (targetWord.Contains(guessedLetter.ToString()))
            {
                letterComponent.SetColor(Color.yellow);
            }
            else
            {
                letterComponent.SetColor(Color.gray);
            }
        }

        currentGuess++;

        if (playerGuess == targetWord)
        {
            Debug.Log("Player guessed the word correctly!");
        }
        else if (currentGuess >= maxGuesses)
        {
            Debug.Log("Player ran out of guesses!");
        }
        else
        {
            currentRowFilling = 0;
            currentRowObjects = GetCurrentRowObjects();
        }
    }

    void HandleLetterInput()
    {
        if (currentRowFilling < targetWord.Length && Input.inputString.Length > 0)
        {
            char inputChar = Input.inputString[0];

            if (char.IsLetter(inputChar))
            {
                currentRowObjects[currentRowFilling].GetComponent<Letter>().SetLetter(inputChar.ToString().ToUpper());
                currentRowFilling++;
            }
        }
    }

    void DeleteGrid()
    {
        foreach (Transform child in canvas.transform)
        {
            Destroy(child.gameObject);
        }
        letterObjects.Clear();
        gameGenerated = false;
        currentGuess = 0;
    }
}
