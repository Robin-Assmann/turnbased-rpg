using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempCharacter{

	public int ID;
	public string Name;
	public string Type;

	public int Health;
	public int Armor;
	public float Dodge;

	public string Position;

	public int MinDPS;
	public int MaxDPS;

	public int Speed;

	public int FireResistance;
	public int IceResistance;
	public int LightningResistance;

	public string SkillString;
	public int expGain;

	public Character Init(){
	
		//bool tp_isEnemy ,int tp_characterid, string tp_charactername ,int tp_maxhealth,int tp_currenthealth, int tp_armor, float tp_dodge, int tp_position, int tp_mindps, int tp_maxdps, int tp_speed, string tp_type, int tp_fire, int tp_ice, int tp_shock, string tp_skills, string tp_slots, string tp_backpack, int tp_exp, int tp_strength, int tp_dexterity, int tp_intelligence, int tp_magicdamage
		Character temp_char = new Character(true ,ID*-1, Name ,Health ,Health , Armor, Dodge, int.Parse(Position), MinDPS, MaxDPS, Speed, Type, FireResistance, IceResistance, LightningResistance, SkillString, null, null, 0, 0, 0, 0, 0);
		temp_char.expGain = this.expGain;
		return temp_char;
	
	}

	public string ToString(){
	
		return Position + "|" + Health + "|" + Armor + "|" + Dodge + "|" + MinDPS + "|" + MaxDPS + "|" + Speed + "|" + FireResistance + "|" + IceResistance + "|" + LightningResistance + "|" + SkillString + "|" + expGain;
	}
}
