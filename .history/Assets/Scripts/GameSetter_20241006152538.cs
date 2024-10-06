using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameSetter : MonoBehaviour
{
    string[] turkishWords = new string[] { "Ağaç", "Aile", "Akıl", "Akşam", "Araç", "Arkadaş", "Aşk", "Ayak", "Balık", "Bahçe", 
                                             "Barış", "Başarı", "Başka", "Başlangıç", "Beden", "Bir", "Biyoloji", "Cennet", "Çiçek", "Daha", 
                                             "Deniz", "Ders", "Dünya", "Eğitim", "Ekmek", "Eller", "Göz", "Güneş", "Hava", "Hayat", 
                                             "İlk", "İnsan", "Kedi", "Kış", "Köpek", "Kütüphane", "Masa", "Meyve", "Müzik", "Odalar", 
                                             "Oyun", "Öğrenci", "Pasta", "Rüzgar", "Sahil", "Sanat", "Sıcak", "Sözlük", "Şehir", "Tarih", 
                                             "Tarif", "Uçak", "Uzak", "Yıldız", "Yüz", "Yaz", "Yemek", "Zaman", "Zeytin", "Aşkın", 
                                             "Çocuk", "Hediye", "Yarış", "Havaalanı", "Öğretmen", "Yavaş", "Yağmur", "Okul", "Sınıf", "Bilim", 
                                             "Futbol", "Kütük", "Tahta", "Ülke", "Şarkı", "Kütüphane", "Kedi", "Renk", "Ses", "Hayal", 
                                             "Fikir", "Şans", "Süre", "Dostluk", "Sevgi", "Tatlı", "Söz", "Korku", "Küçük", "Büyük", 
                                             "Fırtına", "Mola", "Orman", "Köy", "Deniz", "Köfte", "Çorba", "Dondurma", "Bahar", "Spor" };

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
        Debug.Log("Generated Word: " + randomWord); // Log the generated word
    }

    void GenerateGame()
    {
        GenerateRandomWord();
        gridSize = randomWord.Length; // Use the length of the word directly

        float screenWidth = Camera.main.orthographicSize * 2 * Screen.width / Screen.height;

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
            GameObject letterObject = Instantiate(LetterObjectPrefab, canvas.transform);

            letterObject.transform.position = new Vector3(
                gridStartPos.x + (i * letterXOffset), // Only use i for horizontal position
                gridStartPos.y,
                gridStartPos.z
            );

            letterObjects.Add(letterObject);
            letterObject.GetComponent<Letter>().point = new Vector2(i, 0); // Use (i, 0) since it's a single row

            if (i == gridSize - 1)
            {
                letterObject.GetComponent<Letter>().isAnEdge = true;
            }
        }

        gameGenerated = true;

        letterObjects[0].GetComponent<Letter>().currentlyActive = true;
        currentPoint = new Vector2(0, 0);

        // Automatically focus on the first input field
        EventSystem.current.SetSelectedGameObject(letterObjects[0].GetComponent<Letter>().inputField.gameObject);
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
        foreach (Transform child in canvas.transform)
        {
            if (child.tag == "Letter")
            {
                Letter letter = child.GetComponent<Letter>();
                letter.inputField.enabled = letter.currentlyActive;

                if (letter.currentlyActive && !string.IsNullOrEmpty(letter.inputField.text))
                {
                    if (currentPoint.x == gridSize - 1) // Check if last letter in row
                    {
                        string answer = "";

                        // Put the letters together and form a word
                        foreach (Transform c in canvas.transform)
                        {
                            if (c.tag == "Letter" && c.GetComponent<Letter>().point.y == 0)
                            {
                                answer += c.GetComponent<Letter>().inputField.text;
                            }
                        }

                        // Set the answer
                        givenAnswers.Add(i, answer);
                        Debug.Log("Answer given: " + answer);

                        // Evaluate each letter's position
                        for (int k = 0; k < gridSize; k++)
                        {
                            if (k < answer.Length)
                            {
                                char currentChar = answer[k];
                                if (randomWord[k] == currentChar)
                                {
                                    letterObjects[k].GetComponent<Letter>().SetColor(Color.green);
                                }
                                else if (randomWord.Contains(currentChar.ToString()))
                                {
                                    letterObjects[k].GetComponent<Letter>().SetColor(Color.yellow);
                                }
                                else
                                {
                                    letterObjects[k].GetComponent<Letter>().SetColor(Color.red);
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

                    // Move to the next input field
                    if (i < letterObjects.Count)
                    {
                        letterObjects[i].GetComponent<Letter>().currentlyActive = true;
                        letterObjects[i].GetComponent<Letter>().inputField.enabled = true;

                        // Automatically move focus to the next input field
                        EventSystem.current.SetSelectedGameObject(letterObjects[i].GetComponent<Letter>().inputField.gameObject);
                    }
                }
            }
        }
    }

    public bool containsAndCorrectSpot(string realAnswer, string givenAnswer)
    {
        bool isMatch = true;

        for (int i = 0; i < givenAnswer.Length; i++)
        {
            if (givenAnswer[i].ToString() != realAnswer[i].ToString())
            {
                isMatch = false; // If any letter doesn't match, it's not a complete match
            }
        }

        return isMatch;
    }
}
