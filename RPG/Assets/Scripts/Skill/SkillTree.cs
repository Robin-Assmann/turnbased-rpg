using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTree{

	public List<SkillDetail> skills;

	public SkillTree(string tp_skills){

		skills = new List<SkillDetail> ();
		string[] Skills = tp_skills.Split (";" [0]); //20|0/0|-1/-1|-1|-1

		for (int i = 0; i < Skills.Length; i++) {
			string[] tp_s = Skills [i].Split ("|" [0]);
			skills.Add(new SkillDetail(int.Parse(tp_s[0]),tp_s[1],tp_s[2],tp_s[3],tp_s[4]));
		}
	}

	public string MakeSkillTreeText(){
	
		string outs ="";
		foreach(SkillDetail s in skills.ToArray()) {
		
			outs= outs+ s.ID+"|";
			for (int i = 0; i < s.FirstRow.Count; i++) {
				if (i != s.FirstRow.Count - 1) {
					outs = outs + s.FirstRow [i] + "/";
				} else {
					outs = outs + s.FirstRow [i] + "|";
				}
			}
			for (int i = 0; i < s.SecondRow.Count; i++) {
				if (i != s.SecondRow.Count - 1) {
					outs = outs + s.SecondRow [i] + "/";
				} else {
					outs = outs + s.SecondRow [i] + "|";
				}
			}
			for (int i = 0; i < s.ThirdRow.Count; i++) {
				if (i != s.ThirdRow.Count - 1) {
					outs = outs + s.ThirdRow [i] + "/";
				} else {
					outs = outs + s.ThirdRow [i] + "|";
				}
			}
			outs= outs+ s.Ultimate + ";";
		}
		outs = outs.Remove (outs.Length-1);
		return outs;
	}
}
