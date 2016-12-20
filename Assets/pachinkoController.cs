﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pachinkoController : MonoBehaviour {

	public int score;
	GameObject toDestroy;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D (Collider2D c) {
		SwipeListener s = GameObject.FindGameObjectWithTag("Player").GetComponent<SwipeListener>();
		s.ReceiveScore(score);
		toDestroy = c.gameObject;
		StartCoroutine(destroyObject(1.5f));
	}

	IEnumerator destroyObject(float t) {
		yield return new WaitForSeconds(t);

		Destroy(toDestroy);
	}
}
