using UnityEngine;
using System.Collections;
using System.Linq;
using System.IO;

public class OnlineData : MonoBehaviour {

	public bool DataReceived = false;

	public string[] tp_data = new string[20];
	public string[] tp_dataItems = new string[100];
	public string[] tp_dataSkills = new string[100];
	public string[] tp_dataEnemySkills = new string[100];
	public string[] tp_dataInventoryItems = new string[100];
	public string[] tp_dataCharacters = new string[30];

	public static OnlineData k_storage;
	public Sprite[] k_Skillimages;
	public SkillData skill_data;

	void Awake(){
	
		tp_data[0]= "1";
		tp_dataItems[0]= "1";
		tp_dataSkills[0]= "1";
		tp_dataEnemySkills[0]= "1";
		tp_dataInventoryItems[0]= "1";
		tp_dataCharacters[0]= "1";




		k_storage = this;
	
	}
	void Start(){
		k_Skillimages = Resources.LoadAll<Sprite> ("Skills");
		skill_data = SkillData.Load(Path.Combine(Application.dataPath, "Skills.xml"));

		print (skill_data.PriestSkills [0].Name);
	}
}
