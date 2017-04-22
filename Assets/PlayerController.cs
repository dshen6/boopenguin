using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public int PLAYER_ID = 1;

	protected Rigidbody2D rigidbody;
	protected PlayerInput playerInput;
	protected Vector2 velocity;

	void Start () {
		rigidbody = GetComponent<Rigidbody2D> ();
		playerInput = GetComponent<PlayerInput> ();
	}

	void Update () {
		Debug.Log (playerInput.aimVector);
	}
}
