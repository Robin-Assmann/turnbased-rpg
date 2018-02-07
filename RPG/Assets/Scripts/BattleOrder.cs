using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BattleOrder : MonoBehaviour {

	public Image[] k_OrderList { get; set;}

	void Awake () {

		List<Image> tp_List = new List<Image> ();
		Component[] tp_Images = GetComponentsInChildren<Image> ();
		foreach (Image tp in tp_Images) {
			tp_List.Add (tp);
		}
		tp_List.RemoveAt (0);
		k_OrderList = tp_List.ToArray ();
	}
}
