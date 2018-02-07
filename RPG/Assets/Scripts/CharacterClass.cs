using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class CharacterClass : MonoBehaviour {

	public Character k_Character { get; set;}
	public GameObject k_CharacterObject { get; set;}
	public int[] k_Position; // Row;Col
	public int Health;
	private string k_CharacterType;
	private Slider k_HealthSlider;
	private Skill[] Skills;
	public List<Skill> AvailableSkills;
	//private Sprite[] k_SkillImages; //unefficent
	private bool isClicked;
	public GameObject pre_Text;
	private GameObject k_CharacterPanel;
	private int SkillUsedEnemy;

	#region Setup
	void Awake(){
		//k_SkillImages = Resources.LoadAll<Sprite> ("Skills");
		pre_Text = GameManager.k_Manager.pre_Text;

		AvailableSkills = new List<Skill>();

		k_Position = new int[2];
		Skills = new Skill[4];

		k_HealthSlider = transform.FindChild ("Canvas").FindChild ("Slider").transform.GetComponent<Slider> ();
	}

	void Start () {

		k_CharacterPanel = GameObject.FindGameObjectWithTag ("CharacterPanel");
		k_CharacterObject = k_Character.Char;
		k_CharacterType = k_Character.Type;
		int i = k_Character.MaxHealth;
		k_HealthSlider.maxValue = i;
		k_HealthSlider.value = k_Character.Health;

		if (!k_Character.isSimple) {
			if (k_Character.isEnemy) {
				StartCoroutine (LoadEnemySkills ());
			} else {
		
				StartCoroutine (LoadSkills ());
			}
		}
	}

	void Update(){
	
		Health = k_Character.Health;
		k_HealthSlider.value = Health;
	}

	#endregion // Awake / Start /Update
		
	#region Skill Setup
		
	IEnumerator LoadSkills(){

		string sql_text = "SELECT * FROM " + k_Character.Type + "_skills WHERE skill_id IN('" + k_Character.SkillArray [0] + "','" + k_Character.SkillArray [1] + "','" + k_Character.SkillArray [2] + "','" + k_Character.SkillArray [3] + "');";
		string tp_skillstring = "";
		string[] tp_skillarray = new string[4];
		if (!PhotonNetwork.isNonMasterClientInRoom) {
			WWWForm form = new WWWForm ();
			form.AddField ("sql_text_post", sql_text);
			WWW itemsdata = new WWW ("https://localhost/RPG/GetSkills.php", form);
			yield return itemsdata;
			tp_skillstring = itemsdata.text;
			tp_skillarray = tp_skillstring.Split(";"[0]);
		} else {
			int pos = int.Parse(OnlineData.k_storage.tp_dataSkills [0]);
			OnlineData.k_storage.tp_dataSkills [0] = (pos +5) +"";

			string skillar = "";
			for (int i = 0; i < k_Character.SkillArray.Length; i++) {
			
				if (i == k_Character.SkillArray.Length - 1) {
					skillar = skillar + k_Character.SkillArray [i];
					break;
				}

				skillar = skillar + k_Character.SkillArray [i]+"-";
			}


			string[] content = new string[] {pos+"", k_Character.Type,skillar};
			RaiseEventOptions opt = new RaiseEventOptions ();
			opt.Receivers = ReceiverGroup.MasterClient;
			PhotonNetwork.RaiseEvent (12, content, false, opt);

			string starting = OnlineData.k_storage.tp_dataSkills [pos];
			yield return new WaitUntil (() => (!starting.Equals (OnlineData.k_storage.tp_dataSkills [pos])));

			tp_skillstring = OnlineData.k_storage.tp_dataSkills [pos];
			print ("Skills are "+ tp_skillstring);
			OnlineData.k_storage.DataReceived = false;

			List<string> tp_list = new List<string> ();
			for(int i=0;i<4;i++){
				try{
					if(OnlineData.k_storage.tp_dataSkills [pos + i]!="")
						tp_list.Add(OnlineData.k_storage.tp_dataSkills [pos + i]);
				}catch{
					break;
				}
			}
			tp_list.Add ("1");
			tp_skillarray = tp_list.ToArray ();
		}
		for (int i = 0; i < tp_skillarray.Length -1; i++) {
			string[] tp_new = tp_skillarray [i].Split ("|" [0]);
			print (i);

			//ID + "|" + Name + "|" + Modifier + "|" + TargetMode + "|" + SkillType + "|" + Target_Activate + "|" + Target + "|" + Target_Individual + "|" + this.Image + "|" + TargetEnemy + "|" + TurnCount + "|" + Dodgeable + "|" + BuffCount + "|" + Buffstring +";";
			AddAvailableSkills(int.Parse (tp_new [0]),tp_new [1],float.Parse (tp_new [2]),int.Parse (tp_new [3]),int.Parse (tp_new [4]),tp_new[5],tp_new [6],tp_new [7],int.Parse (tp_new [8]),tp_new [9],int.Parse(tp_new [10]),tp_new [11],int.Parse(tp_new [12]),tp_new [13]);
		}
		ApplySkills ();
	}

	IEnumerator LoadEnemySkills(){
		string sql_text = "SELECT * FROM enemy_skills WHERE skill_id IN('" + k_Character.SkillArray [0] + "','" + k_Character.SkillArray [1] + "','"+ k_Character.SkillArray [2] +"','"+ k_Character.SkillArray [3] +"');";
		string tp_skillstring = "";
		string[] tp_skillarray = new string[4];
		if (!PhotonNetwork.isNonMasterClientInRoom) {
			WWWForm form = new WWWForm ();
			form.AddField ("sql_text_post", sql_text);
			WWW itemsdata = new WWW ("https://localhost/RPG/GetSkills.php", form);
			yield return itemsdata;
			tp_skillstring = itemsdata.text;
			tp_skillarray = tp_skillstring.Split(";"[0]);
		} else {
			int pos = int.Parse(OnlineData.k_storage.tp_dataEnemySkills [0]);
			OnlineData.k_storage.tp_dataEnemySkills [0] = (pos +5) +"";
			string[] content = new string[] {pos+"", sql_text};
			RaiseEventOptions opt = new RaiseEventOptions ();
			opt.Receivers = ReceiverGroup.MasterClient;
			PhotonNetwork.RaiseEvent (13, content, false, opt);

			string starting = OnlineData.k_storage.tp_dataEnemySkills [pos];
			yield return new WaitUntil (() => (!starting.Equals (OnlineData.k_storage.tp_dataEnemySkills [pos])));

			tp_skillstring = OnlineData.k_storage.tp_dataEnemySkills [pos];
			OnlineData.k_storage.DataReceived = false;

			List<string> tp_list = new List<string> ();
			for(int i=0;i<4;i++){
				try{
					if(OnlineData.k_storage.tp_dataEnemySkills [pos + i]!="")
						tp_list.Add(OnlineData.k_storage.tp_dataEnemySkills [pos + i]);
				}catch{
					break;
				}
			}
			tp_list.Add ("1");
			tp_skillarray = tp_list.ToArray ();
		}

		for (int i = 0; i < tp_skillarray.Length -1; i++) {
			string[] tp_new = tp_skillarray [i].Split ("|" [0]);
			AddAvailableSkills(int.Parse (tp_new [0]),tp_new [1],float.Parse (tp_new [2]),int.Parse (tp_new [3]),int.Parse (tp_new [4]),tp_new[5],tp_new [6],tp_new [7],int.Parse (tp_new [8]),tp_new [9],int.Parse(tp_new [10]),tp_new [11],int.Parse(tp_new [12]),tp_new [13]);
		}
		ApplySkills ();
	}

	public void AddAvailableSkills(int tp_id, string tp_name, float tp_mod, int tp_mode, int tp_type, string tp_activate, string tp_target, string tp_individual, int tp_image, string tp_enemy,int tp_turn, string tp_dodge, int tp_buffcount, string tp_buffstring){

//		bool enemy = false;
//		if (tp_enemy == 1)
//			enemy = true;
//		bool dodge = false;
//		if (tp_dodge == 1)
//			dodge = true;

		bool enemy = (tp_enemy.Equals("True"));
		bool dodge = (tp_dodge.Equals("True"));

		AvailableSkills.Add (new Skill (tp_id, tp_name, tp_mod, tp_mode, tp_type, tp_activate, tp_target, tp_individual, OnlineData.k_storage.k_Skillimages[tp_image], enemy,tp_turn, dodge, tp_buffcount, tp_buffstring));
	}

	public void ApplySkills(){
	
		for (int i = 0; i < Skills.Length; i++) {
			for (int j = 0; j < k_Character.AvailableSkills.Count; j++) {
			
				if (k_Character.SkillArray [i] == k_Character.AvailableSkills [j].ID) {
					Skills [i] = k_Character.AvailableSkills [j];
				}
			}
		}
		GameManager.k_Manager.SkillLoad++;

		int u = 0;
		for (int i = 0; i < GameManager.k_Manager.Friendly.Count; i++) {
			if (!GameManager.k_Manager.Friendly [i].isSimple)
				u++;
		}

		if (GameManager.k_Manager.SkillLoad >= u)
				GameManager.k_Manager.SkillsLoaded = true;
//		} else {
//		
//			GameManager.k_Manager.EnemySkillLoad++;
//			if (GameManager.k_Manager.EnemySkillLoad >= GameManager.k_Manager.Enemy.Count)
//				GameManager.k_Manager.EnemySkillsLoaded = true;
//		}
	}

	#endregion

	#region TurnedOn
	public void TurnedOn(){


			try {
				bool tp_action = false;

				for (int i = 0; i < k_Character.ActionList.Length; i++) {

					if (k_Character.ActionList [i].appliedSkill != null) {
						tp_action = true;
						break;
					}
				}
				if (tp_action) {
					bool skip = false;
					for (int i = 0; i < k_Character.ActionList.Length; i++) {

						k_Character.ActionList [i].TurnCountCurrent++;
						//Checking if Skill should be used or not /Dot =0 every round /Buff,Debuff = after 3 rounds
						if (k_Character.ActionList [i].TurnCount < k_Character.ActionList [i].TurnCountCurrent) {
							k_Character.ActionList [i].appliedSkill.UseSkill (false, k_Character.ActionList [i].BuffCounter, k_Character.ActionList [i].appliedCharacter); //true or false no difference
						}
						if (k_Character.ActionList [i].ActionType == 4) {
							skip = true;
						}
						if (k_Character.ActionList [i].TurnCountCurrent > k_Character.ActionList [i].DeathCount) {
							k_Character.ActionList [i] = new Action (0, 0, null, 0, 0, null);
						}
					}
				if (skip) {
						GameManager.k_Manager.EndTurn ();
					}
					return;
				}
			} catch {
			}

		if (!k_Character.isEnemy && !k_Character.isSimple) {

			ChangeText ("ArmorNumber", k_Character.Armor + "", AttributeChanged (k_Character.Armor, k_Character.Starting_Armor));
			ChangeText ("HealthNumber", k_Character.Health + "", 0);
			ChangeText ("DamageNumber", k_Character.MinDPS + " - " + k_Character.MaxDPS, AttributeChanged (k_Character.MinDPS, k_Character.Starting_MinDPS));
			ChangeText ("SpeedNumber", k_Character.Speed + "", AttributeChanged (k_Character.Speed, k_Character.Starting_Speed));
			ChangeText ("FireResistanceNumber", k_Character.FireResistance + "", AttributeChanged (k_Character.FireResistance, k_Character.Starting_FireResistance));
			ChangeText ("IceResistanceNumber", k_Character.IceResistance + "", AttributeChanged (k_Character.IceResistance, k_Character.Starting_IceResistance));
			ChangeText ("ShockResistanceNumber", k_Character.LightningResistance + "", AttributeChanged (k_Character.LightningResistance, k_Character.Starting_LightningResistance));
	
			GameObject[] tp_buttons = GameObject.FindGameObjectsWithTag ("SkillButton");
			tp_buttons [0].GetComponent<ButtonClass> ().ChangeAllBack ();

			GameManager.k_Manager.k_RetreatButton.transform.GetComponent<Button> ().interactable = true;
			GameManager.k_Manager.k_EndTurnButton.transform.GetComponent<Button> ().interactable = true;
		
			for (int i = 0; i < 4; i++) {
				Sprite tp_sprite = Skills [i].SkillImage;
				tp_buttons [i].transform.GetChild (0).GetComponent<Image> ().sprite = tp_sprite;
				int o = i;
				tp_buttons [i].transform.GetComponent<ButtonClass> ().appliedSkill = Skills [i];
//				tp_buttons [i].transform.GetComponent<ButtonClass> ().appliedSkill.k_relatedButton = tp_buttons [i];
				tp_buttons [i].GetComponent<ButtonClass> ().Clicked = false;
				tp_buttons [i].GetComponent<ButtonClass> ().Changed = true;
				tp_buttons [i].GetComponent<Button> ().onClick.RemoveAllListeners ();
				tp_buttons [i].GetComponent<Button> ().onClick.AddListener (() => {
					if(GameManager.k_Manager.UsingSkill==null){
						GameManager.k_Manager.UsingSkill = StartCoroutine (UseSkill (tp_buttons [o].transform, Skills[o]));}
					else{
						GameObject tp_movebutton = GameObject.FindGameObjectWithTag("MovementButton");
						tp_movebutton.GetComponent<Button>().interactable = true;

						GameManager.k_Manager.SetClickable();
						GameManager.k_Manager.OldSkillUsed = GameManager.k_Manager.UsingSkill;
						GameManager.k_Manager.UsingSkill = StartCoroutine (UseSkill (tp_buttons [o].transform, Skills[o]));
					}
				});

				SetButtonsInteractable (tp_buttons [i], i);
			}
			GameObject tp_mov = GameObject.FindGameObjectWithTag ("MovementButton");
			tp_mov.GetComponent<ButtonClass> ().isMovement = true;
			tp_mov.transform.GetComponent<ButtonClass> ().InitializeField ();
			tp_mov.transform.GetComponent<Button> ().interactable = true;
	
		} else {
			
			GameObject[] tp_buttons = GameObject.FindGameObjectsWithTag ("SkillButton");
			tp_buttons [0].GetComponent<ButtonClass> ().ChangeAllBack ();

			for (int i = 0; i < tp_buttons.Length; i++) {
				tp_buttons [i].GetComponent<Button> ().interactable = false;
			}
			GameManager.k_Manager.k_EndTurnButton.transform.GetComponent<Button> ().interactable = false;
			GameManager.k_Manager.k_RetreatButton.transform.GetComponent<Button> ().interactable = false;
			GameObject tp_mov = GameObject.FindGameObjectWithTag ("MovementButton");
			tp_mov.transform.GetComponent<Button> ().interactable = false;
		}
	}

	public void SetButtonsInteractable(GameObject tp_button, int i){

		tp_button.GetComponent<Button> ().interactable = true;

		bool tp_check = CheckSkill (i);
		if (!tp_check) {
			tp_button.GetComponent<Button> ().interactable = false;
		}

		tp_button.GetComponent<ButtonClass> ().InitializeField ();


		if (!CheckTarget (tp_button) && tp_check) {
			tp_button.GetComponent<Button> ().interactable = false;
		}
	}

	IEnumerator Delaying(){
	
		yield return new WaitForSeconds (0.01f);
		GameManager.k_Manager.EndTurn ();
	}

	#endregion

	#region Check
	public bool CheckSkill(int i){
		Skill tp_skill = Skills [i];
		for (int j = 0; j < 4; j++) {
			if(k_Position [1] == tp_skill.ActivateRow[j]){
				tp_skill.Usable = true;
				return true;
			}
		}
		tp_skill.Usable = false;
		return false;
	}

	public bool CheckTarget(GameObject tp_button){
		foreach (Transform trans in tp_button.GetComponent<ButtonClass>().tp_list.ToArray()) {
			if (trans.FindChild ("Character(Clone)") != null) {
				return true;
			}
		}
		return false;
	}
	public int AttributeChanged(int tp_current, int tp_starting){

		if (tp_current == tp_starting) {
			return 0;
		}

		if (tp_current > tp_starting) {
			return 1;
		} else {
			return 2;
		}
	}

	#endregion //CheckSkills / CheckTarget / AttributeChanged

	#region ChangeUse

	public void ChangeText(string tp_string, string tp_value, int tp_buffed){

		k_CharacterPanel.transform.FindChild (tp_string).GetComponent<Text> ().text = tp_value;

		switch (tp_buffed) {
		case 0:
			k_CharacterPanel.transform.FindChild (tp_string).GetComponent<Text> ().color = new Color (1.0f, 1.0f, 1.0f);
			break;
		case 1:
			k_CharacterPanel.transform.FindChild (tp_string).GetComponent<Text> ().color = new Color (0.0f, 1.0f, 0.0f);
			break;
		case 2:
			k_CharacterPanel.transform.FindChild (tp_string).GetComponent<Text> ().color = new Color (1.0f, 0.0f, 0.0f);
			break;
		}
	}

	IEnumerator UseSkill(Transform tp_button, Skill tp_skill){

		tp_button.GetComponent<Button> ().interactable = false;

		tp_button.GetComponent<ButtonClass>().ChangeColor();
		if (!GameManager.k_Manager.SkillWaiting ) {
			
			GameManager.k_Manager.SkillWaiting = true;
		} else {
			StopCoroutine (GameManager.k_Manager.OldSkillUsed);
			GameObject[] tp_buttons = GameObject.FindGameObjectsWithTag ("SkillButton");
			List<GameObject> tp_list = new List<GameObject> (tp_buttons);
			tp_list.Remove (tp_button.gameObject);
			tp_buttons = tp_list.ToArray ();

			for (int i = 0; i < tp_buttons.Length; i++) {
				SetButtonsInteractable (tp_buttons [i], i);
			}
		}
		tp_button.GetComponent<ButtonClass>().Clicked = true;
		if (tp_skill.TargetMode == 0) {
			foreach (Transform trans in tp_button.GetComponent<ButtonClass>().tp_list.ToArray()) {
				if (trans.FindChild ("Character(Clone)") != null) {
					trans.FindChild ("CharacterSpawn").GetComponent<CharacterTrigger> ().clickable = true;
				}
			}
		}
		if (tp_skill.TargetMode == 2) {
			foreach (Transform trans in tp_button.GetComponent<ButtonClass>().tp_list.ToArray()) {
				trans.FindChild ("CharacterSpawn").GetComponent<CharacterTrigger> ().clickable = true;
			}
		}

		if (k_Character.isEnemy)
			yield return new WaitForSeconds (1.0f);

		yield return new WaitUntil (() => GameManager.k_Manager.TargetChosen);

		tp_button.GetComponent<ButtonClass> ().Clicked = false;
		tp_button.GetComponent<ButtonClass>().ChangeColorBack();
		tp_button.GetComponent<Button> ().interactable = true;
		tp_skill.UseSkill (true, 0, null);
//		GameManager.k_Manager.CheckDeadCharacters ();
		GameManager.k_Manager.Target = null;
		GameManager.k_Manager.WantToEndTurn ();
	}

	public void MakeText(string st, Color tp_color){
	
		GameObject tp_text = Instantiate (pre_Text) as GameObject;

		tp_text.transform.SetParent (transform.GetChild (1).GetChild (1), false);
		tp_text.GetComponent<Text> ().text = st;
		tp_text.GetComponent<Text> ().color = tp_color;
	}

	#endregion //ChangeText / UseSkill / Maketext
}
