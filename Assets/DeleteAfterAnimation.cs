using UnityEngine;
using System.Collections;

public class DeleteAfterAnimation : MonoBehaviour {
	public float delay = 2f;

	// Use this for initialization
	void Start () {
		Destroy (gameObject, delay);
	}

	void Update() {
		
	}
}