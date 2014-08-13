using UnityEngine;
using System.Collections;

public class TestSquare : MonoBehaviour {

	// Use this for initialization
	void Start () {
		MeshFilter mf = GetComponent<MeshFilter>();
		mf.mesh.vertices = new Vector3[]{ new Vector3(-0.5F,-0.5F,4.0F),
							 new Vector3(-0.5F,0.5F,4.0F),
							 new Vector3(0.5F,0.5F,4.0F),
							 new Vector3(0.5F,-0.5F,4.0F) };
		mf.mesh.triangles = new int[]{ 0,1,2,0,2,3 };
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
