using UnityEngine;
using System;
using System.Collections;

public class Terrain : MonoBehaviour {
	const int MAX_POINTS = DrawLines.MAX_POINTS;
	MeshFilter mf;
	
	Vector3[] vertices = new Vector3[MAX_POINTS*4];
	int[] triangles = new int[(MAX_POINTS-1)*6*3];
	int numPoints = 0;
	int numTriangles = 0;
	
	// Use this for initialization
	void Start () {
		mf = GetComponent<MeshFilter>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void generate(DrawLines dl) {
		numPoints = 0;
		numTriangles = 0;
		
		foreach (int chain in dl.iterChainStarts()) {
			loadPoint(chain,MAX_POINTS,dl);
			int i = chain;
			while (dl.nextpt[i] < MAX_POINTS) {
				loadPoint(dl.nextpt[i],i,dl);
				i = dl.nextpt[i];
			}
		}
		
		Vector3[] truncVertices = new Vector3[numPoints*4];
		int[] truncTriangles = new int[numTriangles*3];
		
		Array.Copy(vertices,truncVertices,numPoints*4);
		Array.Copy(triangles,truncTriangles,numTriangles*3);
		
		mf.mesh.vertices = truncVertices;
		mf.mesh.triangles = truncTriangles;
		
		GetComponent<MeshCollider>().sharedMesh = null;
		GetComponent<MeshCollider>().sharedMesh = mf.mesh;
	}
	
	Vector3 transformDLPoint(Vector3 dlpoint) {
		return new Vector3((dlpoint.x+1.5F)*10.0F,(dlpoint.y-1.0F)*10.0F,10.0F);
	}
	
	void loadPoint(int point, int lastpoint, DrawLines dl) {
		bool connected = lastpoint != MAX_POINTS;
		if (!connected && dl.nextpt[point] == MAX_POINTS) return;
		
		Vector3 pointVector = dl.getPointVector(point,lastpoint);
		
		Debug.Log ("loading point "+point+" with last point "+lastpoint);
		addPointVerts(transformDLPoint(dl.points[point]),pointVector);
		if (connected) {
			Debug.Log ("numPoints is "+numPoints);
			surfacePrevious();
		}
	}
	
	// Connect the last two rows of vertices created
	void surfacePrevious() {
		int ind1 = (numPoints-2)*4;
		int ind2 = (numPoints-1)*4;
		Debug.Log (ind1+" "+ind2);
		
		addTriangle(ind1,ind2,ind2+1);
		addTriangle(ind1,ind2+1,ind1+1);
		
		addTriangle(ind1+1,ind2+1,ind2+2);
		addTriangle(ind1+1,ind2+2,ind1+2);
		
		addTriangle(ind1+2,ind2+2,ind2+3);
		addTriangle(ind1+2,ind2+3,ind1+3);
		
		// Reverse
		addTriangle(ind1,ind2+1,ind2);
		addTriangle(ind1,ind1+1,ind2+1);
		
		addTriangle(ind1+1,ind2+2,ind2+1);
		addTriangle(ind1+1,ind1+2,ind2+2);
		
		addTriangle(ind1+2,ind2+3,ind2+2);
		addTriangle(ind1+2,ind1+3,ind2+3);
	}
	
	void addPointVerts(Vector3 point, Vector3 pointVector) {
		int ind = numPoints*4;
		vertices[ind] = point+pointVector*0.3F;
		vertices[ind].z += 1.0F;
		++ind;
		vertices[ind] = point;
		vertices[ind].z += 0.4F;
		++ind;
		vertices[ind] = point;
		vertices[ind].z -= 0.4F;
		++ind;
		vertices[ind] = point+pointVector*0.3F;
		vertices[ind].z -= 1.0F;
		++numPoints;
	}
	
	void addTriangle(int vert1, int vert2, int vert3) {
		int ind = numTriangles*3;
		triangles[ind] = vert1;
		triangles[ind+1] = vert2;
		triangles[ind+2] = vert3;
		++numTriangles;
	}
}
