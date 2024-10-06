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
        
        foreach(char c in randomWord){
            GameObject letterObject = Instantiate(LetterObjectPrefab, transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
