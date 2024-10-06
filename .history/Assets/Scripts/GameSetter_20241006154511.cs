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

    public GameObject currentLetterObjectFilling;
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

        float screenWidth = Camera.main.orthographicSize * 2 * Screen.width / Screen.height;

        letterXOffset = 1;
        letterYOffset = 1;

        // Calculate total grid width and height
        float totalGridWidth = (gridSize - 1) * letterXOffset;
        float totalGridHeight = (gridSize - 1) * letterYOffset;

        Vector3 gridStartPos = new Vector3(
            originalPos.position.x - totalGridWidth / 2f,
            originalPos.position.y + totalGridHeight / 2f,
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

        // automatically focus on the first input field
        EventSystem.current.SetSelectedGameObject(letterObjects[0].GetComponent<Letter>().inputField.gameObject);
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
                Letter letter = child.GetComponent<Letter>();
                if (letter.currentlyActive)
                {
                    letter.inputField.enabled = true;
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

                        // put the letters together and form a word
                        foreach (Transform c in parentTransform)
                        {
                            if (c.tag == "Letter")
                            {
                                if (c.GetComponent<Letter>().point.x == i)
                                {
                                    answer += c.GetComponent<Letter>().inputField.text;
                                }
                            }
                        }

                        // set the answer
                        givenAnswers.Add(i, answer);

                        Debug.Log("answer given: " + answer);

                        foreach (Transform c in parentTransform)
                        {
                            if (c.tag == "Letter")
                            {
                                if (c.GetComponent<Letter>().point.x == i)
                                {
                                    string inputText = c.GetComponent<Letter>().inputField.text;
                                    char givenLetter = inputText.Length > 0 ? inputText[0] : '\0';

                                    if (givenLetter != '\0')
                                    {
                                        bool isCorrectPosition = ContainsAndCorrectSpot(randomWord, givenLetter, (int)c.GetComponent<Letter>().point.y);
                                        bool isInWord = IsLetterInWord(randomWord, givenLetter);

                                        if (isCorrectPosition)
                                        {
                                            c.GetComponent<Letter>().SetColor(Color.green);
                                        }
                                        else if (isInWord)
                                        {
                                            c.GetComponent<Letter>().SetColor(Color.yellow);
                                        }
                                    }
                                }
                            }
                        }

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

                            // automatically move focus to the next input field
                            EventSystem.current.SetSelectedGameObject(nextChild.GetComponent<Letter>().inputField.gameObject);
                            break;
                        }
                    }
                }
            }
        }
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
