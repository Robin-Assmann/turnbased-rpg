using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreation : MonoBehaviour {


	[SerializeField]
	GameObject NameInput, TypeDrop, RaceDrop, AttributeField;

	[SerializeField]
	GameObject StrField, ConField, DexField, IntField, LuckField;

	[SerializeField]
	GameObject Dmg, DmgCrit, MgDmg, MgDmgCrit, CritChance, HP, Dodge, Speed, FireRes, IceRes, LightRes, Armor, Weight;

	private int AttributePoints;

	private int Str, Con, Dex, Int, Luck;
	private int Att_Dmg, Att_DmgCrit, Att_MgDmg, Att_MgDmgCrit, Att_CritChance, Att_HP, Att_Dodge, Att_Speed, Att_FireRes, Att_IceRes, Att_LightRes, Att_Armor, Att_Weight;


	bool changed = false;

	void Awake(){

		AttributePoints = 10;
		Str = Con = Dex = Int = Luck = 3;
		Att_Dmg = Att_DmgCrit = Att_MgDmg = Att_MgDmgCrit = Att_CritChance = Att_HP = Att_Dodge = Att_Speed = Att_FireRes = Att_IceRes = Att_LightRes = Att_Armor = Att_Weight = 0;
		UpdateAll ();
	}

	public void Create(){

		string name = NameInput.GetComponent<InputField> ().text;
		List<Dropdown.OptionData> types = TypeDrop.GetComponent<Dropdown> ().options;
		string type = types [TypeDrop.GetComponent<Dropdown> ().value].text;
		List<Dropdown.OptionData> races = RaceDrop.GetComponent<Dropdown> ().options;
		string race = races [RaceDrop.GetComponent<Dropdown> ().value].text;

		GameObject tp_player = GameObject.FindGameObjectWithTag ("Player");
		int id = tp_player.GetComponent<Player> ().PlayerID;
	
		string[] content = new string[] {"('"+id+"','"+name+"','3','"+race+"','"+type+"','"+Att_HP+"','"+Att_HP+"','16','1/1/1/1','0','"+Str+"','"+Con+"','"+Dex+"','"+Int+"','"+Luck+"','0/0/0/0/0/0/0/0/0/0','0/0/0/0/0/0/0/0')"};
		print (content [0]);
		RaiseEventOptions opt = new RaiseEventOptions ();
		opt.Receivers = ReceiverGroup.MasterClient;
		PhotonNetwork.RaiseEvent (16, content, false, opt);
	}

	public void ChangeInput (){
	
		switch (RaceDrop.GetComponent<Dropdown> ().value) {

		case 0:
			Int = 5;
			Con = 4;
			Str = Dex = Luck = 2;
			break;
		case 1:
			Int = 5;
			Dex = 4;
			Str = Con = Luck = 2;
			break;
		case 2:
			Con = 5;
			Str = 4;
			Int = Dex = Luck = 2;
			break;
		case 3:
			Con = 5;
			Dex = 4;
			Str = Dex = Luck = 2;
			break;
		case 4:
			Str = 5;
			Dex = 4;
			Int = Con = Luck = 2;
			break;
		case 5:
			Str = 5;
			Con = 4;
			Int = Dex = Luck = 2;
			break;
		case 6:
			Luck = 5;
			Dex = 4;
			Str = Con = Int = 2;
			break;
		case 7:
			Luck = 5;
			Int = 4;
			Str = Dex = Con = 2;
			break;
		case 8:
			Dex = 5;
			Luck = 4;
			Str = Con = Int = 2;
			break;
		case 9:
			Dex = 5;
			Int = 4;
			Str = Con = Luck = 2;
			break;
		}
		AttributePoints = 10;
		UpdateAll ();
	} 

	public void Randomize(){

		Str = Con = Dex = Int = Luck = 0;
		Att_Dmg = Att_DmgCrit = Att_MgDmg = Att_MgDmgCrit = Att_CritChance = Att_HP = Att_Dodge = Att_Speed = Att_FireRes = Att_IceRes = Att_LightRes = Att_Armor = Att_Weight = 0;

		int Points = 25;
		int change = 0;
		int value = 0;

		do {
			value = Random.Range(1,Mathf.FloorToInt(Points/5));

				switch(Random.Range(0,5)){

				case 0:	
					Str += value;
					Points-= value;
					break;

				case 1:
					Con += value;
					Points-= value;
					break;

				case 2:
					Dex += value;
					Points-= value;
					break;

				case 3:
					Int += value;
					Points-= value;
					break;

				case 4:
					Luck += value;
					Points-= value;
					break;
				}

		} while (Points >0);

		AttributePoints = 0;

		UpdateAll ();
	}

	public void IncreaseAttribute(int stat){

		if (AttributePoints > 0) {
			ChangeAttribute (stat, 1);
			if(changed)
			AttributePoints--;
			UpdateAttributePoints ();
		}
	}

	public void DecreaseAttribute(int stat){

		if (AttributePoints <= 25) {
			ChangeAttribute (stat, -1);
			if(changed)
			AttributePoints++;
			UpdateAttributePoints ();
		}
	}

	public void ChangeAttribute(int stat, int value){
		changed = false;
		switch (stat) {

		case 0:
			if (Str == 0 && value == -1) {
				break;
			}
			Str += value;
			changed = true;
			UpdateStrength ();
			break;
		case 1:
			if (Con == 0 && value==-1) {
				break;
			}
			Con += value;
			changed = true;
			UpdateConstituition ();

			break;
		case 2:
			if (Dex == 0 && value == -1) {
				break;
			}
			Dex += value;
			changed = true;
			UpdateDexterity ();
			
			break;
		case 3:
			if (Int == 0 && value == -1) {
				break;
			}
			Int += value;
			changed = true;
			UpdateIntelligence ();
			
			break;
		case 4:
			if (Luck == 0 && value == -1) {
				break;
			}
			Luck += value;
			changed = true;
			UpdateLuck ();
			
			break;
		}
	}

	#region Update
	void UpdateAttributePoints(){
	
		AttributeField.GetComponent<Text> ().text = AttributePoints + "";
	}

	void UpdateAll(){
		UpdateStrength ();
		UpdateConstituition ();
		UpdateDexterity ();
		UpdateIntelligence ();
		UpdateLuck ();
		UpdateAttributePoints ();
	}

	void UpdateStrength(){
	
		StrField.GetComponent<Text> ().text = Str + "";

		this.Att_Dmg = 4*Str+1*Dex;
		this.Att_DmgCrit = 2*Str;
		this.Att_HP = 2*Str+5*Con;
		this.Att_Weight = 2*Str+1*Con;

		Dmg.GetComponent<Text> ().text = Att_Dmg + "";
		DmgCrit.GetComponent<Text> ().text = Att_DmgCrit + "";
		Weight.GetComponent<Text> ().text = Att_Weight + "";
		HP.GetComponent<Text> ().text = Att_HP + "";
	
	}

	void UpdateConstituition(){

		ConField.GetComponent<Text> ().text = Con + "";

		this.Att_Weight = 2*Str+1*Con;
		this.Att_HP = 2*Str+5*Con;
		this.Att_Armor = 1*Con;
		this.Att_FireRes = 1*Con+1*Int;
		this.Att_IceRes = 1*Con+1*Int;
		this.Att_LightRes = 1*Con+1*Int;

		Weight.GetComponent<Text> ().text = Att_Weight + "";
		HP.GetComponent<Text> ().text = Att_HP + "";
		Armor.GetComponent<Text> ().text = Att_Armor + "";
		FireRes.GetComponent<Text> ().text = Att_FireRes + "";
		IceRes.GetComponent<Text> ().text = Att_IceRes + "";
		LightRes.GetComponent<Text> ().text = Att_LightRes + "";

	}
	void UpdateDexterity(){

		DexField.GetComponent<Text> ().text = Dex + "";

		this.Att_Dmg = 4*Str+1*Dex;
		this.Att_Speed = 4*Dex;
		this.Att_Dodge = 3*Dex;
		this.Att_CritChance = 2*Dex+6 *Luck;

		Dmg.GetComponent<Text> ().text = Att_Dmg + "";
		Speed.GetComponent<Text> ().text = Att_Speed + "";
		Dodge.GetComponent<Text> ().text = Att_Dodge + "";
		CritChance.GetComponent<Text> ().text = Att_CritChance + " %";

	}

	void UpdateIntelligence(){

		IntField.GetComponent<Text> ().text = Int + "";

		this.Att_MgDmg = 5*Int;
		this.Att_MgDmgCrit = 2*Int;
		this.Att_FireRes = 1*Con+1*Int;
		this.Att_IceRes = 1*Con+1*Int;;
		this.Att_LightRes = 1*Con+1*Int;

		MgDmg.GetComponent<Text> ().text = Att_MgDmg + "";
		MgDmgCrit.GetComponent<Text> ().text = Att_MgDmgCrit + "";
		FireRes.GetComponent<Text> ().text = Att_FireRes + "";
		IceRes.GetComponent<Text> ().text = Att_IceRes + "";
		LightRes.GetComponent<Text> ().text = Att_LightRes + "";

	}

	void UpdateLuck(){

		LuckField.GetComponent<Text> ().text = Luck + "";

		this.Att_CritChance = 2*Dex+6 *Luck;

		CritChance.GetComponent<Text> ().text = Att_CritChance + "";
	}
	#endregion
}
