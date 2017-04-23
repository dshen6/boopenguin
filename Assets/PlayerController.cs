using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public int PLAYER_ID = 1;
	public Vector2 velocity;
	public float maxWalkSpeed;
	public float maxSlideSpeed;
	public float walkSpeed;
	public float slideSpeed;
	public float friction = .94f;
	public float slideFriction = .98f;
	public bool isSliding;
	private float ignoreInputMillisRemaining;

	protected PlayerInput playerInput;
	protected Rigidbody2D rigidBody2d;

	void Start () {
		playerInput = GetComponent<PlayerInput>();
		rigidBody2d = GetComponent<Rigidbody2D>();
	}

	void Update () {
		if (ignoreInputMillisRemaining <= 0) {
			if (playerInput.isAiming()) {
				stopSliding();
			}
			velocity += (isSliding ? slideSpeed : walkSpeed) * playerInput.aimVector; 
			velocity = Vector2.ClampMagnitude (velocity, isSliding ? maxSlideSpeed : maxWalkSpeed);
		}
		velocity *= isSliding ? slideFriction : friction;

		rigidBody2d.MovePosition(rigidBody2d.position + velocity * Time.fixedDeltaTime);
	}

	void OnTriggerEnter2D(Collider2D other) {
		velocity *= -.3f;
		StartCoroutine(IgnoreInputForDuration());
	}
		
	public void Fire1Down() {
		StartCoroutine(Slide());
	}
		
	IEnumerator IgnoreInputForDuration(float millis = 300) {
		ignoreInputMillisRemaining = Mathf.Max(ignoreInputMillisRemaining, millis);
		while (ignoreInputMillisRemaining > 0) {
			yield return null;
			ignoreInputMillisRemaining -= Time.deltaTime * 1000;
		}
	}

	IEnumerator Slide(float millis = 600) {
		startSliding ();
		ignoreInputMillisRemaining = Mathf.Max(ignoreInputMillisRemaining, millis);
		while (ignoreInputMillisRemaining > 0) {
			yield return null;
			ignoreInputMillisRemaining -= Time.deltaTime * 1000;
		}
	}

	private void startSliding (){
		if (isSliding) {
			return;
		}
		isSliding = true;
		rigidBody2d.MoveRotation(-60);
		velocity *= 1.1f;
	}

	private void stopSliding () {
		if (isSliding) {
			isSliding = false;
			rigidBody2d.MoveRotation (0);
		}
	}

}
