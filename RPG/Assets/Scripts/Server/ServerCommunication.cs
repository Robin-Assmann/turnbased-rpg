using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ServerCommunication : MonoBehaviour {

	public static ServerCommunication k_conn;

	void Awake(){
		k_conn = this;

	}

	public void HandleEvent(int eventid, int senderid, string[] para){
	
		print ("Event "+eventid+ " from "+senderid);
		string tp_string;
		PhotonPlayer tp_player = PhotonPlayer.Find (senderid);
	
		switch(eventid){

		case 50: //Battle Stuff	
			
			switch (para [0]) {
			case "Level":
				break;
			case "Enemy":
				print ("hussa");
				break;
			case "Attack":
				print ("hussa");
				break;
			}
			break;
		case 0: 		
			StartCoroutine (GetPlayer (eventid,tp_player,para));
			break;
		case 1: 		
			StartCoroutine (InsertPlayer (para));
			break;
		case 2: 		
			StartCoroutine (GetCharacters(eventid,tp_player,para));
			break;
		case 3: 		
			StartCoroutine (GetItem (eventid,tp_player,para));
			break;
		case 4: 		
			StartCoroutine (GetPlayerID (eventid,tp_player,para));
			break;
		case 5: 		
			StartCoroutine (GetItem (eventid,tp_player,para));
			break;
		case 6: 		
			StartCoroutine (UpdatePlayer (para));
			break;
		case 7: 		
			StartCoroutine (DeleteItems (para));
			break;
		case 8: 		
			StartCoroutine (UpdateCharacter (para));
			break;
		case 9: 		
			StartCoroutine (InsertData (eventid,tp_player,para));
			break;
		case 10: 		
			StartCoroutine (GetEnemies (eventid,tp_player,para));
			break;
		case 11:
			StartCoroutine (GetLevel (eventid,tp_player,para));
			break;
		case 12: 		
			GetSkills (eventid,tp_player,para);
			break;
		case 13: 		
			StartCoroutine (GetEnemySkills (eventid,tp_player,para));
			break;
		case 14: 		
			MakeGroup (tp_player);
			break;
		case 15: 		
			JoinGroup (tp_player, para);
			break;
		case 16: 		
			StartCoroutine(InsertCharacter (eventid,tp_player,para));
			break;
		case 17: //GetCharacterSkillTree		
			StartCoroutine(GetCharacterSkill (eventid,tp_player,para));
			break;
		case 18: //ChangeSkillArray		
			StartCoroutine(UpdateSkillArray (tp_player,para));
			break;
		case 19: //UpdateSkillTree		
			StartCoroutine(UpdateSkillTree (tp_player,para));
			break;
		case 20: 		
			ChangePosition (tp_player, para);
			break;
		case 21: 		
			StartCoroutine(UpdateSkillPoints (tp_player,para));
			break;
		case 30: //Check if battle is right	
			StartBattle(senderid);
			break;
		case 49: //Player is ready		
			ServerStorage.Sv_Storage.B_Storages[int.Parse(para[0])].PlayerReady();
			break;
		case 52: //Make Player Characters
			Server.k_server.MakeGroupCharacters (para,senderid);
			break;
		case 55: //Request Order IDs
			Server.k_server.InitializeServerBattle(bool.Parse(para[0]),int.Parse(para[1]));
			break;
		case 57: //EndTurn
			ServerStorage.Sv_Storage.B_Storages[int.Parse(para[0])].EndTurn();
			break;
		case 58: //Want to Roll Dmg
			Server.k_server.RollDmg(int.Parse(para[0]),int.Parse(para[1]),float.Parse(para[2]),bool.Parse(para[3]),int.Parse(para[4]),int.Parse(para[5]),int.Parse(para[6]),int.Parse(para[7]));
			break;
		case 61: //Want to Move Character
			Server.k_server.MoveCharacter(int.Parse(para[0]),int.Parse(para[1]),int.Parse(para[2]));
			break;
		case 77: 		
			print (para[0]);
			break;
		default:
			break;
		}
	}

	#region Methods Database

	IEnumerator GetPlayer(int eventid, PhotonPlayer tp_player, string[] para){

		WWWForm form = new WWWForm ();
		form.AddField ("sql_text_post","SELECT * FROM player WHERE player_name='" + para[1] +"';");
		form.AddField ("row_name_post", "password");

		WWW itemsdata = new WWW ("https://localhost/RPG/GetPlayer.php", form);
		yield return itemsdata;

		string itemsdatastring = "-1";
		if (para [3].Equals (itemsdata.text)) {
		
			WWWForm form2 = new WWWForm ();
			form2.AddField ("sql_text_post","SELECT * FROM player WHERE player_name='" + para[1] +"';");
			form2.AddField ("row_name_post", "player_id");

			WWW itemsdata2 = new WWW ("https://localhost/RPG/GetPlayer.php", form2);
			yield return itemsdata2;
			itemsdatastring = itemsdata2.text;

			SavePlayer (tp_player.ID, int.Parse (itemsdatastring));

		}

		print ("ID is"+itemsdatastring +" password matches:"+para [3].Equals (itemsdata.text));
		SendData (eventid, tp_player, itemsdatastring, int.Parse(para[0]));
	}

	private void SavePlayer(int PhID, int PlayID){

		ServerPlayer sv = new ServerPlayer (PlayID, PhID);
		ServerStorage.Sv_Storage.activeUser.Add (sv);

	}

	IEnumerator GetPlayerID(int eventid, PhotonPlayer tp_player, string[] para){

		print (para [1]);
		WWWForm form = new WWWForm ();
		form.AddField ("sql_text_post","SELECT * FROM player WHERE player_id='" + para[1] +"';");
		form.AddField ("row_name_post", para[2]);
		WWW itemsdata = new WWW ("https://localhost/RPG/GetPlayer.php", form);
		yield return itemsdata;
		string itemsdatastring = itemsdata.text;
		print (para[2] + " - " + itemsdatastring);
		SendData (eventid, tp_player, itemsdatastring, int.Parse(para[0]));
	}

	IEnumerator InsertPlayer(string[] para){
	
		WWWForm form = new WWWForm ();
		form.AddField ("player_name_post", para[0]);
		form.AddField ("password_post", para[1]);
		WWW www = new WWW ("https://localhost/RPG/InsertPlayer.php", form);
	
		return null;
	}

	IEnumerator GetCharacters(int eventid, PhotonPlayer tp_player, string[] para){
		WWWForm form = new WWWForm ();
		form.AddField ("player_id_post", para[1]);
		WWW itemsdata = new WWW ("https://localhost/RPG/GetCharacters.php", form);
		yield return itemsdata;
		string tp_string = itemsdata.text;
		string[] tp_characterArray = tp_string.Split (";"[0]);

		int id = 0;
		foreach (ServerPlayer p in ServerStorage.Sv_Storage.activeUser.ToArray()) {
			if (p.PhotonID == tp_player.ID) {
				if (p.PlayerID == int.Parse(para [1])) {
					id = ServerStorage.Sv_Storage.activeUser.IndexOf (p);
				}
			}
		}

		for (int i = 0; i < tp_characterArray.Length-1; i++) {

			SaveCharacters (tp_characterArray [i], id);
//			string output = Server.k_server.ApplyCharacterAttributes (tp_characterArray [i]);
			string output = tp_characterArray [i];
			print (output);
			SendData (eventid, tp_player,output, int.Parse(para[0])+i);
		}
	}

	private void SaveCharacters(string tp_ch, int index){
	
		string[] tp_chars = tp_ch.Split ("|" [0]);
		Character tp_char = new Character (false,int.Parse (tp_chars [0]),tp_chars [1],int.Parse(tp_chars [2]),tp_chars [3],tp_chars [4],int.Parse (tp_chars [5]),int.Parse(tp_chars[6]),int.Parse(tp_chars [7]),tp_chars [8],int.Parse (tp_chars [9]),int.Parse (tp_chars [10]),int.Parse (tp_chars [11]),int.Parse(tp_chars [12]),int.Parse (tp_chars [13]),int.Parse (tp_chars [14]),tp_chars [15],tp_chars[16],tp_chars[17]);
		tp_char.PlayerID = ServerStorage.Sv_Storage.activeUser [index].PlayerID;
		ServerStorage.Sv_Storage.activeUser [index].characters.Add (tp_char);
		print ("added Player in Active with ID: " + int.Parse (tp_chars [0])+" at position: "+ int.Parse (tp_chars [7]));
	
	}

	IEnumerator GetCharacterSkill(int eventid, PhotonPlayer tp_player, string[] para){

		WWWForm form = new WWWForm ();
		form.AddField ("character_id_post", para[1]);
		WWW itemsdata = new WWW ("https://localhost/RPG/GetCharacterSkill.php", form);
		yield return itemsdata;

		string tp_string = itemsdata.text;
		SendData (eventid, tp_player, tp_string, int.Parse(para[0]));	
	}

	IEnumerator GetItem(int eventid, PhotonPlayer tp_player, string[] para){

		WWWForm form = new WWWForm ();
		form.AddField ("sql_text_post", "SELECT * FROM items WHERE item_id='" + para[1] + "';");
		WWW itemsdata = new WWW ("https://localhost/RPG/GetItem.php", form);
		yield return itemsdata;
		print ("GotItem send Event" + eventid);
		string tp_string = itemsdata.text;
		SendData (eventid, tp_player, tp_string, int.Parse(para[0]));	
	}

	IEnumerator UpdatePlayer(string[] para){
		
		WWWForm form = new WWWForm ();
		form.AddField ("sql_text_post", "UPDATE player SET inventory='" + para[0] + "' WHERE player_id=" + para[1] + ";");
		form.AddField ("mode_post", "0");
		WWW www = new WWW ("https://localhost/RPG/UseData.php", form);
		yield return www;
	}

	IEnumerator DeleteItems(string[] para){

		WWWForm form = new WWWForm ();
		form.AddField ("sql_text_post", "DELETE FROM items WHERE item_id =" + para[0] + ";");
		form.AddField ("mode_post", "0");
		WWW www = new WWW ("https://localhost/RPG/UseData.php", form);
		yield return www;
	}

	public IEnumerator UpdateCharacter(string[] para){
		WWWForm form = new WWWForm ();
		form.AddField ("sql_text_post", para[0]);
		form.AddField ("mode_post", "0");
		WWW www = new WWW ("https://localhost/RPG/UseData.php", form);
		yield return www;
	}

	IEnumerator InsertData(int eventid, PhotonPlayer tp_player, string[] para){

		WWWForm form = new WWWForm ();
		//"' AND type='"+ tp_type +
		form.AddField ("sql_text_post", "INSERT INTO items (item_name, type, rarity, value, item_level, strength, dexterity,intelligence,health,armor,dodge,speed,res_fire,res_ice,res_lightning,min_dps,max_dps, magic_damage) VALUES  "+para[1]);
		form.AddField ("mode_post", "1");
		WWW itemsdata = new WWW ("https://localhost/RPG/InsertData.php", form);
		yield return itemsdata;

		string tp_string = itemsdata.text;

		SendData (eventid, tp_player, tp_string, int.Parse(para[0]));
	}

	IEnumerator InsertCharacter(int eventid, PhotonPlayer tp_player, string[] para){

		WWWForm form = new WWWForm ();
		//"' AND type='"+ tp_type +
		string tp_sql = para[0];
		form.AddField ("sql_text_post", "INSERT INTO characters (player_id, character_name, rarity, race, type, max_health, current_health,position,skills,exp,strength,constituition,dexterity,intelligence,luck,slots, backpack) VALUES  "+tp_sql);
		WWW itemsdata = new WWW ("https://localhost/RPG/InsertData.php", form);
		yield return itemsdata;
		string tp_string = itemsdata.text;
		print (tp_string);

		SendData (eventid, tp_player, tp_string, 1);
	}

	IEnumerator GetEnemies(int eventid, PhotonPlayer tp_player, string[] para){
		WWWForm form = new WWWForm ();
		//print ("SQL TExt is " + para [1]);
		form.AddField ("sql_text_post", para[1]);
		WWW itemsdata = new WWW ("https://localhost/RPG/GetEnemies.php", form);
		yield return itemsdata;
		string tp_string = itemsdata.text;

		SendData (eventid, tp_player,tp_string, int.Parse(para[0]));
	}

	IEnumerator GetLevel(int eventid, PhotonPlayer tp_player, string[] para){
		WWWForm form = new WWWForm ();
		form.AddField ("sql_text_post", "SELECT * FROM level WHERE level_id=" + para[1] + ";");
		form.AddField ("mode_post", para[2] + "");
		WWW data = new WWW ("https://localhost/RPG/GetLevel.php", form);
		yield return data;
		string tp_string = data.text;
		print (tp_string);

		SendData (eventid, tp_player,tp_string, int.Parse(para[0]));
	}

	private void GetSkills(int eventid, PhotonPlayer tp_player, string[] para){
//		WWWForm form = new WWWForm ();
//		form.AddField ("sql_text_post", para[1]);
//		WWW itemsdata = new WWW ("https://localhost/RPG/GetSkills.php", form);
//		yield return itemsdata;
//		string tp_string = itemsdata.text;


		string type = para [1];

		string[] skill_ids = para [2].Split ("-" [0]);
		print (type + " . " + para [2]);
		Skill[] tp_skills = Server.k_server.data.WarriorSkills;

		switch (type) {
		case "Warrior":
			tp_skills = Server.k_server.data.WarriorSkills;
			break;
		case "Archer":
			tp_skills = Server.k_server.data.ArcherSkills;
			break;
		case "Priest":
			tp_skills = Server.k_server.data.PriestSkills;
			break;
		}

		string tp_skillstring = "";
		for (int i = 0; i < skill_ids.Length; i++) {

			tp_skillstring = tp_skillstring +tp_skills [int.Parse(skill_ids[i])-1].MakeString();
		}

		string[] tp_array = tp_skillstring.Split (";" [0]);

		print (tp_skillstring);
		for (int i = 0; i < tp_array.Length-1; i++) {
			SendData (eventid, tp_player, tp_array[i], int.Parse (para [0]) +i);
		}
	}

	IEnumerator GetEnemySkills(int eventid, PhotonPlayer tp_player, string[] para){
		WWWForm form = new WWWForm ();
		form.AddField ("sql_text_post", para[1]);
		WWW itemsdata = new WWW ("https://localhost/RPG/GetSkills.php", form);
		yield return itemsdata;
		string tp_string = itemsdata.text;
		string[] tp_array = tp_string.Split (";" [0]);

		for (int i = 0; i < 4; i++) {
			SendData (eventid, tp_player, tp_array[i], int.Parse (para [0]) +i);
		}
	}

	private void ChangePosition(PhotonPlayer tp_player, string[] para){
	
		int char_id = int.Parse (para [0]);
		int pos = int.Parse (para [1]);


		foreach (ServerPlayer sv in ServerStorage.Sv_Storage.activeUser.ToArray()) {
		
			if (sv.PhotonID == tp_player.ID) {
				for (int i = 0; i < sv.characters.Count; i++) {
				
					if (char_id == sv.characters [i].ID) {
						sv.characters [i].Position = pos;
						print ("Updating");
						string sql_text = "UPDATE characters SET position=" + pos + " WHERE character_id=" + char_id + ";";
						Server.k_server.StartCoroutine(ServerCommunication.k_conn.UpdateCharacter (new string[]{ sql_text }));

						break;
					}
				}
				break;
			}
		}
	}

	IEnumerator UpdateSkillArray(PhotonPlayer tp_player, string[] para){
		WWWForm form = new WWWForm ();
		form.AddField ("sql_text_post", "UPDATE characters SET skills='" + para[0] + "' WHERE character_id=" + para[1] + ";");
		form.AddField ("mode_post", "0");
		WWW www = new WWW ("https://localhost/RPG/UseData.php", form);
		yield return www;
	}

	IEnumerator UpdateSkillTree(PhotonPlayer tp_player, string[] para){
		WWWForm form = new WWWForm ();
		form.AddField ("sql_text_post", "UPDATE characters SET skilltree='" + para[0] + "' WHERE character_id=" + para[1] + ";");
		form.AddField ("mode_post", "0");
		WWW www = new WWW ("https://localhost/RPG/UseData.php", form);
		yield return www;
	}

	IEnumerator UpdateSkillPoints(PhotonPlayer tp_player, string[] para){
		WWWForm form = new WWWForm ();
		form.AddField ("sql_text_post", "UPDATE characters SET skillpoints='" + para[0] + "' WHERE character_id=" + para[1] + ";");
		form.AddField ("mode_post", "0");
		WWW www = new WWW ("https://localhost/RPG/UseData.php", form);
		yield return www;
	}

	#endregion

	#region Methods

	private void MakeGroup(PhotonPlayer tp_owner){
	
		ServerStorage.Sv_Storage.MakeNewGroup (tp_owner);
	
	}

	private void JoinGroup(PhotonPlayer tp_owner, string[] data){

		int MemberID = int.Parse (data [0]);
		PhotonPlayer tp_member = PhotonPlayer.Find (MemberID);
		ServerStorage.Sv_Storage.AddToGroup (tp_owner, tp_member);
	}

	private void StartBattle(int OwnerID){

		List<List<PhotonPlayer>> tp_Group = ServerStorage.Sv_Storage.Groups;
		int o = 0;

		for (int i = 0; i < tp_Group.Count; i++) {
			if (tp_Group [i] [0].ID == OwnerID) {
				o = i;
				break;			
			}
		}

		List<int> tp_ids = new List<int> ();
		foreach (PhotonPlayer play in tp_Group[o].ToArray()) {
			tp_ids.Add (play.ID);
		}
		int BattleID = ServerStorage.Sv_Storage.MakeNewBattleStorage (OwnerID);
	
		RaiseEventOptions options = new RaiseEventOptions ();
		options.TargetActors = tp_ids.ToArray ();
		PhotonNetwork.RaiseEvent(30, BattleID, true, options);

		Server.k_server.LoadLevel (BattleID);
	}


	#endregion

	public void SendData(int eventid,PhotonPlayer tp_player, string tp_string, int pos){

		byte evCode = (byte) eventid;	
		string content = tp_string + "+" + pos; 
		bool reliable = true;
		RaiseEventOptions options = new RaiseEventOptions ();
		options.Receivers = ReceiverGroup.Others;
		//options.TargetActors = new int[]{ tp_player.ID };

		PhotonNetwork.RaiseEvent(evCode, content, reliable, options);
	}
}
