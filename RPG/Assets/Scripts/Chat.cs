using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Chat : MonoBehaviour {

	private InputField input;
	private Text chatcontent;

	public static Chat chat;

	void Start () {
	
		chat = this;

		input = transform.FindChild ("InputField").GetComponent<InputField>();
		input.characterLimit = 50;

		chatcontent = transform.FindChild ("Scroll View").GetChild (0).GetChild (0).GetComponent<Text> ();
		chatcontent.text = "";

	}

	public void ChangeText(){

		if (input.text != "") {
			string msg = "\n" + PhotonNetwork.player.name + ": " + input.text;

			object[] content = new object[] {msg};
			RaiseEventOptions opt = new RaiseEventOptions ();
			opt.Receivers = ReceiverGroup.Others;
			PhotonNetwork.RaiseEvent (105, content, true, opt);

			this.chatcontent.text += msg;
			input.text = "";
			input.ActivateInputField ();
		} else {
			input.ActivateInputField ();
		}
	}

	public void ChangeManually(string msg){
	
		this.chatcontent.text += msg;
	
	}

}
