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
        letterXOffset
        
        for(int i=0; i < randomWord.Length; i++){
            GameObject letterObjectCol = Instantiate(LetterObjectPrefab, canvas.transform);
            letterObjectCol.transform.position = new Vector3(originalPos.position.x + letterXOffset, originalPos.position.y,
            originalPos.position.z);

            for(int j=0; j < randomWord.Length; j++){
                GameObject letterObjectRow = Instantiate(LetterObjectPrefab, canvas.transform);
                letterObjectRow.transform.position = new Vector3(originalPos.position.x, originalPos.position.y + ,
                originalPos.position.z);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
