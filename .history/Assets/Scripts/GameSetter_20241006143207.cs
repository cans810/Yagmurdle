using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetter : MonoBehaviour
{
    public GameObject canvas;
    public GameObject LetterObjectPrefab;
    public string randomWord;
    public Transform originalPos;
    public float letterXOffset;
    public float letterYOffset;

    public bool gameGenerated;
    public bool deleteGame;

    int gridSize;

    public GameObject currentLetterObjectFilling;
    public List<GameObject> letterObjects;
    public Vector2 currentPoint;
    public int i;
    public int j;

    void Start()
    {
        letterXOffset = 1;
        letterYOffset = 1;  
        i = 0;
        j = 0;
        currentPoint = new Vector2(i, j);
    }

    void Update()
    {
        if (!gameGenerated)
        {
            GenerateGame();
        }

        if (deleteGame)
        {
            DeleteGame();
        }

        // Game loop logic
        if (gameGenerated)
        {
            UpdateActiveLetter();
        }
    }

    void GenerateGame()
    {
        string[] randomWordArray = randomWord.Split();

        gridSize = randomWord.Length; 

        float totalGridWidth = (gridSize - 1) * letterXOffset;
        float totalGridHeight = (gridSize - 1) * letterYOffset;

        Vector3 gridStartPos = new Vector3(
            originalPos.position.x - totalGridWidth / 2f,
            originalPos.position.y - totalGridHeight / 2f,
            originalPos.position.z
        );

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                GameObject letterObject = Instantiate(LetterObjectPrefab, canvas.transform);

                letterObject.transform.position = new Vector3(
                    gridStartPos.x + (j * letterXOffset),
                    gridStartPos.y - (i * letterYOffset),
                    gridStartPos.z
                );

                letterObjects.Add(letterObject);

                letterObject.GetComponent<Letter>().point = new Vector2(i, j);

                if (j == gridSize - 1)
                {
                    letterObject.GetComponent<Letter>().isAnEdge = true;
                }
            }
        }

        gameGenerated = true;

        letterObjects[0].GetComponent<Letter>().currentlyActive = true;
        i = 0;
        j = 0;
        currentPoint = new Vector2(i, j);
    }

    void DeleteGame()
    {
        Transform parentTransform = canvas.transform;

        foreach (Transform child in parentTransform)
        {
            if (child.GetComponent<Letter>() != null)
            {
                Destroy(child.gameObject);
            }
        }

        letterObjects.Clear();
        gameGenerated = false;
        deleteGame = false;
    }

    void UpdateActiveLetter()
    {
        Transform parentTransform = canvas.transform;

        foreach (Transform child in parentTransform)
        {
            if (child.tag == "Letter")
            {
                // enable input for the currently active letter
                Letter letter = child.GetComponent<Letter>();
                if (letter.currentlyActive)
                {
                    letter.inputField.enabled = true;
                }
                else
                {
                    letter.inputField.enabled = false;
                }

                // move to the next letter if text is entered
                if (letter.currentlyActive && !string.IsNullOrEmpty(letter.inputField.text))
                {
                    if (j == gridSize - 1)
                    {
                        j = 0;
                        i++;
                    }
                    else
                    {
                        j++;
                    }

                    currentPoint = new Vector2(i, j);

                    letter.currentlyActive = false;

                    foreach (Transform nextChild in parentTransform)
                    {
                        if (nextChild.tag == "Letter" && nextChild.GetComponent<Letter>().point == currentPoint)
                        {
                            nextChild.GetComponent<Letter>().currentlyActive = true;
                            nextChild.GetComponent<Letter>().inputField.enabled = true;
                            break;
                        }
                    }
                }
            }
        }
    }
}

