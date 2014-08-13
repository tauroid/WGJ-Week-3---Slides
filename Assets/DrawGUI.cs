using UnityEngine;
using System.Collections;

public class DrawGUI : MonoBehaviour {
	Camera drawcam;
	Camera slidecam;
	Rigidbody slidebody;
	GameObject sl;
	
	GameObject cv;
	DrawLines dl;
	MeshRenderer cmr;
	MeshRenderer rmr;
	
	bool playing = false;
	
	// Use this for initialization
	void Start () {
		sl = GameObject.Find ("slidecam");
		
		cv = GameObject.Find ("Canvas");
		dl = (DrawLines)cv.GetComponent<MonoBehaviour>();
		cmr = cv.GetComponent<MeshRenderer>();
		rmr = GameObject.Find ("ramp/default").GetComponent<MeshRenderer>();
		rmr.enabled = false;
		
		drawcam = (Camera)GameObject.Find ("drawcam").GetComponent<Camera>();
		slidecam = (Camera)sl.GetComponent<Camera>();
		drawcam.enabled = true;
		slidecam.enabled = false;
		slidebody = sl.GetComponent<Rigidbody>();
		setSlideFrozen(true);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI() {
		
		if (!playing && GUI.Button (new Rect(20,20,80,40), "START")) {
			setSlide();
		} else if (playing && GUI.Button (new Rect(20,20,80,40),"RESTART")) {
			setDraw();
		}
	}
	
	public void setSlide() {
		playing = true;
		Debug.Log ("Sliding");
		cmr.enabled = false;
		rmr.enabled = true;
		Terrain terrain = (Terrain)GameObject.Find ("Terrain").GetComponent<MonoBehaviour>();
		terrain.generate (dl);
		drawcam.enabled = false;
		slidecam.enabled = true;
		setSlideFrozen(false);
	}
	
	public void setDraw() {
		Debug.Log ("Drawing");
		dl.reset ();
		playing = false;
		cmr.enabled = true;
		rmr.enabled = false;
		drawcam.enabled = true;
		slidecam.enabled = false;
		setSlideFrozen(true);
		sl.transform.position = new Vector3(0.0F,0.0F,10.0F);
		sl.transform.localEulerAngles = new Vector3(20.0F,90.0F,0.0F);
	}
	
	void setSlideFrozen(bool frozen) {
		if (frozen) slidebody.constraints = RigidbodyConstraints.FreezeAll;
		else slidebody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX;
	}
}
