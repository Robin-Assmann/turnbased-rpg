using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class Loot : MonoBehaviour {


	public static Loot k_loot;
	public List<Item> LootItems;
	private int PlayerLevel , TierValue, RarityTier;
	private GameObject LootSlot, pre_Item;

	[SerializeField] int Level;

	List<string> Attributes = new List<string> { "fire_res","ice_res","lightning_res", "armor", "dodge", "speed"};
	List<string> Base_Attributes = new List<string> { "strength", "intelligence", "dexterity", "health"};
	List<string> Special_Attributes = new List<string> { "dmg%", "hp%", "healed%", "fire_dmg%", "ice_dmg%", "lightning_dmg%"};
	List<string> Common_Attributes = new List<string> {"armor", "dodge", "speed", "strength", "intelligence", "dexterity", "health"};

	List<string> Mains;
	List<string> Sub;

	int[] AttributeRange = new int[] {1,20,15,41,35,56,50,81};

	float[] MainStats = new float[] {0.5f,0.6f,0.7f,0.8f,1.0f,1.2f};
	float[] AttributeStats = new float[] {2.0f,2.4f,2.8f,3.2f,4.0f,4.8f};

	float[] WeaponStats = new float[] {};

	//HP = 10+ lvl*lvl-1;


	//		{{	0.1,		0.3,		0.5}, 		//common
	//		{	0.12f,	0.36f,	0.6},			//uncommon
	//		{	0.15f,	4.5f,	7.5f},		//magic
	//		{	0.18f,	5.7f,	9},			//rare
	//		{	0.21f,	6.3f,	10.5f},		//epic
	//		{	0.25f,	7.5f,	12.5f}};



	void Awake(){
		pre_Item = Resources.Load("Item") as GameObject;
		LootSlot = null;
		LootItems = new List<Item> ();
		k_loot = this;	
	}

	public void Reset(){
		LootItems = new List<Item> ();
	}
		
	public void DropNewItem(BattleStorage b){

		this.TierValue = 0;

		//		GameManager.k_Manager.LootLoaded = false;

		//Get Level of Lowest Group Member
		List<Character> tp_char = new List<Character> ();
		tp_char = b.Players;
		tp_char = tp_char.OrderBy (go => go.Level).ToList ();
		PlayerLevel = tp_char [0].Level;
		print ("lowest Player is "+tp_char[0].Name);

		string Rarity;
		string Type;
		int Value = 0;
		Skill AppliedSkill;
		string[,] Attributes;

		float ran = Random.value;
		RarityTier = 0;

		if (ran > 0.99999f) {
			Rarity = "Legendary";
			RarityTier = 5;
			Value += 5000;
		} else if (ran > 0.95f) {
			Rarity = "Epic";
			RarityTier = 4;
			Value += 1200;
		} else if (ran > 0.8f) {
			Rarity = "Rare";
			RarityTier = 3;
			Value += 1200;
		} else if (ran > 0.65f) {
			Rarity = "Magic";
			RarityTier = 2;
			Value += 200;
		} else if (ran > 0.5f) {
			Rarity = "Uncommon";
			RarityTier = 1;
			Value += 50;
		} else {
			Rarity = "Common";
			Value += 10;
		}

		int rand = Random.Range (0, 6);
		switch (rand) {

		case 0:
			Type = "Amulet";
			break;
		case 1:
			Type = "Ring";
			break;
		case 2:
			Type = "Boots";
			break;
		case 3:
			Type = "Helm";
			break;
		case 4:
			Type = "Shoulder";
			break;
		case 5:
			Type = "Weapon";
			break;
		default:
			Type = "";
			break;
		}

		Attributes = GiveNewAttributes (Rarity, Type);

		Value +=(int)(Value * (0.05f * TierValue));

		print ("New Item :" + Rarity + " - " + Type);
		print (Attributes.Length);
		for(int i=0;i<Attributes.Length/2;i++){
			print (Attributes[i,0]+ " : "+ Attributes[i,1]);
		}


		//Problem with getting item ID
		StartCoroutine (InsertItem (b,Rarity, Type, Attributes, PlayerLevel, Value));
	}

	private string[,] GiveNewAttributes(string rarity, string type){

		List<string> AttributeList = new List<string> ();
		this.Mains = new List<string> { "Strength","Dexterity","Constitution", "Intelligence", "Luck"};
		this.Sub = new List<string> { "Dodge","Health","Armor", "Speed", "Crit Chance", "Damage", "Crit Damage", "Magic Damage","Crit Magic", "Fire Resistance", "Ice Resistance", "Lightning Resistance", "Magic Find", "Gold Find"};

		int u;
		switch(rarity){
		case "Common": //common
			u = Random.Range (0, this.Mains.Count);
			AttributeList.Add (this.Mains [u]);
			break;
		case "Uncommon": //base normal
			u = Random.Range (0, this.Mains.Count);
			AttributeList.Add (this.Mains [u]);
			u = Random.Range (0, this.Sub.Count);
			AttributeList.Add (this.Sub [u]);
			break;
		case "Magic": // Base base normal
			u = Random.Range (0,this. Mains.Count);
			AttributeList.Add (this.Mains [u]);

			u = Random.Range (0, this.Sub.Count);
			AttributeList.Add (this.Sub [u]);
			this.Sub.RemoveAt (u);

			u = Random.Range (0, Sub.Count);
			AttributeList.Add (Sub [u]);

			break;
		case "Rare": // base base normal special
			u = Random.Range (0, this.Mains.Count);
			AttributeList.Add (this.Mains [u]);
			this.Mains.RemoveAt (u);

			u = Random.Range (0, this.Mains.Count);
			AttributeList.Add (this.Mains[u]);

			u = Random.Range (0, this.Sub.Count);
			AttributeList.Add (this.Sub [u]);

			//			u = Random.Range (0, Special_Attributes.Count);
			//			AttributeList.Add (Special_Attributes [u]);
			break;
		case "Epic": // base base normal special
			u = Random.Range (0, this.Mains.Count);
			AttributeList.Add (this.Mains [u]);
			this.Mains.RemoveAt (u);

			u = Random.Range (0, this.Mains.Count);
			AttributeList.Add (this.Mains [u]);

			u = Random.Range (0, this.Sub.Count);
			AttributeList.Add (this.Sub [u]);
			this.Sub.RemoveAt (u);

			u = Random.Range (0, this.Sub.Count);
			AttributeList.Add (this.Sub [u]);
			break;
		case "Legendary":
			u = Random.Range (0, this.Mains.Count);
			AttributeList.Add (this.Mains [u]);
			break;
		}

		int e = AttributeList.Count;
		string[,] tp_attribute = new string[e,2];


		for (int i = 0;i< AttributeList.Count; i++) {

			int tierRandom = Random.Range (1, 6);
			this.TierValue += tierRandom;


			tp_attribute [i,0] = AttributeList[i];
			int value = 0;
			if (AttributeList [i].Equals ("Strength") || AttributeList [i].Equals ("Intelligence") || AttributeList [i].Equals ("Luck") || AttributeList [i].Equals ("Dexterity") || AttributeList [i].Equals ("Constitution")) {

				float tp_val = Random.value * (MainStats [RarityTier] * 1.2f - MainStats [RarityTier] * 0.8f) + MainStats [RarityTier] * 0.8f;
				value = Mathf.CeilToInt(tp_val* Level);
				print (tp_val + " - " + Level + " - " + value);
			} else {
				float tp_val = Random.value * (AttributeStats [RarityTier] * 1.2f - AttributeStats [RarityTier] * 0.8f) + AttributeStats [RarityTier] * 0.8f;
				value = Mathf.CeilToInt(tp_val* Level);
			
			}

			tp_attribute [i,1] =  ""+value;
		}
		return tp_attribute;
	}

	#region Old
	public void DropItem(){
		//BattleStorage b
		this.TierValue = 0;

//		GameManager.k_Manager.LootLoaded = false;

		//Get Level of Lowest Group Member
//		List<Character> tp_char = new List<Character> ();
//		tp_char = b.Players;
//		tp_char = tp_char.OrderBy (go => go.Level).ToList ();
//		PlayerLevel = tp_char [0].Level;

		string Rarity;
		string Type;
		int Value = 0;
		Skill AppliedSkill;
		int[,] Attributes;

		float ran = Random.value;

		if (ran > 0.99999f) {
			Rarity = "Legendary";
			Value += 5000;
		} else if (ran > 0.25f) {
			Rarity = "Rare";
			Value += 1200;
		} else if (ran > 0.20f) {
			Rarity = "Magic";
			Value += 200;
		} else if (ran > 0.10f) {
			Rarity = "Uncommon";
			Value += 50;
		} else {
			Rarity = "Common";
			Value += 10;
		}

		int rand = Random.Range (0, 6);
		switch (rand) {

		case 0:
			Type = "Amulet";
			break;
		case 1:
			Type = "Ring";
			break;
		case 2:
			Type = "Boots";
			break;
		case 3:
			Type = "Helm";
			break;
		case 4:
			Type = "Shoulder";
			break;
		case 5:
			Type = "Weapon";
			break;
		default:
			Type = "";
			break;
		}

		Attributes = GiveAttributes (Rarity, Type);

		Value +=(int)(Value * (0.05f * TierValue));

		//Problem with getting item ID
		//StartCoroutine (InsertItem (b, Rarity, Type, Attributes, 1, Value));
	}

	private int[,] GiveAttributes(string rarity, string type){

		List<string> AttributeList = new List<string> ();
		int u;
		switch(rarity){
		case "Common": //common
			u = Random.Range (0, Common_Attributes.Count);
			AttributeList.Add (Common_Attributes [u]);
			break;
		case "Uncommon": //base normal
			u = Random.Range (0, Base_Attributes.Count);
			AttributeList.Add (Base_Attributes [u]);
			u = Random.Range (0, Attributes.Count);
			AttributeList.Add (Attributes [u]);
			break;
		case "Magic": // Base base normal
			u = Random.Range (0, Base_Attributes.Count);
			AttributeList.Add (Base_Attributes [u]);
			Base_Attributes.RemoveAt (u);

			u = Random.Range (0, Base_Attributes.Count);
			AttributeList.Add (Base_Attributes [u]);

			u = Random.Range (0, Attributes.Count);
			AttributeList.Add (Attributes [u]);

			break;
		case "Rare": // base base normal special
			u = Random.Range (0, Base_Attributes.Count);
			AttributeList.Add (Base_Attributes [u]);
			Base_Attributes.RemoveAt (u);

			u = Random.Range (0, Base_Attributes.Count);
			AttributeList.Add (Base_Attributes [u]);

			u = Random.Range (0, Attributes.Count);
			AttributeList.Add (Attributes [u]);

//			u = Random.Range (0, Special_Attributes.Count);
//			AttributeList.Add (Special_Attributes [u]);
			break;
		case "Legendary":
			u = Random.Range (0, Base_Attributes.Count);
			AttributeList.Add (Base_Attributes [u]);
			break;
		}

		int range = Level / 20;
		//ranges
		string[] order = new string[] {"strength","dexterity","intelligence","health","armor","dodge","speed","fire_res","ice_res","lightning_res",	"min_dps","max_dps" ,"magic_damage"};
		int e = order.Length;
		int[,] tp_attribute = new int[e,1];


		for (int i = 0;i< AttributeList.Count; i++) {
		
			int tierRandom = Random.Range (1, 6);
			this.TierValue += tierRandom;

			int o= System.Array.IndexOf (order, AttributeList [i]);

			int diff = (AttributeRange [1 + range] - AttributeRange [0 + range]);
			int value = Random.Range (AttributeRange[0+range] + (int)(diff* (0.15f * tierRandom))  , (int)AttributeRange[1+range] + (int)(diff* (0.15f * (-5 +tierRandom))));
			tp_attribute [o,0] = value;
		}
		return tp_attribute;
	}

	#endregion

	IEnumerator InsertItem(BattleStorage b,string tp_rarity, string tp_type, string[,] tp_attributes, int tp_itemlevel, int tp_value){
	
		print (tp_itemlevel + tp_type);

		float rate = 1.0f;
		switch(tp_rarity){
		case "Common": //common
			rate = 0.8f;
			break;
		case "Uncommon": //base normal
			rate = 0.9f;
			break;
		case "Magic": // Base base normal
			rate = 1.0f;
			break;
		case "Rare": // base base normal special
			rate = 1.1f;
			break;
		case "Epic": // base base normal special
			rate = 1.25f;
			break;
		case "Legendary":
			rate = 1.4f;
			break;
		}

		int minDps = 0;
		int maxDps = 0;
		if (tp_type.Equals ("Weapon")) {

			int avg = (int)Mathf.Round((tp_itemlevel * ((rate-0.05f)+(Random.value*0.10f))));
			int dif = (int)(Random.value * avg * 0.5f);
			maxDps = avg + dif;
			minDps = avg - dif;
		}


		string tp_sql = "('New','" + tp_type + "','" + tp_rarity + "','"+tp_value+ "','10','" + minDps + "','" + maxDps + "',";
		string stats = "";
		string[] input = new string[19];
		for (int i = 0; i < input.Length; i++) {
			input [i] = 0+"";
		}
		for (int i = 0; i < tp_attributes.Length / 2; i++) {
		
			//stats = stats + tp_attributes [i] [0] + "|" + tp_attributes [i] [1] + ";";

			switch (tp_attributes [i,0]) {

			case "Strength":
				input [0] = tp_attributes [i,1];
				break;
			case "Constitution":
				input [1] = tp_attributes [i,1];
				break;
			case "Dexterity":
				input [2] = tp_attributes [i,1];
				break;
			case "Intelligence":
				input [3] = tp_attributes [i,1];
				break;
			case "Luck":
				input [4] = tp_attributes [i,1];
				break;
			case "Dodge":
				input [5] = tp_attributes [i,1];
				break;
			case "Health":
				input [6] = tp_attributes [i,1];
				break;
			case "Armor":
				input [7] = tp_attributes [i,1];
				break;
			case "Speed":
				input [8] = tp_attributes [i,1];
				break;
			case "Crit Chance":
				input [9] = tp_attributes [i,1];
				break;
			case "Damage":
				input [10] = tp_attributes [i,1];
				break;
			case "Crit Damage":
				input [11] = tp_attributes [i,1];
				break;
			case "Magic Damage":
				input [12] = tp_attributes [i,1];
				break;
			case "Crit Magic":
				input [13] = tp_attributes [i,1];
				break;
			case "Fire Resistance":
				input [14] = tp_attributes [i,1];
				break;
			case "Ice Resistance":
				input [15] = tp_attributes [i,1];
				break;
			case "Lightning Resistance":
				input [16] = tp_attributes [i,1];
				break;
			case "Magic Find":
				input [17] = tp_attributes [i,1];
				break;
			case "Gold Find":
				input [18] = tp_attributes [i,1];
				break;
			}
		}

		for(int i =0; i<input.Length;i++){

			tp_sql = tp_sql+"'"+ input[i]+"',";
		}
		int weight = (int)(Random.value * 100);
		tp_sql = tp_sql+ "'"+weight+"','0');";

		print (tp_sql);
		WWWForm form = new WWWForm ();
		//"' AND type='"+ tp_type +
		form.AddField ("sql_text_post", "INSERT INTO items (item_name, type, rarity, value, item_level,min_dps,max_dps, strength,constitution, dexterity,intelligence,luck ,health,armor,dodge,speed,res_fire,res_ice,res_lightning,damage,crit_damage, magic_damage,crit_magic,crit_chance,magicpercent,goldpercent,weight, skill) VALUES "+tp_sql);
		WWW itemsdata = new WWW ("https://localhost/RPG/InsertData.php", form);
		yield return itemsdata;
		print ("Dropped Item is listed at ID: " + itemsdata.text);

		WWWForm form2 = new WWWForm ();
		form.AddField ("sql_text_post", "SELECT * FROM items WHERE item_id='" + itemsdata.text + "';");
		WWW itemsdata2 = new WWW ("https://localhost/RPG/GetItem.php", form);
		yield return itemsdata2;
		string tp_string = itemsdata2.text;

		string[] content = new string[] {tp_string};
		RaiseEventOptions opt = new RaiseEventOptions ();
		opt.TargetActors = b.GroupIds;
		PhotonNetwork.RaiseEvent (14, content, false, opt);
	}

	IEnumerator LoadItem(string tp_id){
		string tp_itemstring = "";

			int pos = int.Parse(OnlineData.k_storage.tp_dataItems [0]);
			OnlineData.k_storage.tp_dataItems [0] = (pos +1) +"";
			string[] content = new string[] {pos+"", tp_id};
			RaiseEventOptions opt = new RaiseEventOptions ();
			opt.Receivers = ReceiverGroup.MasterClient;
			PhotonNetwork.RaiseEvent (3, content, false, opt);

			string starting = OnlineData.k_storage.tp_dataItems [pos];
			yield return new WaitUntil (() => (!starting.Equals (OnlineData.k_storage.tp_dataItems [pos])));

			tp_itemstring = OnlineData.k_storage.tp_dataItems [pos];
			OnlineData.k_storage.DataReceived = false;
		
//		MakeItems (tp_itemstring);
	}

		
}
