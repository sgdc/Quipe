using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChuteController : MonoBehaviour {

	public GameObject topChute;



	void OnTriggerEnter2D (Collider2D c) {
		c.transform.position = topChute.transform.position;
	}
}
