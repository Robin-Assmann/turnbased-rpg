using UnityEngine;
using System.Collections;

public class Movetext : MonoBehaviour {

	private int lifetime;

	void Start () {
	
		lifetime = 40;

	}

	void FixedUpdate () {
	
		if (lifetime > 0) {
			transform.localPosition = new Vector2 (transform.localPosition.x , transform.localPosition.y + 0.01f);
			lifetime--;
		} else {
			DestroyImmediate (gameObject);
		}


	}
}
