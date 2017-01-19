using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour {
	public Player p;
	public AudioClip[] blips;

	void OnCollisionEnter2D(Collision2D c) {
		AudioSource a = this.GetComponent<AudioSource>();
		a.clip = blips[UnityEngine.Random.Range(0, blips.Length - 1)];
		a.Play();
	}
}
