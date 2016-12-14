using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
struct Player {
	public int score;
	public string name;
	public string ID;
}



public class SwipeListener : MonoBehaviour {


	public string currentID;
	bool isListening;
	UnityEngine.UI.InputField inputField;
	private int notificationClear = -1, cornerNotificationClear = -1, endGame = -1;
	private bool gameActive = false;
	public UnityEngine.UI.Text notification; //do it the hacky way bc the better way stopped working arbitrarily
	public UnityEngine.UI.Text cornerNotification;

	List<string> swipes;
	List<Player> users;
	Queue<string> queuedPlayers;

	bool changingPlayer = false;
	Player playerToChange;
	string changing;

	private string outputPath, userDatabasePath;

	// Use this for initialization
	void Start ()
	{
		swipes = new List<string> ();
		queuedPlayers = new Queue<string>();
		inputField = (UnityEngine.UI.InputField)Canvas.FindObjectOfType<UnityEngine.UI.InputField> ();
		//notification = (UnityEngine.UI.Text)Canvas.FindObjectOfType<UnityEngine.UI.Text>(); //this literally stopped working for no reason. idk.
		currentID = "";
		outputPath = @"C:\Users\Adam\Desktop\";
		outputPath += DateTime.Now.ToString ("MM-dd-yyyy");
		outputPath += " Quipe Log.txt";
		if (System.IO.File.Exists (outputPath)) {
			string line;

			// Read the file line by line.
			System.IO.StreamReader file = 
				new System.IO.StreamReader (outputPath);
			while ((line = file.ReadLine ()) != null) {
				swipes.Add (line);
			}
			file.Close ();
		} else { 
			System.IO.File.WriteAllText (outputPath, ""); //ensures file exists and is empty
		}

		users = new List<Player> ();
		userDatabasePath = @"C:\Users\Adam\Desktop\QuipeUsers.data";

		if (System.IO.File.Exists (userDatabasePath)) {
			using (var file = System.IO.File.OpenRead(userDatabasePath))
			{
				var reader = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter ();
			    users = (List<Player>) reader.Deserialize(file); // Reads the entire list.
			}
		}


	}
	
	// Update is called once per frame
	void Update () {
		//Give input field constant focus (this ensures we are listening for input always)
		UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(inputField.gameObject, null);
		inputField.OnPointerClick(new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current));

		alarms();

		if (!gameActive && queuedPlayers.Count > 0) {
			//another player is enqueued, gets to play
			validSwipe(queuedPlayers.Dequeue());
		}
	}

	void SendMessage (string m)
	{
		inputField.text = ""; //clear input field, prevents double swipes
		TextMesh t = GetComponent<TextMesh> ();

		if (changingPlayer) {
			if (changing.Equals("name")){
				playerToChange.name = m;
				notification.text = "Awesome. The name for this card is now " + playerToChange.name + ".";
				notification.color = Color.green;
				notificationClear = seconds(2.5f);

				updatePlayerInDatabase(playerToChange);
				changing = "";
				changingPlayer = false;
			}
		}
		else if ((m.StartsWith("%") || m.StartsWith(";")) && m.EndsWith ("?") && !m.EndsWith ("E?") && m.Length >= 12) { //ID starts with ; or %, ends with a ?, but doesn't end with an E?, which is an error
			if (swipes.Contains (m)) {
				//user already swiped in
				notification.text = "ID " + m.Substring (2, 8) + ", you already swiped in.";
				notification.color = Color.black;
				notificationClear = seconds (2.5f);
			} else {
				//add to swipes list
				swipes.Add (m);

				using (System.IO.StreamWriter file = new System.IO.StreamWriter (outputPath, true)) {
					file.WriteLine (m); //write swipes to output file
				}

				cornerNotification.text = "ID " + m.Substring(2, 8) + " swiped in successfully.";
				cornerNotificationClear = seconds(2);


				// ENQUEUE IF GAMEACTIVE, ELSE EXECUTE IMMEDIATELY. UPDATE CHECKS IF !GAMEACTIVE AND SOMETHING IN QUEUE. IF SOMETHING IN QUEUE, RE-EXECUTE GAME IMMEDIATELY
				if (!gameActive)
					validSwipe(m);
				else {
					queuedPlayers.Enqueue(m);
				}
			}
		}
		else if (m.StartsWith("n") && m.EndsWith ("?") && !m.EndsWith ("E?") && m.Length >= 12 && findPlayerInDatabase(m.Substring(1)).score != -1)//valid ID starting with n, name update
		{
			Player p = findPlayerInDatabase(m.Substring(1));
			playerToChange = p;
			changingPlayer = true;
			changing = "name";
			notification.text = "Type a new name for account " + p.name + ".";
			notification.color = Color.green;
			notificationClear = seconds(2);
		}
		else {
			notification.text = "Swipe did not read successfully.";
			notification.color = Color.grey;
			notificationClear = seconds(2.5f);
		}
	}

	void validSwipe(string m) {
		string notif = "";
		Player p = findPlayerInDatabase(m);
		if (p.score == -1) {
			//new user
			p = new Player();
			p.ID = m;
			p.name = m.Substring(2, 8);
			p.score = 0;
			notif = "Welcome to Quipe! Your ID will be " + p.name + " and your current score is: 0";
			users.Add(p);
		} else {
			p.score += 1; //todo: remove this
			notif = "Welcome back, " + p.name + "! Your current score is: " + p.score.ToString();
			updatePlayerInDatabase(p);
		}


		notification.text = notif;
		notification.color = Color.green;
		notificationClear = seconds (3f);

		saveDatabase();

		ExecuteGame(p);
	}

	int seconds(float s) {
		return Mathf.FloorToInt(60 * s);
	}

	void saveDatabase() {
		using (var file = System.IO.File.OpenWrite (userDatabasePath)) {
			var writer = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter ();
			writer.Serialize (file, users); // Writes the entire list to file
		}
	}

	Player findPlayerInDatabase(string searchingID) {
		Player ret = new Player();
		ret.score = -1;
		foreach(Player p in users) {
			if (p.ID == searchingID) {
				ret = p;
			}
		}

		return ret;
	}

	void updatePlayerInDatabase(Player n) {
		Player toUpdate = users.Find(x => x.ID == n.ID);
		users.Remove(toUpdate);
		users.Add(n);
		saveDatabase();
	}

	void alarms() {
		//alarms, timed events (kinda hacky)

		//decrement each timer by 1
		if (notificationClear > -1)
			notificationClear--;
		if (cornerNotificationClear > -1)
			cornerNotificationClear--;
		if (endGame > -1)
			endGame--;

		//if 0, execute code
		if (notificationClear == 0)
			notification.text = ""; //clear notification text
		if (cornerNotificationClear == 0)
			cornerNotification.text = "";
		if (endGame == 0)
			gameActive = false;
	}

	void ExecuteGame(Player p) {
		//todo, game
		gameActive = true;
		endGame = seconds(5);
	}
}
