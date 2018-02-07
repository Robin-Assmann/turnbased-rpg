using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class GameManager : MonoBehaviour {

	public List<Character> Enemy =  new List<Character>();
	public List<Character> Friendly =  new List<Character>();
	public List<int> CharacterOrder =  new List<int>();
	public List<Transform> GroupPositions = new List<Transform>();
	public List<Transform> EnemyPositions = new List<Transform>();
	public GameObject pre_Character, k_LeftBattlePanel, k_RightBattlePanel, k_TurnText, k_EndTurnButton, pre_Player, k_RetreatButton, k_EvenText;
	private Sprite[] k_Images;
	private Image[] k_OrderList;
	private bool k_TurnEnd, k_Counting;
	public bool TargetChosen, SkillWaiting, Applied, CharactersUp ,Loaded, Ordered, EnemiesLoaded, LevelLoaded, FriendsLoaded, SkillsLoaded, EnemySkillsLoaded;
	public int LoadedPlayers, PlayerCount, BattleID;
	public GameObject Target, pre_Text;
	public GameObject[] k_Buttons;
	public static GameManager k_Manager;
	public Coroutine k_CountingRoutine, UsingSkill, OldSkillUsed;
	private GameObject k_TimeText;
	public GameObject[] k_Spawns;
	public bool now, EnemiesUp, LootLoaded, CharacterSend;
	private int CharacterCount;
	private int Shift; // 
	private List<Coroutine> routines;
	public int SkillLoad, EnemySkillLoad, ExpGain, FriendsLoadedCount;
	public Character CurrentCharacter;
	public GameObject Description;

	private int[] MinMax, EnemyIDs;
	private string tp_characterString;

	private int enemycount, enemyloaded;
	private List<int> EnemyPosValues;

	private bool isMulti;

	[SerializeField] private GameObject k_EndPanel;

	#region Starting
	void Awake(){
		k_EndPanel.SetActive (false);
		routines = new List<Coroutine>();
		ExpGain = 0;
		Shift = 0;
		SkillLoad = 0;
		EnemySkillLoad = 0;
		FriendsLoadedCount = 0;
		PlayerCount = 0;
		LootLoaded = true;
		SkillsLoaded = false;
		EnemySkillsLoaded = false;
		enemycount = 0;
		enemyloaded = 0;
		LoadedPlayers = 0;
		CharacterCount = 0;
		LevelLoaded = false;
		FriendsLoaded = false;
		CharactersUp = false;
		EnemiesLoaded = false;
		EnemiesUp = false;
		Applied = false;
		Loaded = false;
		now = false;
		CharacterSend = false;
		k_EvenText = GameObject.FindGameObjectWithTag ("EventText");
		k_RetreatButton = GameObject.FindGameObjectWithTag ("RetreatButton");
		k_EndTurnButton = GameObject.FindGameObjectWithTag ("EndTurn");
		k_Buttons = GameObject.FindGameObjectsWithTag ("SkillButton");
		k_CountingRoutine = null;
		k_TimeText = GameObject.FindGameObjectWithTag ("TurnTime");
		k_Spawns = GameObject.FindGameObjectsWithTag ("Spawn");
		k_Counting = false;
		UsingSkill = null;
		OldSkillUsed = null;
		TargetChosen = false;
		SkillWaiting = false;

		k_Manager = this;
		k_Images = Resources.LoadAll<Sprite> ("Classes");

		Player.BattleLoaded = true;
	}

	void Start(){

		Loot.k_loot.Reset ();
		k_TurnEnd = false;

		Transform[] positions = k_LeftBattlePanel.GetComponent<CharacterFields>().AllPositions.ToArray();

		for(int i=0;i<positions.Length;i++){
			GroupPositions.Add (positions[i]);
		}
		positions = k_RightBattlePanel.GetComponent<CharacterFields>().AllPositions.ToArray();

		for(int i=0;i<positions.Length;i++){
			EnemyPositions.Add (positions[i]);
		}
		StartCoroutine (Delay());
	}

	//Keyboard Input
	void Update(){
	
		if (Input.GetKey (KeyCode.Q) && k_Buttons [0].GetComponent<Button> ().IsInteractable()) {
			k_Buttons [0].GetComponent<ButtonClass> ().ChangeColor ();
			k_Buttons [0].GetComponent<Button> ().onClick.Invoke();
		}
		if (Input.GetKey (KeyCode.W) && k_Buttons [1].GetComponent<Button> ().IsInteractable()) {
			k_Buttons [1].GetComponent<ButtonClass> ().ChangeColor ();
			k_Buttons [1].GetComponent<Button> ().onClick.Invoke();
		}
		if (Input.GetKey (KeyCode.E) && k_Buttons [2].GetComponent<Button> ().IsInteractable()) {
			k_Buttons [2].GetComponent<ButtonClass> ().ChangeColor ();
			k_Buttons [2].GetComponent<Button> ().onClick.Invoke();
		}
		if (Input.GetKey (KeyCode.R) && k_Buttons [3].GetComponent<Button> ().IsInteractable()) {
			k_Buttons [3].GetComponent<ButtonClass> ().ChangeColor ();
			k_Buttons [3].GetComponent<Button> ().onClick.Invoke();
			}
		if (Input.GetKeyDown (KeyCode.Space)) {
			EndTurn ();
		}

	}
	#endregion

	#region Player
	// Loads Characters of every Player into Player List
	void LoadPlayers(){
		GameObject[] tp_Players = GameObject.FindGameObjectsWithTag ("Player");

		for (int i = 0; i < tp_Players.Length; i++) {
			//add every Character in Players variable Characters to List Player
			foreach (Character tp_char in tp_Players[i].GetComponent<Player>().Characters) {
				//Updates all Positions
//				UpdateCharacters (tp_char);
				if(tp_char.Position!=16)	//check if regular Position
					Friendly.Add (tp_char);
			}
		}
	}
	#endregion

	#region Character

	//Connects Characters with their spawned objects Transform
	void PlaceCharacters(){
	
		for (int i =0; i < Friendly.Count; i++) {
			Friendly [i].Spawn = GroupPositions [Friendly[i].Position];
		}
		for (int i =0; i < Enemy.Count; i++) {
			Enemy [i].Spawn = EnemyPositions [Enemy[i].Position];
		}
	}

	//Spawns Objects and connects their Characterclass to their Character
	void SpawnCharacters(List<Character> tp_CharacterList){

		for (int i =0; i < tp_CharacterList.Count; i++) {
			if (pre_Character != null) {

				tp_CharacterList [i].UpdateExperience ();

				InitializeStarting (tp_CharacterList [i]);
				GameObject tp_CharacterObject = Instantiate (pre_Character) as GameObject;
				tp_CharacterObject.transform.SetParent(tp_CharacterList [i].Spawn, false);

				tp_CharacterObject.AddComponent<CharacterClass> ();
				tp_CharacterObject.transform.GetComponent<CharacterClass> ().k_Character = tp_CharacterList [i];

				tp_CharacterList [i].Char = tp_CharacterObject;
			}
		}
	}

	void InitializeStarting(Character tp_char){
	
		tp_char.Starting_Health = tp_char.MaxHealth;
		tp_char.Starting_Armor = tp_char.Armor;
		tp_char.Starting_MagicDamage = tp_char.MagicDamage;
		tp_char.Starting_MinDPS = tp_char.MinDPS;
		tp_char.Starting_MaxDPS = tp_char.MaxDPS;
		tp_char.Starting_Speed = tp_char.Speed;
		tp_char.Starting_Dodge = tp_char.Dodge;

		tp_char.Starting_FireResistance = tp_char.FireResistance;
		tp_char.Starting_IceResistance = tp_char.IceResistance;
		tp_char.Starting_LightningResistance = tp_char.LightningResistance;
	
	}

	//Gives Spawns Images
	void InitializeCharacters(List<Character> tp_CharacterList){

		string tp_type;
	
		for (int i =0; i < tp_CharacterList.Count; i++) {

			tp_type = tp_CharacterList [i].Type;

			switch (tp_type) {

			case "Warrior":
				tp_CharacterList [i].Char.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<SpriteRenderer> ().sprite = k_Images [0];
				break;
			case "Priest":
				tp_CharacterList [i].Char.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<SpriteRenderer> ().sprite = k_Images [1];
				break;
			case "Archer":
				tp_CharacterList [i].Char.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<SpriteRenderer> ().sprite = k_Images [2];
				break;
			}
		}
	}

	void SendSimpleCharacters(){

		string[] simpleChars = new string[Friendly.Count+1];

		for (int i = 0; i < simpleChars.Length-1; i++) {
			simpleChars [i] = Friendly [i].MakeSimpleString ();
		}
		simpleChars [Friendly.Count] = this.BattleID+"";
	
		RaiseEventOptions options = new RaiseEventOptions ();

		if (Group.k_Group.GroupIDs.ToArray ().Length != 0) {
			options.TargetActors = Group.k_Group.GroupIDs.ToArray ();
			PhotonNetwork.RaiseEvent (52, simpleChars, true, options);
		}


		string[] ids = new string[Friendly.Count + 2];
		for (int i = 0; i < ids.Length-2; i++) {
			ids [i] = Friendly [i].ID+"";
		}
		ids [Friendly.Count] = this.BattleID+"";
		int id = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player> ().PlayerID;
		ids [Friendly.Count+1] = id+"";

		options = new RaiseEventOptions ();
		options.Receivers = ReceiverGroup.MasterClient;
		PhotonNetwork.RaiseEvent(52, ids, true, options);
	}

	public void ChangeHealth(int ID, int dmg){

		Character target = null;
		if (ID < 0) {
			foreach (Character c in Enemy.ToArray()) {
				if (c.ID == ID) {
					target = c;
				}			
			}
		} else {
			foreach (Character c in Friendly.ToArray()) {
				if (c.ID == ID) {
					target = c;
				}			
			}
		}

		target.Health += dmg;

		if(dmg<0)
			target.Char.gameObject.GetComponent<CharacterClass>().MakeText (""+ dmg, new Color (255, 0, 0));
		else
			target.Char.gameObject.GetComponent<CharacterClass>().MakeText ("+ " + dmg, new Color (0, 255, 0));
	}

	public void MakeBattleText(int ID, int mode){

		Character target = null;
		if (ID < 0) {
			foreach (Character c in Enemy.ToArray()) {
				if (c.ID == ID) {
					target = c;
				}			
			}
		} else {
			foreach (Character c in Friendly.ToArray()) {
				if (c.ID == ID) {
					target = c;
				}			
			}
		}

		switch (mode) {

		case 0:
			target.Char.gameObject.GetComponent<CharacterClass> ().MakeText ("Dodged", new Color (255, 255, 0));
			break;
		default:
			target.Char.gameObject.GetComponent<CharacterClass> ().MakeText ("Error", new Color (255, 255, 0));
			break;
		}
	}

	public void ShowBattleFields(int[] Fields){

		List<Transform> fields = new List<Transform>();
		if (Fields [Fields.Length-1] == 0) {
			fields = GroupPositions;
		} else {
			fields = EnemyPositions;
		}
	
		for (int i = 0; i < Fields.Length - 3; i++) {
		
			if (Fields [i] == 1) {
				if (i == Fields [Fields.Length - 2]) {
					fields [i].GetChild(0).GetComponent<CharacterTrigger> ().ChangeColor (1.0f, 0.0f, 0.0f, 0.6f);
					continue;
				}
				fields [i].GetChild(0).GetComponent<CharacterTrigger> ().ChangeColor (0.0f, 0.0f, 0.0f, 0.1f);
			}
		}
	}


	#endregion

	#region Order 'n' Battle
	void InitializeOrder() {
		GameObject Order = GameObject.FindGameObjectWithTag ("BattleOrder");
		k_OrderList = Order.GetComponent<BattleOrder> ().k_OrderList;
	}

	void InitializeBattle(bool tp_bool){
		Ordered = false;
		GameObject player = GameObject.FindGameObjectWithTag ("Player");

		if (player.GetComponent<Player> ().isStarter) {
			player.GetComponent<Player> ().isStarter = false;
			RaiseEventOptions options = new RaiseEventOptions ();
			options.Receivers = ReceiverGroup.MasterClient;
			PhotonNetwork.RaiseEvent (55, new string[]{ tp_bool + "", BattleID + "" }, true, options);
		}
	}

	public void MoveOrder(string[] Ids){

		if (CharacterOrder.Count > 0) {
			CharacterOrder.RemoveRange (0, CharacterOrder.Count);
		}
		for (int i = 0; i < Ids.Length; i++) {
			CharacterOrder.Add (int.Parse (Ids [i]));
		}
		for (int i = 0; i < 6; i++) {
			if (CharacterOrder [i]< 0) {
				k_OrderList [i].color = new Color (50, 0, 0,0.4f);
			} else {
				k_OrderList [i].color = new Color (0, 0.1f*CharacterOrder [i], 0.1f*CharacterOrder [i],1.0f);
			}
		}
		Ordered = true;
	}
	#endregion

	//Battle Setup
	IEnumerator Delay(){

		LoadPlayers ();
		//Server created Enemies
		yield return new WaitUntil(()=>EnemiesLoaded);
		print ("enemiesloaded");
		SendSimpleCharacters ();

		if(Group.k_Group.GroupIDs.Count>0)
		yield return new WaitUntil(()=>FriendsLoaded);

		PlaceCharacters ();
		SpawnCharacters (Friendly);
		SpawnCharacters (Enemy);

		InitializeCharacters (Friendly);
		InitializeCharacters (Enemy);

		InitializeOrder ();
		InitializeBattle (true);
		CharactersUp = true;
		EnemiesUp = true;

		yield return new WaitUntil (() => Applied);
		yield return new WaitUntil (() => SkillsLoaded);

		RaiseEventOptions opt = new RaiseEventOptions ();
		opt.Receivers = ReceiverGroup.MasterClient;
		PhotonNetwork.RaiseEvent (49, BattleID+"", true, opt);
	}

	#region Turn/Counting
	public void StartTurn(string[] ids){
		MoveOrder (ids);
		int ID = CharacterOrder[0];
		if (ID < 0) {
			foreach (Character c in Enemy.ToArray()) {
				if (c.ID == ID) {
					CurrentCharacter = c;
					break;
				}
			}
		} else {
			foreach (Character c in Friendly.ToArray()) {
				if (c.ID == ID) {
					CurrentCharacter = c;
					break;
				}
			}
		}
		k_EndTurnButton.transform.GetComponent<Button> ().interactable = false;

		if (k_CountingRoutine != null)
			StopCoroutine (k_CountingRoutine);
			
		k_CountingRoutine = StartCoroutine (Count ());

		if (CurrentCharacter.isEnemy) {
			k_TurnText.GetComponent<Text> ().text = "Wait for Enemy turn ...";

		} else {
			if(!CurrentCharacter.isSimple)
				k_TurnText.GetComponent<Text> ().text = "Your turn ...";
			else
				k_TurnText.GetComponent<Text> ().text = "Your Teammate's turn ...";
		}

		CurrentCharacter.Char.transform.parent.GetChild (0).GetComponent<CharacterTrigger> ().TriggerHighlight (true);
		CurrentCharacter.Char.transform.parent.GetChild (0).GetComponent<CharacterTrigger> ().TriggerStarted ();

		for (int i = 0; i < 4; i++) {
			k_Buttons [i].GetComponent<ButtonClass> ().ChangeCharacter ();
		}
		SetClickable ();

		CurrentCharacter.Char.transform.GetComponent<CharacterClass> ().TurnedOn ();
	}

	public void WantToEndTurn(){
		RaiseEventOptions opt = new RaiseEventOptions ();
		opt.Receivers = ReceiverGroup.MasterClient;
		PhotonNetwork.RaiseEvent (57, BattleID+"", true, opt);
	}

	//Avoid Problems when Time runs out - Divide FinishTurn and EndTurn
	public void FinishTurn(){
	
		//Turn ends
		CurrentCharacter.Char.transform.parent.GetChild(0).GetComponent<CharacterTrigger> ().TriggerHighlight (false);
		CurrentCharacter.Char.transform.parent.GetChild (0).GetComponent<CharacterTrigger> ().TriggerStarted ();

		EndTurn ();
	}

	IEnumerator Count(){

		float tp_time = 20.0f;
		while (tp_time > 0) {
			yield return new WaitForSeconds (0.01f);
			tp_time -= 0.01f;
			k_TimeText.GetComponent<Text> ().text = tp_time.ToString("F2") + " sec";
		}
		EndTurn ();
	}
	#endregion

	#region Functions (End SetClickable)
	public void EndTurn(){
		SkillWaiting = false;
		TargetChosen = false;

		RaiseEventOptions opt = new RaiseEventOptions ();
		opt.Receivers = ReceiverGroup.MasterClient;
		PhotonNetwork.RaiseEvent (49, BattleID+"", true, opt);
	}
		
	public void SetClickable(){
		foreach (GameObject trans in k_Spawns) {
			trans.transform.GetChild (0).GetComponent<CharacterTrigger> ().clickable = false;
		}
	}

	public void RemoveCharacters(int[] DeadChars){

		int ID = 0;
		for (int i = 0; i < DeadChars.Length; i++) {
		
			ID = DeadChars [i];

			if (ID < 0) {
				foreach (Character chara in Enemy.ToArray()) {
					if (chara.ID == ID) {
						DestroyImmediate (chara.Char);
						Enemy.Remove (chara);
					}
				}
			} else {
				foreach (Character chara in Friendly.ToArray()) {
					if (chara.ID == ID) {
						DestroyImmediate (chara.Char);
						Friendly.Remove (chara);
					}
				}
			}
		}
			
	}

	public void PlaceCharacter(int ID, int Position){

		Character b = null;
		foreach (Character c in Friendly.ToArray()) {
			if (c.ID == ID) {
				b = c;
				break;
			}
		}
		Transform parent = GameManager.k_Manager.k_LeftBattlePanel.GetComponent<CharacterFields> ().AllPositions[Position];

		b.Char.transform.SetParent (parent);
		b.Char.transform.localPosition = new Vector3 (0, 0.75f, 0);
		CharacterFields.k_CharacterFields.Start ();
	}

	public void EndBattle(){
	
		foreach (Coroutine c in routines.ToArray()) {
			StopCoroutine (c);
		}
		StopCoroutine (k_CountingRoutine);
		k_EndPanel.SetActive (true);
		Player.BattleLoaded = false;
	}

	public void LoadScene(int i){

		SceneManager.LoadScene (i);
	}

	public void Retreat(){

		k_RetreatButton.transform.GetComponent<Button> ().interactable = false;

		if (Random.value > 0.99f) {
			foreach (Coroutine c in routines.ToArray()) {
				StopCoroutine (c);
			}
			StopCoroutine (k_CountingRoutine);
//			UpdateCharactersStart ();
			SceneManager.LoadScene (2);
		} else {
			StartCoroutine (MakeEventText ("Retreat Failed"));
		}
	}

	IEnumerator MakeEventText(string tp_msg){

		k_EvenText.GetComponent<Text> ().text = tp_msg;

		yield return new WaitForSeconds (1.0f);

		k_EvenText.GetComponent<Text> ().text = "";
	}

	#endregion // EndTurn /SetClickable // UpdateCharacters / CheckDead / EndBattle



}
