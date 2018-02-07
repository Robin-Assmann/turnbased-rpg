using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BattleStorage {

	public int[] GroupIds;

	public List<Character> Enemy =  new List<Character>();
	public List<Character> Players =  new List<Character>();
	public List<Character> CharacterOrder =  new List<Character>();
	public bool EnemiesLoaded, EnemiesUp, TurnEnd, PlayersSynced;
	private List<int> EnemyPosValues;
	public int enemycount, enemyloaded, PlayersReady, BattleID, ExpGain;
	public List<Skill> AvailableSkills;

	private int Shift, CharacterCount;

	public int[] Positions;
	private bool FirstTime=true;
	private bool GotCharacters = false;

	public BattleStorage(int[] Ids){
		
		this.AvailableSkills = new List<Skill> ();
		this.GroupIds = Ids;
		this.enemyloaded = 0;
		this.PlayersReady = 0;
		this.CharacterCount = 0;
		this.Shift = 0;
		this.ExpGain = 0;
		this.Positions = new int[32];
		this.PlayersSynced = false;

		for (int i = 0; i < Positions.Length; i++) {
			Positions [i] = 0;
		}
	}

	public void PlayerReady(){
		PlayersReady++;
		if (GroupIds.Length == PlayersReady) {
			PlayersReady = 0;
			PlayersSynced = true;

			if (FirstTime) {
				FirstTime = false;
				Server.k_server.StartCoroutine (this.Turn ());
			}
		}
	}

	IEnumerator Turn(){

		int tp_TurnCount = 0;

		while (true) {
			yield return new WaitUntil(() => PlayersSynced);
			PlayersSynced = false;
			RaiseEventOptions opt = new RaiseEventOptions ();
			string[] content = new string[6];
			yield return new WaitUntil(() => GotCharacters);
			for (int i = 0; i < 6; i++) {
				content [i] = CharacterOrder [i].ID + "";
			}
			opt.TargetActors = GroupIds;
			PhotonNetwork.RaiseEvent (56, content, true, opt);

			if (CharacterOrder [0].ID < 0) {
				Server.k_server.StartCoroutine(EnemyTurnedOn ());
			}
			yield return new WaitUntil (() => TurnEnd);

			UpdateCharactersStart ();

			CheckDead ();

			if (Enemy.Count <= 0)
				break;

			//Turn ends
			TurnEnd = false;
			tp_TurnCount++;
			if (tp_TurnCount >= CharacterCount) {
				Debug.Log ("new round");
				tp_TurnCount = 0;
				CharacterOrder.RemoveRange (0, CharacterOrder.Count);
				Server.k_server.StartCoroutine(InitializeBattle (true));
			} else {
				CharacterOrder.Add (CharacterOrder [Shift]);
				CharacterOrder.RemoveAt (0);
			}
		}

		Server.k_server.StartCoroutine (EndBattle ());
	}
	public void Order(){

		Debug.Log (CharacterOrder.Count);
		this.CharacterOrder = this.CharacterOrder.OrderByDescending (go => go.Speed).ToList ();

		Shift = 0;
		if (CharacterOrder.Count < 6) {
			Shift = 6 - CharacterOrder.Count;
			if (Shift >= 3)
				Shift = 0;
		}
		int i = 0;

		while (CharacterOrder.Count < 6) {
			CharacterOrder.Add (CharacterOrder [i]);
			i++;
		}
	}

	IEnumerator EndBattle(){

		RaiseEventOptions opt = new RaiseEventOptions ();
		opt.TargetActors = GroupIds;
		PhotonNetwork.RaiseEvent (77, null, true, opt);

//		yield return new WaitUntil(() =>LootLoaded);
		yield return new WaitForSeconds(0.1f);


		foreach (Character tp_char in Players.ToArray()) {
			tp_char.Exp += ExpGain;		
		}
		UpdateCharactersStart ();
	}

	void UpdateCharactersStart(){

		foreach (Character c in Players.ToArray()) {
			if(c.PlayerID!=0)
				UpdateCharacters (c);
		}
	}

	void UpdateCharacters(Character tp_char){

		string sql_text = "UPDATE characters SET current_health=" + tp_char.Health + ", position=" + tp_char.Position + ", exp=" + tp_char.Exp + " WHERE character_id=" + tp_char.ID + ";";
		Server.k_server.StartCoroutine(ServerCommunication.k_conn.UpdateCharacter (new string[]{ sql_text }));
	}

	public void CheckDead(){

		List<Character> DeadPeople = new List<Character> ();

		int o = CharacterOrder.Count;

		for (int i = 0; i < Enemy.Count; i++) {
			if (Enemy [i].Health <= 0) {
				Positions[Enemy[i].Position]=0;
				DeadPeople.Add (Enemy [i]);
				Enemy.RemoveAt (i);
				Loot tp_loot = Loot.k_loot;
				tp_loot.DropNewItem (this);
				ExpGain += CharacterOrder [i].expGain;
			}
		}
		for (int i = 0; i < Players.Count; i++) {
			if (Players [i].Health <= 0) {
				Positions[Players[i].Position]=0;
				DeadPeople.Add (Players [i]);
				Players.RemoveAt (i);
			}
		}

		int j=1;
		foreach (Character tp_char in DeadPeople.ToArray()) {
			if (CharacterCount < 6) {
				foreach (Character tp_ch in CharacterOrder.ToArray()) {
					if (tp_ch.ID == tp_char.ID) {

						if (System.Array.IndexOf (CharacterOrder.ToArray(), tp_ch) < Shift) {
							j++;
						}
						CharacterOrder.Remove (tp_ch);
						CharacterCount--;
					}
				}
			}

			if (CharacterCount >= 6) {
				int i = CharacterOrder.FindIndex (go => go.ID == tp_char.ID);
				CharacterOrder.RemoveAt (i);
				CharacterCount--;
			}
			for (int i = 0; i <= j; i++) {
				CharacterOrder.Add (CharacterOrder [Shift + i]);
			}
		}

		int[] content = new int[DeadPeople.Count] ;

		for (int i = 0; i < content.Length; i++) {
			content [i] = DeadPeople [i].ID;
		}

		RaiseEventOptions opt = new RaiseEventOptions ();
		opt.TargetActors = GroupIds;
		PhotonNetwork.RaiseEvent (62, content, true, opt);

		if (Enemy.Count <= 0) {
//			EndBattle ();
		}

	}

	public IEnumerator InitializeBattle(bool tp_bool){
		Debug.Log ("this"+ tp_bool);
		while (true) {
			if (Players.Count > 0) {
				if (tp_bool) {
					Debug.Log ("PlayerCount =" + Players.Count);
					foreach (Character c in Players) {
						CharacterOrder.Add (c);
					}
					foreach (Character c in Enemy) {
						CharacterOrder.Add (c);
					}
				}
				break;
			}
			yield return new WaitForEndOfFrame ();
		}
		CharacterCount = CharacterOrder.Count;
		Debug.Log ("CharacterCount =" + CharacterCount);
		Order ();

		string[] tp_ids = new string[CharacterOrder.Count];
		for (int i = 0;i< tp_ids.Length; i++) {
			tp_ids [i] = CharacterOrder [i].ID+"";
		}

		RaiseEventOptions opt = new RaiseEventOptions ();
		opt.TargetActors = GroupIds;
		PhotonNetwork.RaiseEvent (55, tp_ids, true, opt);
		this.GotCharacters = true;
	}

	public void EndTurn(){

		TurnEnd = true;
		RaiseEventOptions opt = new RaiseEventOptions ();
		opt.TargetActors = GroupIds;
		PhotonNetwork.RaiseEvent (57, "", true, opt);
	}

	IEnumerator EnemyTurnedOn(){

		EnemyBehaviour behave = new EnemyBehaviour(1,2,3,4,BattleID);
		Character c = CharacterOrder [0];
		for (int i = 0; i < c.Skills.Length; i++) {
			int o = behave.CheckUsable (c.Skills[i], BattleID, c.Position);
			c.Skills [i].TargetChosen = o;
			if (o != 0)
				c.Skills [i].Usable = true;
			else
				c.Skills [i].Usable = false;
		}
		behave.UsableSkills (CharacterOrder[0].Skills);
		Skill tp_skill = behave.PickSkill ();

		if (tp_skill != null) {
			Debug.Log ("picked skill =" +tp_skill.Name);
			int target = tp_skill.TargetChosen;
			if (target >= 0) {
				
				List<int> content = tp_skill.TargetFields;
				content.Add (target);
				content.Add (0);
				int[] con = content.ToArray ();
				RaiseEventOptions opt = new RaiseEventOptions ();
				opt.TargetActors = GroupIds;
				PhotonNetwork.RaiseEvent (60, con, true, opt);
			
				yield return new WaitForSeconds (1.0f);

				Server.k_server.UseSkill (BattleID, tp_skill, target);
			}else{
				Debug.Log ("target is wrongly chosen +" + target);

			}
		} else {
			yield return new WaitForSeconds (1.0f);
			EndTurn ();
		}
	}
}
