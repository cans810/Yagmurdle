using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameSetter : MonoBehaviour
{
    string[] turkishWords = new string[]
    {
        "AĞAÇ", "AİLE", "AKIL", "AKŞAM", "ARAÇ", 
        "ARKADAŞ", "AŞK", "AYAK", "BALIK", "BAHÇE", 
        "BARIŞ", "BAŞARI", "BAŞKA", "BAŞLANGIÇ", "BEDEN", 
        "BİR", "BİYOLOJİ", "CENNET", "ÇİÇEK", "DAHA", 
        "DENİZ", "DERS", "DÜNYA", "EĞİTİM", "EKMEK", 
        "ELLER", "GÖZ", "GÜNEŞ", "HAVA", "HAYAT", 
        "İLK", "İNSAN", "KEDİ", "KIŞ", "KÖPEK", 
        "KÜTÜPHANE", "MASA", "MEYVE", "MÜZİK", "ODALAR", 
        "OYUN", "ÖĞRENCİ", "PASTA", "RÜZGAR", "SAHİL", 
        "SANAT", "SICAK", "SÖZLÜK", "ŞEHİR", "TARİH", 
        "TARİF", "UÇAK", "UZAK", "YILDIZ", "YÜZ", 
        "YAZ", "YEMEK", "ZAMAN", "ZEYTİN", "AŞKIN", 
        "ÇOCUK", "HEDİYE", "YARIŞ", "HAVAALANI", "ÖĞRETMEN", 
        "YAVAŞ", "YAĞMUR", "OKUL", "SINIF", "BİLİM", 
        "FUTBOL", "KÜTÜK", "TAHTA", "ÜLKE", "ŞARKI", 
        "KÜTÜPHANE", "KEDİ", "RENK", "SES", "HAYAL", 
        "FİKİR", "ŞANS", "SÜRE", "DOSTLUK", "SEVGİ", 
        "TATLI", "SÖZ", "KORKU", "KÜÇÜK", "BÜYÜK", 
        "FIRTINA", "MOLA", "ORMAN", "KÖY", "DENİZ", 
        "KÖFTE", "ÇORBA", "DONDURMA", "BAHAR", "SPOR"
    };

    public GameObject canvas;
    public GameObject LetterObjectPrefab;
    public string randomWord;
    public Transform originalPos;
    public float letterXOffset;
    public float letterYOffset;

    public bool gameGenerated;
    public bool deleteGame;

    public Dictionary<int, string> givenAnswers = new Dictionary<int, string>();

    public List<GameObject> letterObjects;
    public Vector2 currentPoint;
    public int i;
    public int j;

    private int gridSize;

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

        if (gameGenerated)
        {
            UpdateActiveLetter();
        }
    }

    public void GenerateRandomWord()
    {
        int randomWordIndex = UnityEngine.Random.Range(0, turkishWords.Length);
        randomWord = turkishWords[randomWordIndex];
    }

    void GenerateGame()
    {
        GenerateRandomWord();

        gridSize = randomWord.Length;

        // Calculate the grid's starting position
        Vector3 gridStartPos = CalculateGridStartPos();

        // Instantiate letter objects
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
            }
        }

        gameGenerated = true;

        // Automatically set the first letter as active
        ActivateLetterAt(0, 0);
        SetFocusOnLetter(currentPoint);
    }

    Vector3 CalculateGridStartPos()
    {
        float totalGridWidth = (gridSize - 1) * letterXOffset;
        float totalGridHeight = (gridSize - 1) * letterYOffset;

        Vector3 screenCenter = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));

        screenCenter.z = originalPos.position.z;

        return new Vector3(
            screenCenter.x - totalGridWidth / 2f,
            screenCenter.y + totalGridHeight / 2f,
            screenCenter.z
        );
    }


    void DeleteGame()
    {
        foreach (Transform child in canvas.transform)
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
                Letter letter = child.GetComponent<Letter>();

                if (letter.currentlyActive)
                {
                    letter.inputField.enabled = true;

                    // Check if Enter key is pressed
                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        // Complete the row, check the letters, and move to the next row
                        if (j == gridSize - 1) 
                        {
                            string answer = "";

                            // Gather the letters in the current row
                            foreach (Transform c in parentTransform)
                            {
                                if (c.tag == "Letter" && c.GetComponent<Letter>().point.x == i)
                                {
                                    answer += c.GetComponent<Letter>().inputField.text.ToUpper();
                                }
                            }

                            // Store the answer
                            givenAnswers.Add(i, answer);
                            Debug.Log("Answer given: " + answer);

                            // Start coroutine to color the letters one by one
                            StartCoroutine(ColorLettersOneByOne(i, randomWord));

                            // Move to the next row
                            j = 0;
                            i++;

                            if (i < gridSize) 
                            {
                                // Automatically move focus to the first letter of the next row
                                currentPoint = new Vector2(i, j);
                                ActivateLetterAt(i, j);
                                SetFocusOnLetter(currentPoint);
                            }
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.Backspace))
                    {
                        // Handle backspace input as before
                        HandleBackspace();
                    }

                    // Limit input to one letter
                    if (letter.currentlyActive && !string.IsNullOrEmpty(letter.inputField.text))
                    {
                        // Prevent automatic movement after typing
                        if (j < gridSize - 1)
                        {
                            j++;
                        }

                        letter.GetComponent<Animator>().SetBool("LetterTyped", true);
                        currentPoint = new Vector2(i, j);
                        letter.currentlyActive = false;

                        // Move focus to the next letter in the row
                        foreach (Transform nextChild in parentTransform)
                        {
                            if (nextChild.tag == "Letter" && nextChild.GetComponent<Letter>().point == currentPoint)
                            {
                                nextChild.GetComponent<Letter>().currentlyActive = true;
                                nextChild.GetComponent<Letter>().inputField.enabled = true;

                                // Automatically move focus to the next input field
                                EventSystem.current.SetSelectedGameObject(nextChild.GetComponent<Letter>().inputField.gameObject);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    // Lock the row after the player presses Enter and the letters are checked
    void LockRow(int rowIndex)
    {
        foreach (GameObject letterObject in letterObjects)
        {
            Letter letter = letterObject.GetComponent<Letter>();
            if (letter.point.x == rowIndex)
            {
                // Disable input for the letters in the completed row
                letter.inputField.enabled = false;
            }
        }
    }


    IEnumerator ColorLettersOneByOne(int row, string correctWord)
    {
        Transform parentTransform = canvas.transform;

        for (int col = 0; col < gridSize; col++)
        {
            foreach (Transform c in parentTransform)
            {
                if (c.tag == "Letter" && c.GetComponent<Letter>().point.x == row && c.GetComponent<Letter>().point.y == col)
                {
                    string inputText = c.GetComponent<Letter>().inputField.text.ToUpper();
                    char givenLetter = inputText.Length > 0 ? inputText[0] : '\0';

                    if (givenLetter != '\0')
                    {
                        bool isCorrectPosition = ContainsAndCorrectSpot(correctWord, givenLetter, col);
                        bool isInWord = IsLetterInWord(correctWord, givenLetter);

                        if (isCorrectPosition)
                        {
                            c.GetComponent<Letter>().SetColor(Color.green);
                        }
                        else if (isInWord)
                        {
                            c.GetComponent<Letter>().SetColor(Color.yellow);
                        }
                    }

                    yield return new WaitForSeconds(0.15f);
                }
            }
        }
    }

    void UpdateActiveLetter()
    {
        Transform parentTransform = canvas.transform;

        foreach (Transform child in parentTransform)
        {
            if (child.tag == "Letter")
            {
                Letter letter = child.GetComponent<Letter>();
                Vector2 letterPoint = letter.point;

                // Only enable input field for current position
                if (letterPoint.x == i && letterPoint.y == j)
                {
                    letter.inputField.enabled = true;
                    letter.currentlyActive = true;
                }
                else
                {
                    letter.inputField.enabled = false;
                    letter.currentlyActive = false;
                }

                if (letter.currentlyActive)
                {
                    // Check if Enter key is pressed
                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        // Complete the row, check the letters, and move to the next row
                        if (j == gridSize - 1) 
                        {
                            string answer = "";

                            // Gather the letters in the current row
                            foreach (Transform c in parentTransform)
                            {
                                if (c.tag == "Letter" && c.GetComponent<Letter>().point.x == i)
                                {
                                    answer += c.GetComponent<Letter>().inputField.text.ToUpper();
                                }
                            }

                            // Store the answer
                            givenAnswers.Add(i, answer);
                            Debug.Log("Answer given: " + answer);

                            // Start coroutine to color the letters one by one
                            StartCoroutine(ColorLettersOneByOne(i, randomWord));

                            // Lock the current row
                            LockRow(i);

                            // Move to the next row
                            j = 0;
                            i++;

                            if (i < gridSize) 
                            {
                                // Automatically move focus to the first letter of the next row
                                currentPoint = new Vector2(i, j);
                                ActivateLetterAt(i, j);
                                SetFocusOnLetter(currentPoint);
                            }
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.Backspace))
                    {
                        HandleBackspace();
                    }

                    // Limit input to one letter
                    if (!string.IsNullOrEmpty(letter.inputField.text))
                    {
                        if (j < gridSize - 1)
                        {
                            // Move to next letter
                            j++;
                            currentPoint = new Vector2(i, j);
                            letter.GetComponent<Animator>().SetBool("LetterTyped", true);
                            
                            // Find and activate next letter
                            Letter nextLetter = letterObjects[(int)(i * gridSize + j)].GetComponent<Letter>();
                            DeactivateAllLetters();
                            nextLetter.currentlyActive = true;
                            nextLetter.inputField.enabled = true;
                            EventSystem.current.SetSelectedGameObject(nextLetter.inputField.gameObject);
                        }
                    }
                }
            }
        }
    }

    void HandleBackspace()
    {
        // Check if we're trying to backspace in a completed row
        if (i < currentPoint.x)
            return;

        // Get the current letter
        Letter currentLetter = letterObjects[(int)(i * gridSize + j)].GetComponent<Letter>();

        // If we're at the start of a row with no input, do nothing
        if (j == 0 && string.IsNullOrEmpty(currentLetter.inputField.text))
            return;

        // If current position is empty and we're not at the start, move back one position
        if (string.IsNullOrEmpty(currentLetter.inputField.text) && j > 0)
        {
            j--;
            currentPoint = new Vector2(i, j);
        }

        // Get the letter we want to edit (either current or previous position)
        Letter letterToEdit = letterObjects[(int)(i * gridSize + j)].GetComponent<Letter>();
        
        // Clear the input field
        letterToEdit.inputField.text = string.Empty;
        letterToEdit.GetComponent<Animator>().SetBool("LetterTyped", false);
        
        // Activate and focus the letter
        DeactivateAllLetters();
        letterToEdit.currentlyActive = true;
        letterToEdit.inputField.enabled = true;
        EventSystem.current.SetSelectedGameObject(letterToEdit.inputField.gameObject);
    }

    void LockRow(int rowIndex)
    {
        foreach (GameObject letterObject in letterObjects)
        {
            Letter letter = letterObject.GetComponent<Letter>();
            if (letter.point.x == rowIndex)
            {
                letter.currentlyActive = false;
                letter.inputField.enabled = false;
                letter.inputField.interactable = false;  // Prevent mouse selection
            }
        }
    }

    private void DeactivateAllLetters()
    {
        foreach (GameObject letterObject in letterObjects)
        {
            letterObject.GetComponent<Letter>().currentlyActive = false;
            letterObject.GetComponent<Letter>().inputField.enabled = false;
        }
    }


    void ActivateLetterAt(int x, int y)
    {
        int index = x * gridSize + y;

        // Deactivate all letters
        foreach (GameObject letterObject in letterObjects)
        {
            letterObject.GetComponent<Letter>().currentlyActive = false;
        }

        // Activate the specified letter
        letterObjects[index].GetComponent<Letter>().currentlyActive = true;
        SetFocusOnLetter(new Vector2(x, y));
    }

    void SetFocusOnLetter(Vector2 point)
    {
        int index = (int)(point.x * gridSize + point.y);
        EventSystem.current.SetSelectedGameObject(letterObjects[index].GetComponent<Letter>().inputField.gameObject);
    }

    public bool ContainsAndCorrectSpot(string realAnswer, char givenLetter, int index)
    {
        if (index < realAnswer.Length)
        {
            if (givenLetter == realAnswer[index])
            {
                Debug.Log($"Letter '{givenLetter}' is correct and in the correct position.");
                return true;
            }
        }

        return false;
    }

    public bool IsLetterInWord(string realAnswer, char givenLetter)
    {
        return realAnswer.IndexOf(givenLetter) >= 0;
    }
}
