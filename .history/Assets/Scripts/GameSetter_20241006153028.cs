using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameSetter : MonoBehaviour
{
    string[] turkishWords = new string[]
    {
        "Ağaç", "Aile", "Akıl", "Akşam", "Araç", 
        "Arkadaş", "Aşk", "Ayak", "Balık", "Bahçe", 
        "Barış", "Başarı", "Başka", "Başlangıç", "Beden", 
        "Bir", "Biyoloji", "Cennet", "Çiçek", "Daha", 
        "Deniz", "Ders", "Dünya", "Eğitim", "Ekmek", 
        "Eller", "Göz", "Güneş", "Hava", "Hayat", 
        "İlk", "İnsan", "Kedi", "Kış", "Köpek", 
        "Kütüphane", "Masa", "Meyve", "Müzik", "Odalar", 
        "Oyun", "Öğrenci", "Pasta", "Rüzgar", "Sahil", 
        "Sanat", "Sıcak", "Sözlük", "Şehir", "Tarih", 
        "Tarif", "Uçak", "Uzak", "Yıldız", "Yüz", 
        "Yaz", "Yemek", "Zaman", "Zeytin", "Aşkın", 
        "Çocuk", "Hediye", "Yarış", "Havaalanı", "Öğretmen", 
        "Yavaş", "Yağmur", "Okul", "Sınıf", "Bilim", 
        "Futbol", "Kütük", "Tahta", "Ülke", "Şarkı", 
        "Kütüphane", "Kedi", "Renk", "Ses", "Hayal", 
        "Fikir", "Şans", "Süre", "Dostluk", "Sevgi", 
        "Tatlı", "Söz", "Korku", "Küçük", "Büyük", 
        "Fırtına", "Mola", "Orman", "Köy", "Deniz", 
        "Köfte", "Çorba", "Dondurma", "Bahar", "Spor"
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

    public void GenerateRandomWord(){
        int randomWordIndex = UnityEngine.Random.Range(0, turkishWords.Length);

        randomWord = turkishWords[randomWordIndex];
    }

    void GenerateGame()
    {
        GenerateRandomWord();

        string[] randomWordArray = randomWord.Split();

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
            letter.inputField.enabled = letter.currentlyActive;

            if (letter.currentlyActive && !string.IsNullOrEmpty(letter.inputField.text))
            {
                if (j == gridSize - 1)
                {
                    string answer = "";
                    
                    // Form the answer from the input fields in the current row
                    foreach (Transform c in parentTransform)
                    {
                        if (c.tag == "Letter" && c.GetComponent<Letter>().point.x == i)
                        {
                            answer += c.GetComponent<Letter>().inputField.text;
                        }
                    }

                    // Set the answer
                    givenAnswers.Add(i, answer);
                    Debug.Log("answer given: " + answer);
                    
                    // Check the answer against the random word
                    CheckAnswer(answer, i, parentTransform);

                    j = 0; // Reset for the next row
                    i++; // Move to the next row
                }
                else
                {
                    j++; // Move to the next letter in the row
                }

                currentPoint = new Vector2(i, j);
                letter.currentlyActive = false;

                // Activate the next letter
                ActivateNextLetter(parentTransform);
            }
        }
    }
}

void CheckAnswer(string givenAnswer, int rowIndex, Transform parentTransform)
{
    bool[] isLetterChecked = new bool[randomWord.Length];
    bool[] isLetterFound = new bool[randomWord.Length];

    // First pass: Check for correct letters (green)
    for (int i = 0; i < givenAnswer.Length; i++)
    {
        if (givenAnswer[i].ToString() == randomWord[i].ToString())
        {
            Debug.Log($"Letter '{givenAnswer[i]}' is correct and in the correct position.");
            SetLetterColor(rowIndex, i, Color.green, parentTransform);
            isLetterChecked[i] = true;
            isLetterFound[i] = true; // Mark it as found
        }
    }

    // Second pass: Check for correct letters in wrong positions (yellow)
    for (int i = 0; i < givenAnswer.Length; i++)
    {
        if (!isLetterChecked[i]) // Only check if not already marked green
        {
            for (int j = 0; j < randomWord.Length; j++)
            {
                if (!isLetterChecked[j] && givenAnswer[i].ToString() == randomWord[j].ToString() && !isLetterFound[j])
                {
                    Debug.Log($"Letter '{givenAnswer[i]}' is in the word but in the wrong position.");
                    SetLetterColor(rowIndex, i, Color.yellow, parentTransform);
                    isLetterChecked[j] = true; // Mark it as found
                    isLetterFound[j] = true;
                    break;
                }
            }
        }
    }

    // Final pass: Mark letters not in the word (red)
    for (int i = 0; i < givenAnswer.Length; i++)
    {
        if (!isLetterChecked[i])
        {
            Debug.Log($"Letter '{givenAnswer[i]}' is not in the word.");
            SetLetterColor(rowIndex, i, Color.red, parentTransform);
        }
    }
}

void SetLetterColor(int rowIndex, int letterIndex, Color color, Transform parentTransform)
{
    foreach (Transform c in parentTransform)
    {
        if (c.tag == "Letter" && c.GetComponent<Letter>().point.x == rowIndex && c.GetComponent<Letter>().point.y == letterIndex)
        {
            c.GetComponent<Letter>().SetColor(color);
            break;
        }
    }
}

void ActivateNextLetter(Transform parentTransform)
{
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

    public bool containsAndCorrectSpot(string realAnswer, string givenAnswer)
    {
        bool[] isLetterChecked = new bool[realAnswer.Length];
        bool[] isLetterFound = new bool[realAnswer.Length];
        bool isMatch = true;

        for (int i = 0; i < givenAnswer.Length; i++)
        {
            if (givenAnswer[i].ToString() == realAnswer[i].ToString())
            {
                Debug.Log($"Letter '{givenAnswer[i]}' is correct and in the correct position.");
                isLetterChecked[i] = true;
                isLetterFound[i] = true;
            }
            else
            {
                isMatch = false;
            }
        }

        for (int i = 0; i < givenAnswer.Length; i++)
        {
            if (!isLetterChecked[i])
            {
                for (int j = 0; j < realAnswer.Length; j++)
                {
                    if (!isLetterChecked[j] && givenAnswer[i].ToString() == realAnswer[j].ToString())
                    {
                        Debug.Log($"Letter '{givenAnswer[i]}' is in the word but in the wrong position.");
                        isLetterChecked[j] = true;
                        break;
                    }
                }
            }
        }

        for (int i = 0; i < givenAnswer.Length; i++)
        {
            if (!isLetterFound[i])
            {
                Debug.Log($"Letter '{givenAnswer[i]}' is not in the word.");
            }
        }

        return isMatch;
    }

}


