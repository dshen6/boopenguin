using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentTilter : MonoBehaviour {
	public float cx = 3.14f;
	public float cy = 0f;
	public float cz = 1f;

	public float sx = 1f;
	public float sy = 1f;
	public float sz = 1f;

	public float ax = 1f;
	public float ay = 0.2f;
	public float az = 0.05f;

	void Start () {
	}

	void FixedUpdate () {
		float t = Time.time * 5;
		gameObject.transform.eulerAngles = new Vector3(
			sx * Mathf.Cos(ax * t + cx),
			sy * Mathf.Cos(ay * t + cy),
			sz * Mathf.Cos(az * t + cz)
		);
	}
	
	void Update () {
		
	}
}
