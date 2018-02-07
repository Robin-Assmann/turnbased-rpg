using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using ExitGames.Client.Photon;

public class Login : MonoBehaviour {

	public GameObject k_input_name, k_input_password, pre_Player;
	public bool PlayerLoaded, Save, Connected;

	public static Login k_Login;

	private int[] exparray;

	public int exp;

	public void Awake(){

		string tp_sql = "INSERT INTO items (item_name, type, rarity, item_level, strength, dexterity,intelligence,health,armor,dodge,speed,res_fire,res_ice,res_lightning,min_dps,max_dps, magic_damage) VALUES ('New','0','0','10','0','0','0','0','0','0','0','0','0','0','0','0','0');";

		ConnectAndJoinRandom tp_connect = new ConnectAndJoinRandom ();
		tp_connect.Connect ();

		exparray = new int[100];

		for (int i = 0; i < 50; i++) {
		
			exparray [i] = (int) (Mathf.Pow (i + 1, 3.0f) + i * i/4);
		}
		Connected = false;
		Save = false;
		k_Login = this;

		PhotonNetwork.OnEventCall += this.OnEvent;

		if (PlayerPrefs.GetInt ("Saved") == 1) {
		
			k_input_name.GetComponent<InputField> ().text = PlayerPrefs.GetString ("PlayerName");
			k_input_password.GetComponent<InputField> ().text = PlayerPrefs.GetString ("PlayerPassword");
			//StartCoroutine (Go ());
		}
	}

	void Update(){
	
		if (Input.GetKeyDown (KeyCode.Tab) && k_input_name.GetComponent<InputField> ().isFocused) {

			k_input_password.GetComponent<InputField> ().Select ();
		}

		if(Input.GetKeyDown (KeyCode.Return)){
			TryLogin ();
		}
	}

	public int GetLevel(){
	
		int level = 1;
		for (int i = 0; i < 100; i++) {
		
			exp -= exparray [i];
			if (exp < 0) {
			
				return level;
			} else {
				level++;
			}
		}

		return 1;
	}

	public void TryLogin(){
		StartCoroutine (Go ());
	}

	IEnumerator Go(){
		yield return new WaitUntil(()=> Connected);
		string itemsdatastring = "";

			int u = int.Parse(OnlineData.k_storage.tp_data [0]);
			OnlineData.k_storage.tp_data [0] = (u +1) +"";
		string[] content = new string[] {u+"", k_input_name.GetComponent<InputField> ().text , "password",k_input_password.GetComponent<InputField> ().text};
			RaiseEventOptions opt = new RaiseEventOptions ();
			opt.Receivers = ReceiverGroup.MasterClient;
			PhotonNetwork.RaiseEvent (0, content, false, opt);

			string starting = OnlineData.k_storage.tp_data [u];
			yield return new WaitUntil (() => (!starting.Equals (OnlineData.k_storage.tp_data[u])));

			itemsdatastring = OnlineData.k_storage.tp_data [u];
			OnlineData.k_storage.DataReceived = false;

		int id = int.Parse (itemsdatastring);

		if (id > 0) {
			StartCoroutine (GetID (id));
		
		}


//			if (itemsdatastring == k_input_password.GetComponent<InputField> ().text && itemsdatastring != "") {
//
//				StartCoroutine (GetID ());
//
//			} else {
//			}

	}

	public void CreateUser(){

//		if (k_input_password.GetComponent<InputField> ().text != "" && k_input_name.GetComponent<InputField> ().text != "") {
//
//			string[] content = new string[] { k_input_name.GetComponent<InputField> ().text, k_input_password.GetComponent<InputField> ().text };
//			RaiseEventOptions opt = new RaiseEventOptions ();
//			opt.Receivers = ReceiverGroup.MasterClient;
//			PhotonNetwork.RaiseEvent (1, content, true, opt);
//
//			StartCoroutine(GetID ());
//
//		} else {
//		}
	}

	IEnumerator GetID(int id){

		PhotonNetwork.OnEventCall -= this.OnEvent;
		GameObject Playermanager = GameObject.FindGameObjectWithTag ("PlayerManager");
		DestroyImmediate (Playermanager.GetComponent<OnlineData> ());

		Transform tp_spawn = GameObject.FindGameObjectWithTag("PlayerManager").transform;
		GameObject tp_Player = Instantiate (pre_Player) as GameObject;
		tp_Player.transform.SetParent(tp_spawn, false);
//		string itemdata = "";
//
//			int u = int.Parse(OnlineData.k_storage.tp_data [0]);
//			OnlineData.k_storage.tp_data [0] = (u +1) +"";
//			string[] content = new string[] {u+"", k_input_name.GetComponent<InputField> ().text,"player_id" };
//			RaiseEventOptions opt = new RaiseEventOptions ();
//			opt.Receivers = ReceiverGroup.MasterClient;
//			PhotonNetwork.RaiseEvent (0, content, true, opt);
//
//			string starting = OnlineData.k_storage.tp_data [u];
//			yield return new WaitUntil (() => (!starting.Equals (OnlineData.k_storage.tp_data[u])));
//
//			itemdata = OnlineData.k_storage.tp_data [u];
//			OnlineData.k_storage.DataReceived = false;

		GameObject tp_manager = GameObject.FindGameObjectWithTag ("PlayerManager");
		tp_Player.GetComponent<Player> ().PlayerID = id;
		tp_Player.GetComponent<Player> ().PlayerName = k_input_name.GetComponent<InputField> ().text;
		tp_Player.GetComponent<Player> ().LoginCorrect = true;

		yield return new WaitUntil (() => PlayerLoaded);
		if (Save) {
			PlayerPrefs.SetInt ("Saved", 1);
			PlayerPrefs.SetString ("PlayerName", k_input_name.GetComponent<InputField> ().text);
			PlayerPrefs.SetString ("PlayerPassword", k_input_password.GetComponent<InputField> ().text);
		}

		RaiseEventOptions newopt = new RaiseEventOptions ();
		newopt.Receivers = ReceiverGroup.MasterClient;
		PhotonNetwork.RaiseEvent (14, "", true, newopt);
		SceneManager.LoadScene (1);
	}

	public void SavePassword(){

		Save = !Save;

	}

	private void OnEvent(byte eventCode, object content, int senderid){
		if (PhotonNetwork.isNonMasterClientInRoom && (int)eventCode >= 0 && (int)eventCode !=2 && (int)eventCode !=3 && (int)eventCode !=5) {
			string tp_content = "";
			tp_content = (string)content;
			string[] tp_array = tp_content.Split ("+" [0]);
			OnlineData.k_storage.tp_data [int.Parse(tp_array[1])] = tp_array[0];
			OnlineData.k_storage.DataReceived = true;
		}
	}
}
