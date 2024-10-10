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

                    // Check for backspace input
                    if (Input.GetKeyDown(KeyCode.Backspace))
                    {
                        HandleBackspace();
                    }
                }
                else
                {
                    letter.inputField.enabled = false;
                }

                if (letter.currentlyActive && !string.IsNullOrEmpty(letter.inputField.text))
                {
                    if (j == gridSize - 1)
                    {
                        string answer = "";

                        // Gather the letters together and form a word
                        foreach (Transform c in parentTransform)
                        {
                            if (c.tag == "Letter")
                            {
                                if (c.GetComponent<Letter>().point.x == i)
                                {
                                    answer += c.GetComponent<Letter>().inputField.text.ToUpper();
                                }
                            }
                        }

                        // Store the answer
                        givenAnswers.Add(i, answer);

                        Debug.Log("Answer given: " + answer);

                        // Start coroutine to color the letters one by one
                        StartCoroutine(ColorLettersOneByOne(i, randomWord));

                        j = 0;
                        i++;
                    }
                    else
                    {
                        j++;
                    }

                    letter.GetComponent<Animator>().SetBool("LetterTyped", true);
                    currentPoint = new Vector2(i, j);
                    letter.currentlyActive = false;

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

    void HandleBackspace()
    {
        if (currentPoint.y > 0)
        {
            currentPoint.y--;
        }
        else if (currentPoint.x > 0)
        {
            currentPoint.x--;
            currentPoint.y = gridSize - 1; // Move to the last column of the previous row
        }

        // Clear the input field of the current position
        letterObjects[(int)(currentPoint.x * gridSize + currentPoint.y)].GetComponent<Letter>().inputField.text = string.Empty;

        // Activate the previous letter
        letterObjects[(int)(currentPoint.x * gridSize + currentPoint.y)].GetComponent<Letter>().currentlyActive = true;
        EventSystem.current.SetSelectedGameObject(letterObjects[(int)(currentPoint.x * gridSize + currentPoint.y)].GetComponent<Letter>().inputField.gameObject);
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
