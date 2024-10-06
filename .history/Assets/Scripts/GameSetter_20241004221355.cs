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


    // Start is called before the first frame update
    void Start()
    {
        randomWord = "Yagmur";

        letterXOffset = randomWord.Length;
        letterYOffset = randomWord.Length;
        
        for (int i = 0; i < randomWord.Length; i++)
        {
            for (int j = 0; j < randomWord.Length; j++)
            {
                // Instantiate each letter object in the correct grid position
                GameObject letterObject = Instantiate(LetterObjectPrefab, canvas.transform);

                // Update the position based on row (i) and column (j) offsets
                letterObject.transform.position = new Vector3(
                    originalPos.position.x + (i * letterXOffset),  // Adjust X position for each column
                    originalPos.position.y + (j * letterYOffset),  // Adjust Y position for each row
                    originalPos.position.z  // Z position remains unchanged
                );
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
