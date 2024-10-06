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

    public GameObject currentLetterObjectFilling;
    public List<GameObject> letterObjects;
    public Vector2 currentPoint;
    public int i;
    public int j;


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
                        gridStartPos.x + (j * letterXOffset),
                        gridStartPos.y - (i * letterYOffset),
                        gridStartPos.z
                    );

                    letterObjects.Add(letterObject);

                    letterObject.GetComponent<Letter>().point = new Vector2(i, j);

                    //letterObject.GetComponent<Letter>().SetLetter(letterObject.GetComponent<Letter>().point.ToString());

                    if (j == gridSize){
                        letterObject.GetComponent<Letter>().isAnEdge = true;
                    }
                }
            }

            gameGenerated = true;

            letterObjects[0].GetComponent<Letter>().currentlyActive = true;

            i = 0;
            j = 0;
            currentPoint = new Vector2(i,j);
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
                if (child.tag == "Letter"){
                    if (child.GetComponent<Letter>().currentlyActive){
                        child.GetComponent<Letter>().inputField.enabled = true;
                    }
                    else{
                        child.GetComponent<Letter>().inputField.enabled = false;
                    }
                }
            }

            foreach (Transform child in parentTransform)
            {
                if (child.tag == "Letter"){
                    // then text is entered, skip to next point
                    if (child.GetComponent<Letter>().currentlyActive && (!string.IsNullOrEmpty(inputField.text))){
                        if (child.GetComponent<Letter>().isAnEdge){
                            i = 0;
                            j++;
                        }
                        else{
                            i++;
                        }
                        
                        foreach (Transform child1 in parentTransform)
                        {
                            if (child1.tag == "Letter" && child1.GetComponent<Letter>().point == currentPoint){
                                child1.GetComponent<Letter>().inputField.enabled = true;
                            }
                        }
                    }
                }
            }

        }
    }
}
