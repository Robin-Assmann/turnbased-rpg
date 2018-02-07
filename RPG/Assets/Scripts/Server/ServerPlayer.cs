using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerPlayer{


	public List<Character> characters;
	public int PlayerID, PhotonID;

	public ServerPlayer(int playID, int phId){

		characters = new List<Character> ();

		this.PlayerID = playID;
		this.PhotonID = phId;

	}
}
