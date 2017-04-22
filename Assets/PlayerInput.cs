using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {

	public bool isFacingRight;
	public Vector2 aimVector;
	PlayerController controller;

	float horizontal;
	float vertical;

	private string A_INPUT = "A_Keyboard_";
	private string B_INPUT = "B_Keyboard_";
	private string A_GAMEPAD = "A_";
	private string B_GAMEPAD = "B_";
	private string X_GAMEPAD = "X_";
	private string HORIZONTAL_INPUT = "L_XAxis_Keyboard_";
	private string VERTICAL_INPUT = "L_YAxis_Keyboard_";
	private string HORIZONTAL_GAMEPAD = "L_XAxis_";
	private string VERTICAL_GAMEPAD = "L_YAxis_";
	private string START_INPUT = "Start_";

	void Start () {
		controller = GetComponent<PlayerController> ();

		int playerId = controller.PLAYER_ID;
		A_INPUT += playerId;
		B_INPUT += playerId;
		HORIZONTAL_INPUT += playerId;
		VERTICAL_INPUT += playerId;
		A_GAMEPAD += playerId;
		B_GAMEPAD += playerId;
		X_GAMEPAD += playerId;
		HORIZONTAL_GAMEPAD += playerId;
		VERTICAL_GAMEPAD += playerId;
		START_INPUT += playerId;

		aimVector = Vector2.zero;
	}

	void Update () {
		horizontal = Input.GetAxisRaw(HORIZONTAL_INPUT) + Input.GetAxisRaw(HORIZONTAL_GAMEPAD);
		vertical = Input.GetAxisRaw(VERTICAL_INPUT) + Input.GetAxisRaw(VERTICAL_GAMEPAD);
		aimVector.x = horizontal;
		aimVector.y = vertical;

		if (aimVector == Vector2.zero) {
			if (isFacingRight) {
				aimVector = Vector2.right;
			}
			else {
				aimVector = -Vector2.right;
			}
		}

	}
}
