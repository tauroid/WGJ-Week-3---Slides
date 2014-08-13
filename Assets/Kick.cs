using UnityEngine;
using System.Collections;

public class Kick : MonoBehaviour {
	float timefinished;
	bool finished = false;
	bool failed = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			GetComponent<Rigidbody>().AddForce(0.0F,20.0F,0.0F);
		}
		if (transform.position.y < -40.0F) {
			((Camera)GameObject.Find ("drawcam").GetComponent<Camera>()).backgroundColor = new Color(1.0F,0.5F,0.5F);
			((DrawGUI)GameObject.Find ("Draw UI").GetComponent<MonoBehaviour>()).setDraw ();
			failed = true;
		}
		if (finished && Time.time - timefinished > 4.0) {
			if (failed) {
				failed = false;
				finished = false;
			} else { 
				finished = false;
				timefinished = Time.time;
				((DrawGUI)GameObject.Find ("Draw UI").GetComponent<MonoBehaviour>()).setDraw ();
				((Camera)GameObject.Find ("drawcam").GetComponent<Camera>()).backgroundColor = new Color(0.5F,0.7F,0.5F);
			}
		}
	}
	
	void OnCollisionEnter(Collision other) {
		if (other.collider.name == "pool" && !finished) {
			finished = true;
			timefinished = Time.time;
		}
	}
}
