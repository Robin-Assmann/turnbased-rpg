using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;


[XmlRoot ("Data")]
public class DataManager {

	[XmlArray("EnemySkills"), XmlArrayItem("Skill")]
	public Skill[] EnemySkills;

	[XmlArray("PriestSkills"), XmlArrayItem("Skill")]
	public Skill[] PriestSkills;

	[XmlArray("ArcherSkills"), XmlArrayItem("Skill")]
	public Skill[] ArcherSkills;

	[XmlArray("WarriorSkills"), XmlArrayItem("Skill")]
	public Skill[] WarriorSkills;

	[XmlArray("Levels"), XmlArrayItem("Level")]
	public Level[] Levels;

	[XmlArray("EnemyCharacters"), XmlArrayItem("Character")]
	public TempCharacter[] Enemies;

	public static DataManager Load(string path){

		var serializer = new XmlSerializer (typeof(DataManager));
		using (var stream = new FileStream (path, FileMode.Open)) {

			return serializer.Deserialize (stream) as DataManager;
		}
	}
}
