using UnityEngine;
using System.Collections;

public class Action{

	public int TurnCount { get; set;}
	public int DeathCount { get; set;}
	public int ActionType { get; set;}	//0=dmg 1=heal 2=buff 3=debuff
	public Skill appliedSkill { get; set;}
	public int TurnCountCurrent { get; set;}
	public int BuffCounter{ get; set;}
	public Character appliedCharacter { get; set;}

	public Action(int tp_turn,int tp_type,Skill tp_skill, int tp_deathturn, int tp_buffcount, Character tp_char){
	
		this.TurnCount = tp_turn;
		this.appliedSkill = tp_skill;
		this.ActionType = tp_type;
		this.DeathCount = tp_deathturn;
		this.BuffCounter = tp_buffcount;
		this.appliedCharacter = tp_char;
	}

	void Start(){
	
		TurnCountCurrent = 0;
	}

}
