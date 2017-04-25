using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStateController : MonoBehaviour {


	public Canvas canvas;
	public GameObject playerPrefab;

	public int numberPlayers = 2;

	void OnEnable() {
		PlayerController.OnPlayerFall += PlayerFall;
	}

	void OnDisable() {
		PlayerController.OnPlayerFall -= PlayerFall;
	}

	void PlayerFall(int playerId) {
		StartCoroutine(instantiatePlayerWithDelay(playerId));
	}

	void Start() {
		for (int i = 1; i <= numberPlayers; i++) {
			instantiatePlayer(i);
		}
	}

	IEnumerator instantiatePlayerWithDelay(int playerId) {
		yield return new WaitForSeconds(.7f);
		instantiatePlayer(playerId);
	}

	void instantiatePlayer(int playerId) {
		GameObject player = Instantiate(playerPrefab) as GameObject;
		player.GetComponent<PlayerController>().PLAYER_ID = playerId;
		player.transform.position = positionForPlayerId(playerId);
		canvas.GetComponentsInChildren<ControlsGuiController>()[playerId - 1].setPlayer(player);
	}

	Vector3 positionForPlayerId(int playerId) {
		if (numberPlayers == 4) {
			switch (playerId) {
			case 1:
				return new Vector3(-.75f, .75f, 0);
			case 2:
				return new Vector3(.75f, .75f, 0);
			case 3:
				return new Vector3(-.75f, -.75f, 0);
			case 4:
				return new Vector3(.75f, -.75f, 0);
			default:
				return Vector3.zero;
			} 
		} else if (numberPlayers == 3) {
			switch (playerId) {
			case 1:
				return new Vector3(-.75f, .75f, 0);
			case 2:
				return new Vector3(.75f, .75f, 0);
			case 3:
				return new Vector3(0, -.75f, 0);
			default:
				return Vector3.zero;
			}
		} else {
			switch (playerId) {
			case 1:
				return new Vector3(-.75f, 0, 0);
			case 2:
				return new Vector3(.75f, 0, 0);
			default:
				return Vector3.zero;
			}
		}
	}
	
	// Update is called once per frame
	void Update() {
		
	}
}
