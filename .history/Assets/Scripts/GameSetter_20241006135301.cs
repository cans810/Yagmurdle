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


    public int currentLetterFillingNum;
    public GameObject currentLetterObjectFilling;
    public List<GameObject> letterObjects;


    // Start is called before the first frame update
    void Start()
    {
        letterXOffset = 1;
        letterYOffset = 1;  
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameGenerated){
            string[] randomWordArray = randomWord.Split();

            int gridSize = randomWord.Length; 


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
                        gridStartPos.x + (i * letterXOffset),
                        gridStartPos.y + (j * letterYOffset),
                        gridStartPos.z
                    );


                    letterObjects.Add(letterObject);
                }
            }

            gameGenerated = true;
            currentRowFilling = gridSize - 1;
            currentLetterObjectFilling = letterObjects[0];
            currentLetterFillingNum = 0;
        }

        if (deleteGame){
            Transform parentTransform = canvas.transform;

            foreach (Transform child in parentTransform)
            {
                if (child.GetComponent<Letter>() != null){
                    Destroy(child.gameObject);
                }
            }
        }

        // game loop
        if (gameGenerated){
            Transform parentTransform = canvas.transform;

            foreach (Transform child in parentTransform)
            {

            }

        }
    }
}