using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour{

	public int PlayerID;
	public Character[] Characters { get; set;}
	public bool LoginCorrect;
	private GameObject pre_Item, k_CurrencyText;
	public List<Item> InventoryItems;
	public List<Item> OutstandingLoot;
	private string tp_characters;
	private string[] tp_characterArray;
	private bool ready, itemsloaded;
	private int itemCount, loadedItems;
	public int CharactersLoaded;
	public bool sceneloaded;
	public bool isStarter;
	public string PlayerName = "";

	public int Currency;

	public OnlineData k_storage;
	public static bool BattleLoaded;

	void Awake(){
		isStarter = false;
		BattleLoaded = false;
		tp_characters = "";
		k_storage = transform.GetComponent<OnlineData> ();
		CharactersLoaded = 0;
		itemCount = 0;
		loadedItems = 0;
		InventoryItems = new List<Item> ();
		OutstandingLoot = new List<Item> ();
		itemsloaded = false;
		LoginCorrect = false;
		sceneloaded = false;
		Characters = new Character[0];
		StartCoroutine (WaitForLogin());
		GameObject tp_manager = GameObject.FindGameObjectWithTag ("PlayerManager");
		DontDestroyOnLoad (tp_manager);
		PhotonNetwork.OnEventCall += this.OnEvent;
	}

	private void SendData(){

		PhotonNetwork.player.name = PlayerName;

		//if (PhotonNetwork.room.playerCount > 1) {
			object[] content = new object[] {this.PlayerName, PhotonNetwork.player};
			RaiseEventOptions opt = new RaiseEventOptions ();
			opt.Receivers = ReceiverGroup.Others;
			PhotonNetwork.RaiseEvent (99, content, true, opt);

			foreach(PhotonPlayer tp_player in PhotonNetwork.otherPlayers){
				if(!tp_player.isMasterClient)
				CreationManager.k_Manager.MakePlayerOnline (tp_player.name, tp_player);
			}
		//}
	}

	IEnumerator WaitTillLoaded(){
		yield return new WaitUntil(() => itemsloaded);
		yield return new WaitUntil(() => (CharactersLoaded == Characters.Length));
		Login.k_Login.PlayerLoaded = true;
	}

	IEnumerator WaitForLogin(){
	
		yield return new WaitUntil (() => LoginCorrect);


		StartCoroutine (LoadCharacters ());
		StartCoroutine (LoadInventory ());

		yield return new WaitUntil (() => sceneloaded);

		SendData ();
		StartCoroutine (LoadCurrency ());
	}

	#region Character

	public void MakeCharacters(){
		for (int i = 0; i < tp_characterArray.Length -1; i++) {
			string[] tp_new = tp_characterArray [i].Split ("|" [0]);
//			CreateCharacter (int.Parse (tp_new [0]),tp_new [1],int.Parse (tp_new [2]),int.Parse (tp_new [3]),int.Parse (tp_new [4]),float.Parse(tp_new[5]),int.Parse (tp_new [6]),int.Parse (tp_new [7]),int.Parse (tp_new [8]),int.Parse (tp_new [9]),tp_new [10],int.Parse (tp_new [11]),int.Parse (tp_new [12]),int.Parse (tp_new [13]),tp_new[14], tp_new[15], tp_new[16],int.Parse(tp_new[17]),int.Parse(tp_new[18]),int.Parse(tp_new[19]),int.Parse(tp_new[20]),int.Parse(tp_new[21]));
			CreateCharacter (int.Parse (tp_new [0]),tp_new [1],int.Parse(tp_new [2]),tp_new [3],tp_new [4],int.Parse (tp_new [5]),int.Parse(tp_new[6]),int.Parse(tp_new [7]),tp_new [8],int.Parse (tp_new [9]),int.Parse (tp_new [10]),int.Parse (tp_new [11]),int.Parse(tp_new [12]),int.Parse (tp_new [13]),int.Parse (tp_new [14]),tp_new [15],tp_new[16],tp_new[17]);

		}
		StartCoroutine (WaitTillLoaded ());
	}

	public void CreateCharacter(int tp_characterid, string tp_charactername, int rarity, string race, string type, int tp_maxhealth,int tp_currenthealth,int tp_position, string tp_skills, int tp_exp, int tp_strength,int constituition, int tp_dex, int tp_intelligence, int luck,  string tp_slots, string tp_backpack, string tp_skillpoints){
										//bool tp_isEnemy, int tp_characterid, string tp_charactername, int tp_rarity, string tp_race, string tp_type, int tp_maxhealth,int tp_currenthealth,int tp_position, string tp_skills, int tp_exp, int tp_strength,int tp_constituition, int tp_dexterity, int tp_intelligence, int tp_luck,  string tp_slots, string tp_backpack
		Character tp_char = new Character (false, tp_characterid, tp_charactername, rarity, race, type, tp_maxhealth, tp_currenthealth, tp_position, tp_skills, tp_exp, tp_strength, constituition, tp_dex, tp_intelligence, luck,  tp_slots, tp_backpack, tp_skillpoints);

		List<Character> tp_List =  new List<Character> (Characters);
		tp_List.Add (tp_char);

		Characters = tp_List.ToArray ();
	}

//	public void CreateCharacter(int tp_characterid, string tp_charactername ,int tp_maxhealth,int tp_currenthealth, int tp_armor, float tp_dodge, int tp_position, int tp_mindps, int tp_maxdps, int tp_speed, string tp_type, int tp_fire, int tp_ice, int tp_shock, string tp_skills, string tp_slots, string tp_backpack, int tp_exp, int tp_strength, int tp_dex, int tp_intelligence, int tp_magdmg){
//
//		Character tp_char = new Character (false,tp_characterid,tp_charactername ,tp_maxhealth,tp_currenthealth,tp_armor,tp_dodge,tp_position,tp_mindps,tp_maxdps,tp_speed,tp_type,tp_fire,tp_ice, tp_shock,tp_skills, tp_slots, tp_backpack, tp_exp , tp_strength, tp_dex,tp_intelligence, tp_magdmg);
//	
//		List<Character> tp_List =  new List<Character> (Characters);
//		tp_List.Add (tp_char);
//
//		Characters = tp_List.ToArray ();
//	}
//
	public void CutString(){

		tp_characterArray = tp_characters.Split (";"[0]);

	}

	IEnumerator LoadCharacters(){

			int u = int.Parse(OnlineData.k_storage.tp_dataCharacters [0]);
			OnlineData.k_storage.tp_dataCharacters [0] = (u + 8) +"";

			string[] content = new string[] {u+"", PlayerID +"" };
			RaiseEventOptions opt = new RaiseEventOptions ();
			opt.Receivers = ReceiverGroup.MasterClient;
			PhotonNetwork.RaiseEvent (2, content, true, opt);

			string starting = OnlineData.k_storage.tp_dataCharacters [u];

			yield return new WaitUntil (() => (!starting.Equals (OnlineData.k_storage.tp_dataCharacters[u])));

			int amount = 1;
			for (int i = 0; i < 10; i++) {

				int e = u + i;
				if (e > 29)
					e -= 30;

				if (OnlineData.k_storage.tp_dataCharacters [e].Length > 0) {
					amount++;
				} else {
					break;
				}
			}
			int o= amount;
			tp_characterArray = new string[o];
			for (int i = 0; i < o; i++) {
				tp_characterArray [i] = OnlineData.k_storage.tp_dataCharacters [u + i];	
			}
			OnlineData.k_storage.DataReceived = false;
		
		MakeCharacters ();
	}

	public void MakeSimpleCharacters(bool isEnemy, string[] Characters){
		int o = Characters.Length;

		if (!isEnemy)
			o = o - 1;

		for (int i = 0; i < o; i++) {

			string[] Para = Characters [i].Split ("/" [0]);
		
			Character tp_char = new Character (isEnemy,int.Parse(Para [0]), Para [1],int.Parse(Para [2]),int.Parse(Para [3]),int.Parse(Para [4]),int.Parse(Para [5]),int.Parse(Para [6]), Para [7],int.Parse(Para [8]),int.Parse(Para [9]),int.Parse(Para [10]));

			StartCoroutine (WaitTillGameManagerLoaded (isEnemy, tp_char));
		}
	}

	IEnumerator WaitTillGameManagerLoaded(bool trigger, Character tp_char){
	
		yield return new WaitUntil (() => BattleLoaded);

		if (trigger) {
			GameManager.k_Manager.Enemy.Add (tp_char);
			GameManager.k_Manager.EnemiesLoaded = true;
		} else {
			GameManager.k_Manager.Friendly.Add (tp_char);
		}

	}


	#endregion

	#region Inventory

	IEnumerator LoadInventory(){

		string tp_invstring = "";

//		if (!PhotonNetwork.isNonMasterClientInRoom) {
//			WWWForm form = new WWWForm ();
//			form.AddField ("sql_text_post", "SELECT * FROM player WHERE player_id='" + PlayerID + "';");
//			form.AddField ("row_name_post", "inventory");
//			WWW itemsdata = new WWW ("https://localhost/RPG/GetPlayer.php", form);
//			yield return itemsdata;
//			tp_invstring = itemsdata.text;
//		} else {

			int u = int.Parse(OnlineData.k_storage.tp_data [0]);
			OnlineData.k_storage.tp_data [0] = (u + 1) +"";
			string[] content = new string[] {u+"", PlayerID+"","inventory"};
			RaiseEventOptions opt = new RaiseEventOptions ();
			opt.Receivers = ReceiverGroup.MasterClient;
			PhotonNetwork.RaiseEvent (4, content, true, opt);

			string starting = OnlineData.k_storage.tp_data [u];

			yield return new WaitUntil (() => (!starting.Equals (OnlineData.k_storage.tp_data [u])));

			tp_invstring = OnlineData.k_storage.tp_data [u];
			OnlineData.k_storage.DataReceived = false;
	
		UseInventory (tp_invstring);
	}

	public void UseInventory(string tp_string){
		string[] tp_array = tp_string.Split ("/" [0]);
		for (int i = 0; i < tp_array.Length; i++) {
			if (int.Parse (tp_array [i]) != 0) {
				itemCount++;
				StartCoroutine (LoadItem (int.Parse (tp_array [i]), i));
			}
		}
		if (itemCount == 0) {
			itemsloaded = true;
		}
	}

	IEnumerator LoadItem(int tp_id, int tp_pos){

		string tp_itemstring = "";

//		if (!PhotonNetwork.isNonMasterClientInRoom) {
//			WWWForm form = new WWWForm ();
//			form.AddField ("sql_text_post", "SELECT * FROM items WHERE item_id='" + tp_id + "';");
//			WWW itemsdata = new WWW ("https://localhost/RPG/GetItem.php", form);
//			yield return itemsdata;
//			tp_itemstring = itemsdata.text;	
//
//		} else {

			int pos = int.Parse(OnlineData.k_storage.tp_dataInventoryItems [0]);
			OnlineData.k_storage.tp_dataInventoryItems [0] = (pos + 2) +"";
			string[] content = new string[] {pos+"", tp_id+""};
			RaiseEventOptions opt = new RaiseEventOptions ();
			opt.Receivers = ReceiverGroup.MasterClient;
			PhotonNetwork.RaiseEvent (5, content, false, opt);
			string starting = OnlineData.k_storage.tp_dataInventoryItems [pos];
			yield return new WaitUntil (() => (!starting.Equals (OnlineData.k_storage.tp_dataInventoryItems [pos])));
			tp_itemstring = OnlineData.k_storage.tp_dataInventoryItems [pos];
			OnlineData.k_storage.DataReceived = false;
		

		string[] tp_itemarray = tp_itemstring.Split ("|" [0]);

		//int tp_id, string tp_name,string tp_type, string tp_rarity,int tp_position,int tp_health, int tp_armor, float tp_dodge, int tp_mindps, int tp_maxdps, int tp_speed, int tp_fire, int tp_ice,int tp_light
		InventoryItems.Add (new Item (int.Parse (tp_itemarray [0]), tp_itemarray [1], tp_itemarray [2], tp_itemarray [3],int.Parse (tp_itemarray [4]),int.Parse (tp_itemarray [5]), tp_pos, int.Parse (tp_itemarray [6]), int.Parse (tp_itemarray [7]), int.Parse (tp_itemarray [8]), int.Parse (tp_itemarray [9]), int.Parse (tp_itemarray [10]), int.Parse (tp_itemarray [11]), int.Parse (tp_itemarray [12]), int.Parse (tp_itemarray [13]),int.Parse (tp_itemarray [14]),float.Parse (tp_itemarray [15]),int.Parse (tp_itemarray [16]),int.Parse (tp_itemarray [17]),int.Parse (tp_itemarray [18]),int.Parse (tp_itemarray [19]),int.Parse (tp_itemarray [20]),int.Parse (tp_itemarray [21]),int.Parse (tp_itemarray [22]),int.Parse (tp_itemarray [23]),int.Parse (tp_itemarray [24]),int.Parse (tp_itemarray [25]),int.Parse (tp_itemarray [26]),int.Parse (tp_itemarray [27])));
		loadedItems++;
		if (loadedItems == itemCount) {
			itemsloaded = true;
		}
	}

	public void MakeItems(string tp_array){
		string[] tp_itemarray = tp_array.Split ("|" [0]);
		Item tp_item = new Item (int.Parse (tp_itemarray [0]), tp_itemarray [1], tp_itemarray [2], tp_itemarray [3],int.Parse (tp_itemarray [4]),int.Parse (tp_itemarray [5]), 0, int.Parse (tp_itemarray [6]), int.Parse (tp_itemarray [7]), int.Parse (tp_itemarray [8]), int.Parse (tp_itemarray [9]), int.Parse (tp_itemarray [10]), int.Parse (tp_itemarray [11]), int.Parse (tp_itemarray [12]), int.Parse (tp_itemarray [13]),int.Parse (tp_itemarray [14]),float.Parse (tp_itemarray [15]),int.Parse (tp_itemarray [16]),int.Parse (tp_itemarray [17]),int.Parse (tp_itemarray [18]),int.Parse (tp_itemarray [19]),int.Parse (tp_itemarray [20]),int.Parse (tp_itemarray [21]),int.Parse (tp_itemarray [22]),int.Parse (tp_itemarray [23]),int.Parse (tp_itemarray [24]),int.Parse (tp_itemarray [25]),int.Parse (tp_itemarray [26]),int.Parse (tp_itemarray [27]));
		OutstandingLoot.Add (tp_item);
//		GameManager.k_Manager.LootLoaded = true;
	}

	#endregion

	#region Currency
	IEnumerator LoadCurrency (){

		k_CurrencyText = GameObject.FindGameObjectWithTag ("Currency");
		string tp_invstring = "";

//		if (!PhotonNetwork.isNonMasterClientInRoom) {
//			WWWForm form = new WWWForm ();
//			form.AddField ("sql_text_post", "SELECT * FROM player WHERE player_id='" + PlayerID + "';");
//			form.AddField ("row_name_post", "currency");
//			WWW itemsdata = new WWW ("https://localhost/RPG/GetPlayer.php", form);
//			yield return itemsdata;
//			tp_invstring = itemsdata.text;
//		} else {

			int u = int.Parse(OnlineData.k_storage.tp_data [0]);
			OnlineData.k_storage.tp_data [0] = (u + 1) +"";
			string[] content = new string[] {u+"", PlayerID+"","currency"};
			RaiseEventOptions opt = new RaiseEventOptions ();
			opt.Receivers = ReceiverGroup.MasterClient;
			PhotonNetwork.RaiseEvent (4, content, true, opt);

			string starting = OnlineData.k_storage.tp_data [u];

			yield return new WaitUntil (() => (!starting.Equals (OnlineData.k_storage.tp_data [u])));

			tp_invstring = OnlineData.k_storage.tp_data [u];
			OnlineData.k_storage.DataReceived = false;
		

		this.Currency = int.Parse (tp_invstring);

		k_CurrencyText.GetComponent<Text> ().text = this.Currency+ "";
	}

	public void UpdateCurrency(){

		StartCoroutine (UpdateCurrencys());
	
	}

	IEnumerator UpdateCurrencys (){
	
		string sql_text = "UPDATE player SET currency=" + this.Currency + " WHERE player_id=" + this.PlayerID + ";";
		if (!PhotonNetwork.isNonMasterClientInRoom) {
			WWWForm form = new WWWForm ();
			form.AddField ("sql_text_post", sql_text);
			WWW www = new WWW ("https://localhost/RPG/UpdateCharacter.php", form);
			yield return www;
		} else {
			string[] content = new string[] { sql_text };
			RaiseEventOptions opt = new RaiseEventOptions ();
			opt.Receivers = ReceiverGroup.MasterClient;
			PhotonNetwork.RaiseEvent (8, content, true, opt);
		}
	}

	#endregion

	IEnumerator DelayBattleID(int id){
	
		yield return new WaitUntil (() => BattleLoaded);

		GameManager.k_Manager.BattleID = id;

	}

	private void OnEvent(byte eventCode, object content, int senderid){

		print ("Event+" + eventCode);

		if ((int)eventCode == 105) {

			object[] con = (object[])content;

			string msg = (string)con[0];

			Chat.chat.ChangeManually (msg);
			return;
		}

//currently not working
		if ((int)eventCode == 14 ) {
			
			string[] con = (string[])content;
			MakeItems (con[0]);
			return;
		}

		//LevelLoaded
		if ((int)eventCode == 50 ) {
			return;
		}
		//EnemiesLoaded
		if ((int)eventCode == 51 ) {
			print ("now");
			MakeSimpleCharacters (true, (string[])content);
			return;
		}

		//SimpleCharacters
		if ((int)eventCode == 52 ) {
			MakeSimpleCharacters (false, (string[])content);
			GameManager.k_Manager.FriendsLoadedCount++;
			if (GameManager.k_Manager.FriendsLoadedCount >= Group.k_Group.GroupIDs.Count) {
				GameManager.k_Manager.FriendsLoaded = true;
			}
			return;
		}
		//Order IDs
		if ((int)eventCode == 55 ) {
			GameManager.k_Manager.MoveOrder ((string[])content);
			return;
		}

		//StartTurn
		if ((int)eventCode == 56 ) {
			string[] tp_string = (string[])content;
			GameManager.k_Manager.StartTurn (tp_string);
			return;
		}

		//EndTurn
		if ((int)eventCode == 57 ) {
			GameManager.k_Manager.FinishTurn ();
			return;
		}

		//ChangeHealth
		if ((int)eventCode == 58 ) {
			int[] tp_ints = (int[])content;
			GameManager.k_Manager.ChangeHealth (tp_ints[0],tp_ints[1]);
			return;
		}

		//SpecialText
		if ((int)eventCode == 59 ) {
			int[] tp_ints = (int[])content;
			GameManager.k_Manager.MakeBattleText (tp_ints[0],tp_ints[1]);
			return;
		}

		//TargetFields
		if ((int)eventCode == 60 ) {
			int[] tp_ints = (int[])content;
			GameManager.k_Manager.ShowBattleFields (tp_ints);
			return;
		}

		//MoveCharacter
		if ((int)eventCode == 61 ) {
			int[] tp_ints = (int[])content;
			GameManager.k_Manager.PlaceCharacter (tp_ints[0],tp_ints[1]);
			return;
		}

		//DeadCharacters
		if ((int)eventCode == 62 ) {
			int[] tp_ints = (int[])content;
			GameManager.k_Manager.RemoveCharacters (tp_ints);
			return;
		}

		//EndBattle
		if ((int)eventCode == 77 ) {
			GameManager.k_Manager.EndBattle();
			return;
		}

		if (PhotonNetwork.isNonMasterClientInRoom && (int)eventCode == 2) {
			string tp_content = "";
			tp_content = (string)content;
			//print (tp_content);
			string[] tp_array = tp_content.Split ("+" [0]);

			int Position = int.Parse (tp_array [1]);

			if (Position > 29)
				Position -= 30;

			OnlineData.k_storage.tp_dataCharacters [Position] = tp_array[0];
			return;
		}

		if (PhotonNetwork.isNonMasterClientInRoom && (int)eventCode == 3)  {
			string tp_content = "";
			tp_content = (string)content;
			string[] tp_array = tp_content.Split ("+" [0]);
			OnlineData.k_storage.tp_dataItems [int.Parse(tp_array[1])] = tp_array[0];
			OnlineData.k_storage.DataReceived = true;
			return;
		}

		if (PhotonNetwork.isNonMasterClientInRoom && (int)eventCode == 4) {
			string tp_content = "";
			tp_content = (string)content;
			string[] tp_array = tp_content.Split ("+" [0]);
			OnlineData.k_storage.tp_data [int.Parse(tp_array[1])] = tp_array[0];
			OnlineData.k_storage.DataReceived = true;
			return;
		}

		if (PhotonNetwork.isNonMasterClientInRoom && ((int)eventCode == 5 || (int)eventCode == 9)) {
			string tp_content = "";
			tp_content = (string)content;
			string[] tp_array = tp_content.Split ("+" [0]);
			OnlineData.k_storage.tp_dataInventoryItems [int.Parse(tp_array[1])] = tp_array[0];
			OnlineData.k_storage.DataReceived = true;
			return;
		}

		if (PhotonNetwork.isNonMasterClientInRoom && (int)eventCode == 10) {
			string tp_content = "";
			tp_content = (string)content;
			string[] tp_array = tp_content.Split ("+" [0]);
			OnlineData.k_storage.tp_dataCharacters [int.Parse(tp_array[1])] = tp_array[0];
			OnlineData.k_storage.DataReceived = true;
			return;
		}

		if (PhotonNetwork.isNonMasterClientInRoom && (int)eventCode == 12 ) {
			string tp_content = "";
			tp_content = (string)content;
			string[] tp_array = tp_content.Split ("+" [0]);
			OnlineData.k_storage.tp_dataSkills [int.Parse(tp_array[1])] = tp_array[0];
			OnlineData.k_storage.DataReceived = true;
			return;
		}

		if (PhotonNetwork.isNonMasterClientInRoom && (int)eventCode == 13 ) {
			string tp_content = "";
			tp_content = (string)content;
			string[] tp_array = tp_content.Split ("+" [0]);
			OnlineData.k_storage.tp_dataEnemySkills [int.Parse(tp_array[1])] = tp_array[0];
			OnlineData.k_storage.DataReceived = true;
			return;
		}

		if (PhotonNetwork.isNonMasterClientInRoom && (int)eventCode == 17 ) {
			string tp_content = "";
			tp_content = (string)content;
			string[] tp_array = tp_content.Split ("+" [0]);
			OnlineData.k_storage.tp_data [int.Parse(tp_array[1])] = tp_array[0];
			print (tp_array [0]);
			OnlineData.k_storage.DataReceived = true;
			return;
		}

		//Update Friends Character Positions => String
		if (PhotonNetwork.isNonMasterClientInRoom && (int)eventCode == 20 ) {
			int[] tp_content = new int[2];
			string[] con = new string[2];
			con = (string[])content;

			tp_content[0] = int.Parse(con[0]);
			tp_content[1] = int.Parse(con[1]);

			CreationManager.k_Manager.AddFriendPosition (tp_content[1],senderid,tp_content[0]);
			return;
		}

		//nicht belegt
		if (PhotonNetwork.isNonMasterClientInRoom && (int)eventCode == 21 ) {
			int tp_content = 0;
			tp_content = (int)content;
			return;
		}

		//StartBattle
		if (PhotonNetwork.isNonMasterClientInRoom && (int)eventCode == 30 ) {
			SceneManager.LoadScene ("Battle");
			int tp_string = (int)content;
			StartCoroutine (DelayBattleID (tp_string));
			return;
		}


		//Friend online
		if ((int)eventCode == 99) {

			object[] tp_content = (object[])content;
			string name = (string)tp_content [0];
			PhotonPlayer player = (PhotonPlayer)tp_content [1];
			CreationManager.k_Manager.MakePlayerOnline (name,player);
			return;
		}

		//Get Request
		if ((int)eventCode == 100) {

			CreationManager.k_Manager.ShowInvite(senderid);
			return;
		}
		//Accept
		if ((int)eventCode == 101) {

//			CreationManager.k_Manager.AddCharacterToRoom ();
			CreationManager.k_Manager.PlayerAccepted (senderid);
			CreationManager.k_Manager.SetInvitePanel (false);

			PlayerManager.k_manager.GroupIDs.Add (senderid);
			PlayerManager.k_manager.isMultiplayer = true;
			//Sending Server Info about new Member
			RaiseEventOptions opt = new RaiseEventOptions ();
			opt.Receivers = ReceiverGroup.MasterClient;
			PhotonNetwork.RaiseEvent (15, senderid+"", true, opt);

			return;
		}
		//Decline
		if ((int)eventCode == 102) {
			CreationManager.k_Manager.SetInvitePanel (false);
			return;
		}

		// Get Characters
		if ((int)eventCode == 103) {
			Player groupplayer = (Player)content;

			print (groupplayer.Characters.Length);
			return;
		}

		if (PhotonNetwork.isNonMasterClientInRoom && (int)eventCode >= 0) {
			string tp_content = "";
			tp_content = (string)content;
			string[] tp_array = tp_content.Split ("+" [0]);
			OnlineData.k_storage.tp_data [int.Parse(tp_array[1])] = tp_array[0];
			OnlineData.k_storage.DataReceived = true;
			return;
		}

	}
}
