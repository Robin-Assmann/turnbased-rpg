using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour {


	public void LoadScene(int i){
	
		if (i == 0) {
			PlayerManager.k_manager.isMultiplayer = true;
		} else {
			PlayerManager.k_manager.isMultiplayer = false;
		}
		SceneManager.LoadScene (2);
	}
}
