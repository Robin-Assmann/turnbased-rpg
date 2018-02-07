using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class Server : MonoBehaviour {

	public byte Version = 1;

	[SerializeField] private GameObject T_Texts;
	[SerializeField] private GameObject T_Group;
	[SerializeField] private GameObject T_IGroup;
	[SerializeField] private GameObject If_Number;

	public static Server k_server;
	private Sprite[] k_SkillImages;

	public DataManager data;


	#region EnemyInit

	private bool wait = false;

	#endregion

	// Use this for initialization
	void Awake () {
		k_server = this;
		k_SkillImages = Resources.LoadAll<Sprite> ("Skills");

		data = DataManager.Load(Path.Combine(Application.dataPath, "Data.xml"));
		foreach (Skill c in data.EnemySkills.ToArray()) {
			c.Init ();
		}
		T_Texts.GetComponent<Text> ().text =data.Levels [0].LegendaryCount;

		Connect ();

		PhotonNetwork.OnEventCall += this.OnEvent;
	}

	private void OnEvent(byte eventCode, object content, int senderid){

		try{
		if (PhotonNetwork.isMasterClient && (int)eventCode < 80) {
			ServerCommunication.k_conn.HandleEvent ((int)eventCode, senderid,(string[]) content);
			return;
			} 
		}catch{
			try{
			if (PhotonNetwork.isMasterClient && (int)eventCode < 80) {

				string[] data = new string[2];
				data [0] = (string)content;
				data [1] = "";

				ServerCommunication.k_conn.HandleEvent ((int)eventCode, senderid,data);
				return;
			}
			}catch{
			
				Debug.Log ((int)eventCode);
			
			}
		}
	}

	#region Network
	public void Connect(){
		PhotonNetwork.ConnectUsingSettings(Version + "");
	}

	public virtual void OnConnectedToMaster(){
		PhotonNetwork.JoinLobby ();
	}

	public virtual void OnJoinedLobby(){

		PhotonNetwork.JoinOrCreateRoom("Lobby",  new RoomOptions (), TypedLobby.Default);
	}

	public virtual void OnFailedToConnectToPhoton(DisconnectCause cause){
		Debug.LogError("Cause: " + cause);
	}

	public void OnJoinedRoom(){
		T_Texts.GetComponent<Text> ().text = "Lobby beigetreten, Server läuft";

	}
	#endregion

	#region Level

	public void LoadLevel(int BattleID){
		int i =1;
		string mode = "common";

		GettingLevel (i, mode, BattleID);
	}

	void GettingLevel(int tp_int, string tp_string, int id){

		string datastring = "";

//		WWWForm form = new WWWForm ();
//		form.AddField ("sql_text_post", "SELECT * FROM level WHERE level_id=" + tp_int + ";");
//		form.AddField ("mode_post", tp_string + "");
//		WWW data = new WWW ("https://localhost/RPG/GetLevel.php", form);
//		yield return data;
//		datastring = data.text;

		switch (tp_string) {

		case "common":
			datastring = data.Levels [tp_int - 1].CommonCount + "|" + data.Levels [tp_int - 1].CommonMobs;
			break;
		case "rare":
			datastring = data.Levels [tp_int - 1].RareCount + "|" + data.Levels [tp_int - 1].RareMobs;
			break;
		case "elite":
			datastring = data.Levels [tp_int - 1].EliteCount + "|" + data.Levels [tp_int - 1].EliteMobs;
			break;
		case "legendary":
			datastring = data.Levels [tp_int - 1].LegendaryCount + "|" + data.Levels [tp_int - 1].LegendaryMobs;
			break;
		default:
			datastring = "Error in Level";
			break;
		}
		string[] array = datastring.Split ("|" [0]);
		string[] min_max = array [0].Split ("-" [0]);
		int[] MinMax = new int[min_max.Length];
		for (int i = 0; i < min_max.Length; i++) {
			MinMax [i] = int.Parse(min_max [i]);
		}
		string[] enemy_characters = array [1].Split ("/" [0]);
		int[] EnemyIDs = new int[enemy_characters.Length];
		for (int i = 0; i < enemy_characters.Length; i++) {
			EnemyIDs [i] = int.Parse(enemy_characters [i]);
		}

		RaiseEventOptions opt = new RaiseEventOptions ();
		opt.TargetActors = ServerStorage.Sv_Storage.B_Storages[id].GroupIds;
		PhotonNetwork.RaiseEvent (50, "", true, opt);

		EstimateEnemies (id,MinMax,EnemyIDs);
	}
	#endregion

	#region Group

	public void CheckGroup(){

		int u = int.Parse (If_Number.GetComponent<InputField> ().text);

		T_IGroup.GetComponent<Text> ().text = "Gruppenführer: " + ServerStorage.Sv_Storage.Groups [u] [0].name + "\b Mitglieder: " + (ServerStorage.Sv_Storage.Groups [u].Count - 1);
	}

	public void MakeGroupCharacters(string[] CharacterStrings, int PhotonID){

		List<string> Ids = new List<string>();
		for (int i = 0; i < CharacterStrings.Length - 2; i++) {
			Ids.Add (CharacterStrings [i]);
		}
		int p_id = int.Parse (CharacterStrings [CharacterStrings.Length - 1]);
		int b_id = int.Parse (CharacterStrings [CharacterStrings.Length - 2]);
		List<Character> posChar = new List<Character> ();

		foreach (ServerPlayer sv in ServerStorage.Sv_Storage.activeUser.ToArray()) {
		
			if (sv.PhotonID == PhotonID) {
			
				if (sv.PlayerID == p_id) {
					posChar = sv.characters;
				}
			}
		}
		for (int i = 0; i < Ids.Count; i++) {
		
			foreach (Character tp_ch in posChar.ToArray()) {
			
				if (tp_ch.ID == int.Parse (Ids [i])) {

					ServerStorage.Sv_Storage.B_Storages [b_id].Players.Add (tp_ch);
					print ("added Player with ID: " + tp_ch.ID+" at position: "+ tp_ch.Position);
					ServerStorage.Sv_Storage.B_Storages [b_id].Positions [tp_ch.Position] = tp_ch.ID;
					posChar.Remove (tp_ch);
					break;
				}
			}
		}



//		WWWForm form = new WWWForm ();
//		form.AddField ("player_id_post", p_id);
//		WWW itemsdata = new WWW ("https://localhost/RPG/GetCharacters.php", form);
//		yield return itemsdata;
//		string tp_string = itemsdata.text;
//		print ("Characters :"+tp_string);
//		string[] tp_characterArray = tp_string.Split (";"[0]);
//
//
//
//		print ("playerID =" + p_id);
//
//		for (int i = 0; i < tp_characterArray.Length; i++) {
//		
//			string[] tp_chars = tp_characterArray [i].Split("|"[0]);
//			if (Ids.Contains (tp_chars [0])) {
//				//bool tp_isEnemy,int tp_ID, string tp_Name,int tp_Health,int tp_MaxHealth,int tp_Armor,int tp_Speed,int tp_Position,string tp_Type,int tp_FireResistance,int tp_IceResistance,int tp_LightningResistance
//				Character tp_char = new Character (false,int.Parse (tp_chars [0]),tp_chars [1],int.Parse(tp_chars [2]),tp_chars [3],tp_chars [4],int.Parse (tp_chars [5]),int.Parse(tp_chars[6]),int.Parse(tp_chars [7]),tp_chars [8],int.Parse (tp_chars [9]),int.Parse (tp_chars [10]),int.Parse (tp_chars [11]),int.Parse(tp_chars [12]),int.Parse (tp_chars [13]),int.Parse (tp_chars [14]),tp_chars [15],tp_chars[16]);
//				tp_char.PlayerID = p_id;
//				ServerStorage.Sv_Storage.B_Storages [b_id].Players.Add (tp_char);
//				print ("added Player with ID: " + int.Parse (tp_chars [0])+" at position: "+ int.Parse (tp_chars [7]));
//				ServerStorage.Sv_Storage.B_Storages [b_id].Positions [int.Parse (tp_chars [7])] = int.Parse (tp_chars [0]);
//			
//			}
//		
//		}


//		int BattleID = int.Parse(CharacterStrings [CharacterStrings.Length-1]);
//
//		for (int i = 0; i < CharacterStrings.Length-1; i++) {
//			string[] a = CharacterStrings[i].Split("/"[0]);
//			//bool tp_isEnemy,int tp_ID, string tp_Name,int tp_Health,int tp_MaxHealth,int tp_Armor,int tp_Speed,int tp_Position,string tp_Type,int tp_FireResistance,int tp_IceResistance,int tp_LightningResistance
//			Character tp_char = new Character (false, int.Parse (a [0]), a [1], int.Parse (a [2]), int.Parse (a [3]), int.Parse (a [4]), int.Parse (a [5]), int.Parse (a [6]), a [7], int.Parse (a [8]), int.Parse (a [9]), int.Parse (a [10]));
//			tp_char.PlayerID = PlayerID;
//			ServerStorage.Sv_Storage.B_Storages [BattleID].Players.Add (tp_char);
//			print ("added Player with ID: " + int.Parse (a [0])+" at position: "+ int.Parse (a [6]));
//			ServerStorage.Sv_Storage.B_Storages [BattleID].Positions [int.Parse (a [6])] = int.Parse (a [0]);
//		}
	}

	#endregion

	#region Player/Character

	public string ApplyCharacterAttributes(string Character){
	

		//int tp_characterid, string tp_charactername ,int tp_maxhealth,int tp_currenthealth, int tp_armor, float tp_dodge, int tp_position, int tp_mindps, int tp_maxdps, int tp_speed, string tp_type, int tp_fire, int tp_ice, int tp_shock, string tp_skills, string tp_slots, string tp_backpack, int tp_exp, int tp_strength, int tp_dexterity, int tp_intelligence, int tp_magicdamage
		//id, name,rarity, race, type,max_hp, current_hp,position,skills,exp,strength,con,dex,int,luck,slots,backpack
		string[] Character_Attributes = Character.Split("|" [0]);
		print (Character_Attributes.Length);
		string[] new_Attributes = new string[22];
		new_Attributes [0] = Character_Attributes [0]; 
		new_Attributes [1] = Character_Attributes [1];
		new_Attributes [2] = Character_Attributes [5];
		new_Attributes [3] = Character_Attributes [6];
		new_Attributes [4] = "10";	//Armor
		new_Attributes [5] = "1";	//Dodge
		new_Attributes [6] = Character_Attributes [7]; //Position
		new_Attributes [7] = "1";	//MinDps
		new_Attributes [8] = "2";	//MaxDps
		new_Attributes [9] = "10";	//Speed
		new_Attributes [10] = Character_Attributes [4]; //Type
		new_Attributes [11] = "5";	//FireREs
		new_Attributes [12] = "6";	//IceREs
		new_Attributes [13] = "7";	//ShockREs
		new_Attributes [14] = Character_Attributes [8]; //Skills
		new_Attributes [15] = Character_Attributes [15]; //Slots
		new_Attributes [16] = Character_Attributes [16]; //Backpack
		new_Attributes [17] = Character_Attributes [9];
		new_Attributes [18] = Character_Attributes [10]; //str
		new_Attributes [19] = Character_Attributes [12]; //dex
		new_Attributes [20] = Character_Attributes [13]; //int
		new_Attributes [21] = "1"; //MagicDmg

		return new_Attributes [0]+"|"+new_Attributes [1]+"|"+new_Attributes [2]+"|"+new_Attributes [3]+"|"+new_Attributes [4]+"|"+new_Attributes [5]+"|"+new_Attributes [6]+"|"+new_Attributes [7]+"|"+new_Attributes [8]+"|"+new_Attributes [9]+"|"+new_Attributes [10]+"|"+new_Attributes [11]+"|"+new_Attributes [12]+"|"+new_Attributes [13]+"|"+new_Attributes [14]+"|"+new_Attributes [15]+"|"+new_Attributes [16]+"|"+new_Attributes [17]+"|"+new_Attributes [18]+"|"+new_Attributes [19]+"|"+new_Attributes [20]+"|"+new_Attributes [21];
	
	}


	#endregion

	#region Enemy

	private void EstimateEnemies(int BattleID, int[] MinMax, int[] EnemyIDs){
	
		int i = (int)(Random.Range(MinMax[0], MinMax[1] +1));
		BattleStorage b = ServerStorage.Sv_Storage.B_Storages [BattleID];
		b.enemycount = i;
		print ("EnemyCount =" + i);
		for (int j = 0; j < i; j++) {

			if (j == 0) {
				LoadEnemies (EnemyIDs [0], j+1, BattleID);
				continue;
			}
			int o = Random.Range (0, EnemyIDs.Length);
			LoadEnemies (EnemyIDs[o], j+1, BattleID);
		}
	}

//	IEnumerator LoadEnemies(int tp_int, int tp_customid, int BattleID){
//
//		string sql_text = "SELECT * FROM enemies WHERE enemy_id='" + tp_int + "';";
//		string tp_string = "";
//		WWWForm form = new WWWForm ();
//		form.AddField ("sql_text_post", sql_text);
//		WWW itemsdata = new WWW ("https://localhost/RPG/GetEnemies.php", form);
//		yield return itemsdata;
//		tp_string = itemsdata.text;
//		print (tp_string);
//		MakeEnemies (tp_string, tp_customid, BattleID);
//	}

	private void LoadEnemies(int tp_int, int tp_customid, int BattleID){

		string tp_string = "";
		tp_string = data.Enemies[tp_int-1].ToString();
		print (tp_string);
		MakeEnemies (tp_string, tp_customid, BattleID);
	}

	public void MakeEnemies(string tp_string , int tp_customid, int BattleID){

		string[] tp_new = tp_string.Split ("|" [0]);
		string[] tp_i = tp_new [0].Split ("/" [0]);
		List<string> tp_array = new List<string> (tp_i);
		for (int i = 0; i < tp_array.Count; i++) {
			if (int.Parse(tp_array [i]) == 0) {
				tp_array.RemoveAt (i);
				i--;
			}
		}
		tp_i = tp_array.ToArray ();
		int pos =0;
		BattleStorage b = ServerStorage.Sv_Storage.B_Storages [BattleID];
		while (true) {
			int ran = (int)Random.Range (0, tp_i.Length);
			int u = int.Parse (tp_i [ran]);
			pos = (u - 1) + 4 * (int)(Random.Range (0, 4));

			if (b.Positions [pos+16]==0) {
				b.Positions [pos+16] = tp_customid*-1;
				break;
			}
		}
		CreateEnemies (tp_customid,int.Parse (tp_new [1]),int.Parse (tp_new [2]),float.Parse(tp_new[3]), pos,int.Parse (tp_new [4]),int.Parse (tp_new [5]),int.Parse (tp_new [6]),int.Parse (tp_new [7]),int.Parse (tp_new [8]),int.Parse (tp_new [9]),tp_new[10], int.Parse (tp_new[11]),BattleID);
	}

	public void CreateEnemies(int tp_enemyid,int tp_maxhealth, int tp_armor, float tp_dodge, int tp_position, int tp_mindps, int tp_maxdps, int tp_speed, int tp_fire, int tp_ice, int tp_shock, string tp_skills, int tp_exp, int BattleID){
		//Enemy.Add (new Character (true ,0 ,"Enemy", 30, 1, 30, 0.2f, 8, 2, 4, 20,"Warrior",25,25,25,"","","",0));
		Character tp_char = new Character (true,tp_enemyid *-1,"Enemy",tp_maxhealth,tp_maxhealth,tp_armor,tp_dodge,tp_position,tp_mindps,tp_maxdps,tp_speed,"Archer",tp_fire,tp_ice, tp_shock,tp_skills, null, null, 0, 0, 0, 0, 0);
		tp_char.expGain = tp_exp;

		BattleStorage b = ServerStorage.Sv_Storage.B_Storages [BattleID];

		b.Enemy.Add (tp_char);

		b.enemyloaded++;
		LoadEnemySkills (tp_enemyid*-1, BattleID);
		if (b.enemycount <= b.enemyloaded) {
			
			string[] simpleChars = new string[b.Enemy.Count];

			for (int i = 0; i < simpleChars.Length; i++) {
				string tp_string = b.Enemy [i].MakeSimpleString ();
				simpleChars [i] = tp_string;
			}
			print ("Sending " + simpleChars.Length + " IDs "+ b.GroupIds[0]);
			RaiseEventOptions opt = new RaiseEventOptions ();
			opt.TargetActors = b.GroupIds;
			PhotonNetwork.RaiseEvent (51, simpleChars, true, opt);	
		}
	}

	private void LoadEnemySkills(int CharID, int BattleID){
		Character k_Character= null;

		foreach (Character c in ServerStorage.Sv_Storage.B_Storages [BattleID].Enemy.ToArray()) {
			if (c.ID == CharID) {
				k_Character = c;
				break;
			}
		}
		print (k_Character.SkillArray.Length);
		string tp_skillstring = "";

		for (int i = 0; i < k_Character.SkillArray.Length; i++) {
		
			tp_skillstring = tp_skillstring + data.EnemySkills [k_Character.SkillArray [i]-1].MakeString();
		}



		string[] tp_skillarray = tp_skillstring.Split(";"[0]);;

//		string sql_text = "SELECT * FROM enemy_skills WHERE skill_id IN('" + k_Character.SkillArray [0] + "','" + k_Character.SkillArray [1] + "','"+ k_Character.SkillArray [2] +"','"+ k_Character.SkillArray [3] +"');";
//		string tp_skillstring = "";
//		string[] tp_skillarray = new string[4];
//
//		WWWForm form = new WWWForm ();
//		form.AddField ("sql_text_post", sql_text);
//		WWW itemsdata = new WWW ("https://localhost/RPG/GetSkills.php", form);
//		yield return itemsdata;
//		tp_skillstring = itemsdata.text;
//		tp_skillarray = tp_skillstring.Split(";"[0]);

		List<Skill> ASkills = ServerStorage.Sv_Storage.B_Storages [BattleID].AvailableSkills;
		for (int i = 0; i < tp_skillarray.Length -1; i++) {
			string[] tp_new = tp_skillarray [i].Split ("|" [0]);

			bool enemy = (tp_new[9]=="true");
			bool dodge = (tp_new[11]=="true");

			ASkills.Add (new Skill (int.Parse (tp_new [0]), tp_new [1], float.Parse (tp_new [2]),int.Parse (tp_new [3]),int.Parse (tp_new [4]),tp_new[5],tp_new [6],tp_new [7],k_SkillImages[int.Parse (tp_new [8])],enemy,int.Parse(tp_new [10]),dodge,int.Parse(tp_new [12]),tp_new [13]));
		}

		for (int i = 0; i < k_Character.Skills.Length; i++) {
			for (int j = 0; j < ASkills.Count; j++) {

				if (k_Character.SkillArray [i] == ASkills [j].ID) {
					k_Character.Skills [i] = ASkills [j];
				}
			}
		}
	}
	#endregion

	#region Battle

	private Character GetTargetWithPosition(BattleStorage b,int Site, int Position){

		if (b.Positions [Position + 16 * Site] == 0) {
			print ("error no target found");
		}

		int ID = b.Positions [Position + 16 * Site];
		if (ID < 0) {
			foreach (Character c in b.Enemy.ToArray()) {
				if (c.ID == ID) {
					return c;
				}			
			}
		} else {
			foreach (Character c in b.Players.ToArray()) {
				if (c.ID == ID) {
					return c;
				}			
			}
		}
		return null;
	}

	public void UseSkill(int BattleID, Skill UsedSkill , int Target){
	
		switch (UsedSkill.SkillType) {
		case 0:

			if (UsedSkill.TargetMode == 4)
//				Dot ();
				Debug.Log("nuthing");
			else{
				Character c = ServerStorage.Sv_Storage.B_Storages [BattleID].CharacterOrder [0];
				RollDmg (BattleID, 0, UsedSkill.Modifier, UsedSkill.Dodgeable, Target,0,c.MinDPS,c.MaxDPS);
			}
			break;
		case 1:
//			Heal ();
			break;
		case 2:
//			if (tp_start) {
//				for (int i = 0; i < UsedSkill.BuffCount; i++) {
//					ApplyBuff (tp_start, i, null);
//				}
//			} else {
//				ApplyBuff (tp_start, tp_int, tp_chara);
//			}
			break;
		case 3:
//			ApplyDot ();
			break;
		case 4:
//			MovementAttack ();
			break;
		case 5: 
//			if (tp_start) {
//				for (int i = 0; i < UsedSkill.BuffCount; i++) {
//					ApplyDebuff (tp_start, i);
//				}
//			} else {
//				ApplyDebuff (tp_start, tp_int);
//			}
			break;
		}
	}

	public void MoveCharacter(int BattleID, int Position, int NewPosition){
	
		BattleStorage b = ServerStorage.Sv_Storage.B_Storages [BattleID];
		Debug.Log ("moving Id=" + b.Positions [Position] + " to Position=" + NewPosition);

		if (b.Positions [NewPosition] == 0) {
			b.Positions [NewPosition] = b.Positions [Position];
			b.Positions [Position] = 0;		
		}
	
		RaiseEventOptions opt = new RaiseEventOptions ();
		opt.TargetActors = b.GroupIds;
		PhotonNetwork.RaiseEvent (61, new int[]{b.Positions [NewPosition], NewPosition}, true, opt);

	}


	public void RollDmg(int BattleID,int TargetMode, float Skill_Mod, bool Dodgeable, int targetPos, int Site, int MinDPS, int MaxDPS){

		BattleStorage b = ServerStorage.Sv_Storage.B_Storages [BattleID];
		int tp_damage = (int)((MinDPS + MaxDPS * Random.value) * Skill_Mod);
		Character Target = GetTargetWithPosition(b,Site,targetPos);

		//Single Target
		if (TargetMode == 0) {
			if ((Dodgeable && (Random.value > Target.Dodge)) || !Dodgeable) {
				//Dmged
				Target.Health -= tp_damage;
				RaiseEventOptions opt = new RaiseEventOptions ();
				opt.TargetActors = b.GroupIds;
				PhotonNetwork.RaiseEvent (58, new int[]{Target.ID,-tp_damage}, true, opt);
			} else {
				//Dodeged
				GameManager.k_Manager.Target.transform.parent.GetChild (1).GetComponent<CharacterClass> ().MakeText ("Dodged", new Color (255, 255, 0));
				RaiseEventOptions opt = new RaiseEventOptions ();
				opt.TargetActors = b.GroupIds;
				PhotonNetwork.RaiseEvent (59, new int[]{Target.ID,0}, true, opt);
			}
		}
//				//AOE
//				if (TargetMode == 2) {
//					foreach (Transform trans in k_relatedButton.GetComponent<ButtonClass>().tp_list.ToArray()) {
//						if (trans.FindChild ("Character(Clone)")!=null) {
//							if ((Dodgeable && Random.value > trans.FindChild ("Character(Clone)").GetComponent<CharacterClass> ().k_Character.Dodge) || !Dodgeable) {
//						tp_damage = (int)((b.CharacterOrder [0].MinDPS + b.CharacterOrder [0].MaxDPS * Random.value) * Skill_Mod);
//								trans.FindChild ("Character(Clone)").GetComponent<CharacterClass> ().k_Character.Health -= tp_damage;
//								trans.FindChild ("Character(Clone)").GetComponent<CharacterClass> ().MakeText ("- " + tp_damage, new Color (255, 0, 0));
//							} else {
//								trans.FindChild ("Character(Clone)").GetComponent<CharacterClass> ().MakeText ("Dodged", new Color (255, 255, 0));
//							}
//						}
//					}
//				}
	


		b.EndTurn ();
	}

	#endregion

	public void InitializeServerBattle(bool tp_bool, int BattleID){

		BattleStorage b = ServerStorage.Sv_Storage.B_Storages [BattleID];
		if (tp_bool) {
			StartCoroutine(b.InitializeBattle (tp_bool));
		}	
	}
}
