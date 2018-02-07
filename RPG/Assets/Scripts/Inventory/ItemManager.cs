using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour, IPointerClickHandler{

	public Item appliedItem;

	public bool equipped, changed;

	private Transform tp_game, tp_info, tp_stats, tp_value, tp_main, tp_dmg;

	void Awake(){

		tp_game = transform.FindChild ("Canvas").GetChild (0);

		tp_game.gameObject.SetActive (false);

		equipped = false;
		changed = false;

		tp_info = transform.FindChild ("Canvas").GetChild (0).FindChild("Info");
		tp_main = transform.FindChild ("Canvas").GetChild (0).FindChild("Main");
		tp_stats = transform.FindChild ("Canvas").GetChild (0).FindChild("Stats");
		tp_value = transform.FindChild ("Canvas").GetChild (0).FindChild("Value");
		tp_dmg = transform.FindChild ("Canvas").GetChild (0).FindChild ("DPS");
	}

	public void ApplyTexts(){

		if (!appliedItem.Type.Equals ("Weapon")) {
			this.tp_dmg.gameObject.SetActive (false);
		}
	
		int[] stats = this.appliedItem.ChangeStats;
		print (appliedItem.ID);
		for (int i = 0; i < stats.Length; i++) {
		
			if (stats [i] == 0) {
				continue;
			}

			switch (i) {

			case 0:
				CreateStat (tp_main,"Strength", stats [i]);
				break;
			case 1:
				CreateStat (tp_main,"Constitution", stats [i]);
				break;
			case 2:
				CreateStat (tp_main,"Dexterity", stats [i]);
				break;
			case 3:
				CreateStat (tp_main,"Intelligence", stats [i]);
				break;
			case 4:
				CreateStat (tp_main,"Luck", stats [i]);
				break;
			case 5:
				CreateStat (tp_stats,"Health", stats [i]);
				break;
			case 6:
				CreateStat (tp_stats,"Armor", stats [i]);
				break;
			case 9:
				CreateStat (tp_stats,"Dodge", stats [i]);
				break;
			case 10:
				CreateStat (tp_stats,"Speed", stats [i]);
				break;
			case 11:
				CreateStat (tp_stats,"Fire Resistance", stats [i]);
				break;
			case 12:
				CreateStat (tp_stats,"Ice Resistance", stats [i]);
				break;
			case 13:
				CreateStat (tp_stats,"Lightning Resistance", stats [i]);
				break;
			case 14:
				CreateStat (tp_stats,"Damage", stats [i]);
				break;
			case 15:
				CreateStat (tp_stats,"Crit. Damage", stats [i]);
				break;
			case 16:
				CreateStat (tp_main,"Magic Damage", stats [i]);
				break;
			case 17:
				CreateStat (tp_main,"Crit. Magic Damage", stats [i]);
				break;
			case 18:
				CreateStat (tp_main,"Crit. Chance", stats [i]);
				break;
			case 20:
				CreateStat (tp_main,"Magic Find", stats [i]);
				break;
			case 21:
				CreateStat (tp_main,"Gold Find", stats [i]);
				break;
			}
		}

		tp_info.FindChild ("Name").GetComponent<Text> ().text = appliedItem.Name;
		tp_value.FindChild ("Value").GetComponent<Text> ().text = appliedItem.Value + "";
		tp_info.FindChild ("Rarity").GetComponent<Text> ().text = appliedItem.Rarity;
		tp_info.FindChild ("ItemLevel").GetComponent<Text> ().text = appliedItem.ItemLevel + "";
		tp_value.FindChild ("Weight").GetComponent<Text> ().text = stats[19] + " lb";

		if (this.tp_dmg.gameObject.activeSelf) {
			print (this.appliedItem.ChangeMinDps);
			tp_dmg.FindChild ("Min-Max").GetComponent<Text> ().text = stats [7] + " - " + stats [8];
		}

	}

	public void CreateStat(Transform parent, string stat, int value){
	
		GameObject tp_text = new GameObject ("Stat", typeof(RectTransform));
		tp_text.AddComponent<Text> ();
		tp_text.GetComponent<Text> ().resizeTextForBestFit = true;
		tp_text.GetComponent<Text> ().color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
		tp_text.GetComponent<Text> ().font = Resources.GetBuiltinResource<Font> ("Arial.ttf");
		//tp_text.GetComponent<Text> ().fontSize = 10;
		tp_text.GetComponent<Text> ().text = stat +" "+ value;
		tp_text.GetComponent<Transform> ().SetParent (parent);
	
	}


//	public void ApplyTexts(){
//
//		List<int> tp_values = new List<int> ();
//
//		tp_values.Add (appliedItem.ChangeStrength);
//		tp_values.Add (appliedItem.ChangeDexterity);
//		tp_values.Add (appliedItem.ChangeIntelligence);
//		tp_values.Add(appliedItem.ChangeHealth);
//		tp_values.Add(appliedItem.ChangeArmor);
//		tp_values.Add((int)appliedItem.ChangeDodge);
//		tp_values.Add (appliedItem.ChangeMagicDamage);
//		tp_values.Add(appliedItem.ChangeMinDps);
//		tp_values.Add(appliedItem.ChangeMaxDps);
//		tp_values.Add(appliedItem.ChangeSpeed);
//		tp_values.Add( appliedItem.ChangeResFire);
//		tp_values.Add(appliedItem.ChangeResIce);
//		tp_values.Add(appliedItem.ChangeResLightning);
//
//		List<string> tp_list = new List<string> ();
//
//		for (int i = 0; i < tp_values.Count; i++) {
//		
//			if (tp_values [i] != 0) {
//			
//				switch (i) {
//
//				case 0:
//					tp_list.Add ("Strength ");
//					tp_list.Add ("+ " + tp_values[i]);
//					break;
//				case 1:
//					tp_list.Add ("Dexterity ");
//					tp_list.Add ("+ " + tp_values[i]);
//					break;
//				case 2:
//					tp_list.Add ("Intelligence ");
//					tp_list.Add ("+ " + tp_values[i]);
//					break;
//				case 3:
//					tp_list.Add ("Health ");
//					tp_list.Add ("+ " + tp_values[i]);
//					break;
//				case 4:
//					tp_list.Add ("Armor ");
//					tp_list.Add ("+ " + tp_values[i]);
//					break;
//				case 5:
//					tp_list.Add ("Dodge");
//					tp_list.Add ("+ " + tp_values[i] + " %");
//					break;
//				case 6:
//					tp_list.Add ("Magic Damage ");
//					tp_list.Add ("+ " + tp_values[i]);
//					break;
//				case 7:
//					tp_list.Add ("MinDps ");
//					tp_list.Add ("+ " + tp_values[i]);
//					break;
//				case 8:
//					tp_list.Add ("MaxDps ");
//					tp_list.Add ("+ " + tp_values[i]);
//					break;
//				case 9:
//					tp_list.Add ("Speed ");
//					tp_list.Add ("+ " + tp_values[i]);
//					break;
//				case 10:
//					tp_list.Add ("FireResistance ");
//					tp_list.Add ("+ " + tp_values[i]);
//					break;
//				case 11:
//					tp_list.Add ("IceResistance ");
//					tp_list.Add ("+ " + tp_values[i]);
//					break;
//				case 12:
//					tp_list.Add ("LightningResistance ");
//					tp_list.Add ("+ " + tp_values[i]);
//					break;
//				}
//			}
//		}
//
//		tp_game.FindChild ("Name").GetComponent<Text> ().text = appliedItem.Name;
//		tp_game.FindChild ("Value").GetComponent<Text> ().text = appliedItem.Value + "";
//		tp_game.FindChild ("Rarity").GetComponent<Text> ().text = appliedItem.Rarity;
//		tp_game.FindChild ("ItemLevel").GetComponent<Text> ().text = appliedItem.ItemLevel + "";
//		try{
//			tp_game.FindChild ("Stat1").GetComponent<Text> ().text = tp_list[0] + tp_list[1];
//		}catch{
//			tp_game.FindChild ("Stat1").GetComponent<Text> ().text = "";
//		}
//		try{
//			tp_game.FindChild ("Stat2").GetComponent<Text> ().text = tp_list[2] + tp_list[3];
//
//		}catch{
//			tp_game.FindChild ("Stat2").GetComponent<Text> ().text = "";
//		}
//		try{
//			tp_game.FindChild ("Stat3").GetComponent<Text> ().text = tp_list[4] + tp_list[5];
//
//		}catch{
//			tp_game.FindChild ("Stat3").GetComponent<Text> ().text = "";
//		}
//		try{
//			tp_game.FindChild ("Stat4").GetComponent<Text> ().text = tp_list[6] + tp_list[7];
//
//		}catch{
//			tp_game.FindChild ("Stat4").GetComponent<Text> ().text = "";
//		}
//	}

	#region IPointerClickHandler implementation
	public void OnPointerClick (PointerEventData eventData){

		if (transform.parent.name == "InventorySlot") {
		
			if (eventData.button == PointerEventData.InputButton.Right) {
			
				Transform[] tp_slots = Inventory.k_inventory.tp_characterslots;

				int RingCount = 0;
				Transform FirstRing =null;

				Transform tp_child = null;

				Transform destination = null;

				for (int i = 0; i < tp_slots.Length; i++) {
					if (tp_slots [i].GetComponent<Slot> ().Description == appliedItem.Type) {

						if (tp_slots [i].childCount > 0) {
						
							if (appliedItem.Type == "Ring" && RingCount==0) {
								FirstRing = tp_slots [i];
								RingCount++;
								continue;
							}
							tp_child = tp_slots [i].GetChild (0);
							tp_child.SetParent (transform.parent);
							tp_child.GetComponent<ItemManager> ().appliedItem.Position = transform.parent.GetComponent<Slot> ().position;
							tp_child.localPosition = new Vector3 (0, 0, 0);
						}
						if (appliedItem.Type == "Ring" && FirstRing !=null && tp_slots [i].childCount > 0) {
							transform.SetParent (FirstRing);
							destination = FirstRing;
						}else{
							transform.SetParent (tp_slots [i]);
							destination = tp_slots [i];
						}
						transform.localPosition = new Vector3(0,0,0);
						Inventory.k_inventory.UpdateThis ();
						break;
					}
				}

				appliedItem.Position = destination.GetComponent<Slot>().position;
				CreationManager.k_Manager.MakeInventoryString ();
				return;
			}

			if (eventData.button == PointerEventData.InputButton.Left && Input.GetKey(KeyCode.LeftShift)) {

				Transform[] tp_slots = Inventory.k_inventory.tp_shopslots;

				for (int i = 0; i < tp_slots.Length; i++) {

					if (tp_slots [i].childCount == 0) {

						transform.SetParent (tp_slots [i]);
						CreationManager.k_Manager.MakeInventoryString ();
						return;
					}
				}		
			}


		}

		if (transform.parent.name == "CharacterSlot") {
			if (eventData.button == PointerEventData.InputButton.Right) {
		
				Transform[] tp_slots = Inventory.k_inventory.tp_inventoryslots;

				for (int i = 0; i < tp_slots.Length; i++) {
				
					if (tp_slots [i].childCount == 0) {
					
						transform.SetParent (tp_slots [i]);
						Inventory.k_inventory.UpdateThis ();
						CreationManager.k_Manager.MakeInventoryString ();
						return;
					}
				}		
			}		
		}

		if (transform.parent.name == "LootSlots") {
			if (eventData.button == PointerEventData.InputButton.Left && Input.GetKey(KeyCode.LeftShift)) {

				Transform[] tp_slots = Inventory.k_inventory.tp_inventoryslots;

				for (int i = 0; i < tp_slots.Length; i++) {

					if (tp_slots [i].childCount == 0) {

						transform.SetParent (tp_slots [i]);
						CreationManager.k_Manager.MakeInventoryString ();
						return;
					}
				}		
			}		
		}

		if (transform.parent.name == "ShopSlot") {
			if (eventData.button == PointerEventData.InputButton.Left && Input.GetKey(KeyCode.LeftShift)) {

				Transform[] tp_slots = Inventory.k_inventory.tp_inventoryslots;

				for (int i = 0; i < tp_slots.Length; i++) {

					if (tp_slots [i].childCount == 0) {

						transform.SetParent (tp_slots [i]);
						CreationManager.k_Manager.MakeInventoryString ();
						return;
					}
				}		
			}		
		}
	}
	#endregion


	#region Show/Hide text
	public void ShowText(){

		Vector3 tp_startingPosition = tp_game.position;
		tp_startingPosition = new Vector3 (tp_startingPosition.x+75,tp_startingPosition.y,tp_startingPosition.z);
		Vector3 tp_view = Camera.main.ScreenToViewportPoint (tp_startingPosition);
		tp_game.gameObject.SetActive (true);
		if (tp_view.x > 1.0f) {
			tp_game.localPosition += new Vector3 (-200, 0, 0);
			changed = true;
		} else {
			tp_view = Camera.main.ScreenToViewportPoint (new Vector3 (tp_startingPosition.x + 200, tp_startingPosition.y, tp_startingPosition.z));
			if(changed && tp_view.x < 1.0f)
			tp_game.localPosition += new Vector3 (200, 0, 0);
		}
	}

	public void HideText(){
		tp_game.gameObject.SetActive (false);

	}
	#endregion
}
