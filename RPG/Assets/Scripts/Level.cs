using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level{

	public int ID;
	public string Name;

	// Syntax = MinMonsterCount-MaxMonsterCount
	public string CommonCount;
	public string RareCount;
	public string EliteCount;
	public string LegendaryCount;

	// Syntax = MonsterID/MonsterID/MonsterID
	public string CommonMobs;
	public string RareMobs;
	public string EliteMobs;
	public string LegendaryMobs;

}
