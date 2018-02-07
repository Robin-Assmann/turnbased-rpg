using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Skill{

	#region XML Init
	public int ID;
	public string Name;
	public float Modifier;
	public float CritModifier;
	public int SkillIdentifier; //0=Offensive 1=Deffensive 2=Utility
	public string DamageType;//	Phys, Fire, Ice, Light
	public int TargetMode; // 0=Single 1=Multi 2=AOE 3=FreeAOE 4=DOT //0=Mov+Skill 1=OptionalMove 2=ForcedMove
	public int SkillType; // 0=Dmg 1=Heal 2=ApplyBuff 3=ApplyDot 4=Movement+ 5=ApplyDebuff
	public string Target_Activate;
	public string Target;
	public string Target_Individual;
	public int Image;
	public bool TargetEnemy; // 0=false 1=true
	public int TargetCount; //How many Targets -1 = all
	public int TurnCount; // how long skill should last
	public bool Dodgeable; // 0=fase 1=true
	public int SkillUse;
	public int BuffCount; // 0=NoBuff 1-*= Buffs
	public string Buffstring;
	public string DescriptionText;
	public string Upgrade; //FirstRow|SecondRow|ThirdRow|...|(UltimateAvailable from Row)1-2-3
	public string UpgradeTree; //UpgradeType/Value/Count/StartCost-Increase|...|...
	#endregion

	public string Description="";
	public int[] ActivateRow{ get; set;} // 1/2/3/4
	public int[] StaticTarget{ get; set;} // 1234/1 columns/x x=0 only 1 row 
	public int[] PositionalTarget{ get; set;}
	public Sprite SkillImage{ get; set;}

	public List<UpgradeDetail> UpgradeDetails;

//	public GameObject k_relatedButton { get; set;}

	public bool Usable { get; set;}
	public bool gotTarget { get; set;}

	public int MinDPS{ get; set;}
	public int MaxDPS{ get; set;}
	//EnemyBehaviour
	public int TargetChosen{ get; set;}
//	public CharacterClass targetChar{ get; set;}

	private Character buff_applied{ get; set;}
	private List<string[]> buffs { get; set;}
	public bool postarget;

	public List<int> TargetFields;

	#region base
	private float base_Modifier;
	private float base_CritModifier;
	private int base_TargetCount;
	private int base_SkillUse;
	#endregion

	public Skill(){

	}

	public void Init(){

		this.UpgradeDetails = new List<UpgradeDetail> ();
		this.TargetFields = new List<int> ();
		InitBases ();

		if (BuffCount != 0 && BuffCount > 0) {
			MakeBuff (Buffstring);
		}
		postarget = false;
		MakeDescription ();
		string[] tp_array;
		tp_array = Target.Split ("/" [0]);
		MakeStaticTarget (tp_array);

		tp_array = Target_Activate.Split ("/"[0]);
		MakeActivateRow (int.Parse (tp_array [0]), int.Parse (tp_array [1]), int.Parse (tp_array [2]), int.Parse (tp_array [3]));

		tp_array = Target_Individual.Split ("/"[0]);
		PositionalTargets (tp_array);

		string[] s = UpgradeTree.Split ("|" [0]);
		for (int i = 0; i < s.Length; i++) {

			string[] t = s [i].Split ("/" [0]);
		
			UpgradeDetails.Add (new UpgradeDetail (int.Parse (t [0]), int.Parse (t [1]), int.Parse (t [2]), t [3]));
		}
	}


	public Skill(int tp_id,string tp_name, float tp_mod, int tp_targetmode, int tp_skilltype, string tp_activate, string tp_target, string tp_postarget, Sprite tp_image, bool tp_tarenemy, int tp_turn, bool tp_dodgeable, int tp_buffcount, string tp_buffstring){

		this.ID = tp_id;
		this.Name = tp_name;
		this.Modifier = tp_mod;
		this.TargetMode = tp_targetmode;
		this.Target = tp_target;
		this.SkillType = tp_skilltype;
		this.SkillImage = tp_image;
		this.TargetEnemy = tp_tarenemy;
		this.TurnCount = tp_turn;
		this.Dodgeable = tp_dodgeable;
		this.BuffCount = tp_buffcount;
		this.TargetFields = new List<int> ();

		if (tp_buffcount != 0 && tp_buffcount > 0) {
			MakeBuff (tp_buffstring);
		}
		postarget = false;

		string[] tp_array = Target.Split ("/"[0]);
		MakeStaticTarget (tp_array);

		tp_array = tp_activate.Split ("/"[0]);
		MakeActivateRow (int.Parse (tp_array [0]), int.Parse (tp_array [1]), int.Parse (tp_array [2]), int.Parse (tp_array [3]));

		tp_array = tp_postarget.Split ("/"[0]);
		PositionalTargets (tp_array);
	}

	#region Different Skills
	//DOT
	public Skill(int tp_skilltype,int tp_targetmode, int tp_mindps, int tp_maxdps, CharacterClass tp_char, string tp_name){
	
		this.Name = tp_name;
		this.SkillType = tp_skilltype;
		this.TargetMode = tp_targetmode;
		this.MinDPS = tp_mindps;
		this.MaxDPS = tp_maxdps;
//		this.targetChar = tp_char;
	}
	#endregion

	#region SplitStrings
	public void MakeStaticTarget(string[] tp_array){

		StaticTarget = new int[tp_array.Length];
		List<int> tp_List = new List<int> ();

		for (int i = 0; i < tp_array.Length; i++) {
			tp_List.Add (int.Parse(tp_array[i]));
			if (int.Parse (tp_array [i])!= 0)
				postarget = false;
		}

		StaticTarget = tp_List.ToArray ();
	}

	public void MakeActivateRow(int o, int j, int k, int l){

		ActivateRow = new int[4];
		List<int> tp_List = new List<int> ();

		tp_List.Add (o);
		tp_List.Add (j);
		tp_List.Add (k);
		tp_List.Add (l);
	
		ActivateRow = tp_List.ToArray ();
	}

	public void PositionalTargets(string[] tp_array){

		PositionalTarget = new int[tp_array.Length];
		List<int> tp_List = new List<int> ();

		for (int i = 0; i < tp_array.Length; i++) {
			tp_List.Add (int.Parse(tp_array[i]));
			if (int.Parse (tp_array [i]) != 0)
				postarget = true;
		}

		PositionalTarget = tp_List.ToArray ();
	}

	public void MakeBuff(string tp_buff){

		buffs = new List<string[]> ();
	
		string[] tp_stringarray = tp_buff.Split ("/" [0]);

		for (int i = 0; i < BuffCount; i++) {
			string[] tp_now = tp_stringarray [i].Split ("," [0]);
			buffs.Add (tp_now);
		}
	}
	#endregion

	public void UseSkill(bool tp_start, int tp_int, Character tp_chara){
		switch (SkillType) {
		case 0:

			if (TargetMode == 4)
				Dot ();
			else
				Damage ();
			break;
		case 1:
			Heal ();
			break;
		case 2:
			if (tp_start) {
				for (int i = 0; i < BuffCount; i++) {
					ApplyBuff (tp_start, i, null);
				}
			} else {
				ApplyBuff (tp_start, tp_int, tp_chara);
			}
			break;
		case 3:
			ApplyDot ();
			break;
		case 4:
			MovementAttack ();
			break;
		case 5: 
			if (tp_start) {
				for (int i = 0; i < BuffCount; i++) {
					ApplyDebuff (tp_start, i);
				}
			} else {
				ApplyDebuff (tp_start, tp_int);
			}
			break;
		}
	}

	#region Skills
	private void Damage(){
		Debug.Log ("dmg rolled + Target Pos " + GameManager.k_Manager.Target.transform.parent.GetChild (1).GetComponent<CharacterClass> ().k_Character.Position);

		string[] content = new string[] {GameManager.k_Manager.BattleID+"", TargetMode+"",Modifier+"",Dodgeable+"",GameManager.k_Manager.Target.transform.parent.GetChild (1).GetComponent<CharacterClass> ().k_Character.Position+"",1+"",GameManager.k_Manager.CurrentCharacter.MinDPS+"", GameManager.k_Manager.CurrentCharacter.MaxDPS+""};
		RaiseEventOptions opt = new RaiseEventOptions ();
		opt.Receivers = ReceiverGroup.MasterClient;
		PhotonNetwork.RaiseEvent (58, content, true, opt);

	}

	private void Heal(){
		Debug.Log ("heal rolled");
		
//		int tp_damage = (int)((GameManager.k_Manager.CharacterOrder [0].MinDPS + GameManager.k_Manager.CharacterOrder [0].MaxDPS * Random.value) * Modifier);
//
//		//Single Target
//		if (TargetMode == 0) {
//
//			CharacterClass tp_charclass = GameManager.k_Manager.Target.transform.parent.GetChild (1).GetComponent<CharacterClass> ();
//
//			if (tp_charclass.k_Character.Health + tp_damage <=tp_charclass.k_Character.MaxHealth) {
//				tp_charclass.k_Character.Health += tp_damage;
//			} else {
//				tp_charclass.k_Character.Health = tp_charclass.k_Character.MaxHealth;
//			}
//			tp_charclass.MakeText ("+ " + tp_damage, new Color (0, 255, 0));
//		}
//		//AOE
//		if (TargetMode == 2) {
//			foreach (Transform trans in k_relatedButton.GetComponent<ButtonClass>().tp_list.ToArray()) {
//				if (trans.FindChild ("Character(Clone)")!=null) {
//					tp_damage = (int)((GameManager.k_Manager.CharacterOrder [0].MinDPS + GameManager.k_Manager.CharacterOrder [0].MaxDPS * Random.value) * Modifier);
//					if (trans.FindChild ("Character(Clone)").GetComponent<CharacterClass> ().k_Character.Health + tp_damage <= trans.FindChild ("Character(Clone)").GetComponent<CharacterClass> ().k_Character.MaxHealth) {
//						trans.FindChild ("Character(Clone)").GetComponent<CharacterClass> ().k_Character.Health += tp_damage;
//					} else {
//						trans.FindChild ("Character(Clone)").GetComponent<CharacterClass> ().k_Character.Health = trans.FindChild ("Character(Clone)").GetComponent<CharacterClass> ().k_Character.MaxHealth;
//					}
//					trans.FindChild ("Character(Clone)").GetComponent<CharacterClass> ().MakeText ("+ " + tp_damage, new Color (0, 255, 0));
//				}
//			}
//		}
	}


	private void MovementAttack(){

//		int tp_damage = (int)((GameManager.k_Manager.CharacterOrder [0].MinDPS + GameManager.k_Manager.CharacterOrder [0].MaxDPS * Random.value) * Modifier);
//	
//		CharacterClass tp_charclass = GameManager.k_Manager.Target.transform.parent.GetChild (1).GetComponent<CharacterClass> ();
//
//		if ((Dodgeable && (Random.value > tp_charclass.k_Character.Dodge)) || !Dodgeable) {
//			tp_charclass.k_Character.Health -= tp_damage;
//			tp_charclass.MakeText ("- " + tp_damage, new Color (255, 0, 0));
//		} else {
//			tp_charclass.MakeText ("Dodged", new Color (255, 255, 0));
//		}
//
//		bool tp_bool = GameManager.k_Manager.CharacterOrder [0].isEnemy;
//		int pos_row = tp_charclass.k_Character.Position;
//		List<Transform> tp_fields ;
//		if (tp_bool) {
//			tp_fields = GameManager.k_Manager.GroupPositions;
//			pos_row += 2;
//			for (int i = 0; i < 2; i++) {
//				if (tp_fields [pos_row ].childCount > 1) {
//					pos_row--;
//				}
//			}
//		} else {
//			tp_fields = GameManager.k_Manager.EnemyPositions;
//			pos_row+=2;
//			for (int i = 0; i < 2; i++) {
//				if (tp_fields [pos_row ].childCount > 1) {
//					pos_row--;
//				}
//			}
//		}
//		tp_charclass.k_Character.Char.transform.SetParent (tp_fields[pos_row], false);
//		tp_charclass.k_Character.Char.transform.localPosition = new Vector3 (0, 0.75f, 0);
//		tp_charclass.k_Character.Position = pos_row;
//
//
//		GameObject tp_game=null;
//
//		if (tp_bool) {
//			tp_game = GameObject.FindGameObjectWithTag("LeftBattleGround");
//		} else {
//			tp_game = GameObject.FindGameObjectWithTag("RightBattleGround");
//		}
//		tp_game.GetComponent<CharacterFields> ().Start ();
	}

	private void ApplyBuff(bool tp_start, int tp_buff, Character tp_char){

		if (tp_start) {
			if (BuffCount != 0) {
				Action[] tp_act = GameManager.k_Manager.Target.transform.parent.GetChild (1).GetComponent<CharacterClass> ().k_Character.ActionList;

				buff_applied = GameManager.k_Manager.Target.transform.parent.GetChild (1).GetComponent<CharacterClass> ().k_Character;

				for (int i = 0; i < tp_act.Length; i++) {
					if (tp_act [i].ActionType == 0) {

						//int tp_turn,int tp_type (0=dmg 1=heal 2=buff 3=debuff),Skill tp_skill, int tp_deathturn
						tp_act [i] = new Action (TurnCount, 2, this, TurnCount, tp_buff, buff_applied);
						break;
					}
				}
				GameManager.k_Manager.Target.transform.parent.GetChild (1).GetComponent<CharacterClass> ().k_Character.ActionList = tp_act;

				switch (buffs [tp_buff] [0]) {
				case "Speed": 

					GameManager.k_Manager.Target.transform.parent.GetChild (1).GetComponent<CharacterClass> ().k_Character.Speed +=int.Parse(buffs [tp_buff] [1]);
					break;

				case "Armor":
					GameManager.k_Manager.Target.transform.parent.GetChild (1).GetComponent<CharacterClass> ().k_Character.Armor += int.Parse(buffs [tp_buff] [1]);
					break;
				}
				GameManager.k_Manager.Target.transform.parent.GetChild (1).GetComponent<CharacterClass> ().MakeText ("Faster", new Color (255, 255, 0));
			}
		} else {
			//Debug.Log(tp_char.Name);

			switch (buffs [tp_buff] [0]) {
			case "Speed": 
				tp_char.Speed -= int.Parse(buffs [tp_buff] [1]);
				break;

			case "Armor":
				tp_char.Armor -= int.Parse(buffs [tp_buff] [1]);
				break;
			}
		}
	}

	private void ApplyDebuff(bool tp_start, int tp_buff){
		if (tp_start) {
			if (BuffCount != 0) {
				Action[] tp_act = GameManager.k_Manager.Target.transform.parent.GetChild (1).GetComponent<CharacterClass> ().k_Character.ActionList;
				buff_applied = GameManager.k_Manager.Target.transform.parent.GetChild (1).GetComponent<CharacterClass> ().k_Character;

				for (int i = 0; i < tp_act.Length; i++) {
					if (tp_act [i].ActionType == 0) {

						//int tp_turn,int tp_type (0=dmg 1=heal 2=buff 3=debuff),Skill tp_skill, int tp_deathturn
						tp_act [i] = new Action (TurnCount, 2, this, TurnCount, tp_buff, buff_applied);
						break;
					}
				}
				GameManager.k_Manager.Target.transform.parent.GetChild (1).GetComponent<CharacterClass> ().k_Character.ActionList = tp_act;

				switch (buffs [tp_buff] [0]) {
				case "Speed": 

					GameManager.k_Manager.Target.transform.parent.GetChild (1).GetComponent<CharacterClass> ().k_Character.Speed -=int.Parse(buffs [tp_buff] [1]);
					break;

				case "Armor":
					GameManager.k_Manager.Target.transform.parent.GetChild (1).GetComponent<CharacterClass> ().k_Character.Armor -= int.Parse(buffs [tp_buff] [1]);
					break;
				}
				GameManager.k_Manager.Target.transform.parent.GetChild (1).GetComponent<CharacterClass> ().MakeText ("Slower", new Color (255, 255, 0));
			}
		} else {

			switch (buffs [tp_buff] [0]) {
			case "Speed": 
				Debug.Log ("now");
				buff_applied.Speed += int.Parse(buffs [tp_buff] [1]);
				break;

			case "Armor":
				buff_applied.Armor += int.Parse(buffs [tp_buff] [1]);
				break;
			}
		}
	}

	private void ApplyDot(){
			Action[] tp_act = GameManager.k_Manager.Target.transform.parent.GetChild (1).GetComponent<CharacterClass> ().k_Character.ActionList;
			for (int i = 0; i < tp_act.Length; i++) {
				if (tp_act [i].ActionType == 0) {
					tp_act [i] = new Action (0, 3, new Skill (0, 4, 1, 2, GameManager.k_Manager.Target.transform.parent.GetChild (1).GetComponent<CharacterClass> (), "Dot"),TurnCount,0, null);
					break;
				}
			}
			GameManager.k_Manager.Target.transform.parent.GetChild (1).GetComponent<CharacterClass> ().k_Character.ActionList = tp_act;
			GameManager.k_Manager.Target.transform.parent.GetChild (1).GetComponent<CharacterClass> ().MakeText ("Bleeding", new Color (255, 255, 0));
	}

	public void Dot(){

//		int tp_damage = (int)((MinDPS + MaxDPS * Random.value));
//
//		targetChar.k_Character.Health -= tp_damage;
//		targetChar.MakeText ("- " + tp_damage, new Color (255, 0, 0));
	}
	#endregion

	public string MakeString(){
	
		return ID + "|" + Name + "|" + Modifier + "|" + TargetMode + "|" + SkillType + "|" + Target_Activate + "|" + Target + "|" + Target_Individual + "|" + this.Image + "|" + TargetEnemy + "|" + TurnCount + "|" + Dodgeable + "|" + BuffCount + "|" + Buffstring +";";
	
	}

	private void MakeDescription(){
	
		switch (this.SkillType) {

		case 0:
			this.Description = string.Format (this.DescriptionText, this.Modifier * 100 + "%", this.TargetCount, this.SkillUse, this.CritModifier * 100 + "%");
			break;
		case 1:
			this.Description = string.Format (this.DescriptionText, this.Modifier * 100 + "%", this.TargetCount, this.SkillUse, this.CritModifier * 100 + "%");
			break;
		case 2:
			this.Description = string.Format (this.DescriptionText, this.TargetCount);
			break;
		case 3:
			this.Description = string.Format (this.DescriptionText, this.Modifier * 100 + "%", this.TargetCount, this.SkillUse, this.CritModifier * 100 + "%");
			break;
		}
	}

	private void InitBases(){
	
		this.base_Modifier = this.Modifier;
		this.base_CritModifier = this.CritModifier;
		this.base_TargetCount = this.TargetCount;
		this.base_SkillUse = this.SkillUse;
	}

	public void UpdateStats(SkillDetail sd){
	
		Debug.Log (this.Name + " updated");
		int upgradecount = 0;

		for (int i = 0; i < sd.FirstRow.Count; i++) {
		
			if (sd.FirstRow [i] > 0) {
				ApplyStats (this.UpgradeDetails [upgradecount], sd.FirstRow [i]);
			}
			upgradecount++;
		}

		for (int i = 0; i < sd.SecondRow.Count; i++) {

			if (sd.SecondRow [i] > 0) {
				ApplyStats (this.UpgradeDetails [upgradecount], sd.SecondRow [i]);
			}
			upgradecount++;
		}
		for (int i = 0; i < sd.ThirdRow.Count; i++) {

			if (sd.ThirdRow [i] > 0) {
				ApplyStats (this.UpgradeDetails [upgradecount], sd.ThirdRow [i]);
			}
			upgradecount++;
		}

		if (sd.Ultimate > 0) {
			ApplyStats (this.UpgradeDetails [upgradecount], sd.Ultimate);
		}
		MakeDescription ();
	}

	private void ApplyStats(UpgradeDetail d, int value){
		switch (d.Type) {

		case 0:
			this.Modifier = base_Modifier + ((float)(d.Value * value) / 100);
			Debug.Log (this.Modifier);
			break;
		case 1:
			this.CritModifier = base_CritModifier+ ((float)(d.Value*value)/100);
			break;
		case 2:
			this.TargetCount = base_TargetCount+ d.Value*value;
			break;
		case 3:
			this.SkillUse = base_SkillUse+ d.Value*value;
			break;

		}
	}
}



