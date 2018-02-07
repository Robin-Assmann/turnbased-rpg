using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Inventory : MonoBehaviour, IHasChanged {
	[SerializeField] Transform slots;
	[SerializeField] Transform inventory;

	public Transform[]	tp_characterslots; //without backpack
	public Transform[] tp_slots; // all character slots with backpack
	public Transform[] tp_backpackslots; // backpack
	public Transform[] tp_inventoryslots; // Inventory
	public Transform[] tp_shopslots; //Shop

	public static Inventory k_inventory;

	private int temp_str;
	private int temp_dex;
	private int temp_int;
	private int temp_con;
	private int temp_lck;

	private int temp_hp;
	private int temp_armor;
	private float temp_dodge;
	private int temp_speed;
	private int temp_fireres;
	private int temp_iceres;
	private int temp_lightres;

	private int  tp_strength;
	private int  tp_constitution;
	private int  tp_dexterity;
	private int  tp_intelligence;
	private int  tp_luck;

	private int  tp_health;
	private int  tp_armor;
	private int  tp_mindps;
	private int  tp_maxdps;
	private int  tp_dodge;
	private int  tp_speed;
	private int  tp_fire;
	private int  tp_ice;
	private int  tp_light;

	private int  tp_damage;
	private int  tp_critdamage;
	private int  tp_magicdamage;
	private int  tp_critmagic;
	private int  tp_critchance;
	private int  tp_weight;
	private int  tp_magicper;
	private int  tp_goldper;



	void Awake(){
		k_inventory = this;

		Transform tp_Shop = GameObject.FindGameObjectWithTag ("ShopSlots").transform;
		tp_shopslots = new Transform[14];

		for (int i = 0; i < tp_Shop.childCount; i++) {
			tp_shopslots[i] = tp_Shop.GetChild (i);
		}

		tp_characterslots = new Transform[10];
		tp_slots = new Transform[12];
		tp_inventoryslots = new Transform[36];

		for (int i = 0; i < tp_inventoryslots.Length; i++) {
			tp_inventoryslots [i] = inventory.GetChild(i);
		}

		for (int i = 0; i < slots.childCount; i++) {
			tp_slots [i] = slots.GetChild (i);
			try{
				tp_slots [i].GetComponent<Slot> ().position = i-1;}
			catch{}
		}
		List<Transform> tp_list = new List<Transform> (tp_slots);
		List<Transform> tp_backpacklist = new List<Transform>();
		tp_list.RemoveAt (0);
		tp_list.RemoveAt (10);
		tp_characterslots = tp_list.ToArray ();

		for (int i = 0; i < tp_slots[11].childCount; i++) {
			tp_list.Add (tp_slots [11].GetChild (i));
			tp_backpacklist.Add (tp_slots [11].GetChild (i));
			tp_slots [11].GetChild (i).GetComponent<Slot> ().position = i;
		}
		tp_backpackslots = tp_backpacklist.ToArray ();
		tp_slots = tp_list.ToArray ();
	}

	#region IHasChanged implementation
	public void HasChanged (bool tp_bool, bool tp_char1){

		if (tp_bool) {
			if (tp_char1) {
				UpdateThis ();
			} else {
				CreationManager.k_Manager.MakeInventoryString ();
			}
		}else {
			int tp_pos = 0;
			for (int i = 0; i < CreationManager.k_Manager.Positions.Length; i++) {
				tp_pos = i;
				if (CreationManager.k_Manager.Positions [i].transform.childCount > 0 && CreationManager.k_Manager.Positions[i].transform.GetChild(0).name!="FriendPositionImage(Clone)" && CreationManager.k_Manager.Positions[i].transform.GetChild(0).transform.GetComponent<DragHandler>().Position_ID ==CreationManager.k_Manager.currentCharacter.ID) {
					CreationManager.k_Manager.currentCharacter.Position = tp_pos;

					RaiseEventOptions opt = new RaiseEventOptions ();
					opt.TargetActors = PlayerManager.k_manager.GroupIDs.ToArray();
					PhotonNetwork.RaiseEvent (20,new string[]{CreationManager.k_Manager.Positions[i].transform.GetChild(0).transform.GetComponent<DragHandler>().Position_ID+"",tp_pos+""}, true, opt);

					break;
				}
			}
		}
	}
	#endregion

	#region UpdateText/ Attributes
	public void UpdateThis(){
	
		CreateTemps ();
		foreach (Transform slotTransform in tp_characterslots) {
			GameObject item = slotTransform.GetComponent<Slot> ().item;
			if (item) {
				item.GetComponent<ItemManager> ().equipped = true;
				ChangeAttributes (item.GetComponent<ItemManager> ().appliedItem);
			}
		}
		ApplyTemps ();
		CreationManager.k_Manager.ChangeTexts ();
		StartCoroutine (CreationManager.k_Manager.currentCharacter.UpdateCharacterSlot());
	}

	public void CreateTemps(){
		temp_hp = 0;
		temp_armor = 0;
		temp_dodge = 0;
		temp_speed = 0;
		temp_fireres = 0;
		temp_iceres = 0;
		temp_lightres = 0;
		temp_str = 0;
		temp_int = 0;
		temp_dex = 0;

		tp_strength = 0;
		tp_constitution = 0;
		tp_dexterity = 0;
		tp_intelligence = 0;
		tp_luck = 0;

		tp_health = 0;
		tp_armor = 0;
		tp_mindps = 0;
		tp_maxdps = 0;
		tp_dodge = 0;
		tp_speed = 0;
		tp_fire = 0;
		tp_ice = 0;
		tp_light = 0;

		tp_damage = 0;
		tp_critdamage = 0;
		tp_magicdamage = 0;
		tp_critmagic = 0;
		tp_critchance = 0;
		tp_weight = 0;
		tp_magicper = 0;
		tp_goldper = 0;

	}

	public void ChangeAttributes(Item tp_item){
		temp_hp += tp_item.ChangeHealth;
		temp_armor+= tp_item.ChangeArmor;
		temp_dodge+=tp_item.ChangeDodge /100;
		temp_speed+=tp_item.ChangeSpeed;
		temp_fireres+=tp_item.ChangeResFire;
		temp_iceres+= tp_item.ChangeResIce;
		temp_lightres+= tp_item.ChangeResLightning;
		temp_str += tp_item.ChangeStrength;
		temp_dex += tp_item.ChangeDexterity;
		temp_int += tp_item.ChangeIntelligence;

		tp_strength += tp_item.ChangeStats[0];
		tp_constitution += tp_item.ChangeStats[1];
		tp_dexterity += tp_item.ChangeStats[2];
		tp_intelligence += tp_item.ChangeStats[3];
		tp_luck += tp_item.ChangeStats[4];

		tp_health += tp_item.ChangeStats[5];
		tp_armor += tp_item.ChangeStats[6];
		tp_mindps += tp_item.ChangeStats[7];
		tp_maxdps += tp_item.ChangeStats[8];
		tp_dodge += tp_item.ChangeStats[9];
		tp_speed += tp_item.ChangeStats[10];
		tp_fire += tp_item.ChangeStats[11];
		tp_ice += tp_item.ChangeStats[12];
		tp_light += tp_item.ChangeStats[13];

		tp_damage += tp_item.ChangeStats[14];
		tp_critdamage += tp_item.ChangeStats[15];
		tp_magicdamage += tp_item.ChangeStats[16];
		tp_critmagic += tp_item.ChangeStats[17];
		tp_critchance += tp_item.ChangeStats[18];
		tp_weight += tp_item.ChangeStats[19];
		tp_magicper += tp_item.ChangeStats[20];
		tp_goldper += tp_item.ChangeStats[21];
	}

	public void ApplyTemps(){

		Character tp_char = CreationManager.k_Manager.currentCharacter;
//		tp_char.MaxHealth = tp_char.Base_Health + temp_hp;
//		tp_char.Armor = tp_char.Base_Armor + temp_armor;
//		tp_char.Dodge = tp_char.Base_Dodge + temp_dodge;
//		tp_char.Speed = tp_char.Base_Speed + temp_speed;
//		tp_char.FireResistance = tp_char.Base_FireResistance + temp_fireres;
//		tp_char.IceResistance = tp_char.Base_IceResistance + temp_iceres;
//		tp_char.LightningResistance = tp_char.Base_LightningResistance + temp_lightres;
//
//		tp_char.Strength = tp_char.Base_Strength + temp_str;
//		tp_char.Dexterity = tp_char.Base_Dexterity + temp_dex;
//		tp_char.Intelligence = tp_char.Base_Intelligence + temp_int;

		tp_char.Strength = tp_char.Base_Strength + tp_strength;
		tp_char.Constituition = tp_char.Base_Constituition + tp_constitution;
		tp_char.Dexterity = tp_char.Base_Dexterity + tp_dexterity;
		tp_char.Intelligence = tp_char.Base_Intelligence + tp_intelligence;
		tp_char.Luck = tp_char.Base_Luck + tp_luck;

		tp_char.MaxHealth = tp_char.Base_Health + tp_health +(tp_strength*2+tp_constitution*5)*2;
		tp_char.Armor = tp_char.Base_Armor + tp_armor + tp_constitution;
		tp_char.MinDPS = tp_char.Base_MinDPS + tp_mindps;
		tp_char.MaxDPS = tp_char.Base_MaxDPS + tp_maxdps;
		tp_char.Dodge = tp_char.Base_Dodge + tp_dodge + (tp_dexterity*3)/25;
		tp_char.Speed = tp_char.Base_Speed + tp_speed + tp_dexterity*4;
		tp_char.FireResistance = tp_char.Base_FireResistance + tp_fire +tp_constitution+tp_intelligence;
		tp_char.IceResistance = tp_char.Base_IceResistance + tp_ice +tp_constitution+tp_intelligence;
		tp_char.LightningResistance = tp_char.Base_LightningResistance + tp_light +tp_constitution+tp_intelligence;

		tp_char.Damage = tp_char.Base_Damage + tp_damage + tp_strength*4 + tp_dexterity;
		tp_char.CritDamage = tp_char.Base_CritDamage + tp_critdamage + tp_strength*2;
		tp_char.MagicDamage = tp_char.Base_MagicDamage + tp_magicdamage+tp_intelligence*5;
		tp_char.CritMagicDamage = tp_char.Base_CritMagicDamage + tp_critmagic + tp_intelligence*2;
		tp_char.CritChance = tp_char.Base_CritChance + tp_critchance + tp_dexterity*2 + tp_luck*6;
		tp_char.Weight = tp_char.Base_Weight + tp_weight +(tp_strength*4+tp_constitution)*1.5f;
		tp_char.MagicPercent = tp_char.Base_MagicPercent + tp_magicper + tp_luck*2;
		tp_char.GoldPercent = tp_char.Base_GoldPercent + tp_goldper + tp_luck*2;
	}

	#endregion
}
namespace UnityEngine.EventSystems {
	public interface IHasChanged : IEventSystemHandler {
		void HasChanged(bool tp_char, bool tp_char1);
	}
}
