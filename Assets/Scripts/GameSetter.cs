using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    public GameObject GameEndCanvas;
    public GameObject YouWon;
    public GameObject YouLost;


    void Start()
    {
        YouWon.SetActive(false);
        YouLost.SetActive(false);

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

                Letter letterComponent = letterObject.GetComponent<Letter>();
                letterComponent.point = new Vector2(i, j);

                // Ensure the input field is properly set up
                TMP_InputField inputField = letterObject.GetComponent<TMP_InputField>();
                if (inputField != null)
                {
                    inputField.characterLimit = 1;
                    inputField.onValidateInput += letterComponent.ValidateInput;
                }

                letterObjects.Add(letterObject);
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
        bool allCorrect = true;  // Track if all letters are correct

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
                        else
                        {
                            allCorrect = false;
                            if (isInWord)
                            {
                                c.GetComponent<Letter>().SetColor(Color.yellow);
                            }
                        }
                    }
                    else
                    {
                        allCorrect = false;
                    }

                    yield return new WaitForSeconds(0.15f);
                }
            }
        }

        // Check if player won
        if (allCorrect)
        {
            YouWon.SetActive(true);
            DisableAllInput();
        }
        else if (row == gridSize - 1) // Check if this was the last row
        {
            YouLost.SetActive(true);
            DisableAllInput();
        }
    }

    void DisableAllInput()
    {
        foreach (GameObject letterObject in letterObjects)
        {
            Letter letter = letterObject.GetComponent<Letter>();
            letter.inputField.enabled = false;
            letter.currentlyActive = false;
        }
    }

    void HandleBackspace()
    {
        // Check if we're trying to backspace in a completed row
        if (i < currentPoint.x) return;

        // If we're at the start and the first letter is empty, do nothing
        if (i == 0 && j == 0 && string.IsNullOrEmpty(letterObjects[0].GetComponent<Letter>().inputField.text))
            return;

        // Get the current letter
        int currentIndex = i * gridSize + j;
        Letter currentLetter = letterObjects[currentIndex].GetComponent<Letter>();

        // If the current letter has text, delete it
        if (!string.IsNullOrEmpty(currentLetter.inputField.text))
        {
            currentLetter.inputField.text = string.Empty;
            currentLetter.GetComponent<Animator>().SetBool("LetterTyped", false);
            return; // Exit after deleting the letter
        }

        // If the current letter is empty, move back one position
        if (j > 0)
        {
            j--; // Move to the previous letter
        }
        else if (i > 0) // If at the start of a row, move to the last letter of the previous row
        {
            i--;
            j = gridSize - 1; // Set to the last letter of the previous row
        }

        // Get the new current letter
        currentLetter = letterObjects[i * gridSize + j].GetComponent<Letter>();
        currentLetter.inputField.text = string.Empty; // Clear text in the new current letter
        currentLetter.GetComponent<Animator>().SetBool("LetterTyped", false);

        // Set focus to the new current letter
        SetFocusOnCurrentLetter(currentLetter);
    }

    private void SetFocusOnCurrentLetter(Letter letter)
    {
        // Deactivate all letters
        DeactivateAllLetters();

        // Set the current letter active and focused
        letter.currentlyActive = true;
        letter.inputField.enabled = true;
        EventSystem.current.SetSelectedGameObject(letter.inputField.gameObject);
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
