using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ScoreManager : MonoBehaviour {

    // Use this for initialization
    public int Score = 0;
    public int MaxMissedEggs = 10;
    public Text scoreText;
    public Text gameOverText;
    public string defaultText = "Score:";

	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {     
        scoreText.text = defaultText + " " + Score.ToString();
        gameOverText.gameObject.SetActive(false);
	}
}
