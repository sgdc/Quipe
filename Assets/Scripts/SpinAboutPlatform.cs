using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAboutPlatform : MonoBehaviour {
	public float rotateSpeed = 10.0f;
	Vector2 initialPosition;
	public Vector2 pointOffset;

	void Start() {
		initialPosition = transform.position;
	}

	void FixedUpdate () {
		this.transform.RotateAround(new Vector3(initialPosition.x + pointOffset.x, initialPosition.y + pointOffset.y, 0), new Vector3(0, 0, 1), rotateSpeed);
	}
}
