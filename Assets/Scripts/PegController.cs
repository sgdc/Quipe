using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PegController : MonoBehaviour {
	private const float FLASH_DURATION = 1;
	Color endColor;
	IEnumerator coroutine;

	void Start() {
		endColor = this.gameObject.GetComponent<SpriteRenderer>().color;
	}

	void OnCollisionEnter2D (Collision2D c)
	{
		if (coroutine != null) {
			StopCoroutine (coroutine);
		    this.gameObject.GetComponent<SpriteRenderer>().color = endColor;
		}
		coroutine = FlashCollision(c);
		StartCoroutine (coroutine);
	}

	IEnumerator FlashCollision (Collision2D c)
	{
		float progress = 0;
		Color startColor = c.gameObject.GetComponent<SpriteRenderer> ().color;

		SpriteRenderer s = this.gameObject.GetComponent<SpriteRenderer>();
		Color temp = Color.white;
		s.color = startColor;



		while(progress < FLASH_DURATION)
		{
			progress += Time.deltaTime;
			temp.r = Mathf.Lerp(startColor.r, endColor.r, progress / FLASH_DURATION);
			temp.g = Mathf.Lerp(startColor.g, endColor.g, progress / FLASH_DURATION);
			temp.b = Mathf.Lerp(startColor.b, endColor.b, progress / FLASH_DURATION);
			s.color = temp;
			yield return null;
		}
	}


}
