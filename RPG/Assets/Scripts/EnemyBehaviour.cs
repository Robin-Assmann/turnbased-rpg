using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class EnemyBehaviour {

	int DamgePriority, HealPriority, BuffPriority, DebuffPriority;
	Character CurrentCharacter;
	GameObject KillButton;
	int Damage;

	public Skill[] AvailableSkills { get; set;}
	private Skill ChosenSkill;
	public List<int> TargetList;
	private int TargetPosition;

	public EnemyBehaviour(int tp_dmg, int tp_heal, int tp_buff, int tp_debuff, int BattleID){
	
		this.DamgePriority = tp_dmg;
		this.HealPriority = tp_heal;
		this.BuffPriority = tp_buff;
		this.DebuffPriority = tp_debuff;
		this.TargetPosition = -1;
		CurrentCharacter = ServerStorage.Sv_Storage.B_Storages[BattleID].CharacterOrder [0];
		Damage = 0;
		KillButton = null;
		TargetList = new List<int> ();
	}

	public int CheckUsable(Skill Skill, int BattleID, int Position){

		BattleStorage b = ServerStorage.Sv_Storage.B_Storages [BattleID];

		int row = 0;
		row = Position - 4;

		if (Position < 4) {
			row = Position;
		}else {
			row = Position - (4 * (int)(Position / 4));
		}

		int col = Position / 4;

		int[] tp_targets = Skill.PositionalTarget;
		TargetList = new List<int> (tp_targets);
		try{
		switch (col) {
		case 0:
			TargetList.RemoveRange (0, 12);
			break;
		case 1:
			TargetList.RemoveRange (0, 8);
			TargetList.RemoveRange (16, 4);
			break;
		case 2:
			TargetList.RemoveRange (0, 4);
			TargetList.RemoveRange (16, 8);
			break;
		case 3:
			TargetList.RemoveRange (16, 12);
			break;
		}
		}catch{
		
			Debug.Log (Skill.Name);
		}

		int[] pos = b.Positions;
		if (Skill.ActivateRow [row] != 0) {
			for (int i = 0; i < TargetList.Count; i++) {
				if (pos [i] !=0 && TargetList [i] == 1) {
					Skill.TargetFields = this.TargetList;
					return i;
				}
			}
		}
		return 0;
	}
	
	public void UsableSkills(Skill[] Skills){
	
		List<Skill> skills = new List<Skill> ();

		for (int i = 0; i < Skills.Length; i++) {
		
			Skill tp_skill = Skills[i];

			if (tp_skill.Name != "null" && tp_skill.Usable) {
				skills.Add (tp_skill);
			}
		}
		this.AvailableSkills = skills.ToArray ();
	}

	public Skill PickSkill(){

		if (this.AvailableSkills.Length <= 0){
			Debug.Log ("No skill available");
			return null;
		}

		if (KillButton != null) {
			return KillButton.GetComponent<ButtonClass> ().appliedSkill;		
		}

		int u = (int)Random.Range (0, AvailableSkills.Length);
		ChosenSkill = AvailableSkills[u];
		return this.AvailableSkills[u];
	}

	public int PickTarget(){
		
		return TargetPosition;

//		if (tp_mode == 0) {
//			List<GameObject> tp_targets = new List<GameObject> ();
//
//			foreach (Transform trans in tp_list.ToArray()) {
//				if (trans.childCount > 1 && trans.GetChild (1).name == "Character(Clone)")
//					tp_targets.Add (trans.GetChild (0).gameObject);
//			}
//			GameObject[] Targets = new GameObject[tp_targets.Count];
//
//			if (tp_targets.Count > 1) {
//				for (int i = 0; i < tp_targets.Count - 1; i++) {
//					try {
//						if (tp_targets [i].transform.parent.GetChild(1).GetComponent<CharacterClass> ().k_Character.Health > tp_targets [i + 1].transform.parent.GetChild(1).GetComponent<CharacterClass> ().k_Character.Health) {
//							Targets [i] = tp_targets [i + 1];
//							tp_targets[i+1]= tp_targets[i];
//						} else {
//							Targets [i] = tp_targets [i];
//						}
//					} catch {
//						Targets [i] = tp_targets [i];
//					}
//				}
//			} else {
//				Targets [0] = tp_targets [0];
//			}
//			GameManager.k_Manager.Target = Targets [0];
//			Targets [0].GetComponent<CharacterTrigger>().ChangeColor(0.3f, 0.3f, 0.3f, 1.0f);
//			GameManager.k_Manager.TargetChosen = true;
//		}
	}

	public void CheckKill(GameObject tp_button, int tp_mode){

		int dmg = 0;
		int higher = 0;
		Skill tp_skill = tp_button.GetComponent<ButtonClass> ().appliedSkill;
		//Check with Lowest/Average/Highest Damage
		for (int i = 0; i < 3; i++) {

			switch (i) {

			case 0:
				dmg =(int) (CurrentCharacter.MinDPS * tp_skill.Modifier);
				higher = 4;
				break;
			case 1: 
				dmg = (int)((CurrentCharacter.MinDPS + CurrentCharacter.MaxDPS) / 2 * tp_skill.Modifier);
				higher = 3;
				break;
			case 2:
				dmg = (int)(CurrentCharacter.MaxDPS * tp_skill.Modifier);
				higher = 2;
				break;
			}

			//Single Target
			if (tp_mode == 0) {
				foreach (Transform trans in tp_button.GetComponent<ButtonClass>().tp_list.ToArray()) {
					if (trans.FindChild ("Character(Clone)") != null && trans.GetChild (1).GetComponent<CharacterClass> ().k_Character.Health - dmg <= 0 && dmg>Damage) {
						Damage = dmg;
						KillButton = tp_button;
						DamgePriority += higher;
						break;
					}
				}
			}
			if (tp_mode == 1) {
				foreach (Transform trans in tp_button.GetComponent<ButtonClass>().tp_list.ToArray()) {
					if (trans.FindChild ("Character(Clone)") != null && trans.GetChild (1).GetComponent<CharacterClass> ().k_Character.Health - dmg <= 0 && dmg>Damage) {
						Damage = dmg;
						KillButton = tp_button;
						DamgePriority += higher;
						break;
					}
				}
			}
		}
	}
}
