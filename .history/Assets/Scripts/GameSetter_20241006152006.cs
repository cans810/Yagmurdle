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
                        foreach(Transform c in parentTransform){
                            if (c.tag == "Letter"){
                                if (c.GetComponent<Letter>().point.x == i){
                                    answer += c.GetComponent<Letter>().inputField.text;
                                }
                            }
                        }

                        // set the answer
                        givenAnswers.Add(i, answer);

                        Debug.Log("answer given: " + answer);

                        foreach(Transform c in parentTransform){
                            if (c.tag == "Letter"){
                                if (c.GetComponent<Letter>().point.x == i){
                                    if (randomWord.Contains(c.GetComponent<Letter>().inputField.text)){
                                        c.GetComponent<Letter>().SetColor(Color.red);
                                    }
                                    else if (randomWord.Contains(c.GetComponent<Letter>().inputField.text)){
                                        c.GetComponent<Letter>().SetColor(Color.red);
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

    public bool containsAndCorrectSpot(string realAnswer, string givenAnswer)
    {
        bool[] isLetterChecked = new bool[realAnswer.Length];
        bool[] isLetterFound = new bool[realAnswer.Length];
        bool isMatch = true; // Flag to check if the given answer matches the real answer

        // First pass: Check for correct letters in the correct position
        for (int i = 0; i < givenAnswer.Length; i++)
        {
            if (givenAnswer[i].ToString() == realAnswer[i].ToString())
            {
                Debug.Log($"Letter '{givenAnswer[i]}' is correct and in the correct position.");
                isLetterChecked[i] = true; // Mark as checked
                isLetterFound[i] = true; // Mark as found
            }
            else
            {
                isMatch = false; // If any letter is not in the correct position, it's not a match
            }
        }

        // Second pass: Check for correct letters in the wrong position
        for (int i = 0; i < givenAnswer.Length; i++)
        {
            if (!isLetterChecked[i]) // Only check if not already marked
            {
                for (int j = 0; j < realAnswer.Length; j++)
                {
                    if (!isLetterChecked[j] && givenAnswer[i].ToString() == realAnswer[j].ToString())
                    {
                        Debug.Log($"Letter '{givenAnswer[i]}' is in the word but in the wrong position.");
                        isLetterChecked[j] = true; // Mark as checked
                        break; // Move on to the next letter in the given answer
                    }
                }
            }
        }

        // Final pass: Mark letters not found in the word
        for (int i = 0; i < givenAnswer.Length; i++)
        {
            if (!isLetterFound[i])
            {
                Debug.Log($"Letter '{givenAnswer[i]}' is not in the word.");
            }
        }

        return isMatch; // Return true if the given answer matches the real answer, false otherwise
    }

}


