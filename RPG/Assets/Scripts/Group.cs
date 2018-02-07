using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Group : MonoBehaviour {


	private Transform ParentPanel;
	private GameObject pre_GroupPlayer;
	public Color PlayerColor;

	public List<int> GroupIDs;

	public static Group k_Group;

	void Awake(){
	
		k_Group = this;
		GroupIDs = new List<int> ();

		pre_GroupPlayer = Resources.Load ("GroupPlayer") as GameObject;
	}


	public void AddToGroup(PhotonPlayer tp_player){
	
		ParentPanel = GameObject.FindGameObjectWithTag ("GroupPlayer").transform;
		GameObject tp_grpPlayer = Instantiate (pre_GroupPlayer) as GameObject;
		tp_grpPlayer.transform.SetParent (ParentPanel, false);
		tp_grpPlayer.transform.FindChild ("NameText").GetComponent<Text>().text = tp_player.name;

		float r = Random.value;
		float g = Random.value;
		float b = Random.value;

		PlayerColor = new Color (r, g, b, 1.0f);
		tp_grpPlayer.GetComponent<Image> ().color = PlayerColor;

	
		GroupIDs.Add (tp_player.ID);
	}
}
