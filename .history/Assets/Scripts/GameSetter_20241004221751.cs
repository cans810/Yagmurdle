using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetter : MonoBehaviour
{
    public GameObject canvas;
    public GameObject LetterObjectPrefab;
    public string randomWord;
    public Transform originalPos;
    public float letterXOffset = 1f;
    public float letterYOffset = 1f;

    // Start is called before the first frame update
    void Start()
    {
        randomWord = "Yagmur";

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
                    gridStartPos.y + (j * letterYOffset),  // Adjust Y position for each row
                    gridStartPos.z  // Z position remains unchanged
                );
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
