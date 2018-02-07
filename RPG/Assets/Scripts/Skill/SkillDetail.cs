using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDetail{

	public int ID;
	public List<int> FirstRow, SecondRow, ThirdRow;
	public int Ultimate;

	public SkillDetail(int id, string first, string second, string third, string ultimate){
	
		this.ID = id;
		this.FirstRow = new List<int> ();
		this.SecondRow = new List<int> ();
		this.ThirdRow = new List<int> ();

		string[] tp_first = first.Split ("/" [0]);
		for (int i = 0; i < tp_first.Length; i++) {
			this.FirstRow.Add (int.Parse (tp_first [i]));
		}
		string[] tp_second = second.Split ("/" [0]);
		for (int i = 0; i < tp_second.Length; i++) {
			this.SecondRow.Add (int.Parse (tp_second [i]));
		}
		string[] tp_third = third.Split ("/" [0]);
		for (int i = 0; i < tp_third.Length; i++) {
			this.ThirdRow.Add (int.Parse (tp_third [i]));
		}
	
		this.Ultimate = int.Parse (ultimate);
	}
}
