using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	private Animator animator;

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
	private float slideDurationMillisRemaining;

	protected PlayerInput playerInput;
	protected Rigidbody2D rigidBody2d;

	void Start () {
		playerInput = GetComponent<PlayerInput>();
		rigidBody2d = GetComponent<Rigidbody2D>();
		animator = GetComponentInChildren<Animator> ();
	}

	void Update () {
		if (ignoreInputMillisRemaining <= 0) {
			if (playerInput.isAiming() && slideDurationMillisRemaining <= 0) {
				stopSliding();
			}
			velocity += (isSliding ? slideSpeed : walkSpeed) * playerInput.aimVector; 
			velocity = Vector2.ClampMagnitude (velocity, isSliding ? maxSlideSpeed : maxWalkSpeed);
		}
		velocity *= isSliding ? slideFriction : friction;

		rigidBody2d.MovePosition(rigidBody2d.position + velocity * Time.fixedDeltaTime);

		AnimationHelper.UpdateAnimator (animator, velocity.magnitude, velocity.normalized);
	}

	void OnTriggerEnter2D(Collider2D other) {
		Vector2 towardsOther = other.attachedRigidbody.position - rigidBody2d.position;
		RaycastHit2D impact = Physics2D.Raycast (rigidBody2d.position, towardsOther);
		velocity = Vector3.Reflect (velocity, impact.normal);
		velocity *= .3f;
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
		slideDurationMillisRemaining = millis;
		while (slideDurationMillisRemaining > 0) {
			yield return null;
			slideDurationMillisRemaining -= Time.deltaTime * 1000;
		}
	}

	private void startSliding () {
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
