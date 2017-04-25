using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlsGuiController : MonoBehaviour {

	private GameObject playerAnchor;
	private PlayerInput input;
	private Camera camera;
	Text text;
	private bool isActive;
	private Canvas canvas;

	void Start () {
		
		text = GetComponent<Text>();
		canvas = GetComponentInParent<Canvas>();
		camera = GetComponent<Camera>();
		isActive = true;
	}

	public void setPlayer(GameObject player) {
		playerAnchor = player;
		input = playerAnchor.GetComponent<PlayerInput>();
	}
	
	void Update () {
		if (!isActive || playerAnchor == null || input == null) {
			return;
		}
		text.rectTransform.localPosition = GetScreenPosition(playerAnchor.transform, canvas, camera) + Vector3.up * 30;

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

	public static Vector3 GetScreenPosition(Transform transform, Canvas canvas, Camera cam)
	{
		Vector3 pos;
		float width = canvas.GetComponent<RectTransform> ().sizeDelta.x;
		float height = canvas.GetComponent<RectTransform > ().sizeDelta.y;
		float x = Camera.main.WorldToScreenPoint (transform.position).x / Screen.width;
		float y = Camera.main.WorldToScreenPoint (transform.position).y / Screen.height;
		pos = new Vector3 (width * x - width / 2, y * height - height / 2); 
		return pos;    
	}
}
