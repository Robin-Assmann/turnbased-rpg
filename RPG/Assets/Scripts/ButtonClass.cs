using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ButtonClass : MonoBehaviour {

	public Skill appliedSkill { get; set;}
	public Skill StartSkill { get; set;}
	public List<Transform> tp_list { get; set;}
	public bool Clicked { get; set;}
	public bool Changed { get; set;}
	public bool isMovement { get; set;}
	private Coroutine currentCorroutine;
	private Character tp_character;
	private CharacterClass tp_characterclass;
	private CharacterFields LeftBattlePanel, RightBattlePanel;

	void Awake(){
		isMovement = false;
		Changed = false;
		Clicked = false;
	}

	void Start(){
		//StartCoroutine (GetTargetFields ());
		LeftBattlePanel = GameManager.k_Manager.k_LeftBattlePanel.GetComponent<CharacterFields> ();
		RightBattlePanel = GameManager.k_Manager.k_RightBattlePanel.GetComponent<CharacterFields> ();
	}

	public void ChangeCharacter(){
		tp_character = GameManager.k_Manager.CurrentCharacter;
		tp_characterclass = tp_character.Char.GetComponent<CharacterClass> ();
	}

	#region Colors
	public void ChangeColor(){
		try{
			if ((Clicked || GameManager.k_Manager.SkillWaiting || GameManager.k_Manager.CurrentCharacter.isEnemy || GameManager.k_Manager.CurrentCharacter.isSimple)) {
			return;
		}
		if (isMovement || !appliedSkill.Usable) {
			foreach (Transform trans in tp_list.ToArray()) {
					trans.GetChild (0).GetComponent<CharacterTrigger> ().ChangeColor (0.0f, 1.0f, 0.0f, 1.0f);
			}
		} else {
			foreach (Transform trans in tp_list.ToArray()) {
					trans.GetChild (0).GetComponent<CharacterTrigger> ().ChangeColor (1.0f, 0.0f, 0.0f, 0.5f);
			}
		}
		}catch{
		}
	}

	public void ChangeColorBack(){

		if ((Clicked || GameManager.k_Manager.SkillWaiting || GameManager.k_Manager.CurrentCharacter.isEnemy || GameManager.k_Manager.CurrentCharacter.isSimple)) {
			return;
		}

		ChangeAllBack ();
//
//		if (GameManager.k_Manager.GroupPositions.Contains (GameManager.k_Manager.CharacterOrder [0].Char.transform.parent)) {
//			foreach (Transform trans in GameManager.k_Manager.EnemyPositions.ToArray()) {
//				trans.GetChild (0).GetComponent<CharacterTrigger> ().ChangeColorBack();
//			}
//		} else {
//			foreach (Transform trans in GameManager.k_Manager.GroupPositions.ToArray()) {
//				trans.GetChild (0).GetComponent<CharacterTrigger> ().ChangeColorBack ();
//			}
//		}
	}

	//Change All Fields except current back to Normal
	public void ChangeAllBack(){
	
		foreach (Transform trans in GameManager.k_Manager.EnemyPositions.ToArray()) {
			if(trans!= GameManager.k_Manager.CurrentCharacter.Char.transform.parent)
			trans.GetChild (0).GetComponent<CharacterTrigger> ().ChangeColorBack();
		}
	
		foreach (Transform trans in GameManager.k_Manager.GroupPositions.ToArray()) {
			if(trans!= GameManager.k_Manager.CurrentCharacter.Char.transform.parent)
			trans.GetChild (0).GetComponent<CharacterTrigger> ().ChangeColorBack ();
		}
	}
	#endregion

	#region Description
	public void ShowText(){

		Transform t = GameManager.k_Manager.Description.transform;
		Vector2 pos = transform.position;
		t.GetChild (0).GetComponent<Text> ().text = appliedSkill.Name;
		t.GetChild (1).GetComponent<Text> ().text = appliedSkill.Description;
		if (pos.y + GetComponent<RectTransform> ().rect.height / 2 + 5 < 450)
			t.position = new Vector2 (pos.x, pos.y + GetComponent<RectTransform> ().rect.height / 2 + 5);
		else{
			t.position = new Vector2 (pos.x-t.GetComponent<RectTransform>().rect.width/2, pos.y - t.GetComponent<RectTransform>().rect.height/2);
		}
		t.gameObject.SetActive (true);

	}
	public void HideText(){

		GameManager.k_Manager.Description.transform.gameObject.SetActive (false);

	}
	#endregion


	#region GetTargetFields
	IEnumerator GetTargetFields(){
		yield return new WaitUntil (() => GameManager.k_Manager.now);
		while (true) {
				Changed = false;
				StartSkill = appliedSkill;
				InitializeField ();
			yield return new WaitUntil (() => Changed);
		}
	}

	public void InitializeField(){

		tp_list = new List<Transform> ();

		if (isMovement) {
			Movement ();
			return;
		}

		int rowPos = tp_characterclass.k_Position [0]-1;
		int colPos = tp_characterclass.k_Position [1]-1;

		//Check if Character is in Group
		if (GameManager.k_Manager.GroupPositions.Contains (tp_character.Char.transform.parent) && (appliedSkill.TargetEnemy || (!appliedSkill.TargetEnemy && !appliedSkill.Usable))) {

			if (!appliedSkill.Usable) {
				SkillNotUsable (LeftBattlePanel);

			} else {
				// Multiple Rows?
				if (!appliedSkill.postarget) {
					StaticTargetApply (RightBattlePanel);
				} else {
					PositionalTargetApply (RightBattlePanel,rowPos);
				}
			}
		//EnemyPosition
		} else {
			if (!GameManager.k_Manager.GroupPositions.Contains (tp_character.Char.transform.parent) && (!appliedSkill.TargetEnemy ||(appliedSkill.TargetEnemy && !appliedSkill.Usable))) {

				//Target own Team
				if (!appliedSkill.Usable) {
					SkillNotUsable (RightBattlePanel);
				} else {
					if (!appliedSkill.postarget) {
						StaticTargetApply (RightBattlePanel);
					} else {
						PositionalTargetApply (RightBattlePanel,rowPos);
					}
				}
			} else {
				if (!appliedSkill.Usable) {
					SkillNotUsable (RightBattlePanel);
				} else {
					if (!appliedSkill.postarget) {
						StaticTargetApply (LeftBattlePanel);
					}else{
						PositionalTargetApply (LeftBattlePanel,rowPos);
					}
				}
			}
		}
	}
	#endregion

	#region Operators
	public void SkillNotUsable(CharacterFields tp_field){
	
		for (int i = 0; i < 4; i++) {
			try {
				foreach (Transform trans in tp_field.Columns[(appliedSkill.ActivateRow[i])-1].ToArray()) {
					tp_list.Add (trans);
				}	
			} catch {
			}
		}
	}
		
	public void StaticTargetApply(CharacterFields tp_field){
		int[] tp_targets = appliedSkill.StaticTarget;
		try{
		for (int i = 0; i < tp_targets.Length; i++) {
			if (tp_targets[i] == 1) {
				tp_list.Add (tp_field.AllPositions [i]);
			}
			}}catch{
		}
	}

	public void PositionalTargetApply(CharacterFields tp_field, int tp_rowPos){

		int[] tp_targets = appliedSkill.PositionalTarget;
		List<int> tp_listed = new List<int> (tp_targets);

		switch (tp_rowPos) {

		case 0:
			tp_listed.RemoveRange (0, 12);
			break;
		case 1:
			tp_listed.RemoveRange (0, 8);
			tp_listed.RemoveRange (16, 4);
			break;
		case 2:
			tp_listed.RemoveRange (0, 4);
			tp_listed.RemoveRange (16, 8);
			break;
		case 3:
			tp_listed.RemoveRange (16, 12);
			break;
		}
	
		List<Transform> tp_fields = new List<Transform> ();
		for(int i=0;i<4;i++){
			for (int j = 0; j < 4; j++) {
				tp_fields.Add (tp_field.Rows [i] [j]);
			}
		}

		for (int i = 0; i < 16; i++) {
			if (tp_listed [i] == 1)
				tp_list.Add (tp_fields [i]);		
		}
	}


	#endregion

	#region Movement
	public void Movement(){
		int u = GameManager.k_Manager.GroupPositions.IndexOf(GameManager.k_Manager.CurrentCharacter.Char.transform.parent);

		List<Transform> tp_grouplist = GameManager.k_Manager.GroupPositions;

		try{
			if(tp_grouplist[u-1]!=null && u % 4!=0 && tp_grouplist[u-1].FindChild("Character(Clone)")==null){
				tp_list.Add(tp_grouplist[u-1]);
			}
		}catch{}
		try{
			if(tp_grouplist[u+1]!=null && u % 4!=3 && tp_grouplist[u+1].FindChild("Character(Clone)")==null){
				tp_list.Add(tp_grouplist[u+1]);
			}
		}catch{}
		try{
			if(tp_grouplist[u-4]!=null && tp_grouplist[u-4].FindChild("Character(Clone)")==null){
				tp_list.Add(tp_grouplist[u-4]);
			}
		}catch{}
		try{
			if(tp_grouplist[u+4]!=null && tp_grouplist[u+4].FindChild("Character(Clone)")==null){
				tp_list.Add(tp_grouplist[u+4]);
			}
		}catch{}
	}

	public void Move(){

		if(GameManager.k_Manager.UsingSkill==null){
			GameManager.k_Manager.OldSkillUsed = null;
			GameManager.k_Manager.UsingSkill = StartCoroutine (Moving ());}
		else{
			GameManager.k_Manager.SetClickable();
			GameManager.k_Manager.OldSkillUsed = GameManager.k_Manager.UsingSkill;
			GameManager.k_Manager.UsingSkill = StartCoroutine (Moving ());
		}
	}

	IEnumerator Moving(){
	
		gameObject.transform.GetComponent<Button> ().interactable = false;

		foreach (Transform trans in tp_list.ToArray()) {
			trans.GetChild (0).GetComponent<CharacterTrigger> ().clickable = true;
		}
		ChangeColor();
		this.Clicked = true;

		if (!GameManager.k_Manager.SkillWaiting ) {

			GameManager.k_Manager.SkillWaiting = true;
		} else {
			StopCoroutine (GameManager.k_Manager.OldSkillUsed);
			GameObject[] tp_buttons = GameObject.FindGameObjectsWithTag ("SkillButton");
			List<GameObject> tp_list = new List<GameObject> (tp_buttons);
			tp_buttons = tp_list.ToArray ();

			for (int i = 0; i < tp_buttons.Length; i++) {
				GameManager.k_Manager.CurrentCharacter.Char.GetComponent<CharacterClass>().SetButtonsInteractable (tp_buttons [i], i);
			}
		}

		yield return new WaitUntil (() => GameManager.k_Manager.TargetChosen);

		GameManager.k_Manager.CurrentCharacter.Char.transform.SetParent (GameManager.k_Manager.Target.transform.parent, false);

		GameManager.k_Manager.CurrentCharacter.Char.transform.localPosition = new Vector3 (0, 0.75f, 0);

		CharacterFields.k_CharacterFields.Start ();

		int o = GameManager.k_Manager.k_LeftBattlePanel.GetComponent<CharacterFields> ().AllPositions.IndexOf (GameManager.k_Manager.Target.transform.parent);

		RaiseEventOptions opt = new RaiseEventOptions ();
		opt.Receivers = ReceiverGroup.MasterClient;
		PhotonNetwork.RaiseEvent (61, new string[]{GameManager.k_Manager.BattleID+"",GameManager.k_Manager.CurrentCharacter.Position+"",o+""}, true, opt);


		Clicked = false;
		ChangeColorBack();
		gameObject.GetComponent<Button> ().interactable = true;
		GameManager.k_Manager.Target = null;

		GameManager.k_Manager.WantToEndTurn ();
	}
	#endregion
}
