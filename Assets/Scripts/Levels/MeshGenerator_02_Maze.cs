using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator_02_Maze : MonoBehaviour {


	LevelGenerator_02_Maze levGen;
//	ControlNode[,] controlNodes;
	public Material matGreen;
	public BoxCollider colliderPrefab;
	BoxCollider[] colliders;
	GameObject collisionHolder;
	GameObject geometryHolder;

	int currentIndexX;
	int currentIndexZ;
	int worldSize;

	public LevelArtPrefab[] floorPrefabs_Size1;
	public LevelArtPrefab[] floorPrefabs_Size4;

	public LevelArtPrefab[] wallPrefabs_Size1;

	public LevelArtPrefab[] voidPrefabs_Size1;
	public LevelArtPrefab[] voidPrefabs_Size4;


	void Update(){
//		if(Input.GetKeyDown(KeyCode.A)){
//			GetInitialStuff ();
//			GenerateCollision ();
//			SetLevelGeometryLoop ();
//		}
	}

	public void GenerateMazeMesh(){
		GetInitialStuff ();
		GenerateCollision ();
		SetLevelGeometryLoop ();
	}

	public void GenerateCollision(){

		for (int x = 0; x < worldSize; x++) {
			for (int z = 0; z < worldSize; z++) {
				if( !LevelGenerator.controlNodes[x,z].set){
					ControlNode endNode = SearchForNodeHomogenity (LevelGenerator.controlNodes [x, z]);
					SetCollider (LevelGenerator.controlNodes [x, z], endNode);
				}
			}
		}
	}

	public void GetInitialStuff(){
		levGen = GetComponent<LevelGenerator_02_Maze> ();
		worldSize = levGen.worldSize;
		collisionHolder = new GameObject ("Collision Holder");
		collisionHolder.transform.parent = transform;

		geometryHolder = new GameObject ("Level Meshes");
		geometryHolder.transform.parent = transform;

		currentIndexX = currentIndexZ = 0;
	}

	ControlNode FindNextNode(){
		ControlNode returnedNode = LevelGenerator.controlNodes[0,0];
		if (currentIndexX < worldSize && currentIndexZ < worldSize) {
			returnedNode = LevelGenerator.controlNodes [currentIndexX, currentIndexZ];
		}
		return returnedNode;
	}

	ControlNode SearchForNodeHomogenity(ControlNode startNode){ //this could be made to Look into both directions and see which one produces the bigger quad
		int posX = startNode.positionX;
		int posZ = startNode.positionZ;
		ControlNode.TileState givenState = startNode.state;
		ControlNode diagonalNode = startNode;
		for (int x = posX +1; x < worldSize; x++) {
			if (LevelGenerator.controlNodes [x, posZ].state != givenState)
			{
				break;}
			diagonalNode = LevelGenerator.controlNodes [x, posZ];
		}
		int storeX =diagonalNode.positionX;
		bool stopped = false;
		for(int z = posZ+1; z < worldSize;z++){
			if(stopped){break;}
			for(int x = posX;x<=diagonalNode.positionX;x++){
				if (LevelGenerator.controlNodes [x, z].state != givenState){
					stopped = true;
					break;
					storeX = x;

				}
			}
			if(! stopped)diagonalNode = LevelGenerator.controlNodes [storeX, z];

		}

		return diagonalNode;
	}

	int counter = 0;
	void SetCollider(ControlNode startNode, ControlNode endNode){
		BoxCollider collider = Instantiate<BoxCollider> (colliderPrefab);

		float posX = startNode.positionX + (endNode.positionX - startNode.positionX)/2f;
		float posZ = startNode.positionZ + (endNode.positionZ - startNode.positionZ)/2f;

		float diffX = (endNode.positionX - startNode.positionX)+1f;
		float diffZ = (endNode.positionZ - startNode.positionZ)+1f;

		float colliderHeight = .1f;

		if (startNode.state == ControlNode.TileState.Wall)
			colliderHeight = 2;


		ControlNode centerNode = LevelGenerator.controlNodes [(int)posX,(int) posZ];
		Vector3 colliderSize = new Vector3 ((int)diffX,colliderHeight,(int)diffZ);
		Vector3 colliderPosition = new Vector3 ();
		colliderPosition.x = posX - worldSize / 2;
		colliderPosition.y = colliderHeight/2 +.1f;
		colliderPosition.z = posZ - worldSize / 2;
		collider.center = colliderPosition;
		collider.size = colliderSize;
		collider.transform.parent = collisionHolder.transform;

		for (int x = startNode.positionX; x <= endNode.positionX; x++) {
			for (int z = startNode.positionZ; z <= endNode.positionZ; z++) {
				LevelGenerator.controlNodes [x, z].set = true;
			}
		}
	}

	void SetLevelGeometryLoop(){
		for (int x = 0; x < worldSize; x++) {
			for (int z = 0; z < worldSize; z++) {
				SetLevelGeometryPiece (LevelGenerator.controlNodes [x, z]);
			}
		}
	}

	void SetLevelGeometryPiece(ControlNode setNode){
		Vector3 position = new Vector3 ();
		position.x = setNode.positionX - worldSize / 2f;
		position.y = 0.1f;
		position.z = setNode.positionZ - worldSize / 2f;
		switch(setNode.state){
		case ControlNode.TileState.Empty:
			LevelArtPrefab voidPrefab = Instantiate (voidPrefabs_Size1 [0]);
			position.y = 2;
			voidPrefab.transform.localPosition = position;
			voidPrefab.transform.parent = geometryHolder.transform;
			break;
		case ControlNode.TileState.Wall:
			LevelArtPrefab wallPrefab = Instantiate (wallPrefabs_Size1 [0]);
			wallPrefab.transform.localPosition = position;
			wallPrefab.transform.parent = geometryHolder.transform;
			break;
		case ControlNode.TileState.Floor:
			LevelArtPrefab floorMesh = Instantiate (floorPrefabs_Size1 [0]);
			floorMesh.transform.localPosition = position;
			floorMesh.transform.parent = geometryHolder.transform;
			break;
		}
	}
}
