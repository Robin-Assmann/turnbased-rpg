using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterFields : MonoBehaviour {

	public List<Transform> Position{ get; set;}
	public List<Transform>[] Rows;
	public List<Transform>[] Columns;
	public List<Transform> AllPositions{ get; set;}

	private List<Transform> Row1{ get; set;}
	private List<Transform> Row2{ get; set;}
	private List<Transform> Row3{ get; set;}
	private List<Transform> Row4{ get; set;}

	private List<Transform> Column1{ get; set;}
	private List<Transform> Column2{ get; set;}
	private List<Transform> Column3{ get; set;}
	private List<Transform> Column4{ get; set;}

	public static CharacterFields k_CharacterFields;

	void Awake () {

		k_CharacterFields = this;

		Rows = new List<Transform>[4];
		Columns = new List<Transform>[4];

		Position = new List<Transform> ();
		Row1 = new List<Transform> ();
		Row2 = new List<Transform> ();
		Row3 = new List<Transform> ();
		Row4 = new List<Transform> ();
		Column1 = new List<Transform> ();
		Column2 = new List<Transform> ();
		Column3 = new List<Transform> ();
		Column4 = new List<Transform> ();

		AllPositions = new List<Transform> ();

		for (int i = 0; i < 16; i++) {
			this.Position.Add (transform.GetChild (i));
		}

		if (gameObject.transform.name == "LeftBattleGround") {

			for (int i = 0; i < 4; i++) {

				Column1.Add (Position [3 + 4*i]);
				Column2.Add (Position [2 + 4*i]);
				Column3.Add (Position [1 + 4*i]);
				Column4.Add (Position [0 + 4*i]);
			}
		} else {

			for (int i = 0; i < 4; i++) {

				Column1.Add (Position [0 + 4*i]);
				Column2.Add (Position [1 + 4*i]);
				Column3.Add (Position [2 + 4*i]);
				Column4.Add (Position [3 + 4*i]);
			}
		}
		Row1 = Position.GetRange (0, 4);
		Row2 = Position.GetRange (4, 4);
		Row3 = Position.GetRange (8, 4);
		Row4 = Position.GetRange (12, 4);
	}

	public void Start(){
		Rows [0] = Row1;
		Rows [1] = Row2;
		Rows [2] = Row3;
		Rows [3] = Row4;

		Columns [0] = Column1;
		Columns [1] = Column2;
		Columns [2] = Column3;
		Columns [3] = Column4;

		StartCoroutine (ApplyPositions ());

		for (int i = 0; i < 4; i++) {
		
			for (int j = 0; j < 4; j++) {
			
				switch (i) {

				case 0:
					AllPositions.Add (Row1 [j]);
					break;
				case 1:					
					AllPositions.Add (Row2 [j]);
					break;
				case 2:					
					AllPositions.Add (Row3 [j]);
					break;
				case 3:
					AllPositions.Add (Row4 [j]);
					break;
				}
			}
		}
	}

	IEnumerator ApplyPositions(){

		yield return new WaitUntil(() => GameManager.k_Manager.CharactersUp);
		yield return new WaitUntil(() => GameManager.k_Manager.EnemiesUp);
		foreach (Transform tp_spawn in transform) {

			if (tp_spawn.name == "SpawnPoint" && tp_spawn.transform.FindChild ("Character(Clone)") != null) {

				while (true) {
					if (Column1.Contains (tp_spawn)) {
						SetPosition (tp_spawn, 1, true);
						break;
					}
					if (Column2.Contains (tp_spawn)) {
						SetPosition (tp_spawn, 2, true);
						break;
					}
					if (Column3.Contains (tp_spawn)) {
						SetPosition (tp_spawn, 3, true);
						break;
					}
					if (Column4.Contains (tp_spawn)) {
						SetPosition (tp_spawn, 4, true);
						break;
					}
				}
				while (true) {
					if (Row1.Contains (tp_spawn)) {
						SetPosition (tp_spawn, 1, false);
						break;
					}
					if (Row2.Contains (tp_spawn)) {
						SetPosition (tp_spawn, 2, false);
						break;
					}
					if (Row3.Contains (tp_spawn)) {
						SetPosition (tp_spawn, 3, false);
						break;
					}
					if (Row4.Contains (tp_spawn)) {
						SetPosition (tp_spawn, 4, false);
						break;
					}
				}

				ApplyPositionToCharacter (tp_spawn.transform.FindChild ("Character(Clone)").GetComponent<CharacterClass> ());
			}
		}

		GameManager.k_Manager.Applied = true;
	}

	private void SetPosition(Transform tp_spawn, int value, bool isColumn){

		if (isColumn && tp_spawn.transform.FindChild ("Character(Clone)") != null) {
			
				tp_spawn.transform.FindChild ("Character(Clone)").GetComponent<CharacterClass> ().k_Position [1] = value;

		} else {

			if (tp_spawn.transform.FindChild ("Character(Clone)") != null) {

				tp_spawn.transform.FindChild ("Character(Clone)").GetComponent<CharacterClass> ().k_Position [0] = value;
			}
		}
	}

	private void ApplyPositionToCharacter(CharacterClass tp_charclass){

		if (gameObject.name == "LeftBattleGround") {
			int x = tp_charclass.k_Position [0] - 1;
			int y = (tp_charclass.k_Position [1] - 4) * -1;
			tp_charclass.k_Character.Position = y + 4 * x;
		} else {
			int x = tp_charclass.k_Position [0] - 1;
			int y = (tp_charclass.k_Position [1] -1);
			tp_charclass.k_Character.Position = y + 4 * x;
		}
	}
}