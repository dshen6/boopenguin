using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour {

	private Text scoreText;

	private int score = 10;

	void OnEnable() {
		PlayerController.OnPenguinFall += PenguinFall;
	}

	void OnDisable() {
		PlayerController.OnPenguinFall -= PenguinFall;
	}

	void PenguinFall() {
		score--;
	}
	// Use this for initialization
	void Start () {
		scoreText = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		scoreText.text = "" + score;
	}
}
