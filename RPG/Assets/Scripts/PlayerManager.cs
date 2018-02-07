using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;


//Storage for Player things
public class PlayerManager : MonoBehaviour {


	public static PlayerManager k_manager;

	public bool isMultiplayer;

	public List<int> GroupIDs;

	public Sprite[] k_Skillimages;
	public SkillData skill_data;

	void Awake () {

		isMultiplayer = false;
		k_manager = this;

		GroupIDs = new List<int> ();
		k_Skillimages = Resources.LoadAll<Sprite> ("Skills");
	}

}
