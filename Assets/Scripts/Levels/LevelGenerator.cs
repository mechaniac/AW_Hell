using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {

	public int worldSize;
	public int seed;
	public ControlNode controlNodePrefab;
	public static ControlNode[,] controlNodes;
	public Transform axisPrefab;

	public bool autoUpdate;

	public Material matFloor;
	public Material matBridges;
	public Material matCorners;
	public Material matWall;
	public Material matDeko;
	public Material matMiddles;
	public Material matEmpty;

	public void GenerateMap(){
		
	}

	protected void GetWorldSize(){
		LevelGeneratorMaster levGenMaster = GetComponent<LevelGeneratorMaster> ();
		worldSize = levGenMaster.halfWorldSize * 2;
	}
	public void SetInitialFields(){
		controlNodes = new ControlNode[worldSize, worldSize];
		//--------------------------Destroy and Create MapHolder-----------------------------
		string holderName = "Control Nodes";
		if(transform.Find(holderName)){
			DestroyImmediate (transform.Find (holderName).gameObject);
		}
		Transform mapHolder = new GameObject (holderName).transform;
		mapHolder.parent = transform;
		GenerateControlGrid ();
	}

	public void GenerateControlGrid ()
	{
		for (int x = 0; x < worldSize; x++) {
			for (int z = 0; z < worldSize; z++) {
				GenerateControlNode (x, z);
			}
		}
	}

	public bool CheckPointValidity(int x, int z){ //checks 
		bool resultBool = true;
		if(x<0||x>=worldSize||z<0||z>=worldSize){
			resultBool = false;
		}
		return resultBool;
	}

	public bool CheckPointOccupied (int x, int z) //returns true if node doesnt exist OR is occupied
	{
		bool resultBool = true;
		if (x < 0 || x >= worldSize || z < 0 || z >= worldSize) {
			resultBool = true;
			return resultBool;
		}
		else if (controlNodes [x, z].occupied == false) {
			resultBool = false;
		}
		return resultBool;
	}

	void GenerateControlNode (int x, int z)
	{
		ControlNode controlNode = Instantiate<ControlNode> (controlNodePrefab);
		Vector3 position = new Vector3 (x - worldSize / 2, 0, z - worldSize / 2);
		controlNode.transform.position = position;
		controlNodes [x, z] = controlNode;
		controlNodes [x, z].lastUsedDirection = 0;

		controlNode.state = ControlNode.TileState.Empty;
		controlNode.positionX = x;
		controlNode.positionZ = z;
		controlNode.iD = new Vector2 (x, z);
		controlNode.transform.parent = transform; //GameObject.Find("mapHolder").transform
		controlNode.transform.parent = GameObject.Find("Control Nodes").transform; //GameObject.Find("mapHolder").transform
		}

	void GenerateAxisNode(int x, int z){
		Transform axisNode = Instantiate<Transform> (axisPrefab)as Transform;
		Vector3 position = new Vector3 (x - worldSize / 2,0, z - worldSize / 2);
		axisNode.transform.position = position;
		axisNode.transform.parent = GameObject.Find ("Control Nodes").transform;
	}

	public void CreateAxisIndicator(){
		for (int i = 1; i < 20; i++) {
			GenerateAxisNode (i-5, -5);
		}
		for (int i = 0; i < 20; i++) {
			GenerateAxisNode (-5, i-5);
		}
	}

	}

