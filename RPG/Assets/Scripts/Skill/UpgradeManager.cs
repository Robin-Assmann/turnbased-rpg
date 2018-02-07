using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour ,IPointerClickHandler{

	public UpgradeDetail appliedUpgrade;
	GameObject name;
	public bool upgrade = true;
	public Text UpgradeCount;
	public int cost;
	public int Upgraded = 0;
	int maxUpgrades;
	public bool buyable = false;
	public bool maxBought = false;
	public bool unlocked = false;
	public bool found = true;
	public int UpgradeNumber;

	public List<int> RecIds;

	public void OnPointerClick (PointerEventData eventData){

		if (buyable && upgrade) {
			BuyUpgrade ();
		}
	}

	void Update(){
	
		if (found) {
			if (unlocked && upgrade && !buyable && !this.maxBought && (cost <= CreationManager.k_Manager.SkillPoints)) {
				buyable = true;
				GetComponent<Image> ().color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
			} else if (buyable && (cost > CreationManager.k_Manager.SkillPoints)) {
				buyable = false;
				GetComponent<Image> ().color = new Color (0.6f, 0.6f, 0.6f, 1.0f);
			}

			if (!unlocked) {
				int nes = 0;

				for (int i = 0; i < RecIds.Count; i++) {
					if (CreationManager.k_Manager.currentSkillList [RecIds [i]].transform.GetChild (0).GetComponent<UpgradeManager> ().maxBought)
						nes++;
				}
		
				if (RecIds.Count == nes)
					unlocked = true;
			}
		}
	}

	public void ApplyUpgrade(UpgradeDetail tp_upgrade, int upgraded, List<int> s, int UpgradeNumber){
		this.UpgradeNumber = UpgradeNumber;
		this.found = true;
		this.maxBought = false;
		if (upgrade) {
			this.UpgradeCount = transform.GetChild (0).GetComponent<Text> ();
			this.UpgradeCount.text = "";
		}
		this.RecIds = new List<int> ();

		appliedUpgrade = tp_upgrade;
		cost = appliedUpgrade.StartCost + Mathf.FloorToInt(appliedUpgrade.CostIncrease*upgraded);
		this.Upgraded = upgraded;
		this.maxUpgrades = this.appliedUpgrade.UpgradeCount;

		GetComponent<Image> ().color = new Color (0.6f, 0.6f, 0.6f, 1.0f);
		if (this.Upgraded > 0) {
			this.UpgradeCount.text = this.Upgraded + "/" + this.maxUpgrades;

			if (this.Upgraded == this.maxUpgrades) {
				this.maxBought = true;
				this.buyable = false;
				GetComponent<Image> ().color = new Color (1.0f, 1.0f, 1.0f, 0.6f);
			}
		}

		this.RecIds = s;

		if (RecIds.Count == 1) {
			if (RecIds [0] == 0) {
				this.unlocked = true;
			} else {
				GetComponent<Image> ().color = new Color (0.6f, 0.6f, 0.6f, 1.0f);
			}
		}
	}

	public void ShowText(){
		if (found) {
			Transform t = CreationManager.k_Manager.Description.transform;
			Vector2 pos = transform.position;
			this.name = t.GetChild (0).gameObject;
			t.GetChild (1).GetComponent<Text> ().text = appliedUpgrade.Description + "Upgrade for " + cost + "SP.";
			if (pos.y + GetComponent<RectTransform> ().rect.height / 2 + 5 < 450)
				t.position = new Vector2 (pos.x, pos.y + GetComponent<RectTransform> ().rect.height / 2 + 5);
			else {
				t.position = new Vector2 (pos.x - t.GetComponent<RectTransform> ().rect.width / 2, pos.y - t.GetComponent<RectTransform> ().rect.height / 2);
			}
			t.GetChild (0).gameObject.SetActive (false);
			t.gameObject.SetActive (true);
		}
	}

	public void HideText(){
		if (found) {
			this.name.SetActive (true);
			CreationManager.k_Manager.Description.transform.gameObject.SetActive (false);
		}
	}

	public void BuyUpgrade(){
	
		CreationManager.k_Manager.ChangeSkillPoints (-cost);
		this.cost = appliedUpgrade.StartCost + Mathf.FloorToInt (appliedUpgrade.CostIncrease * this.Upgraded);
		this.Upgraded++;
		this.UpgradeCount.text = Upgraded + "/" +appliedUpgrade.UpgradeCount;

		foreach (SkillDetail s in CreationManager.k_Manager.currentCharacter.SkillOverview.skills.ToArray()) {
		
			if (s.ID == CreationManager.k_Manager.currentSkill.ID) {
				int f = UpgradeNumber / 10;
				switch(f){
				case 0:
					s.FirstRow [UpgradeNumber % 10] =Upgraded;
					break;
				case 1:
					s.SecondRow [UpgradeNumber % 10] = Upgraded;
					break;
				case 2:
					s.ThirdRow [UpgradeNumber % 10] = Upgraded;
					break;
				case 3:
					s.Ultimate = Upgraded;
					break;
				}
				break;
			}
		}
		CreationManager.k_Manager.currentCharacter.UpdateSkillTree ();
		CreationManager.k_Manager.currentCharacter.UpdateSkill ();


		if (this.Upgraded == this.maxUpgrades) {
			this.maxBought = true;
			this.buyable = false;
			GetComponent<Image> ().color = new Color (1.0f, 1.0f, 1.0f, 0.6f);
		}
	}

	public void Lock(){

		this.found = false;
		transform.GetChild (0).GetComponent<Text> ().text = "";
		GetComponent<Image> ().color = new Color (0.6f, 0.6f, 0.6f, 1.0f);
	}
}
