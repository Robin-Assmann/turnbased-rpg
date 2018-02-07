using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public class CreationManager : MonoBehaviour {

	public GameObject[] Positions;
	public GameObject[] InventorySlots;
	public GameObject[] CharacterSlots;
	public GameObject[] BackpackSlots;
	public GameObject k_player, pre_Character, pre_Item, pre_Position, pre_FriendPosition,pre_Skill, Inventorys, CharacterSlot, BackpackSlot, Stats, PositionOverview, LootPanel, SkillPanel, Off_Tab, Deff_Tab,Util_Tab, Description, SkillPoints_Text;
	public GameObject SkillTree, Skill_Start, Skill_First1, Skill_First2, Skill_Second1, Skill_Second2, Skill_Third1, Skill_Third2, Skill_Ultimate1, Skill_Ultimate2, Skill_Ultimate3;
	public GameObject General_Skillpoints, Type_Skillpoints, Type_Text;
	public GameObject[] SkillList;
	public List<GameObject> currentSkillList;
	public Character currentCharacter;
	public Skill currentSkill;
	public Sprite[] ItemImages, Backgrounds;
	public Sprite Skill_locked, Dmg_Upgrade, CritPer_Upgrade, Twice_Upgrade,AOE_Upgrade;
	public static CreationManager k_Manager;
	public GameObject[] Skills, SkillStats;
	public GameObject Enemy5x4, Enemy4x4;
	public int selectedSkill =-1;
	private GameObject pre_PlayerOnline, PlayerPanel;
	[SerializeField]
	private GameObject Shop, Taverne, Dungeon, PlayPanel, Skill1,Skill2,Skill3,Skill4, SkillStat;

	private List<List<int>> plantedFriends;
	public List<GameObject> listedSkillManagers;

	[SerializeField] private GameObject InvitePanel;

	public List<Transform> Items;
	public int InvitedBy;

	private Coroutine invite;
	private List<PlayerOnline> playersOnline;
	public int SkillPoints;

	#region Setup
	void Awake(){
	
		k_Manager = this;

		plantedFriends = new List<List<int>> ();
		currentSkillList = new List<GameObject> ();
		listedSkillManagers = new List<GameObject> ();

		playersOnline = new List<PlayerOnline> ();
		ItemImages = Resources.LoadAll<Sprite>("RPG_inventory_icons");
		Backgrounds = Resources.LoadAll<Sprite>("Backgrounds");

//		Stats = GameObject.FindGameObjectWithTag ("Stats");
//		PositionOverview = GameObject.FindGameObjectWithTag ("Positions");

//		Inventorys = GameObject.FindGameObjectWithTag ("InventorySlot");
//		CharacterSlot = GameObject.FindGameObjectWithTag ("CharacterSlots");
//		BackpackSlot = GameObject.FindGameObjectWithTag ("Backpack");
		k_player = GameObject.FindGameObjectWithTag ("Player");
		k_player.GetComponent<Player> ().sceneloaded = true;
		pre_Item = Resources.Load("Item") as GameObject;
		pre_FriendPosition = Resources.Load ("FriendPositionImage") as GameObject;

		currentCharacter = k_player.GetComponent<Player> ().Characters [0];

		if (k_player.gameObject.GetComponent<Player>().OutstandingLoot.Count > 0) {
			LootPanel.SetActive (true);
			ApplyLoot ();
		}

		Skills = new GameObject[4];
		Skills [0] = Skill1;
		Skill1.GetComponent<SkillManager> ().id = 0;
		Skills [1] = Skill2;
		Skill2.GetComponent<SkillManager> ().id = 1;
		Skills [2] = Skill3;
		Skill3.GetComponent<SkillManager> ().id = 2;
		Skills [3] = Skill4;
		Skill4.GetComponent<SkillManager> ().id = 3;

		Positions = new GameObject[17];

		for (int i = 0; i < PositionOverview.transform.childCount; i++) {
			Positions [i] = PositionOverview.transform.GetChild (i).gameObject;
		}
		Positions [16] = GameObject.FindGameObjectWithTag ("CharacterPositionSlot");

		InventorySlots = new GameObject[36];
		for (int i = 0; i < Inventorys.transform.childCount; i++) {
			InventorySlots [i] = Inventorys.transform.GetChild (i).gameObject;
		}

		CharacterSlots = new GameObject[10];
		for (int i = 1; i < CharacterSlot.transform.childCount-1; i++) {
			CharacterSlots [i-1] = CharacterSlot.transform.GetChild (i).gameObject;
		}

		BackpackSlots = new GameObject[8];
		for (int i = 0; i < BackpackSlot.transform.childCount; i++) {
			BackpackSlots [i] = BackpackSlot.transform.GetChild (i).gameObject;
		}

		pre_PlayerOnline = Resources.Load ("PlayerOnline") as GameObject;
		PlayerPanel = GameObject.FindGameObjectWithTag("PlayerOnline");

		SkillList = new GameObject[10];

		SkillList [0] = Skill_Start;
		SkillList [1] = Skill_First1;
		SkillList [2] = Skill_First2;
		SkillList [3] = Skill_Second1;
		SkillList [4] = Skill_Second2;
		SkillList [5] = Skill_Third1;
		SkillList [6] = Skill_Third2;
		SkillList [7] = Skill_Ultimate1;
		SkillList [8] = Skill_Ultimate2;
		SkillList [9] = Skill_Ultimate3;

		InstantiateItems ();
		InstatiatePositionItems ();

		this.SkillPoints = 20;

		SkillStats = new GameObject[4];

		for (int i = 0; i < SkillStats.Length; i++) {
			SkillStats [i] = SkillStat.transform.GetChild (i).gameObject;
		}

	}

	void Start(){
		
		if (Shop != null) {
			PlayPanel.SetActive (false);
			Shop.SetActive (false);
			Dungeon.SetActive (false);
			Taverne.SetActive (false);
		}
	}
	#endregion // Awake

	#region Instantiation
	public void InstantiateItems(){
		List<Sprite> tp_img = new List<Sprite> (ItemImages);

		foreach (Item tp_item in k_player.GetComponent<Player>().InventoryItems.ToArray()) {

			GameObject tp_spawn = Instantiate (pre_Item) as GameObject;
			Items.Add (tp_spawn.transform);
			tp_spawn.transform.SetParent (InventorySlots [tp_item.Position].transform, false);

			ChangeItemAttributes (tp_item, tp_spawn, tp_img);
		}
		InstantiateCharacterItems ();
	}

	public void InstantiateCharacterItems(){
	
		List<Sprite> tp_img = new List<Sprite> (ItemImages);

		//make Items with prefab
		foreach (Item tp_item in currentCharacter.InventoryItems.ToArray()) {

			GameObject tp_spawn = Instantiate (pre_Item) as GameObject;
			Items.Add (tp_spawn.transform);
			tp_spawn.transform.SetParent (CharacterSlots [tp_item.Position].transform, false);
			ChangeItemAttributes (tp_item, tp_spawn, tp_img);
		}
		foreach (Item tp_item in currentCharacter.BackpackItems.ToArray()) {

			GameObject tp_spawn = Instantiate (pre_Item) as GameObject;
			Items.Add (tp_spawn.transform);
			tp_spawn.transform.SetParent (BackpackSlots [tp_item.Position].transform, false);
			ChangeItemAttributes (tp_item, tp_spawn, tp_img);
		}
		InstantiateSkills ();
		InstantiateSkillTabs ();

		StartCoroutine (UpdateNow ());
	}

	IEnumerator UpdateNow(){
	
		yield return new WaitWhile(()=>Inventory.k_inventory==null); 
		Inventory.k_inventory.UpdateThis ();
	}

	public void InstatiatePositionItems(){
		GameObject tp_trans = GameObject.FindGameObjectWithTag("CharacterPanel");
		foreach (Character tp_char in k_player.GetComponent<Player>().Characters) {

			int i = tp_char.Position;

			GameObject tp_g = Instantiate (pre_Character) as GameObject;
			tp_g.transform.SetParent (tp_trans.transform, false);
			tp_g.transform.GetComponent<Image> ().color = new Color (0.0f, 0.25f * tp_char.ID , 0.0f, 1.0f);
			tp_g.GetComponent<CreationApply> ().appliedCharacter = tp_char;

			if (i == 16 && currentCharacter!= tp_char)
				continue;

			GameObject tp_pos = Instantiate (pre_Position) as GameObject;
			tp_pos.transform.SetParent (Positions [i].transform, false);
			tp_pos.GetComponent<DragHandler> ().Position_ID = tp_char.ID;
			tp_pos.GetComponent<Image>().color = new Color (0.0f, 0.25f * tp_char.ID , 0.0f, 1.0f);
			tp_pos.GetComponent<DragHandler> ().appliedCharacter = tp_char;
		}
	}

	public void AddCharacterPositionItem(){
		GameObject tp_pos = Instantiate (pre_Position) as GameObject;
		tp_pos.transform.SetParent (Positions [16].transform, false);
		tp_pos.GetComponent<Image>().color = new Color (0.0f, 0.25f * currentCharacter.ID , 0.0f, 1.0f);
		tp_pos.GetComponent<DragHandler> ().Position_ID = currentCharacter.ID;
		tp_pos.GetComponent<DragHandler> ().shouldhave = true;
	}

	public void InstantiateSkills(){

		for (int i = 0; i < currentCharacter.SkillArray.Length; i++) {
			foreach (Skill s in currentCharacter.AvailableSkills) {
				if (s.ID == currentCharacter.SkillArray [i]) {
					Skills [i].GetComponent<SkillManager> ().ApplySkill (s);
					break;
				}
			}
		}
	}

	public void ResetSkillTabs(){
	
		if (Off_Tab.transform.childCount != 0) {
		
			foreach (Transform t in Off_Tab.transform.GetChild(1)) {
				Destroy (t.gameObject);
			}
		}
		if (Deff_Tab.transform.childCount != 0) {

			foreach (Transform t in Deff_Tab.transform.GetChild(1)) {
				Destroy (t.gameObject);
			}
		}
		if (Util_Tab.transform.childCount != 0) {

			foreach (Transform t in Util_Tab.transform.GetChild(1)) {
				Destroy (t.gameObject);
			}
		}
	}

	public void InstantiateSkillTabs(){

		Off_Tab.SetActive (false);
		Deff_Tab.SetActive (false);
		Util_Tab.SetActive (false);

		listedSkillManagers = new List<GameObject> ();

		foreach (Skill s in currentCharacter.AvailableSkills.ToArray()) {
			switch (s.SkillIdentifier) {
			case 0:
				ApplySkills (Off_Tab, s);
				break;
			case 1:
				ApplySkills (Deff_Tab, s);
				break;
			case 2:
				ApplySkills (Util_Tab, s);
				break;
			}
		}
	}

	public void ApplySkills(GameObject g , Skill s){

		g.SetActive (true);
		Transform par = g.transform.GetChild (1);

		GameObject tp_skill = Instantiate (pre_Skill) as GameObject;
		tp_skill.transform.SetParent (par);
		tp_skill.transform.localPosition = new Vector3 (0, 0, 0);
		tp_skill.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
		tp_skill.transform.GetChild (0).GetComponent<SkillManager> ().ApplySkill (s);
		listedSkillManagers.Add (tp_skill.transform.GetChild (0).gameObject);

	}

	public void InstantiateSkillTree(){
		HighlightSkill ();
		ApplySkillPoints ();
		ApplySkillTreebackground ();
		ApplySkillStats ();
		List<GameObject> tp_Skills = new List<GameObject>(SkillList);

		for (int i = 0; i < SkillList.Length; i++) {
			SkillList [i].gameObject.SetActive (true);
		}

		string[] rows = currentSkill.Upgrade.Split ("|" [0]);

		int offset = 0;
		for (int i = 0; i < 3; i++) {
		
			int o = int.Parse (rows [i]);
			if(o==1){
				SkillList [o + (i * 2)].gameObject.SetActive (false);
				tp_Skills.RemoveAt (o + (i * 2)-offset);
				offset++;
			}
		}
		if (rows [3].Equals ("1-2-3") || rows [3].Equals ("3")) {
			
			SkillList [7].gameObject.SetActive (false);
			tp_Skills.RemoveAt (7-offset);
			offset++;
			SkillList [9].gameObject.SetActive (false);
			tp_Skills.RemoveAt (9-offset);
			offset++;
		}
		if (rows [3].Equals ("1") || rows [3].Equals ("1-2")) {
			SkillList [8].gameObject.SetActive (false);
			tp_Skills.RemoveAt (8-offset);
			offset++;
			SkillList [9].gameObject.SetActive (false);
			tp_Skills.RemoveAt (9-offset);
			offset++;
		}
		if (rows [3].Equals ("3") || rows [3].Equals ("2-3")) {
			SkillList [7].gameObject.SetActive (false);
			tp_Skills.RemoveAt (7-offset);
			offset++;
			SkillList [8].gameObject.SetActive (false);
			tp_Skills.RemoveAt (8-offset);
			offset++;
		}

		this.currentSkillList = tp_Skills;
		ApplySkillTreeProgress (tp_Skills);
	}


	public void ApplySkillTreeProgress(List<GameObject> list){

		SkillDetail progress=null;

		foreach (SkillDetail s in currentCharacter.SkillOverview.skills.ToArray()) {
		
			if (s.ID == currentSkill.ID) {
				progress = s;
				break;
			}
		}

		ApplySkillTreeImage (list [0].transform.GetChild(0),0,0, "",-1);

		for (int i = 0; i < progress.FirstRow.Count; i++) {

			if (progress.FirstRow [i] < 0){
				list [i + 1].transform.GetChild (0).GetComponent<Image> ().sprite = Skill_locked;
				list [i + 1].transform.GetChild (0).GetComponent<UpgradeManager> ().Lock ();
			}else if (i != 0) {
				ApplySkillTreeImage (list [i + 1].transform.GetChild (0), i + 1, progress.FirstRow [i], i + "",1);
			}else
				ApplySkillTreeImage (list [i + 1].transform.GetChild (0), i + 1, progress.FirstRow [i], 0 + "",0);
		}
		for (int i = 0; i < progress.SecondRow.Count; i++) {

			if (progress.SecondRow [i] < 0) {
				list [i + progress.FirstRow.Count + 1].transform.GetChild (0).GetComponent<Image> ().sprite = Skill_locked;
				list [i + progress.FirstRow.Count + 1].transform.GetChild (0).GetComponent<UpgradeManager> ().Lock ();
			}else if (i != 0) {
				ApplySkillTreeImage (list [i + progress.FirstRow.Count + 1].transform.GetChild (0), i + progress.FirstRow.Count + 1, progress.SecondRow [i], i + progress.FirstRow.Count+"", 11);
			}else
				ApplySkillTreeImage (list [i + progress.FirstRow.Count + 1].transform.GetChild (0), i + progress.FirstRow.Count + 1, progress.SecondRow [i], 0 + "", 10);
		}
		for (int i = 0; i < progress.ThirdRow.Count; i++) {

			if (progress.ThirdRow [i] < 0) {
				list [i + progress.FirstRow.Count + progress.SecondRow.Count + 1].transform.GetChild (0).GetComponent<Image> ().sprite = Skill_locked;
				list [i + progress.FirstRow.Count + progress.SecondRow.Count + 1].transform.GetChild (0).GetComponent<UpgradeManager> ().Lock ();
			}else if (i != 0) {
				ApplySkillTreeImage (list [i + progress.FirstRow.Count +progress.SecondRow.Count+1].transform.GetChild(0),i + progress.FirstRow.Count +progress.SecondRow.Count+1,progress.ThirdRow [i],i + progress.FirstRow.Count +progress.SecondRow.Count+"",21);
			}else
				ApplySkillTreeImage (list [i + progress.FirstRow.Count +progress.SecondRow.Count+1].transform.GetChild(0),i + progress.FirstRow.Count +progress.SecondRow.Count+1,progress.ThirdRow [i], 0+"",20);
		}

		if (progress.Ultimate < 0) {
			list [list.Count - 1].transform.GetChild (0).GetComponent<Image> ().sprite = Skill_locked;
			list [list.Count - 1].transform.GetChild (0).GetComponent<UpgradeManager> ().Lock ();
		}else 
			ApplySkillTreeImage (list [list.Count - 1].transform.GetChild(0),list.Count - 1, progress.Ultimate,CheckRecForUltimate(list, progress),30);
	}

	private string CheckRecForUltimate(List<GameObject> list, SkillDetail progress){
	
		string[] rows = currentSkill.Upgrade.Split ("|" [0]);
		string ult = rows [3];
		string[] pos;
		try{
			pos = ult.Split("-"[0]);
		}catch{
			pos = new string[0];
			pos [0] = ult;
		}
		List<int> positions = new List<int> ();
		for (int i = 0; i < pos.Length; i++) {
			int offset = 0;
			switch (pos [i]) {

			case "1":
				positions.Add (progress.FirstRow.Count);
				break;
			case "2":
				offset = progress.FirstRow.Count;
				positions.Add (progress.SecondRow.Count+offset);
				break;
			case "3":
				offset = progress.FirstRow.Count+progress.SecondRow.Count ;
				positions.Add (progress.ThirdRow.Count);
				break;
			}
		}
		string o="";
		for (int i = 0; i < positions.Count; i++) {
			o = o + positions [i];
			if (i < positions.Count - 1)
				o = o + "-";
		}
		return o;
	}

	public void ApplySkillTreeImage(Transform g, int id, int v, string t, int UpgradeNumber){
		Sprite s = Skill_locked;

		if (id != 0) {
			switch (currentSkill.UpgradeDetails [id - 1].Type) {

			case 0:
				s = Dmg_Upgrade;
				break;
			case 1:
				s = CritPer_Upgrade;
				break;
			case 2:
				s = AOE_Upgrade;
				break;
			case 3:
				s = Twice_Upgrade;
				break;
			}
			string[] p;
			try{
				p = t.Split("-"[0]);

			}catch{
				p = new string[1];
				p[0]=t;
			}
			List<int> RecIds = new List<int> ();
			for (int i = 0; i < p.Length; i++) {
			
				RecIds.Add (int.Parse (p [i]));
			}
			g.GetComponent<UpgradeManager> ().ApplyUpgrade (currentSkill.UpgradeDetails [id - 1],v, RecIds,UpgradeNumber);
		} else {

			s = currentSkill.SkillImage;
		}
		g.GetComponent<Image> ().sprite = s;
	}

	public void ApplySkillTreebackground(){
	
		switch (currentSkill.Upgrade) {

		case "2|2|1|1-2-3":
			SkillTree.GetComponent<Image> ().sprite = Backgrounds [0];
			break;
		case "2|1|2|1-2":
			SkillTree.GetComponent<Image> ().sprite = Backgrounds [1];
			break;
		case "1|2|1|1-2-3":
			SkillTree.GetComponent<Image> ().sprite = Backgrounds [2];
			break;
		case "1|2|2|2-3":
			SkillTree.GetComponent<Image> ().sprite = Backgrounds [3];
			break;
		}
	}

	public void ApplySkillPoints(){
	
		General_Skillpoints.GetComponent<Text> ().text = currentCharacter.G_SkillPoints + "";

		switch (currentSkill.SkillIdentifier) {

		case 0:
			this.SkillPoints = currentCharacter.G_SkillPoints + currentCharacter.O_Skillpoints;
			Type_Text.GetComponent<Text> ().text = "Offensive Skillpoints";
			Type_Skillpoints.GetComponent<Text> ().text = currentCharacter.O_Skillpoints + "";
			break;
		case 1:
			this.SkillPoints = currentCharacter.G_SkillPoints + currentCharacter.D_Skillpoints;
			Type_Text.GetComponent<Text> ().text = "Deffensive Skillpoints";
			Type_Skillpoints.GetComponent<Text> ().text = currentCharacter.D_Skillpoints + "";
			break;
		case 2:
			this.SkillPoints = currentCharacter.G_SkillPoints + currentCharacter.U_Skillpoints;
			Type_Text.GetComponent<Text> ().text = "Utility Skillpoints";
			Type_Skillpoints.GetComponent<Text> ().text = currentCharacter.U_Skillpoints + "";
			break;
		}
	}

	private void HighlightSkill(){
	
		foreach (GameObject g in listedSkillManagers.ToArray()) {
		
			if (g.GetComponent<SkillManager> ().appliedSkill == currentSkill) {
			
				g.GetComponent<SkillManager> ().Highlight ();

			} else {
				g.GetComponent<SkillManager> ().Deselect ();
			}
		}
	}

	public void SelectSkill(){

		Skills [this.selectedSkill].GetComponent<SkillManager> ().ApplySkill(this.currentSkill);
		currentCharacter.SkillArray [this.selectedSkill] = this.currentSkill.ID;

		string[] content = new string[] { currentCharacter.MakeSkillArrayString(), currentCharacter.ID+"" };
		RaiseEventOptions opt = new RaiseEventOptions ();
		opt.Receivers = ReceiverGroup.MasterClient;
		PhotonNetwork.RaiseEvent (18, content, true, opt);
	}
	#endregion

	#region ChangeItemsTexts / ChangeStats
	public void ChangeItemAttributes(Item tp_item, GameObject tp_spawn, List<Sprite> tp_img){

		tp_spawn.GetComponent<ItemManager> ().appliedItem = tp_item;
		tp_spawn.GetComponent<ItemManager> ().ApplyTexts ();

		switch (tp_item.Type) {

		case "Amulet":
			tp_spawn.GetComponent<Image> ().sprite = tp_img.Find(e => e.name == "necklace");

			break;
		case "Ring":
			tp_spawn.GetComponent<Image> ().sprite = tp_img.Find(e => e.name == "rings");
			break;
		
		case "Boots":
			tp_spawn.GetComponent<Image> ().sprite = tp_img.Find(e => e.name == "boots");
			break;

		case "Helm":
			tp_spawn.GetComponent<Image> ().sprite = tp_img.Find(e => e.name == "helmets");
			break;

		case "Shoulder":
			tp_spawn.GetComponent<Image> ().sprite = tp_img.Find(e => e.name == "shoulders");
			break;

		case "Weapon":
			tp_spawn.GetComponent<Image> ().sprite = tp_img.Find(e => e.name == "sword");
			break;
		}

		switch (tp_item.Rarity) {

		case "Common": 
			tp_spawn.transform.GetChild (0).GetComponent<Image> ().color = new Color (1.0f, 1.0f, 1.0f, 1.0f); //white
			break;
		case "Uncommon": 
			tp_spawn.transform.GetChild (0).GetComponent<Image> ().color = new Color (0.0f, 1.0f, 0.0f, 1.0f); //lightgreen
			break;
		case "Magic":
			tp_spawn.transform.GetChild (0).GetComponent<Image> ().color = new Color (0.0f, 0.0f, 1.0f, 1.0f); //blue
			break;
		case "Rare":
			tp_spawn.transform.GetChild (0).GetComponent<Image> ().color = new Color (1.0f, 1.0f, 0.0f, 1.0f); //yellow
			break;
		case "Epic":
			tp_spawn.transform.GetChild (0).GetComponent<Image> ().color = new Color (1.0f, 0.0f, 1.0f, 1.0f); //darkgreen
			break;
		case "Legendary":
			tp_spawn.transform.GetChild (0).GetComponent<Image> ().color = new Color (1.0f, 0.5f, 0.0f, 1.0f); //orange
			break;
		}
	}

	public void ChangeTexts(){
	
		Stats.transform.FindChild ("Name").GetComponent<Text> ().text = currentCharacter.Name;
		Stats.transform.FindChild ("Health").GetComponent<Text> ().text = currentCharacter.MaxHealth +"";
		Stats.transform.FindChild ("Armor").GetComponent<Text> ().text = currentCharacter.Armor+"";
		Stats.transform.FindChild ("Dodge").GetComponent<Text> ().text = currentCharacter.Dodge * 100+" %";
		Stats.transform.FindChild ("Speed").GetComponent<Text> ().text = currentCharacter.Speed+"";
		Stats.transform.FindChild ("FireRes").GetComponent<Text> ().text = currentCharacter.FireResistance+"";
		Stats.transform.FindChild ("IceRes").GetComponent<Text> ().text = currentCharacter.IceResistance+"";
		Stats.transform.FindChild ("LightningRes").GetComponent<Text> ().text = currentCharacter.LightningResistance+"";
		Stats.transform.FindChild ("Strength").GetComponent<Text> ().text = currentCharacter.Strength+"";
		Stats.transform.FindChild ("Constitution").GetComponent<Text> ().text = currentCharacter.Constituition+"";
		Stats.transform.FindChild ("Dexterity").GetComponent<Text> ().text = currentCharacter.Dexterity+"";
		Stats.transform.FindChild ("Intelligence").GetComponent<Text> ().text = currentCharacter.Intelligence+"";
		Stats.transform.FindChild ("Luck").GetComponent<Text> ().text = currentCharacter.Luck+"";
		Stats.transform.FindChild ("Type").GetComponent<Text> ().text = currentCharacter.Type;
		Stats.transform.FindChild ("Level").GetComponent<Text> ().text = "Lvl "+currentCharacter.Level;
		Stats.transform.FindChild ("Race").GetComponent<Text> ().text = currentCharacter.Race;
		Stats.transform.FindChild ("PhysicalDamage").GetComponent<Text> ().text = currentCharacter.Damage+"";
		Stats.transform.FindChild ("MagicDamage").GetComponent<Text> ().text = currentCharacter.MagicDamage +"";
		Stats.transform.FindChild ("CritDamage").GetComponent<Text> ().text = currentCharacter.CritDamage+"";
		Stats.transform.FindChild ("MagicCrit").GetComponent<Text> ().text = currentCharacter.CritMagicDamage+"";
		Stats.transform.FindChild ("CritChance").GetComponent<Text> ().text = currentCharacter.CritChance+"";
		Stats.transform.FindChild ("MagicFind").GetComponent<Text> ().text = currentCharacter.MagicPercent+"";
		Stats.transform.FindChild ("GoldFind").GetComponent<Text> ().text = currentCharacter.GoldPercent+"";
	}

	#endregion

	#region Inventory 
	public void MakeInventoryString(){
	
		string tp_string ="";
		for (int i = 0; i < InventorySlots.Length; i++) {
			if (InventorySlots [i].transform.childCount > 0) {
				tp_string = tp_string + InventorySlots [i].transform.GetChild (0).GetComponent<ItemManager> ().appliedItem.ID;
			} else {
				tp_string = tp_string + "0";
			}
		
			if (i < InventorySlots.Length - 1)
				tp_string = tp_string + "/";
		}
		StartCoroutine (UpdateInventory (tp_string));
	}
	IEnumerator UpdateInventory(string tp_string){
		
		GameObject player = GameObject.FindGameObjectWithTag ("Player");
		if (!PhotonNetwork.isNonMasterClientInRoom) {
			WWWForm form = new WWWForm ();
			form.AddField ("sql_text_post", "UPDATE player SET inventory='" + tp_string + "' WHERE player_id=" + player.GetComponent<Player> ().PlayerID + ";");
			WWW www = new WWW ("https://localhost/RPG/UpdateCharacter.php", form);
			yield return www;
		} else {
			string[] content = new string[] { tp_string, player.GetComponent<Player> ().PlayerID+"" };
			RaiseEventOptions opt = new RaiseEventOptions ();
			opt.Receivers = ReceiverGroup.MasterClient;
			PhotonNetwork.RaiseEvent (6, content, true, opt);
		}
	}
	#endregion // MakeInventoryString/ UpdateInventory

	#region makePlayeronline

	public void MakePlayerOnline(string tp_name, PhotonPlayer player){
		GameObject tp_playeron = Instantiate (pre_PlayerOnline) as GameObject;
		tp_playeron.GetComponent<PlayerOnline> ().PlayerName = tp_name;
		tp_playeron.GetComponent<PlayerOnline> ().Player = player;
		tp_playeron.transform.SetParent (PlayerPanel.transform,false);

		playersOnline.Add (tp_playeron.GetComponent<PlayerOnline>());
	}

	public void ShowInvite (int senderid){

		InvitedBy = senderid;
		string name = "";
		foreach (PhotonPlayer play in PhotonNetwork.otherPlayers) {
			if (play.ID == senderid) {
				name = play.name;
				break;
			}
		}

		foreach (Transform trans in PlayerPanel.transform) {
			if (trans.GetComponent<PlayerOnline> ().PlayerName == name) {
				trans.GetChild (1).GetComponent<Button> ().interactable = false;
				break;
			}
		}

		InvitePanel.SetActive (true);
		InvitePanel.transform.FindChild ("NameText").GetComponent<Text> ().text = "by " + name;
		invite = StartCoroutine (HideInvite ());
	}

	IEnumerator HideInvite(){
	
		yield return new WaitForSeconds (10.0f);
		SetInvitePanel(false);
	}


	public void AcceptInvite(){
	
		object[] content = new object[] {""};
		RaiseEventOptions opt = new RaiseEventOptions ();
		opt.TargetActors = new int[1]{InvitedBy};
		PhotonNetwork.RaiseEvent (101, content, true, opt);

		PlayerManager.k_manager.isMultiplayer = true;
		PlayerManager.k_manager.GroupIDs.Add (InvitedBy);

//		AddCharacterToRoom ();

		PlayerAccepted (InvitedBy);

		StopCoroutine (invite);
		SetInvitePanel(false);
	
	}

	public void DeclineInvite(){
	
		object[] content = new object[] {""};
		RaiseEventOptions opt = new RaiseEventOptions ();
		opt.TargetActors = new int[1]{InvitedBy};
		PhotonNetwork.RaiseEvent (102, content, true, opt);

		StopCoroutine (invite);
		SetInvitePanel(false);
	
	}

	public void SetInvitePanel(bool tp_bool){
		InvitePanel.SetActive (tp_bool);
	}

	public void PlayerAccepted(int playerid){
	
		foreach (PlayerOnline po in playersOnline.ToArray() ) {
		
			if (po.Player.ID == playerid) {
			
				po.JoinedGroup ();
			}
		}
	}

	#endregion

	public void ChangeSkillPoints(int value){
		int dif = 0;
		switch (currentSkill.SkillIdentifier) {
		case 0:
			if (currentCharacter.O_Skillpoints > -value) {
				currentCharacter.O_Skillpoints += value;
			} else {
				dif = currentCharacter.O_Skillpoints + value;
				currentCharacter.O_Skillpoints =0;
			}
			Type_Skillpoints.GetComponent<Text> ().text = currentCharacter.O_Skillpoints + "";
			break;
		case 1:
			if (currentCharacter.D_Skillpoints > -value) {
				currentCharacter.D_Skillpoints += value;
			} else {
				dif = currentCharacter.D_Skillpoints + value;
				currentCharacter.D_Skillpoints =0;
			}
			Type_Skillpoints.GetComponent<Text> ().text = currentCharacter.D_Skillpoints + "";
			break;
		case 2:
			if (currentCharacter.U_Skillpoints > -value) {
				currentCharacter.U_Skillpoints += value;
			} else {
				dif = currentCharacter.U_Skillpoints + value;
				currentCharacter.U_Skillpoints =0;
			}
			Type_Skillpoints.GetComponent<Text> ().text = currentCharacter.U_Skillpoints + "";
			break;
		}

		this.SkillPoints += value;
		currentCharacter.G_SkillPoints += dif;
		General_Skillpoints.GetComponent<Text> ().text = currentCharacter.G_SkillPoints + "";
	
	}

	public void StartBattle(){

		k_player.GetComponent<Player> ().isStarter = true;

		RaiseEventOptions options = new RaiseEventOptions ();
		options.Receivers = ReceiverGroup.MasterClient;
		PhotonNetwork.RaiseEvent(30, "", true, options);

		MakeInventoryString ();
	}

	public void AddCharacterToRoom(){
	
		Hashtable hash = PhotonNetwork.room.customProperties;
		Player play = k_player.GetComponent<Player> ();
		hash.Add ((string)play.PlayerName, (int)play.Characters.Length);
		for(int i=0; i<play.Characters.Length;i++){
			hash.Add ((string)play.PlayerName + i, play.Characters [i].MakeSimpleString ());
		}
		PhotonNetwork.room.SetCustomProperties (hash);
	}

	#region Item/ Loot
	public void DeleteItems(){
	
		foreach (Transform trans in BackpackSlot.transform) {

			if (trans.childCount > 0) {
				Items.Remove (trans.GetChild (0).transform);
				DestroyImmediate (trans.GetChild (0).gameObject);
			}
		}

		foreach (Transform trans in CharacterSlot.transform) {
			if (trans.childCount > 0 && trans.name== "CharacterSlot") {
				Items.Remove (trans.GetChild (0).transform);
				DestroyImmediate (trans.GetChild (0).gameObject);
			}
		}

		Transform tp_charpos = GameObject.FindGameObjectWithTag ("CharacterPositionSlot").transform;
		if (tp_charpos.childCount > 0)
			for (int i = 0; i < tp_charpos.childCount; i++) {
				DestroyImmediate (tp_charpos.GetChild (i).gameObject);
			}
	}

	public void DeleteLoot(){
	
		GameObject LootSlots = GameObject.FindGameObjectWithTag ("LootSlots");

		for (int i = 0; i < LootSlots.transform.childCount; i++) {
		
			if (LootSlots.transform.GetChild (i).childCount > 0) {
				Items.Remove (LootSlots.transform.GetChild (i).GetChild (0).transform);
				StartCoroutine (DeleteItem (LootSlots.transform.GetChild (i).GetChild (0).GetComponent<ItemManager> ().appliedItem.ID));
				DestroyImmediate (LootSlots.transform.GetChild (i).GetChild (0).gameObject);
			}
		}
		LootPanel.SetActive (false);
	}

	IEnumerator DeleteItem(int item_id){
	
		if (!PhotonNetwork.isNonMasterClientInRoom) {
			WWWForm form = new WWWForm ();
			form.AddField ("sql_text_post", "DELETE FROM items WHERE item_id =" + item_id + ";");
			WWW www = new WWW ("https://localhost/RPG/UpdateCharacter.php", form);
			yield return www;
		} else {
			string[] content = new string[] { item_id+"" };
			RaiseEventOptions opt = new RaiseEventOptions ();
			opt.Receivers = ReceiverGroup.MasterClient;
			PhotonNetwork.RaiseEvent (7, content, true, opt);
		}
	}
	public void ApplyLoot(){

		GameObject LootSlot = GameObject.FindGameObjectWithTag ("LootSlots");
		Transform[] tp_slots = new Transform[9];
		List<Sprite> tp_img = new List<Sprite> (CreationManager.k_Manager.ItemImages);
		List<Item> LootItems = k_player.GetComponent<Player> ().OutstandingLoot;

		for (int i = 0; i < 9; i++) {

			tp_slots [i] = LootSlot.transform.GetChild (i);		
		}

		for (int i = 0; i < LootItems.Count; i++) {

			if (i == 9) {
				break;
			}
			GameObject tp_spawn = Instantiate (pre_Item) as GameObject;
			tp_spawn.transform.SetParent (tp_slots [i].transform, false);
			CreationManager.k_Manager.Items.Add (tp_spawn.transform);
			CreationManager.k_Manager.ChangeItemAttributes (LootItems[i], tp_spawn,tp_img);
		}
	}

	#endregion

	public void Press(){

		byte evCode = 0;    // my event 0. could be used as "group units"
		byte[] content = new byte[] { 1, 2, 5, 10 };    // e.g. selected unity 1,2,5 and 10
		bool reliable = true;

		RaiseEventOptions options = new RaiseEventOptions ();
		options.Receivers = ReceiverGroup.Others;
		PhotonNetwork.RaiseEvent(evCode, content, reliable, options);
	}

	public void ItemsBlockRaycast(bool tp_mode){
		foreach (Transform c in Items.ToArray()) {
			c.GetComponent<CanvasGroup> ().blocksRaycasts = tp_mode;
		}
	}

	public void Sell(){
	
		GameObject tp_ShopSlots = GameObject.FindGameObjectWithTag ("ShopSlots");
		Transform tp_Slot = null;
		GameObject tp_Item = null;

		for (int i = 0; i < tp_ShopSlots.transform.childCount; i++) {
		
			tp_Slot = tp_ShopSlots.transform.GetChild (i);
			if (tp_Slot.childCount > 0) {
			
				tp_Item = tp_Slot.transform.GetChild (0).gameObject;
				k_player.GetComponent<Player> ().Currency += tp_Item.GetComponent<ItemManager> ().appliedItem.Value;
				Items.Remove (tp_Item.transform);
				StartCoroutine (DeleteItem (tp_Item.GetComponent<ItemManager> ().appliedItem.ID));
				DestroyImmediate (tp_Item);			
			}
		}
		k_player.GetComponent<Player> ().UpdateCurrency ();

		GameObject tp_CurrencyText = GameObject.FindGameObjectWithTag ("Currency");

		tp_CurrencyText.GetComponent<Text> ().text = k_player.GetComponent<Player> ().Currency +"";

	}

	public void ResetPositions(){
	
		foreach (Character tp_char in k_player.GetComponent<Player>().Characters) {
			tp_char.Position = 16;
		}
		for (int i = 0; i < Positions.Length; i++) {
			if (Positions [i].transform.childCount > 0 ) {
				GameObject item = Positions [i].transform.GetChild (0).gameObject;
				if (item.name != "FriendPositionImage(Clone)") {
					if (item.GetComponent<DragHandler> ().Position_ID != currentCharacter.ID) {
						DestroyImmediate (item);
					} else {
						item.transform.SetParent (GameObject.FindGameObjectWithTag ("CharacterPositionSlot").transform);
					}
				}
			}
		}
	}

	public void AddFriendPosition(int Pos, int FriendID, int CharacterID){

		for (int i = 0; i < plantedFriends.Count; i++) {
			if (plantedFriends [i] [0] == FriendID && plantedFriends [i].Contains (CharacterID)) {
				GameObject[] Del = GameObject.FindGameObjectsWithTag ("FriendPosition");
				foreach(GameObject e in Del) {
					if (e.GetComponent<FriendPosition> ().ID == CharacterID)
						DestroyImmediate (e);
				}
				if (Pos < 16) {
					GameObject tp_spawn = Instantiate (pre_FriendPosition) as GameObject;
					tp_spawn.transform.SetParent (Positions [Pos].transform, false);
					tp_spawn.GetComponent<FriendPosition> ().ID = CharacterID;
				} else {
					plantedFriends [i].Remove (CharacterID);
				}
				return;
			}
		}

		if (Pos < 16) {
			GameObject tp_spawn = Instantiate (pre_FriendPosition) as GameObject;
			tp_spawn.transform.SetParent (Positions [Pos].transform, false);
			tp_spawn.GetComponent<FriendPosition> ().ID = CharacterID;
		
			int o = -1;
			for (int i = 0; i < plantedFriends.Count; i++) {
				if (plantedFriends [i] [0] == FriendID) {
					o = i;
					break;
				}
			}
			if (o < 0) {

				List<int> New = new List<int> ();
				New.Add (FriendID);
				New.Add (CharacterID);
				plantedFriends.Add (New);
		
			} else {
				plantedFriends [o].Add (CharacterID);
			}
		}
	}

	public void ApplySkillStats(){
	
		Skill s = currentSkill;
	
		SkillStats [1].GetComponent<Text> ().text = s.Name;
		SkillStats [2].GetComponent<Text> ().text = s.DamageType;
		SkillStats [3].GetComponent<Text> ().text = s.Modifier+"";

		Transform[] pos1 = new Transform[SkillStats [0].transform.GetChild (0).childCount];

		for (int i = 0; i < pos1.Length; i++) {

			pos1 [i] = SkillStats [0].transform.GetChild (0).GetChild (i);
		}
		print (pos1.Length);
		for (int i = 0; i < currentSkill.ActivateRow.Length; i++) {
			if (currentSkill.ActivateRow [i] != 0) {
				pos1 [3 - i].GetComponent<Image> ().color = new Color (0.0f, 1.0f, 0.0f, 1.0f);
				pos1 [7 - i].GetComponent<Image> ().color = new Color (0.0f, 1.0f, 0.0f, 1.0f);
				pos1 [11 - i].GetComponent<Image> ().color = new Color (0.0f, 1.0f, 0.0f, 1.0f);
				pos1 [15 - i].GetComponent<Image> ().color = new Color (0.0f, 1.0f, 0.0f, 1.0f);
			} else {
				pos1 [3 - i].GetComponent<Image> ().color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
				pos1 [7 - i].GetComponent<Image> ().color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
				pos1 [11 - i].GetComponent<Image> ().color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
				pos1 [15 - i].GetComponent<Image> ().color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
			}
		}

		if (currentSkill.TargetEnemy) {

			if (currentSkill.PositionalTarget.Length > 2) {
				Enemy4x4.SetActive (false);
				Enemy5x4.SetActive (true);
			} else {
				Enemy4x4.SetActive (true);
				Enemy5x4.SetActive (false);
			}

			Transform[] pos = new Transform[SkillStats [0].transform.GetChild (1).GetChild (1).childCount];

			for (int i = 0; i < pos.Length; i++) {
		
				pos [i] = SkillStats [0].transform.GetChild (1).GetChild (1).GetChild (i);
			}
			List<int> n = null;
			if (currentSkill.PositionalTarget.Length > 2) {
		
				int[] t = currentSkill.PositionalTarget;
				n = new List<int> (t);
				n.RemoveRange (0, 4);
				n.RemoveRange (n.Count - 5, 4);
			} else {
				int[] t = currentSkill.StaticTarget;
				n = new List<int> (t);
			}
			for (int i = 0; i < n.Count; i++) {
				if (n [i] > 0)
					pos [i].GetComponent<Image> ().color = new Color (1.0f, 0.0f, 0.0f, 1.0f);
				else
					pos [i].GetComponent<Image> ().color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
			}
		}
	}

	public void ShowRoom(){
		print(PhotonNetwork.room.customProperties["Crunchy"]);
	}
}
