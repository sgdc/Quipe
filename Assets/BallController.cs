using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour {
	public Player p;
	public AudioClip[] blips;

	float timeAlive = 0f;
	Color startColor;

	void OnCollisionEnter2D(Collision2D c) {
		AudioSource a = this.GetComponent<AudioSource>();
		a.clip = blips[UnityEngine.Random.Range(0, blips.Length - 1)];
		a.Play();
	}

	void Update ()
	{
		timeAlive += Time.deltaTime;

		if (timeAlive > 20f && p.score == -1) {
			Color temp = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(1, 0, (timeAlive - 20) / 2));
			this.GetComponent<SpriteRenderer>().color = temp;
			if (timeAlive > 22f)
				GameObject.Destroy(this.gameObject);
		} else {
			startColor = this.GetComponent<SpriteRenderer>().color;
		}
	}
}
