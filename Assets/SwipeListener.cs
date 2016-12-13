using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeListener : MonoBehaviour {

	public string currentID;
	bool isListening;
	UnityEngine.UI.InputField inputField;
	public UnityEngine.UI.Text notification; //do it the hacky way bc the better way stopped working arbitrarily
	// Use this for initialization
	void Start () {
		inputField = (UnityEngine.UI.InputField)Canvas.FindObjectOfType<UnityEngine.UI.InputField>();
		//notification = (UnityEngine.UI.Text)Canvas.FindObjectOfType<UnityEngine.UI.Text>(); //this literally stopped working for no reason. idk.
		currentID = "";
	}
	
	// Update is called once per frame
	void Update () {
		//Give input field constant focus (this ensures we are listening for input always)
		UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(inputField.gameObject, null);
		inputField.OnPointerClick(new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current));

	}

	void SendMessage(string m) {
		TextMesh t = GetComponent<TextMesh>();


		if (m.Length == 13 && m.StartsWith(";") && m.EndsWith("?")){ //ID is 13 long, starts with ;, ends with ?
			notification.text = "ID " + m.Substring(2, 8) + " checked in successfully.";
			notification.color = Color.green;
			StartCoroutine(clearText(1.25f));
		}
		else {
			notification.text = "Swipe did not read successfully.";
			notification.color = Color.red;
			StartCoroutine(clearText(2));
		}


	}

	IEnumerator clearText(float t) {
		yield return new WaitForSeconds(t);

		notification.text = "";
	}
}
