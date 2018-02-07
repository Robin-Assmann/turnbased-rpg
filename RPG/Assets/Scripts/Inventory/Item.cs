using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class Item {

	public int Position { get; set;}

	public int ID { get; set;}
	public string Name { get; set;}
	public string Type { get; set;}
	public string Rarity { get; set;}

	public int Value { get; set;}
	public int ItemLevel{ get; set;}

	public int ChangeStrength { get; set;}
	public int ChangeDexterity { get; set;} 
	public int ChangeIntelligence { get; set;} 

	public int ChangeMagicDamage { get; set;}

	public int ChangeHealth { get; set;}
	public int ChangeArmor { get; set;}
	public float ChangeDodge { get; set;}
	public int ChangeMinDps { get; set;}
	public int ChangeMaxDps { get; set;}
	public int ChangeSpeed { get; set;}
	public int ChangeResFire { get; set;}
	public int ChangeResIce { get; set;}
	public int ChangeResLightning { get; set;}

	public int[] ChangeStats;

	public Image appliedImage { get; set;}

	public int Skill { get; set;}


	public Item (int tp_id, string tp_name,string tp_type, string tp_rarity,int tp_itemlevel, int tp_value, int tp_position,int tp_strength,int tp_constitution,int tp_dexterity, int tp_intelligence, int tp_luck,  int tp_health, int tp_armor, int tp_mindps, int tp_maxdps, float tp_dodge, int tp_speed, int tp_fire, int tp_ice,int tp_light,int tp_damage, int tp_critdamage, int tp_magicdamage, int tp_critmagic, int tp_critchance,int tp_weight, int tp_magicper,int tp_goldper){

		this.ID = tp_id;
		this.Name = tp_name;
		this.Type = tp_type;
		this.Rarity = tp_rarity;
		this.ItemLevel = tp_itemlevel;
		this.Value = tp_value;
		this.Position = tp_position;

		this.ChangeStats = new int[22];

		this.ChangeStats [0] = tp_strength;
		this.ChangeStats [1] = tp_constitution;
		this.ChangeStats [2] = tp_dexterity;
		this.ChangeStats [3] = tp_intelligence;
		this.ChangeStats [4] = tp_luck;

		this.ChangeStats [5] = tp_health;
		this.ChangeStats [6] = tp_armor;
		this.ChangeStats [7] = tp_mindps;
		this.ChangeStats [8] = tp_maxdps;
		this.ChangeStats [9] = (int) tp_dodge;
		this.ChangeStats [10] = tp_speed;
		this.ChangeStats [11] = tp_fire;
		this.ChangeStats [12] = tp_ice;
		this.ChangeStats [13] = tp_light;

		this.ChangeStats [14] = tp_damage;
		this.ChangeStats [15] = tp_critdamage;
		this.ChangeStats [16] = tp_magicdamage;
		this.ChangeStats [17] = tp_critmagic;
		this.ChangeStats [18] = tp_critchance;
		this.ChangeStats [19] = tp_weight;
		this.ChangeStats [20] = tp_magicper;
		this.ChangeStats [21] = tp_goldper;


		this.ChangeStrength = tp_strength;
		this.ChangeDexterity = tp_dexterity;
		this.ChangeIntelligence = tp_intelligence;
		this.ChangeMagicDamage = tp_magicdamage;

		this.ChangeHealth = tp_health;
		this.ChangeArmor = tp_armor;
		this.ChangeDodge = tp_dodge;
		this.ChangeMinDps = tp_mindps;
		this.ChangeMaxDps = tp_maxdps;
		this.ChangeSpeed = tp_speed;
		this.ChangeResFire = tp_fire;
		this.ChangeResIce = tp_ice;
		this.ChangeResLightning = tp_light;

		Debug.Log ("item loaded"+ID);
	}
}
