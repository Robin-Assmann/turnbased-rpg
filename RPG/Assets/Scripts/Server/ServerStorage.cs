using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ServerStorage : MonoBehaviour {

	public List<List<PhotonPlayer>> Groups;
	public List<BattleStorage> B_Storages;

	public int GroupCounts = 0;

	public static ServerStorage Sv_Storage;

	public List<ServerPlayer> activeUser;
	public List<int[,]> userBattle;

	void Awake(){

		Sv_Storage = this;
		B_Storages = new List<BattleStorage> ();
		this.Groups = new List<List<PhotonPlayer>> ();
		this.activeUser = new List<ServerPlayer> ();
		this.userBattle = new List<int[,]> ();

	}

	#region Group
	public void MakeNewGroup(PhotonPlayer Owner){

		List<PhotonPlayer> tp_group = new List<PhotonPlayer> ();
		tp_group.Add (Owner);
		Groups.Add(tp_group);

	}

	public void AddToGroup(PhotonPlayer Owner, PhotonPlayer Member){

		int o = -1;
		for (int i = 0; i < Groups.Count; i++) {
		
			if (Groups [i] [0].Equals (Owner))
				o = i;
		}
		List<PhotonPlayer> New = new List<PhotonPlayer> ();
		if (o < 0) {
			New.Add (Owner);
			New.Add (Member);
			Groups.Add (New);
		} else {
			New = Groups [o];
			New.Add (Member);
			Groups.Add (New);
		}
	}
	#endregion

	public int MakeNewBattleStorage(int OwnerID){
	
		foreach (List<PhotonPlayer> player in Groups.ToArray()) {

			if (player [0].ID == OwnerID) {

				List<int> ids = new List<int> ();
				foreach (PhotonPlayer play in player.ToArray()) {
					ids.Add (play.ID);
				}

				BattleStorage b_stor = new BattleStorage (ids.ToArray ());
				B_Storages.Add (b_stor);
				int b_id = B_Storages.IndexOf (b_stor);
				b_stor.BattleID = b_id;

				for (int i = 0; i < ids.Count; i++) {
					userBattle.Add (new int[ids [0], b_id]);
				}

				return b_id;
			}
		}
		return 0;
	}


}
