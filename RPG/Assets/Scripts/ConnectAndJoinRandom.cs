using UnityEngine;
using Photon;
using System.Collections;

/// <summary>
/// This script automatically connects to Photon (using the settings file),
/// tries to join a random room and creates one if none was found (which is ok).
/// </summary>
public class ConnectAndJoinRandom : Photon.MonoBehaviour{

    public byte Version = 1;

	public void Connect(){
     PhotonNetwork.ConnectUsingSettings(Version + "");
    }

    public virtual void OnConnectedToMaster(){
		PhotonNetwork.JoinLobby ();
    }

    public virtual void OnJoinedLobby(){

		PhotonNetwork.JoinOrCreateRoom("Lobby",  new RoomOptions (), TypedLobby.Default);
    }

    public virtual void OnFailedToConnectToPhoton(DisconnectCause cause){
        Debug.LogError("Cause: " + cause);
    }

    public void OnJoinedRoom(){
		Login.k_Login.Connected = true;
    }
}
