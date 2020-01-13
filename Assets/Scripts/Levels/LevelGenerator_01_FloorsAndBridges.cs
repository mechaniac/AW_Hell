using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public  class LevelGenerator_01_FloorsAndBridges:MonoBehaviour
{

	public ControlNode controlNodePrefab;
//	int controlNodeCount;
	public static ControlNode[,] controlNodes;
	ControlNode[] bridgeAnchorTiles;
	int bridgeAnchorIndex =0;
	ControlNode[] roomCornerTiles;
	ControlNode[] cornerRoomCornerTiles;
	int cornerRoomCornerTilesIndex;
	ControlNode[] cornerRoomMiddleTiles;
	int cornerRoomMiddleTilesIndex;

	[Header("Level 01 Rooms and Bridges")]
	public int seed;
	[Range(25,100)]
	public int halfWorldSize;
	int worldSize;
	[Range(2,4)]
	public int roomsPerRow;
	int roomCount;
	[Range(3,40)]
	public int roomSize;
	[Range(0,10)]
	public int roomSizeRandomness;
	[Range(0,40)]
	public int roomPositionRandomness;

	public bool bridges;
	[Range(1,5)]
	public int bridgeWidth;

	public bool cornerRooms;
	public bool setWallsFromRooms;
	[Range(3,20)]
	public int cornerRoomSize;
	[Range(0,20)]
	public int cornerRoomPositionRandomness;
	[Range(0,12)]
	public int cornerRoomSizeRandomness;



	[Range(-1,1)]
	public float wallProbabilty;
	[Range(0,1)]
	public float wallProbabilityAttentuation;
//	public int roomWidthRelationRandomness;
	public bool dekotiles;
	[Range(0,20)]
	public int tileStormDensity;
	[Range(1,5)]
	public int tileStormDistance;

	public bool blowHoles;
	[Range(0,20)]
	public int clearTileStormDensity;
	[Range(0,5)]
	public int clearTileStormDistance;

	int gapSize;

	public Material matFloor;
	public Material matBridges;
	public Material matCorners;
	public Material matWall;
	public Material matDeko;
	public Material matMiddles;
	public Material matEmpty;

//	List<ControlNode> mainFloorList;
//	List<ControlNode> mainBridgeList;
	HashSet<ControlNode> mainFloorHSet;
	HashSet<ControlNode> mainBridgeHSet;

	public bool autoUpdate;
	System.Random pRNG;
	System.Random pRNGCornerRooms;
	System.Random pRNGTileStormTiles;
	System.Random pRNGClearTiles;
	//------------------------Debug
//	Vector3 posAdd = new Vector3(0,0,0);
//	Vector3 adder = new Vector3 (0, 0.01f, 0);


	void Awake ()
	{
		

	}

	void Start ()
	{
	}
	public void GenerateMap(){
		
//		mainFloorList=new List<ControlNode>();
//		mainBridgeList = new List<ControlNode> ();

		roomCount =roomsPerRow*roomsPerRow;
		cornerRoomCornerTilesIndex=0;
		cornerRoomMiddleTilesIndex = 0;
//		cornerRoomMiddle
		mainFloorHSet=new HashSet<ControlNode>();
		mainBridgeHSet = new HashSet<ControlNode> ();
		worldSize = halfWorldSize * 2;
		bridgeAnchorIndex = 0;
		pRNG = new System.Random (seed);
		pRNGCornerRooms = new System.Random (seed);
		bridgeAnchorTiles=new ControlNode[roomCount*2];
		roomCornerTiles = new ControlNode[roomCount * 4];
		cornerRoomCornerTiles = new ControlNode[roomCount * 16];
		cornerRoomMiddleTiles = new ControlNode[roomCount * 16];
//		controlNodeCount = worldSize * worldSize;
		gapSize = (worldSize / 3 - roomSize) / 2;
		controlNodes = new ControlNode[worldSize, worldSize];

		string holderName = "Control Nodes";
		if(transform.Find(holderName)){
			DestroyImmediate (transform.Find (holderName).gameObject);
		}
		Transform mapHolder = new GameObject (holderName).transform;
		mapHolder.parent = transform;
		GenerateControlGrid ();

		SetRooms_01 ();

		if(bridges){
			for (int i = 0; i < roomCornerTiles.Length; i++) {
				if(roomCornerTiles[i]!=null) {
					if (CheckPointValidity (roomCornerTiles [i].positionX, roomCornerTiles [i].positionZ)) {
						GenerateBridgesInAllDirections (roomCornerTiles [i], 1);
					}
				}
			}

			for (int i = 0; i < bridgeAnchorTiles.Length; i++) {
				if(bridgeAnchorTiles[i]!= null){
					if(CheckPointValidity(bridgeAnchorTiles[i].positionX,bridgeAnchorTiles[i].positionZ)){
						GenerateBridgesInAllDirections (bridgeAnchorTiles [i],bridgeWidth);
					}
				}

			}
		}


		if(cornerRooms){
			for (int i = 0; i < roomCornerTiles.Length; i++) {
				if(roomCornerTiles[i]!= null){
					if(CheckPointValidity(roomCornerTiles[i].positionX,roomCornerTiles[i].positionZ)){
						CreateCornerRoom (roomCornerTiles [i]);
					}
				}

			}
		}
		if(bridges&&cornerRooms&&dekotiles){
			for (int i = 0; i < cornerRoomCornerTiles.Length; i++) {
				if(cornerRoomCornerTiles[i]!=null){
					TileStorm (cornerRoomCornerTiles [i], 2, 2,.5f,ControlNode.TileState.DekoTiles);
				}
			}
		}


		if(bridges&&cornerRooms&&blowHoles){
			for (int i = 0; i < cornerRoomMiddleTiles.Length; i++) {
				if(cornerRoomMiddleTiles[i]!=null){
					if (!CheckForTileStateInArea(cornerRoomMiddleTiles[i],ControlNode.TileState.Bridge,3)){
//						print ("found!");
						ClearTileStorm (cornerRoomMiddleTiles [i], 1, 2, .5f, ControlNode.TileState.Empty);
					}
				}
			}
		}

//		TestMath ();
	}
	void TestMath(){ //Debug Function
		
		int randomZ = 6;

		int randomX = 12;
		for (int x = -randomX, xUp = -500; x < randomX; x++,xUp += 1000/randomX) {
			print("ROWROWROW");
			for (int z = -randomZ, zUp = -500; z < randomZ; z++,zUp += 1000/randomZ) {

				print (xUp + " + " + zUp + " = " + Mathf.Abs(xUp + zUp)/1000f);
			}

		}

	}


	void Update ()
	{

	}

	void SetRooms_01 ()
	{
		int[] roomSizeXArray = new int[roomCount];
		for (int i = 0; i < roomCount; i++) {
			roomSizeXArray [i] = pRNG.Next (roomSize - roomSizeRandomness, roomSize + roomSizeRandomness);
		}
		int[] roomSizeZArray = new int[roomCount];
		for (int i = 0; i < roomCount; i++) {
			roomSizeZArray [i] = pRNG.Next (roomSize - roomSizeRandomness, roomSize + roomSizeRandomness);
		}
		int firstLoopers = (int)Mathf.Sqrt (roomCount);

		for (int xr = 0,i=0,xSizeArrayCounter =0; xr < firstLoopers; xr++) {

			for (int zr = 0 ; zr < firstLoopers; zr++,xSizeArrayCounter++) {
				int randomOffsetY = pRNG.Next(-roomPositionRandomness,roomPositionRandomness);
				int randomOffsetX = pRNG.Next(-roomPositionRandomness,roomPositionRandomness);
				for (int x = 0; x < roomSizeXArray[xSizeArrayCounter]; x++) {
					for (int z = 0; z < roomSizeZArray[zr]; z++) {
						if (CheckPointValidity ((xr * worldSize / firstLoopers) + x + ((worldSize / firstLoopers - roomSize) / 2) + randomOffsetX, (zr * worldSize / firstLoopers) + z + ((worldSize / firstLoopers - roomSize) / 2) + randomOffsetY)) {
							ControlNode.TileState iState = new ControlNode.TileState ();
							if (x == 0 && z == 0 || x == 0 && z == roomSizeZArray [zr] - 1 || x == roomSizeXArray [xSizeArrayCounter] - 1 && z == 0 || x == roomSizeXArray [xSizeArrayCounter] - 1 && z == roomSizeZArray [zr] - 1) {
								iState = ControlNode.TileState.RoomCorner;
							
								roomCornerTiles [i++] = controlNodes [(xr * worldSize / firstLoopers) + x + ((worldSize / firstLoopers - roomSize) / 2) + randomOffsetX, (zr * worldSize / firstLoopers) + z + ((worldSize / firstLoopers - roomSize) / 2) + randomOffsetY];
							} else if (x == 0 && z == roomSizeZArray [zr] / 2 || x == roomSizeXArray [xSizeArrayCounter] / 2 && z == 0) {
								iState = ControlNode.TileState.BridgeAnchor;
							} else {
								iState = ControlNode.TileState.Floor;
							}
							SetControlNodeTileState (iState, (xr * worldSize / firstLoopers) + x + ((worldSize / firstLoopers - roomSize) / 2) + randomOffsetX, (zr * worldSize / firstLoopers) + z + ((worldSize / firstLoopers - roomSize) / 2) + randomOffsetY);
						}

					}
				}
			}
		}
	}

	void SetRasterBridges_01 ()
	{
		for (int z = 0; z < 3; z++) {
			for (int x = 0; x < worldSize - gapSize * 2 - roomSize; x++) {
				for (int w = 0; w < bridgeWidth; w++) {
					SetControlNodeTileState (ControlNode.TileState.Bridge, gapSize + roomSize / 2 + x, (z * worldSize / 3) + gapSize+roomSize/2+w-bridgeWidth/2);
				}
				}

		}
		for (int x = 0; x < 3; x++) {
			for (int z = 0; z < worldSize-gapSize*2-roomSize; z++) {
				for (int w = 0; w < bridgeWidth; w++) {
					SetControlNodeTileState (ControlNode.TileState.Bridge, (x * worldSize / 3) +gapSize+ roomSize/2+w-bridgeWidth/2, gapSize + roomSize / 2 + z);
				}

			}
		}
	}

	void CreateCornerRoom(ControlNode givenNode){

		int posx = givenNode.positionX-1;
		int posz = givenNode.positionZ-1;
		int randomX = pRNGCornerRooms.Next (cornerRoomSize-cornerRoomSizeRandomness, cornerRoomSize+ cornerRoomSizeRandomness);
		int randomZ = pRNGCornerRooms.Next (cornerRoomSize-cornerRoomSizeRandomness, cornerRoomSize+ cornerRoomSizeRandomness);
		int randomOffsetX = pRNGCornerRooms.Next (-cornerRoomPositionRandomness, cornerRoomPositionRandomness);
		int randomOffsetZ = pRNGCornerRooms.Next (-cornerRoomPositionRandomness, cornerRoomPositionRandomness);
		for (int x = -randomX, xUp = -500; x < randomX; x++,xUp += 1000 / (randomX * 2)) {
			for (int z = -randomZ, zUp = -500; z < randomZ; z++,zUp += 1000 / (randomZ * 2)) {
				if(CheckPointValidity(posx+x+randomOffsetX,posz+z+randomOffsetZ))
				{
					SetControlNodeTileState (ControlNode.TileState.Floor, posx+x+randomOffsetX, posz+z+randomOffsetZ);
					if(setWallsFromRooms){
						if (x == -randomX || x == randomX - 1 || z == randomZ - 1 || z == -randomZ) {
							SetControlNodeTileState (ControlNode.TileState.Wall, posx + x + randomOffsetX, posz + z + randomOffsetZ, (Mathf.Abs (xUp + zUp) / 1000f) *wallProbabilityAttentuation+ wallProbabilty);
						}
					}
					if(x==-randomX&&z==-randomZ||x==randomX-1&&z==randomZ-1||x==-randomX&&z==randomZ-1||x==randomX-1&&z==-randomZ){SetControlNodeTileState (ControlNode.TileState.CornerRoomCorners, posx + x + randomOffsetX, posz + z + randomOffsetZ);}
					if(x>-1&&x<1&&z==-randomZ||x>-1&&x<1&&z==randomZ-1||z>-1&&z<1&&x==-randomX||z>-1&&z<1&&x==randomX-1){SetControlNodeTileState (ControlNode.TileState.CornerRoomMiddles, posx + x + randomOffsetX, posz + z + randomOffsetZ);}
				}
			}
			
		}
	}

	void TileStorm(ControlNode givenNode,int maxTileDistance,int trys,float probability,ControlNode.TileState state){
		int x = givenNode.positionX;
		int z = givenNode.positionZ;
		pRNGTileStormTiles = new System.Random (seed);
		for (int i = 0; i < trys*tileStormDensity; i++) {
			int xMax = pRNGTileStormTiles.Next (-maxTileDistance*tileStormDistance, maxTileDistance*tileStormDistance);
			int zMax = pRNGTileStormTiles.Next (-maxTileDistance*tileStormDistance, maxTileDistance*tileStormDistance);
			if(CheckPointValidity(x+xMax,z+zMax)){
				if(!controlNodes[x+xMax,z+zMax].occupied){SetControlNodeTileState (state, x+xMax, z+zMax,probability);}
			}

		}

	}

	void ClearTileStorm (ControlNode givenNode, int maxTileDistance, int trys, float probability,ControlNode.TileState state){
		int x = givenNode.positionX;
		int z = givenNode.positionZ;
		pRNGClearTiles = new System.Random (seed);
		for (int i = 0; i < trys*clearTileStormDensity; i++) {
			int xMax = pRNGClearTiles.Next (-maxTileDistance * clearTileStormDistance, maxTileDistance * clearTileStormDistance);
			int zMax = pRNGClearTiles.Next (-maxTileDistance * clearTileStormDistance, maxTileDistance * clearTileStormDistance);
			if(CheckPointValidity(x+xMax,z+zMax)){
				SetControlNodeTileState (state, x + xMax, z + zMax, probability);
			}
		}
	}
		
	bool CheckPointValidity(int x, int z){ //checks 
		bool resultBool = true;
		if(x<0||x>=worldSize||z<0||z>=worldSize){
			resultBool = false;
		}
		return resultBool;
	}

	bool CheckForTileStateInArea(ControlNode givenNode, ControlNode.TileState state, int searchDistance){
		bool stateFound = false;
		int positionX = givenNode.positionX;
		int positionZ = givenNode.positionZ;
		for (int x = positionX-searchDistance; x < positionX+searchDistance*2; x++) {
			for (int z = positionZ-searchDistance; z < positionZ+searchDistance*2; z++) {
				if(CheckPointValidity(x,z)){
					if (controlNodes[x,z].state == state){
						stateFound = true;
					}
				}
			}			
		}
		return stateFound;
	}

	void GenerateBridgesInAllDirections(ControlNode givenNode,int _bridgeWidth){
		GenerateBridgeFromPointX (true, givenNode,_bridgeWidth);
		GenerateBridgeFromPointX (false, givenNode,_bridgeWidth);
		GenerateBridgeFromPointZ (true, givenNode,_bridgeWidth);
		GenerateBridgeFromPointZ (false, givenNode,_bridgeWidth);
	}

	void GenerateBridgeFromPointX(bool upDown, int x, int z){
		int direction = -1;
		if (upDown==false){direction = 1;}
		List<ControlNode> bridgeList = new List<ControlNode> ();
		for (int i = 1; i < worldSize; i++) {
			if (CheckPointValidity(x+i*direction,z)==false){bridgeList.Clear (); return;}
			else if(controlNodes[x+i*direction,z].occupied==false){bridgeList.Add (controlNodes [x + i*direction, z]);}
			else {break;
				}}
		foreach (ControlNode node in bridgeList){
			SetControlNodeTileState (ControlNode.TileState.Bridge, node.positionX, node.positionZ);
		}
		bridgeList.Clear ();
	}

	void GenerateBridgeFromPointX(bool upDown, ControlNode givenNode, int _bridgeWidth){
		if(CheckPointValidity(givenNode.positionX,givenNode.positionZ)) {
			int x = givenNode.positionX;
			int z = givenNode.positionZ;
			int direction = -1;
			if (upDown == false) {
				direction = 1;
			}


			for (int iW = 0; iW < _bridgeWidth; iW++) {
				List<ControlNode> bridgeList = new List<ControlNode> ();
				for (int i = 1; i < worldSize; i++) {
					if (CheckPointValidity (x + i * direction, z + iW) == false) {
						bridgeList.Clear ();
						return;
					} else if (controlNodes [x + i * direction, z + iW].occupied == false) {
						bridgeList.Add (controlNodes [x + i * direction, z + iW]);
					} else {
						break;
					}
				}
				foreach (ControlNode node in bridgeList) {
					SetControlNodeTileState (ControlNode.TileState.Bridge, node.positionX, node.positionZ);
				}
				bridgeList.Clear ();
			}
		}


	}

	void GenerateBridgeFromPointZ(bool upDown, int x, int z){
		if(CheckPointValidity(x,z)) {
			int direction = -1;
			if (upDown == false) {
				direction = 1;
			}

			for (int iW = 0; iW < bridgeWidth; iW++) {
				List<ControlNode> bridgeList = new List<ControlNode> ();
				for (int i = 1; i < worldSize; i++) {
					if (CheckPointValidity (x + iW, z + i * direction) == false) {
						bridgeList.Clear ();
						return;
					} else if (controlNodes [x + iW, z + i * direction].occupied == false) {
						bridgeList.Add (controlNodes [x + iW, z + i * direction]);
					} else {
						break;
					}
				}
				foreach (ControlNode node in bridgeList) {
					SetControlNodeTileState (ControlNode.TileState.Bridge, node.positionX, node.positionZ);
				}
				bridgeList.Clear ();
			}
		}

	}

	void GenerateBridgeFromPointZ(bool upDown, ControlNode givenNode, int _bridgeWidth){
		if(CheckPointValidity(givenNode.positionX,givenNode.positionZ)) {
			int x = givenNode.positionX;
			int z = givenNode.positionZ;
			int direction = -1;
			if (upDown == false) {
				direction = 1;
			}

			for (int iW = 0; iW < _bridgeWidth; iW++) {
				List<ControlNode> bridgeList = new List<ControlNode> ();
				for (int i = 1; i < worldSize; i++) {

					if (CheckPointValidity (x + iW, z + i * direction) == false) {
						bridgeList.Clear ();
						return;
					} else if (controlNodes [x + iW, z + i * direction].occupied == false) {
						bridgeList.Add (controlNodes [x + iW, z + i * direction]);
					} else {
						break;
					}
				}
				foreach (ControlNode node in bridgeList) {
					SetControlNodeTileState (ControlNode.TileState.Bridge, node.positionX, node.positionZ);
				}
				bridgeList.Clear ();
			}
		}

	}

	void FindRoomStartPoint () //for one room only
	{
		var position = new Vector3 ();
		position.x = Random.Range ((roomSize / 2), (worldSize - roomSize / 2));
		position.z = Random.Range ((roomSize / 2), (worldSize - roomSize / 2));
		SetControlNodeTileState (ControlNode.TileState.Floor, (int)position.x, (int)position.z);
		if (position.x > (roomSize / 2) - 1 && position.x < (worldSize - roomSize / 2) && position.z > (roomSize / 2) - 1 && position.z < (worldSize - roomSize / 2)) {
			
		}
	}

	void FindAllStartPoints ()  //gets all the points a first room can occupy(debug function)
	{ 
		for (int x = 0; x < worldSize; x++) {
			for (int z = 0; z < worldSize; z++) {
				if (x > (roomSize / 2) - 1 && x < (worldSize - roomSize / 2) && z > (roomSize / 2) - 1 && z < (worldSize - roomSize / 2)) {
					SetControlNodeTileState (ControlNode.TileState.Floor, x, z);	
				}
			}
		}
	}
		
	void GenerateControlGrid ()
	{
		for (int x = 0; x < worldSize; x++) {
			for (int z = 0; z < worldSize; z++) {
				GenerateControlNode (x, z);

			}
		}
	}

	void GenerateControlNode (int x, int z)
	{
		ControlNode controlNode = Instantiate<ControlNode> (controlNodePrefab);
		Vector3 position = new Vector3 (x - worldSize / 2, 0, z - worldSize / 2);
		controlNode.transform.position = position;
		controlNodes [x, z] = controlNode;

		controlNode.state = ControlNode.TileState.Empty;
		controlNode.positionX = x;
		controlNode.positionZ = z;
		controlNode.iD = new Vector2 (x, z);
		controlNode.transform.parent = transform; //GameObject.Find("mapHolder").transform
		controlNode.transform.parent = GameObject.Find("Control Nodes").transform; //GameObject.Find("mapHolder").transform
	}
	void SetControlNodeTileState(ControlNode.TileState state, int x,int z, float probability,float height){
		Vector3 position = new Vector3 (0, -height/10f, 0);
		controlNodes [x, z].gameObject.transform.localPosition += position;
		SetControlNodeTileState (state, x, z, probability);
	}

	void SetControlNodeTileState(ControlNode.TileState state, int x, int z, float probability){
		float random = (float)pRNG.Next(0,10);
		if(random <= probability*10){
			SetControlNodeTileState (state, x, z);
		}
	}

	void SetControlNodeTileState (ControlNode.TileState state, int x, int z)
	{
		if (x < 0 || x >= worldSize || z < 0 || z >= worldSize) {
			return;


//			if(mainFloorList.Contains(controlNodes[x,z])){mainFloorList.Remove (controlNodes [x, z]);}
//			if(mainBridgeList.Contains(controlNodes[x,z])){mainBridgeList.Remove (controlNodes [x, z]);}
		}
		if(mainFloorHSet.Contains(controlNodes[x,z])){mainFloorHSet.Remove (controlNodes [x, z]);}
		if(mainBridgeHSet.Contains(controlNodes[x,z])){mainBridgeHSet.Remove (controlNodes [x, z]);}
		controlNodes [x, z].occupied = true;

		if (state == ControlNode.TileState.Floor) {
			controlNodes [x, z].state = ControlNode.TileState.Floor;
			controlNodes [x, z].GetComponent<Renderer> ().material = matFloor;
			mainFloorHSet.Add (controlNodes [x, z]);
//			mainFloorList.Add (controlNodes [x, z]);
		} else if (state == ControlNode.TileState.Bridge) {
				
			controlNodes [x, z].state = ControlNode.TileState.Bridge;
			controlNodes [x, z].GetComponent<Renderer> ().material = matBridges;
			mainBridgeHSet.Add (controlNodes [x, z]);
//			mainBridgeList.Add (controlNodes [x, z]);
//			posAdd += adder;
//			controlNodes [x, z].gameObject.transform.localPosition += posAdd;
		} else if (state == ControlNode.TileState.RoomCorner) {
			controlNodes [x, z].state = state;
			controlNodes [x, z].GetComponent<Renderer> ().material = matCorners;
		} else if (state == ControlNode.TileState.BridgeAnchor) {
			controlNodes [x, z].state = ControlNode.TileState.BridgeAnchor;
			controlNodes [x, z].GetComponent<Renderer> ().material = matCorners;
			bridgeAnchorTiles [bridgeAnchorIndex++] = controlNodes [x, z];
		} else if (state == ControlNode.TileState.Wall) {
			controlNodes [x, z].state = ControlNode.TileState.Wall;
			controlNodes [x, z].GetComponent<Renderer> ().material = matWall;
			//bridgeAnchorTiles [bridgeAnchorIndex++] = controlNodes [x, z];
		}else if(state== ControlNode.TileState.CornerRoomCorners){
			controlNodes [x, z].state = state;
			controlNodes [x, z].GetComponent<Renderer> ().material = matCorners;
			cornerRoomCornerTiles[cornerRoomCornerTilesIndex++] = controlNodes [x, z];
		}else if(state== ControlNode.TileState.DekoTiles){
			controlNodes [x, z].state = state;
			controlNodes [x, z].GetComponent<Renderer> ().material = matDeko;
		}else if(state==ControlNode.TileState.CornerRoomMiddles){
			controlNodes [x, z].state = state;
			controlNodes [x, z].GetComponent<Renderer> ().material = matMiddles;
			cornerRoomMiddleTiles [cornerRoomMiddleTilesIndex++] = controlNodes [x, z];
		}else if(state==ControlNode.TileState.Empty){
			controlNodes [x, z].state = state;
			controlNodes [x, z].GetComponent<Renderer> ().material = matEmpty;
			controlNodes [x, z].occupied = false;
		}
	}

}

