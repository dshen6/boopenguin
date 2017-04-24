using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public delegate void PenguinFallAction();
	public static event PenguinFallAction OnPenguinFall;

	private Animator animator;

	public int PLAYER_ID = 1;
	public Vector2 velocity;
	public float zVelocity;
	public float maxWalkSpeed;
	public float maxSlideSpeed;
	public float walkSpeed;
	public float slideSpeed;
	public float gravity;
	public float friction = .94f;
	public float slideFriction = .98f;
	public bool isSliding;
	private float ignoreInputMillisRemaining;
	private float slideDurationMillisRemaining;
	private float fallDurationMillis;

	protected PlayerInput playerInput;
	protected Rigidbody2D rigidBody2d;

	private Vector3 UP = new Vector3(0,0,-1);
	private Vector3 DOWN = new Vector3(0,0,1);

	void Start () {
		playerInput = GetComponent<PlayerInput>();
		rigidBody2d = GetComponent<Rigidbody2D>();
		animator = GetComponentInChildren<Animator> ();
	}

	void Update () {
		// Check to see if we're falling
		RaycastHit hit;
		Physics.Raycast (gameObject.transform.position + UP, DOWN, out hit);
		if (hit.collider == null || !hit.collider.name.Equals("Iceberg")) {
			if (fallDurationMillis == 0) {
				if (OnPenguinFall != null) {
					OnPenguinFall();
				}
				Debug.Log("just fell");
			}

			if (fallDurationMillis > 2f) {
				Debug.Log("respawn");
				zVelocity = 0;
				velocity = Vector2.zero;
				rigidBody2d.MovePosition(Vector2.zero);
				gameObject.transform.position = Vector3.zero;

				fallDurationMillis = 0;
				return;
			}

			// let it fall
			zVelocity += gravity * Time.fixedDeltaTime;
			gameObject.transform.position += new Vector3(0, 0, zVelocity * Time.fixedDeltaTime);

			// but keep side momentum
			rigidBody2d.MovePosition(rigidBody2d.position + velocity * Time.fixedDeltaTime);

			fallDurationMillis += Time.fixedDeltaTime;
			return;
		}

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
		if (other.CompareTag("Player")) {
			HandlePlayerTriggerEnter(other);
		}
	}

	void HandlePlayerTriggerEnter(Collider2D other) {
		StartCoroutine(IgnoreInputForDuration());

		// look how fast they're going, attempt to conserve momentum using m1 * v1 = m2 * v2; 

		PlayerController otherPlayer = other.GetComponent<PlayerController>();
		float otherMass = otherPlayer.isSliding ? 3 : 1;
		float myMass = isSliding ? 3 : 1;
		float impulseFactor = otherMass / myMass;

		Vector2 towardsOther = other.attachedRigidbody.position - rigidBody2d.position;
		if (velocity.sqrMagnitude < 1f) { // if small magnitude, just hack it
			velocity = -towardsOther.normalized * .5f * impulseFactor;
			return;
		}

		RaycastHit2D impact = Physics2D.Raycast (rigidBody2d.position, towardsOther);
		velocity = Vector2.Reflect (velocity, impact.normal);

		float adjustedVelocityMagnitude = otherPlayer.velocity.magnitude * impulseFactor;
		adjustedVelocityMagnitude = Mathf.Max(adjustedVelocityMagnitude, .5f);
		velocity = velocity.normalized * adjustedVelocityMagnitude;
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
		animator.SetBool ("Sliding", true);
		velocity *= 1.1f;
	}

	private void stopSliding () {
		if (isSliding) {
			animator.SetBool ("Sliding", false);
			isSliding = false;
		}
	}

}
