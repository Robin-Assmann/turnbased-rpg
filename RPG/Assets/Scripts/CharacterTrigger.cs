using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CharacterTrigger : MonoBehaviour {

	private Color startcolor, activated;

	public bool started {get; set;}
	public bool clickable {get; set;}

	private GameObject k_HighlightPanel;

	void Awake(){
		clickable = false;
		k_HighlightPanel = GameObject.FindGameObjectWithTag("HighlightPanel");
		started = false;
		startcolor = transform.GetComponent<Renderer> ().material.color;
	}

	void OnMouseDown(){
		
		if ((GameManager.k_Manager.SkillWaiting && transform.parent.FindChild ("Character(Clone)") != null && clickable) || clickable) {
			GameManager.k_Manager.Target = gameObject;
			GameManager.k_Manager.TargetChosen = true;
		}
	}

	void OnMouseEnter(){

		if (transform.parent.FindChild ("Character(Clone)") != null  && !started) {
			Character tp_character = transform.parent.FindChild ("Character(Clone)").GetComponent<CharacterClass> ().k_Character;
			TriggerHighlight (true);

			k_HighlightPanel.transform.FindChild ("ArmorNumber").GetComponent<Text> ().text = tp_character.Armor +"";
			k_HighlightPanel.transform.FindChild ("HealthNumber").GetComponent<Text> ().text = tp_character.Health +"";
			k_HighlightPanel.transform.FindChild ("DamageNumber").GetComponent<Text> ().text = tp_character.MinDPS + " - "+ tp_character.MaxDPS;
			k_HighlightPanel.transform.FindChild ("SpeedNumber").GetComponent<Text> ().text = tp_character.Speed +"";
		}
	}

	void OnMouseExit(){

		if (transform.parent.FindChild ("Character(Clone)") != null && !started) {
			
			TriggerHighlight (false);
		}
	}

	public void TriggerHighlight(bool f){
	
		if (f) {
			if (transform.GetComponent<Renderer> ().materials.Length == 1) {
				transform.GetComponent<Renderer> ().material.color = new Color (0.0f, 0.0f, 1.0f, 0.1f);
			}else {
				transform.GetComponent<Renderer> ().materials[1].color = new Color (0.0f, 0.0f, 1.0f, 0.1f);
			}

		} else {

			if (transform.GetComponent<Renderer> ().materials.Length == 1) {
				transform.GetComponent<Renderer> ().material.color = startcolor;
			}else {
				transform.GetComponent<Renderer> ().materials[1].color = startcolor;
			}
		}
	}

	public void TriggerStarted(){
	
		started = !started;
	}

	public void ChangeColor(float r, float g, float b, float a){

		if (transform.GetComponent<Renderer> ().materials.Length == 1) {
			transform.GetComponent<Renderer> ().material.color = new Color (r, g, b, a);
		} else {
			transform.GetComponent<Renderer> ().materials[1].color = new Color (r, g, b, a);
		}
	}

	public void ChangeColorBack(){
		if (transform.GetComponent<Renderer> ().materials.Length == 1) {
			transform.GetComponent<Renderer> ().material.color = startcolor;
		} else {
			transform.GetComponent<Renderer> ().materials[1].color = startcolor;
		}
	}
}
