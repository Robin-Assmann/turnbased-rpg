using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeDetail{

	public int Type; //0=Dmg 1=Crit% 2=AOE+1 3=Twice
	public int Value;
	public int UpgradeCount;
	public int StartCost;
	public float CostIncrease;

	public string Description;

	public UpgradeDetail(){
	}

	public UpgradeDetail(int type, int value, int count, string costs){

		this.Type = type;
		this.Value = value;
		this.UpgradeCount = count;

		string[] t = costs.Split ("-" [0]);

		this.StartCost = int.Parse (t [0]);
		this.CostIncrease = float.Parse (t [1]);

		switch (type) {
		case 0:
			this.Description = "Increase Damage by " + value + "%.";
			break;
		case 1:
			this.Description = "Increase Critical Chance by " + value + "%";
			break;
		case 2:
			this.Description = "Increase Enemies hit by " + value ;
			break;
		case 3:
			this.Description = "Attack will repeat 1 time.";
			break;
		}

	}
}
