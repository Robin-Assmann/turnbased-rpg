using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;


[XmlRoot ("Skills")]
public class SkillData {

	[XmlArray("PriestSkills"), XmlArrayItem("Skill")]
	public Skill[] PriestSkills;

	[XmlArray("ArcherSkills"), XmlArrayItem("Skill")]
	public Skill[] ArcherSkills;

	[XmlArray("WarriorSkills"), XmlArrayItem("Skill")]
	public Skill[] WarriorSkills;

	public static SkillData Load(string path){

		var serializer = new XmlSerializer (typeof(SkillData));
		using (var stream = new FileStream (path, FileMode.Open)) {

			return serializer.Deserialize (stream) as SkillData;
		}
	}
}
