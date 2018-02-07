using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour ,IPointerClickHandler{


	public Skill appliedSkill;

	public bool triggered = false;
	public bool show = false;
	private Transform SkillImage;
	private List<Transform> otherImages;
	public int id = -1;

	void Start(){
		otherImages = new List<Transform>();
		if (show) {
			Transform par = transform.parent.parent;
			for (int i = 0; i < par.childCount; i++) {
				otherImages.Add (par.GetChild (i).GetChild (0));
			}
			otherImages.Remove (transform);
		}
	}

	public void OnPointerClick (PointerEventData eventData){
		if (show) { 
			if (!triggered) {
				this.triggered = true;
				transform.parent.GetComponent<Image> ().color = new Color (1.0f, 0.0f, 0.0f, 1.0f);
				GetComponent<Image> ().color = new Color (1.0f, 1.0f, 1.0f, 1.0f);


				foreach (Transform t in otherImages.ToArray()) {

					if (t.GetComponent<SkillManager> ().triggered) {
						t.GetComponent<SkillManager> ().triggered = false;
						t.parent.GetComponent<Image> ().color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
						t.GetComponent<Image> ().color = new Color (1.0f, 1.0f, 1.0f, 0.6f);

					}
				}
				CreationManager.k_Manager.selectedSkill = this.id;
				CreationManager.k_Manager.currentSkill = this.appliedSkill;
				CreationManager.k_Manager.InstantiateSkillTree ();
				CreationManager.k_Manager.SkillPanel.SetActive (true);

			} else {
				triggered = false;
				CreationManager.k_Manager.selectedSkill = -1;
				transform.parent.GetComponent<Image> ().color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
				GetComponent<Image> ().color = new Color (1.0f, 1.0f, 1.0f, 0.6f);
				CreationManager.k_Manager.SkillPanel.SetActive (false);
			}
		} else {
			if (!triggered) {
				this.triggered = true;
				Highlight ();
				CreationManager.k_Manager.currentSkill = this.appliedSkill;
				CreationManager.k_Manager.SelectSkill ();
				CreationManager.k_Manager.InstantiateSkillTree ();
			} else {
				this.triggered = false;
				transform.parent.GetComponent<Image> ().color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
				GetComponent<Image> ().color = new Color (1.0f, 1.0f, 1.0f, 0.6f);
				CreationManager.k_Manager.SkillPanel.SetActive (false);
			}
		}
	}

	public void ApplySkill(Skill tp_skill){

		appliedSkill = tp_skill;
		GetComponent<Image> ().sprite = appliedSkill.SkillImage;
	}

	public void ShowText(){

		Transform t = CreationManager.k_Manager.Description.transform;
		Vector2 pos = transform.position;
		t.GetChild (0).GetComponent<Text> ().text = appliedSkill.Name;
		t.GetChild (1).GetComponent<Text> ().text = appliedSkill.Description;
		if (pos.y + GetComponent<RectTransform> ().rect.height / 2 + 5 < 450)
			t.position = new Vector2 (pos.x, pos.y + GetComponent<RectTransform> ().rect.height / 2 + 5);
		else{
			t.position = new Vector2 (pos.x-t.GetComponent<RectTransform>().rect.width/2, pos.y - t.GetComponent<RectTransform>().rect.height/2);
		}
		t.gameObject.SetActive (true);

	}

	public void HideText(){

		CreationManager.k_Manager.Description.transform.gameObject.SetActive (false);

	}

	public void Highlight(){
		this.triggered = true;
		GetComponent<Image> ().color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
		transform.parent.GetComponent<Image> ().color = new Color (1.0f, 0.0f, 0.0f, 1.0f);
	
	}

	public void Deselect(){
		this.triggered = false;
		GetComponent<Image> ().color = new Color (1.0f, 1.0f, 1.0f, 0.6f);
		transform.parent.GetComponent<Image> ().color = new Color (1.0f, 1.0f, 1.0f, 1.0f);

	}
}
