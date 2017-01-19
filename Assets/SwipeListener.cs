using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct Player {
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
	public UnityEngine.UI.Text SwipeCount;
	public UnityEngine.UI.Text UserDBSize;

	List<string> swipes;
	List<Player> users;
	Queue<string> queuedPlayers;

	bool changingPlayer = false;
	Player playerToChange;
	string changing;

	private string outputPath, userDatabasePath;

	Player playingGame;

	public GameObject spawner;

	bool spawnerMovingLeft = false;
	float spawnerMoveCurrent = 0;
	float spawnerMoveTotal = 1.4f;

	// Use this for initialization
	void Start ()
	{
		swipes = new List<string> ();
		queuedPlayers = new Queue<string>();
		inputField = (UnityEngine.UI.InputField)Canvas.FindObjectOfType<UnityEngine.UI.InputField> ();
		//notification = (UnityEngine.UI.Text)Canvas.FindObjectOfType<UnityEngine.UI.Text>(); //this literally stopped working for no reason. idk.
		currentID = "";
		outputPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\";
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
			SwipeCount.text = "Swipes: " + swipes.Count.ToString();
		} else { 
			System.IO.File.WriteAllText (outputPath, ""); //ensures file exists and is empty
		}

		users = new List<Player> ();
		userDatabasePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\QuipeUsers.data";

		if (System.IO.File.Exists (userDatabasePath)) {
			using (var file = System.IO.File.OpenRead(userDatabasePath))
			{
				var reader = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter ();
			    users = (List<Player>) reader.Deserialize(file); // Reads the entire list.
			}

			UserDBSize.text = "User Database Size: " + users.Count.ToString();
		}


	}
	
	// Update is called once per frame
	void Update ()
	{
		//Give input field constant focus (this ensures we are listening for input always)
		UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject (inputField.gameObject, null);
		inputField.OnPointerClick (new UnityEngine.EventSystems.PointerEventData (UnityEngine.EventSystems.EventSystem.current));


		if (Input.GetKeyDown (KeyCode.CapsLock)) { //export IDs to AHK file
			string ahkPath = Environment.GetFolderPath (Environment.SpecialFolder.Desktop) + @"\";
			ahkPath += DateTime.Now.ToString ("MM-dd-yyyy");
			ahkPath += " Quipe.ahk";

			System.IO.File.WriteAllText (ahkPath, ""); //ensures file exists and is blank

			using (System.IO.StreamWriter file = new System.IO.StreamWriter (ahkPath, true)) {
				file.WriteLine ("Home::");
				foreach (string m in swipes) {
					file.WriteLine ("\tSendRaw, `" + m);
					file.WriteLine ("\tSend, {Return}");
					file.WriteLine ("\tKeyWait, CapsLock, D");
				}
				file.WriteLine ("Return");

			}

			notification.text = "Swipes exported to AHK.";
			notification.color = Color.blue;
			notificationClear = 60;
		}

		if (Input.GetKeyDown (KeyCode.LeftControl)) {
			string leaderboardPath = Environment.GetFolderPath (Environment.SpecialFolder.Desktop) + @"\";
			leaderboardPath += DateTime.Now.ToString ("MM-dd-yyyy");
			leaderboardPath += " Scores.csv";

			System.IO.File.WriteAllText (leaderboardPath, ""); //ensure file exists and is empty

			users.Sort((s1, s2) => s2.score.CompareTo(s1.score));

			using (System.IO.StreamWriter file = new System.IO.StreamWriter (leaderboardPath, true)) {
				file.WriteLine("Name, Score");
				file.WriteLine();
				foreach (Player p in users) {
					file.WriteLine(p.name + ", " + p.score.ToString());
				}
			}

			notification.text = "Leaderboard exported to text.";
			notification.color = Color.blue;
			notificationClear = 60;

		}

		if (Input.GetKeyDown(KeyCode.Tab) && Application.isEditor) {
			swipes.Clear();
			notification.text = "Clearing instance swipe list. Cheater.";
			notification.color = Color.blue;
			notificationClear = 60;
		}


		alarms();
		moveSpawner();


	}

	void moveSpawner() {
			spawner.transform.position = new Vector2(Mathf.Lerp(-10f, 10f, spawnerMoveCurrent / spawnerMoveTotal), spawner.transform.position.y);
			if (spawnerMovingLeft) {
				spawnerMoveCurrent += Time.deltaTime;
				if (spawnerMoveCurrent >= spawnerMoveTotal)
				{
					spawnerMoveCurrent = spawnerMoveTotal;
					spawnerMovingLeft = false;
				}
			} else {
				spawnerMoveCurrent -= Time.deltaTime;
				if (spawnerMoveCurrent <= 0) {
					spawnerMoveCurrent = 0;
					spawnerMovingLeft = true;
				}
			}
	}

	void SendMessage (string m)
	{
		inputField.text = ""; //clear input field, prevents double swipes
		TextMesh t = GetComponent<TextMesh> ();

		if (changingPlayer) {
			if (changing.Equals ("name")) {
				playerToChange.name = m;
				notification.text = "The name for this card is now " + playerToChange.name + ".";
				notification.color = Color.green;
				notificationClear = seconds (2.5f);

				updatePlayerInDatabase (playerToChange);
				changing = "";
				changingPlayer = false;
			}
		} else if ((m.StartsWith ("%") || m.StartsWith (";")) && m.EndsWith ("?") && !m.EndsWith ("E?") && m.Length >= 12) { //ID starts with ; or %, ends with a ?, but doesn't end with an E?, which is an error
			if (swipes.Contains (m)) {
				//user already swiped in
				Player p = findPlayerInDatabase (m);
				if (p.score > -1) {
					notification.text = p.name + ", you already swiped in.\n\nYour current score is: " + p.score.ToString();
					notification.color = Color.black;
					notificationClear = seconds (2.5f);
				} else {
					notification.text = "ID " + m.Substring (2, 8) + ", you already swiped in.";
					notification.color = Color.black;
					notificationClear = seconds (2.5f);
				}
			} else {
				//add to swipes list
				swipes.Add (m);
				SwipeCount.text = "Swipes: " + swipes.Count.ToString();

				using (System.IO.StreamWriter file = new System.IO.StreamWriter (outputPath, true)) {
					file.WriteLine (m); //write swipes to output file
				}

				cornerNotification.text = "ID " + m.Substring(2, 8) + " swiped in successfully.";
				cornerNotificationClear = seconds(5);

				validSwipe(m);

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
			notification.color = Color.black;
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
			notif = "Welcome to Quipe! Your ID will be " + p.name + ".";
			users.Add(p);
		} else {
			notif = "Welcome back, " + p.name + "! Your score before today was: " + p.score.ToString();
			updatePlayerInDatabase(p);
		}

		UserDBSize.text = "User Database Size: " + users.Count.ToString();


		notification.text = notif;
		notification.color = Color.green;
		notificationClear = -1;

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
	}

	void ExecuteGame(Player p) {
		GameObject newBall = GameObject.Instantiate(spawner);
		newBall.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
		newBall.GetComponent<CircleCollider2D>().isTrigger = false;
		newBall.GetComponent<Rigidbody2D>().velocity = new Vector2(UnityEngine.Random.Range(-0.5f, 0.5f), 0);
		newBall.GetComponent<BallController>().p = p;
		newBall.GetComponent<SpriteRenderer>().color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);

	}

	public void ReceiveScore(Player p, int score) {
		p.score += score;

		string s = p.name + " scored " + score.ToString() + " points! Their total is now: " + p.score.ToString() + "\nThanks for swiping in!";
		notification.text = s;
		notification.color = Color.green;

		updatePlayerInDatabase(p);

		notificationClear = seconds(5);
		endGame = seconds(3.25f);
	}
}
