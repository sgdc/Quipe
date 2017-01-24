using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningPlatform : MonoBehaviour {
	public float rotateSpeed = 10.0f;

	void FixedUpdate () {
		this.GetComponent<Rigidbody2D>().MoveRotation(this.GetComponent<Rigidbody2D>().rotation + rotateSpeed);
	}
}
