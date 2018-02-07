using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character{

	public int expGain{ get; set;}

	public string Name;
	public int ID;
	public int Exp;
	public int Level;
	public string Type;
	public string Race;
	public int Rarity;

	public int Strength, Constituition, Dexterity, Intelligence, Luck;

	public float Weight;

	public int Health, MaxHealth;
	public int Armor;

	public int Damage;
	public int CritDamage;
	public int MagicDamage;
	public int CritMagicDamage;
	public float CritChance;

	public int MinDPS;
	public int MaxDPS;

	public float MagicPercent;
	public float GoldPercent;

	public int Position;
	public int Speed;
	public float Dodge;

	public int FireResistance;
	public int IceResistance;
	public int LightningResistance;


	public bool isEnemy{ get; set;}
	public bool isGroup{ get; set;}
	public bool isSimple{ get; set;}
	public int[] SkillArray { get; set;}

	public Transform Spawn{ get; set;}
	public GameObject Char{ get; set;}

	public Action[] ActionList { get; set;}

	#region Starting

	public int Starting_Strength;   
	public int Starting_Constituition;
	public int Starting_Dexterity;
	public int Starting_Intelligence;
	public int Starting_Luck;

	public int Starting_Damage;
	public int Starting_CritDamage;
	public int Starting_MagicDamage;
	public int Starting_CritMagicDamage;
	public float Starting_CritChance;

	public int Starting_Health{ get; set;}

	public int Starting_Armor{ get; set;}
	public int Starting_MinDPS{ get; set;}
	public int Starting_MaxDPS{ get; set;}
	public int Starting_Speed{ get; set;}
	public float Starting_Dodge{ get; set;}

	public int Starting_FireResistance{ get; set;}
	public int Starting_IceResistance{ get; set;}
	public int Starting_LightningResistance{ get; set;}
	#endregion //Starting values in fight // see whos buffed

	#region Base

	public int Base_Strength;   
	public int Base_Constituition;
	public int Base_Dexterity;
	public int Base_Intelligence;
	public int Base_Luck;

	public int Base_Damage;
	public int Base_CritDamage;
	public int Base_MagicDamage;
	public int Base_CritMagicDamage;
	public float Base_CritChance;

	public int Base_Health{ get; set;}

	public int Base_Armor{ get; set;}
	public int Base_MinDPS{ get; set;}
	public int Base_MaxDPS{ get; set;}
	public int Base_Speed{ get; set;}
	public float Base_Dodge{ get; set;}

	public int Base_FireResistance{ get; set;}
	public int Base_IceResistance{ get; set;}
	public int Base_LightningResistance{ get; set;}

	public int Base_Weight{ get; set;}
	public int Base_MagicPercent{ get; set;}
	public int Base_GoldPercent{ get; set;}

	#endregion //Base stats from loading

	public int[] CharacterInventory { get; set;}
	public int[] CharacterBackpack { get; set;}

	public List<Item> InventoryItems;
	public List<Item> BackpackItems;

	//Only Needed for Enemies
	public Skill[] Skills {get; set;}
	public Skill[] TypeSkills;
	public List<Skill> AvailableSkills;
	public SkillTree SkillOverview;

	public int G_SkillPoints, O_Skillpoints, D_Skillpoints, U_Skillpoints;

	public int PlayerID;

	private int ItemsLoaded, ItemCount;
	private bool skillsLoaded = false;

	public Character(bool tp_isEnemy, int tp_characterid, string tp_charactername, int tp_rarity, string tp_race, string tp_type, int tp_maxhealth,int tp_currenthealth,int tp_position, string tp_skills, int tp_exp, int tp_strength,int tp_constituition, int tp_dexterity, int tp_intelligence, int tp_luck,  string tp_slots, string tp_backpack, string tp_skillpoints){

		InventoryItems = new List<Item> ();
		BackpackItems = new List<Item> ();
		this.AvailableSkills = new List<Skill> ();

		this.isEnemy = tp_isEnemy;
		this.ID = tp_characterid;
		this.Name = tp_charactername;
		this.Health = tp_currenthealth;
		this.Strength = tp_strength;
		this.Constituition = tp_constituition;
		this.Dexterity = tp_dexterity;
		this.Intelligence = tp_intelligence;
		this.Luck = tp_luck;

		this.Type = tp_type;
		this.Race = tp_race;
		this.Rarity = tp_rarity;
		this.Position = tp_position;
		this.Exp = tp_exp;

		this.MaxHealth = tp_maxhealth;
		this.isGroup = false;
		this.isSimple = false;

		ApplyAttributes ();

		ActionList = new Action[20];
		for (int i = 0; i < 20; i++) {
			ActionList [i] = new Action (0, 0, null, 0, 0, null);
		}
		if (!isEnemy) {
			UpdateExperience ();
			ApplyBasicAttributes ();
			if (!PhotonNetwork.player.isMasterClient) {
				string[] tp_array = tp_skills.Split ("/" [0]);
				MakeSkillArray (int.Parse (tp_array [0]), int.Parse (tp_array [1]), int.Parse (tp_array [2]), int.Parse (tp_array [3]));



				MakeInventoryArray (tp_slots);
				MakeBackpackArray (tp_backpack);

				ApplySkillPoints (tp_skillpoints);
				PlayerManager.k_manager.StartCoroutine (LoadSkilltrees ()); // + Loadskills()

				PlayerManager.k_manager.StartCoroutine (DelayUseInventory ());

				switch (Type) {
				case "Warrior":
					TypeSkills = OnlineData.k_storage.skill_data.WarriorSkills;
					break;
				case "Archer":
					TypeSkills = OnlineData.k_storage.skill_data.ArcherSkills;
					break;
				case "Priest":
					TypeSkills = OnlineData.k_storage.skill_data.PriestSkills;
					break;
				default: 
					TypeSkills = OnlineData.k_storage.skill_data.WarriorSkills;
					break;
				}
			}
		} else {
			Skills = new Skill[4];
			string[] tp_array = tp_skills.Split ("/" [0]);
			MakeSkillArray (tp_array);
		}
	}

	IEnumerator DelayUseInventory(){
	
		yield return new WaitUntil (() => this.skillsLoaded);
		UseInventory ();
	
	}

	private void ApplyAttributes(){
	
		this.MaxHealth += (this.Strength * 2 + this.Constituition * 5)*2;
		this.Armor = this.Constituition;
		this.MinDPS = 1;
		this.MaxDPS = 2;
		this.Speed = this.Dexterity*4;
		this.Dodge = (this.Dexterity*3)/25;
		this.FireResistance = this.Constituition+this.Intelligence;
		this.IceResistance = this.Constituition+this.Intelligence;
		this.LightningResistance = this.Constituition+this.Intelligence;

		this.Weight = (this.Strength + 2 + this.Constituition)*1.5f;
		this.MagicPercent = this.Luck * 2;
		this.GoldPercent = this.Luck * 2;

		this.Damage = this.Strength*4+this.Dexterity;
		this.CritDamage = this.Strength*2;
		this.MagicDamage = this.Intelligence*5;
		this.CritMagicDamage = this.Intelligence*2;
		this.CritChance = this.Dexterity*2 + this.Luck*6;

		this.Base_Health = this.MaxHealth;
		this.Base_Armor = this.Armor;
		this.Base_MinDPS = this.MinDPS;
		this.Base_MaxDPS = this.MaxDPS;
		this.Base_Speed = this.Speed;
		this.Base_Dodge = this.Dodge;
		this.Base_FireResistance = this.FireResistance;
		this.Base_IceResistance = this.IceResistance;
		this.Base_LightningResistance = this.LightningResistance;

		this.Base_Strength = this.Strength;
		this.Base_Constituition = this.Constituition;
		this.Base_Dexterity = this.Dexterity;
		this.Base_Intelligence = this.Intelligence;
		this.Base_Luck = this.Luck;

	}


	#region Old Constructor
	public Character(bool tp_isEnemy ,int tp_characterid, string tp_charactername ,int tp_maxhealth,int tp_currenthealth, int tp_armor, float tp_dodge, int tp_position, int tp_mindps, int tp_maxdps, int tp_speed, string tp_type, int tp_fire, int tp_ice, int tp_shock, string tp_skills, string tp_slots, string tp_backpack, int tp_exp, int tp_strength, int tp_dexterity, int tp_intelligence, int tp_magicdamage){

		InventoryItems = new List<Item> ();
		BackpackItems = new List<Item> ();


		this.isEnemy = tp_isEnemy;
		this.ID = tp_characterid;
		this.Name = tp_charactername;
		this.Health = tp_currenthealth;
		this.Strength = tp_strength;
		this.Dexterity = tp_dexterity;
		this.Intelligence = tp_intelligence;

		this.Armor = tp_armor;

		this.MagicDamage = tp_magicdamage;
		this.MinDPS = tp_mindps;
		this.MaxDPS = tp_maxdps;
		this.Speed = tp_speed;
		this.Type = tp_type;
		this.Position = tp_position;
		this.Dodge = tp_dodge;
		this.FireResistance = tp_fire;
		this.IceResistance = tp_ice;
		this.LightningResistance = tp_shock;
		this.Exp = tp_exp;

		this.Base_Health = tp_maxhealth;
		this.Base_Armor = tp_armor;
		this.Base_MinDPS = tp_mindps;
		this.Base_MaxDPS = tp_maxdps;
		this.Base_Speed = tp_speed;
		this.Base_Dodge = tp_dodge;
		this.Base_FireResistance = tp_fire;
		this.Base_IceResistance = tp_ice;
		this.Base_LightningResistance = tp_shock;

		this.MaxHealth = tp_maxhealth;
		this.isGroup = false;
		this.isSimple = false;

		ActionList = new Action[20];


		for (int i = 0; i < 20; i++) {
			ActionList [i] = new Action (0, 0, null, 0, 0, null);
		}
		if (!isEnemy) {
			UpdateExperience ();
			ApplyBasicAttributes ();
			string[] tp_array = tp_skills.Split ("/" [0]);
			MakeSkillArray (int.Parse (tp_array [0]), int.Parse (tp_array [1]), int.Parse (tp_array [2]), int.Parse (tp_array [3]));

			MakeInventoryArray (tp_slots);
			MakeBackpackArray (tp_backpack);

			UseInventory ();

		} else {
			Skills = new Skill[4];
			string[] tp_array = tp_skills.Split ("/" [0]);
			MakeSkillArray (tp_array);
		}
	}
	#endregion 

	//Simple Character
	public Character(bool tp_isEnemy,int tp_ID, string tp_Name,int tp_Health,int tp_MaxHealth,int tp_Armor,int tp_Speed,int tp_Position,string tp_Type,int tp_FireResistance,int tp_IceResistance,int tp_LightningResistance){

		this.ID = tp_ID;
		this.isEnemy = tp_isEnemy;
		this.Name = tp_Name;
		this.Health = tp_Health;
		this.MaxHealth = tp_MaxHealth;
		this.Armor = tp_Armor;
		this.Speed = tp_Speed;
		this.Type = tp_Type;
		this.Position = tp_Position;
		this.FireResistance = tp_FireResistance;
		this.IceResistance = tp_IceResistance;
		this.LightningResistance = tp_LightningResistance;
		this.isSimple = true;
	}

	#region Rest

	public void UpdateExperience(){
	
		int u = Exp;
		Level = 0;
		for (int i = 0; i < 100; i++) {
			if (u - i * i >=0) {
				u -= i * i;
				Level++;				
			}
		}
	}

	public void ApplyBasicAttributes(){
	
		int value = 0;
		int u = Strength;
		if (u >= 10)
			value = u / 10;
		if (value > 0) {
			MaxHealth += value * Level;
			Armor += value * Level;		
		}
		value = 0;
		u = Dexterity;
		if (u >= 10)
			value = u / 10;
		if (value > 0) {
			Dodge += (float)((value * Level) /100);
			Speed += value/25;		
		}
		value = 0;
		u = Intelligence;
		if (u >= 10)
			value = u / 10;
		if (value > 0) {
			MagicDamage += (value * Level) / 10;		
		}
	}

	public void MakeSkillArray(int i, int o, int j, int k){
		SkillArray = new int[4];
		SkillArray [0] = i;
		SkillArray [1] = o;
		SkillArray [2] = j;
		SkillArray [3] = k;
		Skills = new Skill[4];
	}	

	public void MakeSkillArray(string[] array){

		Skills = new Skill[array.Length];
		List<int> list_int = new List<int> ();

		for (int i = 0; i < array.Length; i++) {
			list_int.Add (int.Parse (array [i]));
		}

//		int o = 0;
//		while (list_int.Count < 4) {
//			list_int.Add (o);
//		}
		SkillArray = list_int.ToArray ();
	}

	public void MakeInventoryArray(string tp_string){
	
		CharacterInventory = new int[10];
		string[] tp_array = tp_string.Split ("/" [0]);

		for (int i = 0; i < tp_array.Length; i++) {
		
			CharacterInventory [i] = int.Parse (tp_array [i]);
		}
	}
	public void MakeBackpackArray(string tp_string){

		CharacterBackpack = new int[8];
		string[] tp_array = tp_string.Split ("/" [0]);

		for (int i = 0; i < tp_array.Length; i++) {

			CharacterBackpack[i] = int.Parse (tp_array [i]);
		}
	}

//	IEnumerator UpdateCharactersBase(){
//
//		WWWForm form = new WWWForm ();
//		form.AddField ("sql_text_post","UPDATE characters SET max_health=" + MaxHealth+ ", armor=" +Armor+ ", dodge=" +Dodge+ ", speed=" +Speed+ ", res_fire=" +FireResistance+ ", res_ice=" +IceResistance+ ", res_lightning=" +LightningResistance+ " WHERE character_id=" +ID+ ";");
//		WWW www = new WWW ("http://gamerpg.esy.es/RPG/UpdateCharacter.php", form);
//		yield return www;
//	}

	public string MakeCharacterSlotString(bool tp_inv){

		GameObject slot = GameObject.FindGameObjectWithTag ("CharacterSlots");
		GameObject backpack = GameObject.FindGameObjectWithTag ("Backpack");
		string tp_string = "";

		if (tp_inv) {
			List<Transform> tp_list = new List<Transform> ();
			foreach (Transform trans in slot.transform) {
				tp_list.Add (trans);
			}

			tp_list.RemoveAt (0);
			tp_list.RemoveAt (10);

			List<Item> tp_inventorylist = new List<Item> ();
			int i = 0;
			foreach (Transform trans in tp_list.ToArray()) {
				i++;
				if (trans.name == "CharacterSlot" && trans.GetComponent<Slot> ().item) {

					tp_string = tp_string + trans.GetComponent<Slot> ().item.GetComponent<ItemManager> ().appliedItem.ID;
					tp_inventorylist.Add (trans.GetComponent<Slot> ().item.GetComponent<ItemManager> ().appliedItem);

				} else {
					tp_string = tp_string + "0";
				}
				if (i < tp_list.Count)
					tp_string = tp_string + "/";

			}
			InventoryItems = tp_inventorylist;
			return tp_string;

		} else {

			List<Item> tp_backpacklist = new List<Item> ();
			int i = 0;
			foreach (Transform trans in backpack.transform) {
				i++;
				if (trans.name == "BackpackSlot" && trans.GetComponent<Slot> ().item) {
					tp_string = tp_string + trans.GetComponent<Slot> ().item.GetComponent<ItemManager> ().appliedItem.ID;
					tp_backpacklist.Add (trans.GetComponent<Slot> ().item.GetComponent<ItemManager> ().appliedItem);

				} else {
					tp_string = tp_string + "0";
				}
				if (i < backpack.transform.childCount)
					tp_string = tp_string + "/";

			}
			BackpackItems = tp_backpacklist;
			return tp_string;
		}
	}

	public IEnumerator UpdateCharacterSlot(){

		string sql_text = "UPDATE characters SET slots='" + MakeCharacterSlotString (true) + "', backpack='" + MakeCharacterSlotString (false) + "' WHERE character_id=" + ID + ";";
		if (!PhotonNetwork.isNonMasterClientInRoom) {
			WWWForm form = new WWWForm ();
			form.AddField ("sql_text_post", sql_text);
			WWW www = new WWW ("https://localhost/RPG/UpdateCharacter.php", form);
			yield return www;
		} else {
			string[] content = new string[] {sql_text};
			RaiseEventOptions opt = new RaiseEventOptions ();
			opt.Receivers = ReceiverGroup.MasterClient;
			PhotonNetwork.RaiseEvent (8, content, true, opt);
		}
	}
	#endregion

	#region Skill
	private void LoadSkills(){
//		string sql_text = "SELECT * FROM " + this.Type + "_skills WHERE skill_id IN('" + this.SkillArray [0] + "','" + this.SkillArray [1] + "','" + this.SkillArray [2] + "','" + this.SkillArray [3] + "');";
//		string tp_skillstring = "";
//		string[] tp_skillarray = new string[4];
//
//			int pos = int.Parse(OnlineData.k_storage.tp_dataSkills [0]);
//			OnlineData.k_storage.tp_dataSkills [0] = (pos +5) +"";
//
//			string skillar = "";
//			for (int i = 0; i < this.SkillArray.Length; i++) {
//
//				if (i == this.SkillArray.Length - 1) {
//					skillar = skillar + this.SkillArray [i];
//					break;
//				}
//
//				skillar = skillar + this.SkillArray [i]+"-";
//			}
//
//
//			string[] content = new string[] {pos+"", this.Type,skillar};
//			RaiseEventOptions opt = new RaiseEventOptions ();
//			opt.Receivers = ReceiverGroup.MasterClient;
//			PhotonNetwork.RaiseEvent (12, content, false, opt);
//
//			string starting = OnlineData.k_storage.tp_dataSkills [pos];
//			yield return new WaitUntil (() => (!starting.Equals (OnlineData.k_storage.tp_dataSkills [pos])));
//
//			tp_skillstring = OnlineData.k_storage.tp_dataSkills [pos];
//			OnlineData.k_storage.DataReceived = false;
//
//			List<string> tp_list = new List<string> ();
//			for(int i=0;i<4;i++){
//				try{
//					if(OnlineData.k_storage.tp_dataSkills [pos + i]!="")
//						tp_list.Add(OnlineData.k_storage.tp_dataSkills [pos + i]);
//				}catch{
//					break;
//				}
//			}
//			tp_list.Add ("1");
//			tp_skillarray = tp_list.ToArray ();



		List<int> ids = new List<int> ();

		foreach (SkillDetail d in SkillOverview.skills.ToArray()) {
		
			ids.Add (d.ID);
		}


		for (int i = 0; i < ids.Count; i++) {

			//ID + "|" + Name + "|" + Modifier + "|" + TargetMode + "|" + SkillType + "|" + Target_Activate + "|" + Target + "|" + Target_Individual + "|" + this.Image + "|" + TargetEnemy + "|" + TurnCount + "|" + Dodgeable + "|" + BuffCount + "|" + Buffstring +";";
			AddAvailableSkills(ids[i]);
		}
		ApplySkills ();
	}

	public void AddAvailableSkills(int tp_id){

		Skill tp_skill = TypeSkills [tp_id-1];
		tp_skill.Init ();
		tp_skill.SkillImage = OnlineData.k_storage.k_Skillimages [tp_skill.Image];

		foreach (SkillDetail d in SkillOverview.skills.ToArray()) {

			if (d.ID == tp_skill.ID) {
				tp_skill.UpdateStats (d);
				break;
			}
		}

		if(!AvailableSkills.Contains(tp_skill))
			AvailableSkills.Add (tp_skill);
		

		//AvailableSkills.Add (new Skill (tp_id, tp_name, tp_mod, tp_mode, tp_type, tp_activate, tp_target, tp_individual, OnlineData.k_storage.k_Skillimages[tp_image], enemy,tp_turn, dodge, tp_buffcount, tp_buffstring));
	}

	public void ApplySkills(){

		for (int i = 0; i < 4; i++) {
			for (int j = 0; j < AvailableSkills.Count; j++) {
				if (this.SkillArray [i] == AvailableSkills [j].ID) {
					Skills [i] = AvailableSkills [j];
				}
			}
		}

		this.skillsLoaded = true;
//		GameManager.k_Manager.SkillLoad++;
//
//		int u = 0;
//		for (int i = 0; i < GameManager.k_Manager.Friendly.Count; i++) {
//			if (!GameManager.k_Manager.Friendly [i].isSimple)
//				u++;
//		}
//
//		if (GameManager.k_Manager.SkillLoad >= u)
//			GameManager.k_Manager.SkillsLoaded = true;
		//		} else {
		//		
		//			GameManager.k_Manager.EnemySkillLoad++;
		//			if (GameManager.k_Manager.EnemySkillLoad >= GameManager.k_Manager.Enemy.Count)
		//				GameManager.k_Manager.EnemySkillsLoaded = true;
		//		}
	}

	IEnumerator LoadSkilltrees(){
		string tp_skillstring;

		int pos = int.Parse(OnlineData.k_storage.tp_data [0]);
		OnlineData.k_storage.tp_data [0] = (pos +1) +"";

		string[] content = new string[] {pos+"",this.ID+""};
		RaiseEventOptions opt = new RaiseEventOptions ();
		opt.Receivers = ReceiverGroup.MasterClient;
		PhotonNetwork.RaiseEvent (17, content, false, opt);

		string starting = OnlineData.k_storage.tp_data [pos];
		yield return new WaitUntil (() => (!starting.Equals (OnlineData.k_storage.tp_data [pos])));
		tp_skillstring = OnlineData.k_storage.tp_data [pos];
		OnlineData.k_storage.DataReceived = false;

		SkillOverview = new SkillTree (tp_skillstring);
		this.LoadSkills ();
	}

	private void ApplySkillPoints(string s){
	
		string[] t = s.Split ("/" [0]);

		this.G_SkillPoints = int.Parse (t [0]);
		this.O_Skillpoints = int.Parse (t [1]);
		this.D_Skillpoints = int.Parse (t [2]);
		this.U_Skillpoints = int.Parse (t [3]);
	}

	public void UpdateSkill(){

		Skill tp_skill = null;
		foreach (Skill s in AvailableSkills.ToArray()) {
		
			if (s.ID == CreationManager.k_Manager.currentSkill.ID) {
				foreach (SkillDetail sd in SkillOverview.skills.ToArray()) {

					if (sd.ID == s.ID) {
						s.UpdateStats (sd);
						break;			
					}
				}
				tp_skill = s;
				break;
			}
		}

		foreach (GameObject g in CreationManager.k_Manager.listedSkillManagers.ToArray()) {
		
			if (g.GetComponent<SkillManager> ().appliedSkill.ID == tp_skill.ID) {
				g.GetComponent<SkillManager> ().ApplySkill (tp_skill);
				break;
			}
		}

		foreach (GameObject g in CreationManager.k_Manager.Skills) {
		
			if (g.GetComponent<SkillManager> ().appliedSkill.ID == tp_skill.ID) {
				g.GetComponent<SkillManager> ().ApplySkill (tp_skill);
			}
		}
	}

	#endregion

	#region Items

	public void UseInventory(){

		for (int i = 0; i < CharacterInventory.Length; i++) {
			if (CharacterInventory [i] != 0) {
				PlayerManager.k_manager.StartCoroutine(LoadItem (CharacterInventory [i], i, true, i +11 * ID));
			}
		}
		for (int i = 0; i < CharacterBackpack.Length; i++) {
			if (CharacterBackpack [i] != 0) {
				ItemCount++;
				PlayerManager.k_manager.StartCoroutine(LoadItem (CharacterBackpack [i], i, false, i +12 * ID));
			}
		}
		if (ItemCount == 0) {
			GameObject k_player = GameObject.FindGameObjectWithTag ("Player");
			k_player.GetComponent<Player> ().CharactersLoaded++;
		}
	}

	IEnumerator LoadItem(int tp_id, int tp_pos, bool tp_bool, int pos){

		string tp_itemstring = "";
//		if (!PhotonNetwork.isNonMasterClientInRoom) {
//			WWWForm form = new WWWForm ();
//			form.AddField ("sql_text_post", "SELECT * FROM items WHERE item_id='" + tp_id + "';");
//			WWW itemsdata = new WWW ("https://localhost/RPG/GetItem.php", form);
//			yield return itemsdata;
//			tp_itemstring = itemsdata.text;	
//
//		} else {
			int x = pos;

			string[] content = new string[] { pos+"" ,tp_id + ""};
			RaiseEventOptions opt = new RaiseEventOptions ();
			opt.Receivers = ReceiverGroup.MasterClient;
			PhotonNetwork.RaiseEvent (3, content, true, opt);

			string starting = OnlineData.k_storage.tp_dataItems [x];
			yield return new WaitUntil (() => (!starting.Equals (OnlineData.k_storage.tp_dataItems[x])));
			tp_itemstring = OnlineData.k_storage.tp_dataItems [x];
			OnlineData.k_storage.DataReceived = false;
		
		string[] tp_itemarray = tp_itemstring.Split ("|" [0]);

		GameObject k_player = GameObject.FindGameObjectWithTag ("Player");
		if (tp_bool) {
			//int tp_id, string tp_name,string tp_type, string tp_rarity,int tp_itemlevel, int tp_value, int tp_position,int tp_strength,int tp_constitution,int tp_dexterity, int tp_intelligence, int tp_luck,  int tp_health, int tp_armor, int tp_mindps, int tp_maxdps, float tp_dodge, int tp_speed, int tp_fire, int tp_ice,int tp_light,int tp_damage, int tp_critdamage, int tp_magicdamage, int tp_critmagic, int tp_critchance,int tp_weight, int tp_magicper,int tp_goldper
			//	0				1				2				3				4				5				not given			6				7				8					9					10				11			12			13			14				15					16			17			18			19			20				21					22					23				24					25				26				27														
			try{
				InventoryItems.Add (new Item (int.Parse (tp_itemarray [0]), tp_itemarray [1], tp_itemarray [2], tp_itemarray [3],int.Parse (tp_itemarray [4]),int.Parse (tp_itemarray [5]), tp_pos, int.Parse (tp_itemarray [6]), int.Parse (tp_itemarray [7]), int.Parse (tp_itemarray [8]), int.Parse (tp_itemarray [9]), int.Parse (tp_itemarray [10]), int.Parse (tp_itemarray [11]), int.Parse (tp_itemarray [12]), int.Parse (tp_itemarray [13]),int.Parse (tp_itemarray [14]),float.Parse (tp_itemarray [15]),int.Parse (tp_itemarray [16]),int.Parse (tp_itemarray [17]),int.Parse (tp_itemarray [18]),int.Parse (tp_itemarray [19]),int.Parse (tp_itemarray [20]),int.Parse (tp_itemarray [21]),int.Parse (tp_itemarray [22]),int.Parse (tp_itemarray [23]),int.Parse (tp_itemarray [24]),int.Parse (tp_itemarray [25]),int.Parse (tp_itemarray [26]),int.Parse (tp_itemarray [27])));
			}catch{
				Debug.Log ("Error " + tp_itemarray.Length);
				Debug.Log (tp_itemarray[18]);
			}
				ItemsLoaded++;

			if(ItemsLoaded==ItemCount)
			k_player.GetComponent<Player> ().CharactersLoaded++;

		} else {
			BackpackItems.Add (new Item (int.Parse (tp_itemarray [0]), tp_itemarray [1], tp_itemarray [2], tp_itemarray [3],int.Parse (tp_itemarray [4]),int.Parse (tp_itemarray [5]), tp_pos, int.Parse (tp_itemarray [6]), int.Parse (tp_itemarray [7]), int.Parse (tp_itemarray [8]), int.Parse (tp_itemarray [9]), int.Parse (tp_itemarray [10]), int.Parse (tp_itemarray [11]), int.Parse (tp_itemarray [12]), int.Parse (tp_itemarray [13]),int.Parse (tp_itemarray [14]),float.Parse (tp_itemarray [15]),int.Parse (tp_itemarray [16]),int.Parse (tp_itemarray [17]),int.Parse (tp_itemarray [18]),int.Parse (tp_itemarray [19]),int.Parse (tp_itemarray [20]),int.Parse (tp_itemarray [21]),int.Parse (tp_itemarray [22]),int.Parse (tp_itemarray [23]),int.Parse (tp_itemarray [24]),int.Parse (tp_itemarray [25]),int.Parse (tp_itemarray [26]),int.Parse (tp_itemarray [27])));
			ItemsLoaded++;

			if(ItemsLoaded==ItemCount)
			k_player.GetComponent<Player> ().CharactersLoaded++;
		}
	}
	#endregion

	public string MakeSimpleString(){

		return ID + "/" + Name + "/" + Health + "/" + MaxHealth + "/" + Armor + "/" + Speed + "/" + Position + "/" + Type + "/" + FireResistance + "/" + IceResistance + "/" + LightningResistance;
	}

	public string MakeSkillArrayString(){
	
		string s ="";

		for (int i = 0; i < SkillArray.Length; i++) {
		
			if(i==SkillArray.Length-1)
				s= s + SkillArray[i];
			else
				s= s + SkillArray[i]+"/";
		}
	
		return s;
	}

	public void UpdateSkillTree(){

		string[] content = new string[] {SkillOverview.MakeSkillTreeText(),this.ID+""};
		RaiseEventOptions opt = new RaiseEventOptions ();
		opt.Receivers = ReceiverGroup.MasterClient;
		PhotonNetwork.RaiseEvent (19, content, false, opt);
		UpdateSkillPoints ();
	}

	public void UpdateSkillPoints(){
	
		string s = G_SkillPoints+"/"+ O_Skillpoints+"/"+ D_Skillpoints+"/"+ U_Skillpoints;
		string[] content = new string[] {s,this.ID+""};
		RaiseEventOptions opt = new RaiseEventOptions ();
		opt.Receivers = ReceiverGroup.MasterClient;
		PhotonNetwork.RaiseEvent (21, content, false, opt);
	}
}
