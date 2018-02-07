using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler {
	public static GameObject itemBeingDragged;
	Vector3 startPosition;
	public static Transform startParent;
	Transform dragparent;

	public Character appliedCharacter;

	public bool pos, shouldhave;
	public int Position_ID;

	void Start(){

		if(pos !=true)
		pos = false;
	}

	#region IBeginDragHandler implementation

	public void OnBeginDrag (PointerEventData eventData){
		if (appliedCharacter != null) {
			Click ();
		}
		dragparent = GameObject.FindGameObjectWithTag ("Overview").transform;
		itemBeingDragged = gameObject;
		startPosition = transform.position;
		startParent = transform.parent;
		transform.SetParent (dragparent, false);
		if (startParent.name != "PositionSlot") {
			CreationManager.k_Manager.ItemsBlockRaycast (false);
		} else {
			GetComponent<CanvasGroup> ().blocksRaycasts = false;
		}
	}

	#endregion

	#region IDragHandler implementation

	public void OnDrag (PointerEventData eventData)
	{
		transform.position = eventData.position;
	}

	#endregion

	#region IEndDragHandler implementation

	public void OnEndDrag (PointerEventData eventData){
		itemBeingDragged = null;
		if (startParent.name != "PositionSlot") {
			CreationManager.k_Manager.ItemsBlockRaycast (true);
		} else {
			GetComponent<CanvasGroup> ().blocksRaycasts = true;
		}
		if ((transform.parent == startParent) || (transform.parent == dragparent)) {
			transform.SetParent (startParent, false);
			transform.position = startPosition;
		} else {
			if (appliedCharacter == null && shouldhave) {
				appliedCharacter = CreationManager.k_Manager.currentCharacter;
			}
		}
	}
	#endregion

	public void Click(){
		if (appliedCharacter != null && transform.parent.tag!="CharacterPositionSlot") {
			CreationManager.k_Manager.currentCharacter = appliedCharacter;
			CreationManager.k_Manager.DeleteItems ();
			CreationManager.k_Manager.ResetSkillTabs ();
			CreationManager.k_Manager.InstantiateCharacterItems ();
		}
	}

	#region IPointerClickHandler implementation
	public void OnPointerClick (PointerEventData eventData){
		Click ();
	}
	#endregion

}
