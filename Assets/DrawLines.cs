using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DrawLines : MonoBehaviour {
    public const int MAX_POINTS = 300;
    public const float LINE_WIDTH = 0.05F;
    public const float SAMPLE_DIST = 0.05F;
    
    public bool[] used = new bool[MAX_POINTS];
    public Vector3[] points = new Vector3[MAX_POINTS];
	public int[] nextpt = new int[MAX_POINTS];
	public bool[] usedthisupdate = new bool[MAX_POINTS];
    
    private Vector3[] vertices = new Vector3[MAX_POINTS*2];
    private int[] triangles = new int[(MAX_POINTS-1)*2*3];
    private int numPoints = 0;
    private int numTriangles = 0;
    
    private Mesh mesh;
    
    private bool mouseDown = false;
    private int lastpoint = MAX_POINTS;
    // Use this for initialization
    void Start () {
    	MeshFilter mf = GetComponent<MeshFilter>();
        mesh = mf.mesh;
        for (int i=0; i < MAX_POINTS; ++i) {
        	used[i] = false;
        	nextpt[i] = MAX_POINTS;
        	usedthisupdate[i] = false;
        }
    }
    
    // Update is called once per frame
    void Update () {
    	int point = lastpoint;
    	Vector3 pos = Input.mousePosition*2.0F/(float)(Screen.height)-new Vector3((float)(Screen.width)/(float)(Screen.height)*1.0F,1.0F,-1.0F);
    	bool mousestate = Input.GetMouseButton (0);
    	if (mousestate != mouseDown) {
			//Debug.Log(pos.x+" "+pos.y+" "+pos.z);
    		mouseDown = mousestate;
    		if (mouseDown) {
				//Debug.Log ("Adding new unconnected");
    			lastpoint = addPoint(pos);
    		}
    	} else if (mousestate) {
			if (lastpoint < MAX_POINTS) {
				if ((pos-points[lastpoint]).magnitude > SAMPLE_DIST) {
					//Debug.Log ("Adding point after "+lastpoint);
    				lastpoint = addConnectedPoint(pos,lastpoint);
    			}
			} else {
				//Debug.Log ("Adding new unconnected");
				lastpoint = addPoint(pos);
			}
    	}
    	if (point != lastpoint) updateMesh();
    }
    
    public Vector3 getPointVector(int point, int lastpoint) {
		bool connected = MAX_POINTS != lastpoint;
    
		Vector3 pointsVector;
		Vector3 vec1;
		//Debug.Log ("Last point is "+lastpoint);
		if (connected && nextpt[point] != MAX_POINTS) {
			vec1 = points[point]-points[lastpoint];
			Vector3 vec2 = points[nextpt[point]]-points[point];
			vec1 = new Vector3(-vec1.y,vec1.x,0).normalized;
			vec2 = new Vector3(-vec2.y,vec2.x,0).normalized;
			pointsVector = (vec1+vec2)/2;
			/*Debug.Log ("Next point at "+points[nextpt[point]].x+", "+points[nextpt[point]].y+", "+points[nextpt[point]].z+
					   ", this point at "+points[point].x+", "+points[point].y+
					   ", last point at "+points[lastpoint].x+", "+points[lastpoint].y);*/
		} else if (connected) {
			vec1 = (points[point]-points[lastpoint]).normalized;
			pointsVector = new Vector3(-vec1.y,vec1.x,0);
		} else {
			vec1 = (points[nextpt[point]]-points[point]).normalized;
			pointsVector = new Vector3(-vec1.y,vec1.x,0);
		}
		return pointsVector;
    }
    
    public void reset() {
    	for (int i = 0; i < MAX_POINTS; ++i) {
    		used[i] = false;
    	}
    	mesh.Clear ();
    }
    
    private void updateMesh() {
    	Debug.Log ("Updating mesh");
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		mesh.Clear ();
    	numPoints = 0;
    	numTriangles = 0;
    	foreach (int chain in iterChainStarts ()) {
    		loadChain(chain);
    	}
		Vector3[] truncVertices = new Vector3[numPoints*2];
		int[] truncTriangles = new int[numTriangles*3];
		
		Array.Copy(vertices,truncVertices,numPoints*2);
		Array.Copy(triangles,truncTriangles,numTriangles*3);
		
		mesh.vertices = truncVertices;
		mesh.triangles = truncTriangles;
	}
    
    private void loadChain(int start) {
    	loadPoint(start,MAX_POINTS);
    	int i = start;
    	while (nextpt[i] < MAX_POINTS) {
    		loadPoint(nextpt[i],i);
    		i = nextpt[i];
    	}
    }
    
    private void loadPoint(int point, int lastpoint) {
    	//Debug.Log ("Loaded point "+point);
		bool connected = MAX_POINTS != lastpoint;
		if (!connected && nextpt[point] == MAX_POINTS) return;
    	
    	Vector3 pointsVector = getPointVector(point,lastpoint);
    	
		vertices[numPoints*2] = points[point]+pointsVector*LINE_WIDTH/2;
		vertices[numPoints*2+1] = points[point]-pointsVector*LINE_WIDTH/2;
		/*Debug.Log ("Vertex at "+vertices[numPoints*2].x+", "+vertices[numPoints*2].y+"\n"
				   +" and at "+vertices[numPoints*2+1].x+", "+vertices[numPoints*2+1].y);*/
		if (connected) {
			newTriangle((numPoints-1)*2+1,(numPoints-1)*2,numPoints*2+1);
			newTriangle((numPoints-1)*2,numPoints*2,numPoints*2+1);
		}
		++numPoints;
    }
    
    private void newTriangle(int vert1, int vert2, int vert3) {
    	triangles[numTriangles*3] = vert1;
    	triangles[numTriangles*3+1] = vert2;
    	triangles[numTriangles*3+2] = vert3;
    	++numTriangles;
    }
    
    private void loadPoint(int point, int vert1, int vert2) {
    	
    }
    
    private int addConnectedPoint(Vector3 point, int prev) {
    	nextpt[prev] = addPoint(point);
    	return nextpt[prev];
    }
    
    private int addPoint(Vector3 point) {
    	int newpoint = getFirstUnusedPoint();
    	if (newpoint == MAX_POINTS) return MAX_POINTS;
    	points[newpoint] = point;
    	used[newpoint] = true;
    	nextpt[newpoint] = MAX_POINTS;
    	Debug.Log ("Added point "+newpoint+" at "+point.x+", "+point.y);
    	return newpoint;
    }
    
    private void removePoints(Vector3 point, float radius) {
    	for (int i = 0; i < MAX_POINTS; ++i) {
    		if ((point-points[i]).magnitude < radius) points[i] = new Vector3();
    	}
    }
    
    private int getFirstUnusedPoint() {
    	for (int i = 0; i < MAX_POINTS; ++i) {
    		if (!used[i]) return i;
    	}
    	return MAX_POINTS;
    }
    
    public void resetUsedThisUpdate() {
    	for (int i = 0; i < MAX_POINTS; ++i) {
    		usedthisupdate[i] = false;
    	}
    }
    
    private int getFirstPointUnusedThisUpdate() {
    	for (int i = 0; i < MAX_POINTS; ++i) {
    		if (!usedthisupdate[i] && used[i]) return i;
    	}
    	return MAX_POINTS;
    }
    
    private void markChainUsed(int start) {
    	int i = start;
    	usedthisupdate[i] = true;
    	while (nextpt[i] < MAX_POINTS && used[nextpt[i]]) {
    		usedthisupdate[nextpt[i]] = true;
    		i = nextpt[i];
    	}
    }
    
    public IEnumerable<int> iterChainStarts() {
    	int i = getFirstPointUnusedThisUpdate();
    	while (i < MAX_POINTS) {
    		yield return i;
			markChainUsed(i);
    		i = getFirstPointUnusedThisUpdate();
    	}
    	resetUsedThisUpdate();
    }
}
