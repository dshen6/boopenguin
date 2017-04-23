using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlsGuiController : MonoBehaviour {

	public GameObject player;
	private PlayerInput input;
	Text text;
	private bool isActive;
	// Use this for initialization
	void Start () {
		input = player.GetComponent<PlayerInput>();
		text = GetComponent<Text>();
		isActive = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (!isActive) {
			return;
		}
		if (input.hasPressedAllButtons()) {
			StartCoroutine(FadeTo(0.0f, .4f));
		}
	}

	IEnumerator FadeTo(float aValue, float aTime)
	{
		float alpha = text.color.a;
		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
		{
			Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha,aValue,t));
			text.color = newColor;
			yield return null;
			isActive = false;
		}
	}
}
