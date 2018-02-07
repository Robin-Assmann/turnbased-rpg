using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class CreationApply : MonoBehaviour , IPointerClickHandler{


	public Character appliedCharacter;

	void Awake(){
	
		appliedCharacter = null;
		StartCoroutine (ApplyContext ());
	}

	IEnumerator ApplyContext(){
	
		yield return appliedCharacter != null;

		transform.FindChild ("NameText").GetComponent<Text> ().text = appliedCharacter.Name;
		transform.FindChild ("TypeText").GetComponent<Text> ().text = appliedCharacter.Type;
	}

	public void Click(){
	
		CreationManager.k_Manager.currentCharacter = appliedCharacter;
		CreationManager.k_Manager.DeleteItems ();
		CreationManager.k_Manager.ResetSkillTabs ();
		CreationManager.k_Manager.InstantiateCharacterItems ();

		if (appliedCharacter.Position == 16)
			CreationManager.k_Manager.AddCharacterPositionItem ();
	}

	#region IPointerClickHandler implementation
	public void OnPointerClick (PointerEventData eventData)
	{
		Click ();
	}
	#endregion
}
