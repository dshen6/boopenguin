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
	public float slideBoost;
	public float gravity;
	public float friction = .94f;
	public float slideFriction = .98f;
	public bool isSliding;
	private float ignoreInputMillisRemaining;
	private float slideDurationMillisRemaining;
	private float fallDurationMillis;
	private AudioSource audioSource;

	protected PlayerInput playerInput;
	protected Rigidbody2D rigidBody2d;

	public GameObject splashPrefab;

	private Vector3 UP = new Vector3(0, 0, -1);
	private Vector3 DOWN = new Vector3(0, 0, 1);

	void Start() {
		playerInput = GetComponent<PlayerInput>();
		rigidBody2d = GetComponent<Rigidbody2D>();
		animator = GetComponentInChildren<Animator>();
		audioSource = GetComponent<AudioSource>();
	}

	void OnSplash() {
		GameObject splash = Instantiate(splashPrefab) as GameObject;
		splash.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 0);
	}

	void Update() {
		// Check to see if we're falling
		RaycastHit hit;
		Physics.Raycast(gameObject.transform.position + UP, DOWN, out hit);
		if (hit.collider == null || !hit.collider.name.Equals("Iceberg")) {
			if (fallDurationMillis == 0) {
				if (OnPenguinFall != null) {
					OnPenguinFall();
				}
				Invoke("OnSplash", 0.3f);
				Debug.Log("just fell");
			}

			if (fallDurationMillis > 2f) {
				Debug.Log("respawn");
				zVelocity = 0;
				velocity = Vector2.zero;
				rigidBody2d.MovePosition(Vector2.zero);
				gameObject.transform.position = Vector3.zero;
				playAudio(true);
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
			velocity = Vector2.ClampMagnitude(velocity, isSliding ? maxSlideSpeed : maxWalkSpeed);
		}
		velocity *= isSliding ? slideFriction : friction;

		rigidBody2d.MovePosition(rigidBody2d.position + velocity * Time.fixedDeltaTime);

		AnimationHelper.UpdateAnimator(animator, velocity.magnitude, velocity.normalized);
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag("Player")) {
			HandlePlayerTriggerEnter(other);
		}
	}

	void HandlePlayerTriggerEnter(Collider2D other) {
		if (PLAYER_ID != 1) {
			return;
		}

		Debug.Log("collide");

		PlayerController otherPlayer = other.GetComponent<PlayerController>();
		Vector2 otherVelocity = otherPlayer.velocity;
		Rigidbody2D otherRigidbody2d = otherPlayer.rigidBody2d;

		Vector2 myVelocity = velocity;
		Rigidbody2D myRigidbody2d = rigidBody2d;

		Vector2 myTowards = (otherRigidbody2d.position - myRigidbody2d.position).normalized;
		Vector2 otherTowards = (myRigidbody2d.position - otherRigidbody2d.position).normalized;;

		Vector2 otherTowardsVelocity = otherTowards * Vector2.Dot(otherVelocity, otherTowards);
		Vector2 otherAwayVelocity = otherVelocity - otherTowardsVelocity;

		Vector2 myTowardsVelocity = myTowards * Vector2.Dot(myVelocity, myTowards);
		Vector2 myAwayVelocity = myVelocity - myTowardsVelocity;

		otherPlayer.velocity = otherAwayVelocity + myTowardsVelocity;
		velocity = myAwayVelocity + otherTowardsVelocity;

		if (isSliding) {
			StartCoroutine(IgnoreInputForDuration(300));
		} else {
			StartCoroutine(IgnoreInputForDuration(150));
		}

		if (otherPlayer.isSliding) {
			StartCoroutine(otherPlayer.IgnoreInputForDuration(300));
		} else {
			StartCoroutine(otherPlayer.IgnoreInputForDuration(150));
		}

//		velocity = Vector2.Reflect(velocity, impact.normal);

//		float adjustedVelocityMagnitude = otherPlayer.velocity.magnitude * impulseFactor;
//		adjustedVelocityMagnitude = Mathf.Max(adjustedVelocityMagnitude, .5f);
//		velocity = velocity.normalized * adjustedVelocityMagnitude;
	}

	public void Fire1Down() {
		playAudio(false);
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
		startSliding();
		slideDurationMillisRemaining = millis;
		while (slideDurationMillisRemaining > 0) {
			yield return null;
			slideDurationMillisRemaining -= Time.deltaTime * 1000;
		}
	}

	private void startSliding() {
		if (isSliding) {
			return;
		}
		Debug.Log("sliding");
		isSliding = true;
		animator.SetBool("Sliding", true);
		if (playerInput.isAiming()) {
			velocity += slideBoost * playerInput.aimVector;
		}
		velocity *= 1.1f;
	}

	private void stopSliding() {
		if (isSliding) {
			animator.SetBool("Sliding", false);
			isSliding = false;
		}
	}

	private void playAudio(bool reset) {
		if (reset) {
			audioSource.pitch = 1;
		} else {
			audioSource.pitch *= Mathf.Pow(1.05946f, Random.Range(-7, 7));
		}
		audioSource.Play();
	}

}
