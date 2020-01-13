using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator_02_Maze : LevelGenerator {

	[Range(5,50)]
	public int radius;

	[Range(0,360)]
	public int radiusOfTestCircle;

	public bool startRooms;
	[Range(10,50)]
	public int startRoomHalfWidth;
	int startRoomWidth;
	[Range(1,5)]
	public int startRoomExitNumber;
	[Range(6,50)]
	public int startRoomExitlength;
	[Range(5,50)]
	public int startRoomDepth;

	public bool mainHallways;
	int hallwayWidth;
	[Range(6,20)]
	public int hallwayLengthMain;
	[Range(5,100)]
	public int hallwayRepetitionsMain;

	public bool secondaryHallways;
	public bool tinyWayRooms;
	[Range(6,20)]
	public int hallwayLengthSecondary;
	[Range(5,100)]
	public int hallwayRepetitionsSecondary;

	int lastUsedDirectionSecondary;

	MeshGenerator_02_Maze meshGen;

//	public int firstHallwayRepetitions;

	public enum HallWayWidth{small, big};
	public HallWayWidth widthMainHallWays;
	public HallWayWidth widthSecondaryHallWays;

	public enum HallWayDirection{straight, left, random, right };
	public HallWayDirection directionMainHallWays;
	public HallWayDirection directionSecondaryHallWays;

	int HallWayWidthMain;
	int hallWayWidthSecondary;


	System.Random pRNG;
	System.Random pRNGMazeArm_01;
	System.Random pRNGTilestorm;
	ControlNode[] possibleStartRoomPoints;
	int startRoomIndex;
	int endRoomIndex;

	ControlNode[] startRoomWayPoints;
	ControlNode[] endRoomWayPoints;
	ControlNode[] startRoomWayPoints_02;
	ControlNode[] endRoomWayPoints_02;
	ControlNode[] roomPoints;

	ControlNode[] startRoomExitPoints;
	ControlNode[] endRoomExitPoints;
	bool setMainPath; // used to identify whick path is currently being set, to check the correct direction settings
//	List<ControlNode[]> roomPointsList;
	int[] randomDirections;
	void Start(){
		GenerateMap ();
	}
	public new void GenerateMap(){
		setMainPath = true;
		HallWayWidthMain = 5;
		hallWayWidthSecondary = 5;
		if(widthMainHallWays==HallWayWidth.big){HallWayWidthMain = 7;}
		if(widthSecondaryHallWays==HallWayWidth.big){hallWayWidthSecondary = 7;}
		GetWorldSize ();
		SetInitialFields ();
		startRoomWidth = startRoomHalfWidth * 2 + 1;
		randomDirections = new int[]{0, 1, 2, 3 };
		pRNG=new System.Random(seed);
		pRNGMazeArm_01 = new System.Random (seed);
		pRNGTilestorm = new System.Random (seed);
//		roomPointsList = new List<ControlNode[]> ();

		int startRoomDirection = pRNG.Next (0, 4);
		int endRoomDirection = startRoomDirection + 2;
		if (endRoomDirection > 3)endRoomDirection -= 4;

		CreateAxisIndicator ();


//		controlNodes [100, 100].lastUsedDirection = 0;
//		CreateMazeArm (controlNodes [100, 100], HallWayWidthMain, hallwayLengthMain, 10);
//
		if(startRooms){
			//------------StartRoom------------------------------
			startRoomWayPoints= CreateStartRoomPoints (startRoomDirection,startRoomWidth,startRoomDepth);
			int startRoomIndex = pRNG.Next (0, startRoomWayPoints.Length-1);

			CreateOrientedRoom (startRoomWayPoints [startRoomIndex],startRoomDirection,startRoomWidth, startRoomDepth);
			int startRoomPosX = startRoomWayPoints [startRoomIndex].positionX;
			int startRoomPosZ = startRoomWayPoints [startRoomIndex].positionZ;
			startRoomExitPoints = GenerateStartRoomExitPoints (startRoomDirection, startRoomPosX, startRoomPosZ);




			//		CreateMazeArm(controlNodes[100,100],5,10,40);
			//		SetPathinDirection (controlNodes [50, 50], 5, 10, 0);
			//------------EndRoom------------------------------
			endRoomWayPoints = CreateStartRoomPoints (endRoomDirection, startRoomWidth, startRoomDepth);
			int endRoomIndex=pRNG.Next (0, endRoomWayPoints.Length-1);

			CreateOrientedRoom (endRoomWayPoints [endRoomIndex],endRoomDirection,startRoomWidth,startRoomDepth);
			int endRoomPosX = endRoomWayPoints [endRoomIndex].positionX;
			int endRoomPosZ = endRoomWayPoints [endRoomIndex].positionZ;
			endRoomExitPoints = GenerateStartRoomExitPoints (endRoomDirection, endRoomPosX, endRoomPosZ);
		}

		HashSet<ControlNode[]> startRoomSecondaryHallwayEdgePoints = new HashSet<ControlNode[]> ();
		HashSet<ControlNode[]> endRoomSecondaryHallwayEdgePoints = new HashSet<ControlNode[]> ();

		HashSet<ControlNode[]> allRoomStartPoints = new HashSet<ControlNode[]> ();
		ControlNode[][] startRoomPointsCollection = new ControlNode[startRoomExitPoints.Length][];

		ControlNode[][] endRoomPointsCollection = new ControlNode[startRoomExitPoints.Length][];

		if(mainHallways){
			//----------------Creates Main Hallways---------------------------
			for (int i = 0; i < endRoomExitPoints.Length; i++) {
				endRoomPointsCollection[i] = CreateMazeArm (endRoomExitPoints [i], HallWayWidthMain, hallwayLengthMain, hallwayRepetitionsMain);
			}

			for (int i = 0; i < startRoomExitPoints.Length; i++) {
				startRoomPointsCollection[i] = CreateMazeArm (startRoomExitPoints [i], HallWayWidthMain, hallwayLengthMain, hallwayRepetitionsMain);
			}
		}
//		//--------------------- Searching For and Setting Rooms (BigRooms)  deprecated-----------------------------
//		if(mainHallways){
//			HashSet<ControlNode[]> startBigRoomStartPoints = new HashSet<ControlNode[]> ();
//			HashSet<ControlNode[]> endBigRoomStartPoints = new HashSet<ControlNode[]> ();
//			foreach(ControlNode[]startRoomPoints in startRoomPointsCollection){
//				for(int i = 0; i<startRoomPoints.Length;i++){
//					startBigRoomStartPoints.Add(SearchForBigRoomSpace (startRoomPoints [i],20,10));
//				}
//			}
//			foreach(ControlNode[]endRoomPoints in endRoomPointsCollection){
//				for(int i = 0; i<endRoomPoints.Length;i++){
//					endBigRoomStartPoints.Add(SearchForBigRoomSpace (endRoomPoints [i],20,10));
//				}
//			}
//		}

		//--------------------- Searching for and Setting Rooms (BigRooms) ----------------------------
		//----------------- not creating a HashSet, but setting it rightaway --------------------------
		if(mainHallways){
//			System.Random pRNGBigRooms = new System.Random (seed);
			ControlNode[] startRoomPoints = CombineControlNodeArrays(startRoomPointsCollection);
			ControlNode[] endRoomPoints = CombineControlNodeArrays (endRoomPointsCollection);
			GenerateBigRoom (startRoomPoints, 10, 20,true);
			GenerateBigRoom (endRoomPoints, 10, 20,true);
			GenerateBigRoom (startRoomPoints, 10, 20,false);
			GenerateBigRoom (endRoomPoints, 10, 20,false);
		}

		//----------------------------- Secondary Hallways -----------------------------------
		if(secondaryHallways){
			setMainPath = false;
			foreach(ControlNode[] lastendroomRow in endRoomPointsCollection){
				for (int i = 0; i < lastendroomRow.Length; i++) {
					startRoomSecondaryHallwayEdgePoints.Add (CreateMazeArm (lastendroomRow [i], hallWayWidthSecondary, hallwayLengthSecondary, hallwayRepetitionsSecondary)) ;
				}
			}

			foreach(ControlNode[] lastStartRoomRow in startRoomPointsCollection){
				for (int i = 0; i < lastStartRoomRow.Length; i++) {
					endRoomSecondaryHallwayEdgePoints.Add (CreateMazeArm (lastStartRoomRow [i], hallWayWidthSecondary, hallwayLengthSecondary, hallwayRepetitionsSecondary));
				}
			}
			if(tinyWayRooms){
				foreach(ControlNode[] nodes in startRoomSecondaryHallwayEdgePoints){
					for (int i = 0; i < nodes.Length; i+=3) {
						if(nodes[i].positionX>8&&nodes[i].positionX<worldSize-8&&nodes[i].positionZ>8&&nodes[i].positionZ<worldSize-8)
						CreateRoom (nodes [i], 7, 7);
					}
				}
				foreach(ControlNode[] nodes in endRoomSecondaryHallwayEdgePoints){
					for (int i = 0; i < nodes.Length; i+=3) {
						if(nodes[i].positionX>8&&nodes[i].positionX<worldSize-8&&nodes[i].positionZ>8&&nodes[i].positionZ<worldSize-8)
						CreateRoom (nodes [i], 7, 7);
					}
				}
			}



		}
//		CreateRoom (controlNodes [100, 100], 12, 12);
//		CreateRoundRoom (controlNodes [100, 100], 12);

		//---------------------------------CALLING MESH Instantiation !!!!!!!----------
		meshGen = GetComponent<MeshGenerator_02_Maze>();
		meshGen.GenerateMazeMesh ();
//		MeshGenerator_02_Maze.GenerateCollision();


	}

	void GenerateBigRoom(ControlNode[] inputArray, int roomSize, int roomOffset,bool roomtype){
		int[] inputArrayOrder = new int[inputArray.Length];
		ControlNode startNode;
		ControlNode roomCenterNode;
		for (int i = 0; i < inputArray.Length; i++) {
			inputArrayOrder [i] = i;
		}
		int[] inputArrayShuffledOrder = ShuffleArray (inputArrayOrder);

		for (int i = 0; i < inputArray.Length; i++) {
			roomCenterNode = SearchForBigRoomSpace (inputArray [inputArrayShuffledOrder [i]], roomOffset, roomSize);
			if(roomCenterNode !=null){
				startNode = inputArray [inputArrayShuffledOrder [i]];
				if(roomtype){
					CreateRoundRoom (roomCenterNode, roomSize);
				}else CreateChapelRoom(roomCenterNode,22,13,roomCenterNode.lastUsedDirection);

				int startPosX = startNode.positionX;
				int startPosZ = startNode.positionZ;
				int endPosX = roomCenterNode.positionX;
				int endPosZ = roomCenterNode.positionZ;

				switch(roomCenterNode.lastUsedDirection){

				case 1:
					for(int x = startPosX+2;x<endPosX-roomSize+3;x++){
						for(int z = startPosZ-3;z<startPosZ+4;z++){
							SetControlNodeTileState (ControlNode.TileState.Floor, x, z);
							if(z==startPosZ-3||z==startPosZ+3)SetControlNodeTileState (ControlNode.TileState.Wall, x, z);
						}
					}

					break;
				case 2:
					for(int z=startPosZ+2;z<endPosZ-roomSize+3;z++){
						for(int x=startPosX-3;x<startPosX+4;x++){
							SetControlNodeTileState (ControlNode.TileState.Floor, x, z);
							if(x==startPosX-3||x==startPosX+3)SetControlNodeTileState (ControlNode.TileState.Wall, x, z);
						}
					}

					break;
				case 3:
					for(int x = endPosX+roomSize-2;x<startPosX-2;x++){
						for(int z = endPosZ-3;z<endPosZ+4;z++){
							SetControlNodeTileState (ControlNode.TileState.Floor, x, z);
							if(z==endPosZ-3||z==endPosZ+3)SetControlNodeTileState (ControlNode.TileState.Wall, x, z);
						}
					}
					break;
				case 0:
					for(int z = endPosZ+roomSize-2;z<startPosZ-2;z++){
						for(int x = endPosX-3;x<endPosX+4;x++){
							SetControlNodeTileState (ControlNode.TileState.Floor, x, z);
							if(x==endPosX-3||x==endPosX+3)SetControlNodeTileState (ControlNode.TileState.Wall, x, z);
						}
					}

					break;
				}




				break;
			}
		}
		//-------create bridge----------

	}

	ControlNode[] CombineControlNodeArrays(ControlNode[][] inputArray){ //----------Converts a double to a One dimensional Array----------------!
		int combinedLength = 0;
		int[] allLengths = new int[inputArray.Length+1];
		allLengths[0]=0;
		int xi = 1;
		foreach(ControlNode[] child in inputArray){
			combinedLength += child.Length;
			allLengths [xi++] = combinedLength;
		}
		ControlNode[] returnedArray = new ControlNode[combinedLength];
		for(int i =0;i<inputArray.Length;i++){
			inputArray [i].CopyTo (returnedArray, allLengths [i]);
		}
		return returnedArray;
	}

	void CreateChapelRoom(ControlNode givenNode, int maxLength, int maxWidth,int direction){

		int posX = givenNode.positionX;
		int posZ = givenNode.positionZ;

		for(int length = 1; length<maxLength; length++){
			for(int width = 0; width<maxWidth; width++){
				switch(direction){
				case 1:
					SetControlNodeTileState (ControlNode.TileState.Floor, posX + length-maxLength/2, posZ + width-maxWidth/2);
					if(length==1||length==maxLength-1||width==0||width==maxWidth-1)SetControlNodeTileState (ControlNode.TileState.Wall, posX + length-maxLength/2, posZ + width-maxWidth/2);
					break;
				case 2:
					SetControlNodeTileState (ControlNode.TileState.Floor, posX + width-maxWidth/2, posZ + length-maxLength/2);
					if(length==1||length==maxLength-1||width==0||width==maxWidth-1)SetControlNodeTileState (ControlNode.TileState.Wall, posX + width-maxWidth/2, posZ + length-maxLength/2);
					break;
				case 3:
					SetControlNodeTileState (ControlNode.TileState.Floor, posX - length+maxLength/2, posZ + width-maxWidth/2);
					if(length==1||length==maxLength-1||width==0||width==maxWidth-1)SetControlNodeTileState (ControlNode.TileState.Wall, posX - length+maxLength/2, posZ + width-maxWidth/2);
					break;
				case 0:
					SetControlNodeTileState (ControlNode.TileState.Floor, posX + width-maxWidth/2, posZ -length+maxLength/2);
					if(length==1||length==maxLength-1||width==0||width==maxWidth-1)SetControlNodeTileState (ControlNode.TileState.Wall, posX + width-maxWidth/2, posZ -length+maxLength/2);
					break;
				}
			}
		}
		for (int width = -2;width<maxWidth+2;width+=maxWidth+3){
			for(int length2 = 9;length2<16;length2+=6){
			for(int length = 0;length<4;length++)
				{
					switch(direction){
					case 1:
						SetControlNodeTileState (ControlNode.TileState.Wall, posX + length + length2-maxLength/2, posZ + width-maxWidth/2);
						break;
					case 2:
						SetControlNodeTileState (ControlNode.TileState.Wall, posX + width-maxWidth/2, posZ + length + length2-maxLength/2);
						break;
					case 3:
						SetControlNodeTileState (ControlNode.TileState.Wall, posX - length - length2+maxLength/2, posZ + width-maxWidth/2);
						break;
					case 0:
						SetControlNodeTileState (ControlNode.TileState.Wall, posX + width-maxWidth/2, posZ - length - length2+maxLength/2);
						break;
					}
				}
			}
		}
		for(int width = -1;width<maxWidth+1;width+=maxWidth){
			for(int length =10;length<17;length+=6){
				for(int length2=0;length2<2;length2++){
					for(int width2 = 0;width2<2;width2++){
						switch(direction){
						case 1:
							SetControlNodeTileState (ControlNode.TileState.Floor, posX + length + length2-maxLength/2, posZ + width+width2-maxWidth/2);
							break;
						case 2:
							SetControlNodeTileState (ControlNode.TileState.Floor, posX + width+width2-maxWidth/2, posZ + length + length2-maxLength/2);
							break;
						case 3:
							SetControlNodeTileState (ControlNode.TileState.Floor, posX - length - length2+maxLength/2, posZ + width+width2-maxWidth/2);
							break;
						case 0:
							SetControlNodeTileState (ControlNode.TileState.Floor, posX + width+width2-maxWidth/2, posZ - length - length2+maxLength/2);
							break;
						}
					}
				}
			}
		}
		for(int width = -1;width<maxWidth+2;width+=maxWidth+1){
			for(int length = 9;length<17;length+=6){
				for(int length2 = 0; length2<4;length2+=3){
					switch(direction){
					case 1:
						SetControlNodeTileState (ControlNode.TileState.Wall, posX + length + length2-maxLength/2, posZ + width-maxWidth/2);
						break;
					case 2:
						SetControlNodeTileState (ControlNode.TileState.Wall, posX + width-maxWidth/2, posZ + length + length2-maxLength/2);
						break;
					case 3:
						SetControlNodeTileState (ControlNode.TileState.Wall, posX - length - length2+maxLength/2, posZ + width-maxWidth/2);
						break;
					case 0:
						SetControlNodeTileState (ControlNode.TileState.Wall, posX + width-maxWidth/2, posZ - length - length2+maxLength/2);
						break;
					}
				}
			}
		}
		for(int width = 3;width<maxWidth-3;width+=maxWidth-7){
			for(int length = 9;length<16;length+=6){
				for(int length2 = 0; length2<4;length2+=3){
					switch(direction){
					case 1:
						SetControlNodeTileState (ControlNode.TileState.Wall, posX + length + length2-maxLength/2, posZ + width-maxWidth/2);
						break;
					case 2:
						SetControlNodeTileState (ControlNode.TileState.Wall, posX + width-maxWidth/2, posZ + length + length2-maxLength/2);
						break;
					case 3:
						SetControlNodeTileState (ControlNode.TileState.Wall, posX - length - length2+maxLength/2, posZ + width-maxWidth/2);
						break;
					case 0:
						SetControlNodeTileState (ControlNode.TileState.Wall, posX + width-maxWidth/2, posZ - length - length2+maxLength/2);
						break;
					}
				}
			}
		}
		int lengthOffset = 13;
		int addroomSize = 6;
		int widthOffset = 0;
		for(int width=0;width<=addroomSize;width++){
			for(int length = 0;length<=addroomSize/2;length++){
				switch(direction){
				case 1:
					SetControlNodeTileState (ControlNode.TileState.Floor, posX + length-addroomSize/2+lengthOffset, posZ + width-addroomSize/2+widthOffset);
					if(length==addroomSize/2||width==0||width==addroomSize)SetControlNodeTileState (ControlNode.TileState.Wall, posX + length-addroomSize/2+lengthOffset, posZ + width-addroomSize/2+widthOffset);
					break;
				case 2:
					SetControlNodeTileState (ControlNode.TileState.Floor, posX + width-addroomSize/2+widthOffset, posZ + length-addroomSize/2+lengthOffset);
					if(length==addroomSize/2||width==0||width==addroomSize)SetControlNodeTileState (ControlNode.TileState.Wall, posX + width-addroomSize/2+widthOffset, posZ + length-addroomSize/2+lengthOffset);
					break;
				case 3:
					SetControlNodeTileState (ControlNode.TileState.Floor, posX - length+addroomSize/2-lengthOffset, posZ + width-addroomSize/2+widthOffset);
					if(length==addroomSize/2||width==0||width==addroomSize)SetControlNodeTileState (ControlNode.TileState.Wall, posX - length+addroomSize/2-lengthOffset, posZ + width-addroomSize/2+widthOffset);
					break;
				case 0:
					SetControlNodeTileState (ControlNode.TileState.Floor, posX + width-addroomSize/2+widthOffset, posZ -length+addroomSize/2-lengthOffset);
					if(length==addroomSize/2||width==0||width==addroomSize)SetControlNodeTileState (ControlNode.TileState.Wall, posX + width-addroomSize/2+widthOffset, posZ -length+addroomSize/2-lengthOffset);
					break;
				}
			}
		}
	}


	void CreateRoundRoom(ControlNode givenNode, int radius){
		int posX = givenNode.positionX;
		int posZ = givenNode.positionZ;
		int innerRadius = (int)(radius * .5f);
		for (int x1 = -radius; x1 < radius; x1++) {
			int z = (int)Mathf.Sqrt (radius * radius - x1 * x1);
//			print (z);
			for(int zOffset = -z;zOffset<z;zOffset++){
				SetControlNodeTileState (ControlNode.TileState.Floor, posX + x1, posZ -zOffset);

			}
			SetControlNodeTileState (ControlNode.TileState.Wall, posX + x1, posZ+z);
			SetControlNodeTileState (ControlNode.TileState.Wall, posX - x1, posZ-z);

		}

		for(int z1 = -radius;z1<radius;z1++){
			int x = (int)Mathf.Sqrt (radius * radius - z1 * z1);
			SetControlNodeTileState (ControlNode.TileState.Wall, posX + x, posZ + z1);
			SetControlNodeTileState (ControlNode.TileState.Wall, posX - x, posZ - z1);
		}
		for (int x = -(radius / 3); x <= radius / 3; x++) {
			int z = radius;
			SetControlNodeTileState (ControlNode.TileState.Wall, posX + x, posZ + z);
			SetControlNodeTileState (ControlNode.TileState.Wall, posX - x, posZ - z);
			SetControlNodeTileState (ControlNode.TileState.Wall, posX + z, posZ + x);
			SetControlNodeTileState (ControlNode.TileState.Wall, posX - z, posZ - x);
		}
		SetControlNodeTileState (ControlNode.TileState.Wall, posX + innerRadius-3, posZ + innerRadius);
		SetControlNodeTileState (ControlNode.TileState.Wall, posX + innerRadius, posZ + innerRadius-3);
		SetControlNodeTileState (ControlNode.TileState.Wall, posX - innerRadius+3, posZ - innerRadius);
		SetControlNodeTileState (ControlNode.TileState.Wall, posX - innerRadius, posZ - innerRadius+3);
		SetControlNodeTileState (ControlNode.TileState.Wall, posX - innerRadius+3, posZ + innerRadius);
		SetControlNodeTileState (ControlNode.TileState.Wall, posX + innerRadius, posZ - innerRadius+3);
		SetControlNodeTileState (ControlNode.TileState.Wall, posX + innerRadius-3, posZ - innerRadius);
		SetControlNodeTileState (ControlNode.TileState.Wall, posX - innerRadius, posZ + innerRadius-3);
		int offset = 6;
		SetControlNodeTileState (ControlNode.TileState.Wall, posX + radius-offset, posZ + radius);
		SetControlNodeTileState (ControlNode.TileState.Wall, posX + radius, posZ + radius-offset);
		SetControlNodeTileState (ControlNode.TileState.Wall, posX - radius+offset, posZ - radius);
		SetControlNodeTileState (ControlNode.TileState.Wall, posX - radius, posZ - radius+offset);
		SetControlNodeTileState (ControlNode.TileState.Wall, posX - radius+offset, posZ + radius);
		SetControlNodeTileState (ControlNode.TileState.Wall, posX + radius, posZ - radius+offset);
		SetControlNodeTileState (ControlNode.TileState.Wall, posX + radius-offset, posZ - radius);
		SetControlNodeTileState (ControlNode.TileState.Wall, posX - radius, posZ + radius-offset);

	}

//	void CreateSmallRoundRoom(ControlNode givenNode, int radius){
//		int posX = givenNode.positionX;
//		int posZ = givenNode.positionZ;
//		int radiusSQ = radius * radius;
//		for(int x = -radius;x<radius;x++){
//			int z = (int)Mathf.Sqrt()
//		}
//
//	}

	ControlNode[] CreateStartRoomPoints(int direction,int roomWidth, int roomDepth){

		int stepper =4;

		var roomWayPoints = new ControlNode[((worldSize - roomWidth) / stepper)];
		int offsetFromBorder = roomDepth/2;
		int roomWayPointsCounter = 0;
		for (int i = 0; i < roomWayPoints.Length; i++) {			
			switch(direction){
			case 1:
				roomWayPoints [roomWayPointsCounter++] = controlNodes [roomWidth/2 +i*stepper, offsetFromBorder];
				controlNodes [roomWidth/2 +i*stepper, offsetFromBorder].GetComponent<Renderer> ().material = matDeko;
				break;
			case 2:
				roomWayPoints [roomWayPointsCounter++] = controlNodes [worldSize - offsetFromBorder-1, roomWidth/2+i*stepper];
				controlNodes [worldSize - offsetFromBorder-1, roomWidth/2+i*stepper].GetComponent<Renderer> ().material = matDeko;
				break;
			case 3:
				roomWayPoints [roomWayPointsCounter++] = controlNodes [roomWidth/2+i*stepper, worldSize - offsetFromBorder-1];
				controlNodes [roomWidth/2+i*stepper, worldSize - offsetFromBorder-1].GetComponent<Renderer> ().material = matDeko;
				break;
			case 0:
				roomWayPoints [roomWayPointsCounter++] = controlNodes [offsetFromBorder, roomWidth/2+i*stepper];
				controlNodes [offsetFromBorder, roomWidth/2+i*stepper].GetComponent<Renderer> ().material = matDeko;
				break;
			}

		}
		return roomWayPoints;
	}

	void CreateTileStorm(ControlNode givenNode){
		int posX = givenNode.positionX;
		int posZ = givenNode.positionZ;
		for (int i = 0; i < 10; i++) {
			int x = pRNGTilestorm.Next (-10, 10);
			int z = pRNGTilestorm.Next (-10, 10);
			if(CheckPointValidity(posX+x,posZ+z)){
				if (!CheckPointOccupied (posX + x, posZ + z))
					SetControlNodeTileState (ControlNode.TileState.DekoTiles, posX + x, posZ + z);
			}
		}
	}

	ControlNode[] GenerateStartRoomExitPoints(int direction, int x, int z){
		var exitPoints = new ControlNode[startRoomExitNumber];
		for (int i = 0; i < startRoomExitNumber; i++) {
			int setLength = -startRoomWidth / 2 + (startRoomWidth / (startRoomExitNumber))/2 + (startRoomWidth / startRoomExitNumber) * i;
			switch(direction){
			case 1:
				exitPoints [i] = CreateOrientedRoom (controlNodes [x + setLength, z + startRoomDepth / 2 + startRoomExitlength / 2 - 2], direction, HallWayWidthMain, startRoomExitlength);

				break;
			case 2:
				exitPoints [i] = CreateOrientedRoom (controlNodes [x - startRoomDepth / 2 - startRoomExitlength / 2 + 2, z + setLength], direction, HallWayWidthMain, startRoomExitlength);
				 
				break;
			case 3:
				exitPoints [i] = CreateOrientedRoom (controlNodes [x - setLength, z - startRoomDepth / 2 - startRoomExitlength / 2 + 2], direction, HallWayWidthMain, startRoomExitlength);
				 
				break;
			case 0:
				exitPoints [i] = CreateOrientedRoom (controlNodes [x - 2 + startRoomDepth / 2 + startRoomExitlength / 2, z + setLength], direction, HallWayWidthMain, startRoomExitlength);
				 
				break;
			}
		}
		return exitPoints;
	}




	ControlNode[] CreateMazeArm (ControlNode startNode, int width, int length, int turns)
	{
		lastUsedDirectionSecondary = 0;
		var wayPointSet = new HashSet<ControlNode> ();
		for (int iTurns = 0; iTurns < turns; iTurns++) {
			
			// setting directions to random or not
			int[] randomDirectionsLocal = PreferredDirectionShuffle (randomDirections,startNode.lastUsedDirection);
			if(setMainPath) { randomDirections = AddDirectionToArray (randomDirectionsLocal, directionMainHallWays);
			} else{randomDirections = AddDirectionToArray (randomDirectionsLocal, directionSecondaryHallWays);}

//			print ("lastusedDirection = " + startNode.lastUsedDirection);
//			print ("currentDirection = " + randomDirections [0]);
//			print (randomDirections [1]);
//			print ("length = " + randomDirections.Length);

			for (int i = 0; i < randomDirections.Length; i++) {
				if (SearchPathinDirection (startNode, width, length, randomDirections [i])) {
					startNode = SetPathinDirection (startNode, width, length, randomDirections [i]);
					wayPointSet.Add (startNode);
					break;
				}
			}
		}
		ControlNode[] wayPointArray = new ControlNode[wayPointSet.Count];
		wayPointSet.CopyTo (wayPointArray);
		return wayPointArray;
	
	}

	void PrintArray(int[] input){
		for (int i = 0; i < input.Length; i++) {
			print ("i  = " + input [i]);
		}
	}

	int[] ShuffleArray(int[] input){ // Fisher Yates Shuffle
		int[]returnArray;
		int shuffleTurns = 4;
//		print ("before");
//		PrintArray (input);
		for(int si = 0;si<shuffleTurns;si++){
			for (int i = input.Length-1; i > 0; i--) {
				int rnD = pRNGMazeArm_01.Next (0, i);
				int tmP = input [i];

				input [i] = input [rnD];
				input [rnD] = tmP;
			}
		}



		returnArray = input;
//		print ("after");
//		print (input [0]);
////		PrintArray (input);
//		print ("end of after");
		return returnArray;
	}

	ControlNode[] SearchForBigRoomSpaces (ControlNode givenNode, int roomOffset, int roomHalfSize) //checks for bigroomSpace and returns an Array of ControlNodes for all available directions
	{
		var returnedNodesSet = new HashSet<ControlNode> ();
		int posX = givenNode.positionX;
		int posZ = givenNode.positionZ;
		int searchDistance = roomOffset+roomHalfSize*2;
		for (int directions = 0; directions < 4; directions++) {
			bool pathSucceeded = false;
			switch(directions){
			case 1:
				for (int length = 4; length < searchDistance; length++) {
					if (CheckPointOccupied (posX + length, posZ)) {
						break;
					}
					if (length == searchDistance - 1)
						pathSucceeded = true;
				}
				if(pathSucceeded==true){
					int x = posX + searchDistance - roomHalfSize;
					int z = posZ;
					if(CheckBigRoomSpace(controlNodes[x, z],roomHalfSize)){
						SetControlNodeTileState (ControlNode.TileState.Wall, x, z);
						controlNodes [x, z].lastUsedDirection = directions;
						returnedNodesSet.Add (controlNodes [x, z]);
					}
					pathSucceeded = false;
				}

				break;
			case 2:
				for (int length = 4; length < searchDistance; length++) {
					if (CheckPointOccupied (posX, posZ+ length)) {
						break;
					}
					if (length == searchDistance - 1)
						pathSucceeded = true;
				}
				if(pathSucceeded==true){
					int x = posX;
					int z = posZ + searchDistance - roomHalfSize;
					if(CheckBigRoomSpace(controlNodes[x, z],roomHalfSize)){
						SetControlNodeTileState (ControlNode.TileState.Wall, x,z);
						controlNodes [x, z].lastUsedDirection = directions;
						returnedNodesSet.Add (controlNodes [x, z]);
					}
					pathSucceeded = false;
				}

				break;
			case 3:
				for (int length = 4; length < searchDistance; length++) {
					if (CheckPointOccupied (posX - length, posZ)) {
						break;
					}
					if (length == searchDistance - 1)
						pathSucceeded = true;
				}
				if(pathSucceeded==true){
					int x = posX - searchDistance + roomHalfSize;
					int z = posZ;
					if(CheckBigRoomSpace(controlNodes[x,z],roomHalfSize)){
						SetControlNodeTileState (ControlNode.TileState.Wall, x,z);
						controlNodes [x, z].lastUsedDirection = directions;
						returnedNodesSet.Add (controlNodes [x, z]);
					}

					pathSucceeded = false;
				}

				break;
			case 0:
				for (int length = 4; length < searchDistance; length++) {
					if (CheckPointOccupied (posX, posZ-length)) {
						break;
					}
					if (length == searchDistance - 1)
						pathSucceeded = true;
				}
				if(pathSucceeded==true){
					int x = posX;
					int z = posZ - searchDistance + roomHalfSize;
					if(CheckBigRoomSpace(controlNodes[x,z],roomHalfSize)){
						SetControlNodeTileState (ControlNode.TileState.Wall, x,z);
						controlNodes [x, z].lastUsedDirection = directions;
						returnedNodesSet.Add (controlNodes [x, z]);
					}
					pathSucceeded = false;
				}

				break;
			}
		}
		var returnedNodes = new ControlNode[returnedNodesSet.Count];
		returnedNodesSet.CopyTo (returnedNodes);
		return returnedNodes;

	}

	ControlNode SearchForBigRoomSpace (ControlNode givenNode, int roomOffset, int roomHalfSize) //checks for bigroomSpace and returns One ControlNode for the first available direction
	{
		ControlNode returnedNode = null;
		int posX = givenNode.positionX;
		int posZ = givenNode.positionZ;
		int searchDistance = roomOffset+roomHalfSize*2;
		for (int directions = 0; directions < 4; directions++) {
			bool pathSucceeded = false;
			switch(directions){
			case 1:
				for (int length = 4; length < searchDistance; length++) {
					if (CheckPointOccupied (posX + length, posZ)) {
						break;
					}
					if (length == searchDistance - 1)
						pathSucceeded = true;
				}
				if(pathSucceeded==true){
					int x = posX + searchDistance - roomHalfSize;
					int z = posZ;
					if(CheckBigRoomSpace(controlNodes[x, z],roomHalfSize)){ // from here on identical for all directions
						SetControlNodeTileState (ControlNode.TileState.Wall, x, z);
						controlNodes [x, z].lastUsedDirection = directions;
						returnedNode = controlNodes [x, z];
						return returnedNode;
					}
					pathSucceeded = false;
				}

				break;
			case 2:
				for (int length = 4; length < searchDistance; length++) {
					if (CheckPointOccupied (posX, posZ+ length)) {
						break;
					}
					if (length == searchDistance - 1)
						pathSucceeded = true;
				}
				if(pathSucceeded==true){
					int x = posX;
					int z = posZ + searchDistance - roomHalfSize;
					if(CheckBigRoomSpace(controlNodes[x, z],roomHalfSize)){
						SetControlNodeTileState (ControlNode.TileState.Wall, x,z);
						controlNodes [x, z].lastUsedDirection = directions;
						returnedNode = controlNodes [x, z];
						return returnedNode;
					}
					pathSucceeded = false;
				}

				break;
			case 3:
				for (int length = 4; length < searchDistance; length++) {
					if (CheckPointOccupied (posX - length, posZ)) {
						break;
					}
					if (length == searchDistance - 1)
						pathSucceeded = true;
				}
				if(pathSucceeded==true){
					int x = posX - searchDistance + roomHalfSize;
					int z = posZ;
					if(CheckBigRoomSpace(controlNodes[x,z],roomHalfSize)){
						SetControlNodeTileState (ControlNode.TileState.Wall, x,z);
						controlNodes [x, z].lastUsedDirection = directions;
						returnedNode = controlNodes [x, z];
						return returnedNode;
					}

					pathSucceeded = false;
				}

				break;
			case 0:
				for (int length = 4; length < searchDistance; length++) {
					if (CheckPointOccupied (posX, posZ-length)) {
						break;
					}
					if (length == searchDistance - 1)
						pathSucceeded = true;
				}
				if(pathSucceeded==true){
					int x = posX;
					int z = posZ - searchDistance + roomHalfSize;
					if(CheckBigRoomSpace(controlNodes[x,z],roomHalfSize)){
						SetControlNodeTileState (ControlNode.TileState.Wall, x,z);
						controlNodes [x, z].lastUsedDirection = directions;
						returnedNode = controlNodes [x, z];
						return returnedNode;
					}
					pathSucceeded = false;
				}

				break;
			}
		}
		return returnedNode;

	}

	bool CheckBigRoomSpace(ControlNode givenNode, int roomHalfSize){ // checks a Controlnode in all directions (roomHalfSize) for validity and! availability
		bool isValid = false;
		int posX = givenNode.positionX;
		int posZ = givenNode.positionZ;
		if(posX-roomHalfSize<0||posX+roomHalfSize>worldSize-1||posZ-roomHalfSize<0||posZ+roomHalfSize>worldSize-1){return isValid;}
		for (int direction = 0; direction < 4; direction++) {
			switch(direction){
			case 1:
				for (int length = 0; length < roomHalfSize; length++) {
					if (CheckPointOccupied (posX + length, posZ))
						return isValid;
				}
				break;
			case 2:
				for (int length = 0; length < roomHalfSize; length++) {
					if (CheckPointOccupied (posX, posZ +length))
						return isValid;
				}
				break;
			case 3:
				for (int length = 0; length < roomHalfSize; length++) {
					if (CheckPointOccupied (posX - length, posZ))
						return isValid;
				}
				break;
			case 0:
				for (int length = 0; length < roomHalfSize; length++) {
					if (CheckPointOccupied (posX, posZ - length))
						return isValid;
				}
				break;
			}
		}
		for(int x = -roomHalfSize;x<roomHalfSize;x+=roomHalfSize/2){
			for (int z = -roomHalfSize; z < roomHalfSize; z+=roomHalfSize/2) {
				if (CheckPointOccupied (posX + x, posZ + z))
					return isValid;
			}
		}
		isValid = true;
		return isValid;
	}
		

	int[] PreferredDirectionShuffle(int[] input, int direction){ //shuffles array, but sets a preferred direction at top spot
		var returnArray = ShuffleArray (input);

		int temp = returnArray [0];
		returnArray [0] = direction;
//		print ("direction = " + direction);
		for (int i = 1; i < 4; i++) {
			if(returnArray[i] == direction){returnArray [i] = temp; return returnArray;}
		}
		return returnArray;
	}

	int[] AddDirectionToArray(int [] input, HallWayDirection directions){
		for (int i = 0; i < input.Length; i++) {
			input[i]+=(int)directions;
			if (input [i] > 3)
				input [i] -= 4;
		}
		return input;
	}


	bool SearchPathinDirection(ControlNode startNode, int maxWidth, int maxLength, int direction){
		bool availability = false;
		int x = startNode.positionX;
		int z = startNode.positionZ;

		for (int length = 4; length < maxLength; length++) {
			for (int width = 0; width < maxWidth; width++) {
				int maxWidhtSet = maxWidth/2;
				switch(direction){
				case 1:
					if (CheckPointOccupied (x + length, z + width-maxWidhtSet)) {
						return availability;
					}
					break;
				case 2:
					if (CheckPointOccupied (x + width-maxWidhtSet, z + length)) {
						return availability;
					}
					break;
				case 3:
					if (CheckPointOccupied (x - length, z - width+maxWidhtSet)) {
						return availability;
					}
					break;
				case 0:
					if (CheckPointOccupied (x - width+maxWidhtSet, z - length)) {
						return availability;
					}
					break;
				}
			}
		}
		availability = true;
		return availability;

	}

	ControlNode SetPathinDirection(ControlNode startNode, int maxWidth, int maxLength, int direction){
		ControlNode returnedNode = controlNodes[0,0];
		int x = startNode.positionX;
		int z = startNode.positionZ;
		int maxWidhtSet = maxWidth/2;

		for (int length = maxWidth/2 ; length < maxLength; length++) {
			for(int width=0;width<maxWidth;width++){				
				

				switch(direction){
				case 1:
					SetControlNodeTileState (ControlNode.TileState.Floor, x + length, z + width-maxWidhtSet);
					break;
				case 2:
					SetControlNodeTileState (ControlNode.TileState.Floor, x + width-maxWidhtSet, z + length);
					break;
				case 3:
					SetControlNodeTileState (ControlNode.TileState.Floor, x - length, z - width+maxWidhtSet);
					break;
				case 0:
					SetControlNodeTileState (ControlNode.TileState.Floor, x - width+maxWidhtSet, z - length);
					break;
				}
				if(width==0||length==0||length==maxLength-1||width==maxWidth-1){
					switch(direction){
					case 1:
						SetControlNodeTileState (ControlNode.TileState.Wall, x + length, z + width-maxWidhtSet);
						break;
					case 2:
						SetControlNodeTileState (ControlNode.TileState.Wall, x + width-maxWidhtSet, z + length);
						break;
					case 3:
						SetControlNodeTileState (ControlNode.TileState.Wall, x - length, z - width+maxWidhtSet);
						break;
					case 0:
						SetControlNodeTileState (ControlNode.TileState.Wall, x - width+maxWidhtSet, z - length);
						break;
					}
				}
			}
		}
		int returnedNodelengthOffset = maxWidth/2+1;
		switch(direction){
		case 1:
			SetControlNodeTileState (ControlNode.TileState.DekoTiles, x + maxLength - returnedNodelengthOffset, z);
			returnedNode = controlNodes [x + maxLength - returnedNodelengthOffset, z];
			break;
		case 2:
			SetControlNodeTileState (ControlNode.TileState.DekoTiles, x , z + maxLength - returnedNodelengthOffset);
			returnedNode = controlNodes [x, z + maxLength - returnedNodelengthOffset];
			break;
		case 3:
			SetControlNodeTileState (ControlNode.TileState.DekoTiles, x - maxLength + returnedNodelengthOffset, z );
			returnedNode = controlNodes [x - maxLength + returnedNodelengthOffset, z ];
			break;
		case 0:
			SetControlNodeTileState (ControlNode.TileState.DekoTiles, x , z - maxLength + returnedNodelengthOffset);
			returnedNode = controlNodes [x , z - maxLength + returnedNodelengthOffset];
			break;
		}
		returnedNode.lastUsedDirection = direction;
		if(setMainPath){
			for (int length = maxWidth-3; length < maxLength-maxWidth+2; length+=2) {
				for (int width = 1; width < maxWidth-1; width+=maxWidth-3) {
					switch(direction){
					case 1:
						SetControlNodeTileState (ControlNode.TileState.Wall, x + length, z + width-maxWidhtSet);
						break;
					case 2:
						SetControlNodeTileState (ControlNode.TileState.Wall, x + width-maxWidhtSet, z + length);
						break;
					case 3:
						SetControlNodeTileState (ControlNode.TileState.Wall, x - length, z - width+maxWidhtSet);
						break;
					case 0:
						SetControlNodeTileState (ControlNode.TileState.Wall, x - width+maxWidhtSet, z - length);
						break;
					}
				}
			}
		}
		if(!setMainPath){
			int width = 1;
			for (int length = 2; length < maxLength-maxWidth; length += maxWidth*2) {
				if( lastUsedDirectionSecondary == direction){
					switch(direction){
					case 1:
						SetControlNodeTileState (ControlNode.TileState.Wall, x + length, z + width-maxWidhtSet);
						break;
					case 2:
						SetControlNodeTileState (ControlNode.TileState.Wall, x + width-maxWidhtSet, z + length);
						break;
					case 3:
						SetControlNodeTileState (ControlNode.TileState.Wall, x - length, z - width+maxWidhtSet);
						break;
					case 0:
						SetControlNodeTileState (ControlNode.TileState.Wall, x - width+maxWidhtSet, z - length);
						break;
					}
				}
			}
			lastUsedDirectionSecondary = direction;
			width = maxWidth - 2;
			for (int length = -2; length < maxLength-maxWidth; length += maxWidth*2) {
				if( lastUsedDirectionSecondary == direction){
					switch(direction){
					case 1:
						SetControlNodeTileState (ControlNode.TileState.Wall, x + length, z + width-maxWidhtSet);
						break;
					case 2:
						SetControlNodeTileState (ControlNode.TileState.Wall, x + width-maxWidhtSet, z + length);
						break;
					case 3:
						SetControlNodeTileState (ControlNode.TileState.Wall, x - length, z - width+maxWidhtSet);
						break;
					case 0:
						SetControlNodeTileState (ControlNode.TileState.Wall, x - width+maxWidhtSet, z - length);
						break;
					}
				}
			}
		}



		return returnedNode;
	}



	void GenerateStartRooms(){ //deprecated

		endRoomIndex=0;
		FindPossibleStartPoints ();
		startRoomIndex = pRNG.Next (0, possibleStartRoomPoints.Length - 1);
		if(startRoomIndex<possibleStartRoomPoints.Length/2){endRoomIndex = pRNG.Next (possibleStartRoomPoints.Length / 2 + 2, possibleStartRoomPoints.Length - 2);}
		if(startRoomIndex>=possibleStartRoomPoints.Length/2){endRoomIndex = pRNG.Next (2, possibleStartRoomPoints.Length / 2);}


		CreateRoom (possibleStartRoomPoints [startRoomIndex],6, 6);
		CreateRoom (possibleStartRoomPoints [endRoomIndex],6, 6);
	}

	void FindPossibleStartPoints(){ //deprecated

		var startPointsList = new List<ControlNode> ();
//		int i = 0;
		for (int x = 15; x < worldSize-15; x+=10) {
			for (int z = 5; z < worldSize-5; z+=worldSize-11) {

				startPointsList.Add (controlNodes [x, z]);
				controlNodes [x, z].GetComponent<Renderer> ().material = matDeko;
			}
		}
		for (int z = 15; z < worldSize-15; z+=10) {
			for (int x = 5; x < worldSize-5; x+=worldSize-11) {

				startPointsList.Add (controlNodes [x, z]);
				controlNodes [x, z].GetComponent<Renderer> ().material = matDeko;
			}
		}
		possibleStartRoomPoints = startPointsList.ToArray ();
	}

	ControlNode CreateOrientedRoom(ControlNode startNode, int direction, int maxWidth, int maxLength){
		ControlNode returnedNode = controlNodes[0,0];
//		print (startNode.positionX);
		int x = startNode.positionX;
		int z = startNode.positionZ;
		for (int length = 1; length < maxLength-1; length++) {
			for (int width = 1; width < maxWidth-1; width++) {
				switch(direction){
				case 1:
					SetControlNodeTileState (ControlNode.TileState.Floor, x + width - maxWidth / 2, z + length - maxLength / 2);
					controlNodes [x + width - maxWidth / 2, z + length - maxLength / 2].occupied = true;
					break;
				case 2:
					SetControlNodeTileState (ControlNode.TileState.Floor, x + length - maxLength / 2, z + width - maxWidth / 2);
					controlNodes [x + length - maxLength / 2, z + width - maxWidth / 2].occupied = true;
					break;
				case 3:
					SetControlNodeTileState (ControlNode.TileState.Floor, x - width + maxWidth / 2, z - length + maxLength / 2);
					controlNodes [x - width + maxWidth / 2, z - length + maxLength / 2].occupied = true;
					break;
				case 0:
					SetControlNodeTileState (ControlNode.TileState.Floor, x - length + maxLength / 2, z - width + maxWidth / 2);
					controlNodes [x - length + maxLength / 2, z - width + maxWidth / 2].occupied = true;
					break;
				}

				if(width==maxWidth/2&&length==maxLength-3){
					switch(direction){
					case 1:
						returnedNode = controlNodes [x + width - maxWidth / 2, z + length - maxLength / 2-1];
						returnedNode.GetComponent<Renderer> ().material = matDeko;
						returnedNode.lastUsedDirection = direction;
						break;
					case 2:
						returnedNode = controlNodes [x - length + maxLength / 2+1, z - width + maxWidth / 2];
						returnedNode.GetComponent<Renderer> ().material = matDeko;
						returnedNode.lastUsedDirection = direction;
						break;
					case 3:
						returnedNode = controlNodes [x - width + maxWidth / 2, z - length + maxLength / 2+1];
						returnedNode.GetComponent<Renderer> ().material = matDeko;
						returnedNode.lastUsedDirection = direction;
						break;
					case 0:
						returnedNode = controlNodes [x + length - maxLength / 2-1, z - width + maxWidth / 2];
						returnedNode.GetComponent<Renderer> ().material = matDeko;
						returnedNode.lastUsedDirection = direction;
						break;

					}
				}
			}
		}
		for(int length = 0;length<=maxLength;length+=maxLength-1){
			for (int width = 0; width < maxWidth; width++) {
					switch(direction){
					case 1:
					if(!controlNodes[x+width-maxWidth/2, z+length-maxLength/2].occupied)
						SetControlNodeTileState (ControlNode.TileState.Wall, x+width-maxWidth/2, z+length-maxLength/2);
						break;
					case 2:
					if(!controlNodes[x+length-maxLength/2, z+width-maxWidth/2].occupied)
						SetControlNodeTileState (ControlNode.TileState.Wall, x+length-maxLength/2, z+width-maxWidth/2);
						break;
					case 3:
					if(!controlNodes[ x-width+maxWidth/2, z-length+maxLength/2].occupied)
						SetControlNodeTileState (ControlNode.TileState.Wall,  x-width+maxWidth/2, z-length+maxLength/2);
						break;
				case 0:
//					print ((x - width + maxWidth / 2) + "," + (z - length + maxLength / 2));
					if(!controlNodes[ x-length+maxLength/2, z-width+maxWidth/2].occupied)
						SetControlNodeTileState (ControlNode.TileState.Wall,  x-length+maxLength/2, z-width+maxWidth/2);
						break;
				}
			}
		}

		for(int width = 0;width<=maxWidth;width+=maxWidth-1){
			for (int length = 0; length < maxLength; length++) {
				switch(direction){
				case 1:
					if(!controlNodes[x+width-maxWidth/2, z+length-maxLength/2].occupied)
						
					SetControlNodeTileState (ControlNode.TileState.Wall, x+width-maxWidth/2, z+length-maxLength/2);
					break;
				case 2:
					if(!controlNodes[x+length-maxLength/2, z+width-maxWidth/2].occupied)
						
					SetControlNodeTileState (ControlNode.TileState.Wall, x+length-maxLength/2, z+width-maxWidth/2);
					break;
				case 3:
					if(!controlNodes[ x-width+maxWidth/2, z-length+maxLength/2].occupied)
						
					SetControlNodeTileState (ControlNode.TileState.Wall,  x-width+maxWidth/2, z-length+maxLength/2);
					break;
				case 0:
					if(!controlNodes[ x-length+maxLength/2, z-width+maxWidth/2].occupied)
						
					SetControlNodeTileState (ControlNode.TileState.Wall,  x-length+maxLength/2, z-width+maxWidth/2);
					break;
				}
			}
		}
		for(int length = 0;length<=maxLength;length+=maxLength-1){
			for (int width = 0; width < maxWidth; width++) {
				switch(direction){
				case 1:
					if(!controlNodes[x+width-maxWidth/2, z+length-maxLength/2].occupied)
						SetControlNodeTileState (ControlNode.TileState.Wall, x+width-maxWidth/2, z+length-maxLength/2);
					break;
				case 2:
					if(!controlNodes[x+length-maxLength/2, z+width-maxWidth/2].occupied)
						SetControlNodeTileState (ControlNode.TileState.Wall, x - length + maxLength / 2+1, z - width + maxWidth / 2);
					break;
				case 3:
					if(!controlNodes[ x-width+maxWidth/2, z-length+maxLength/2].occupied)
						SetControlNodeTileState (ControlNode.TileState.Wall,  x - width + maxWidth / 2, z - length + maxLength / 2+1);
					break;
				case 0:
					//					print ((x - width + maxWidth / 2) + "," + (z - length + maxLength / 2));
					if(!controlNodes[ x-length+maxLength/2, z-width+maxWidth/2].occupied)
						SetControlNodeTileState (ControlNode.TileState.Wall,  x + length - maxLength / 2-1, z - width + maxWidth / 2);
					break;
				}
			}
		} //---------------------Create Hallway Stepped Ornament--------------------------------------
		for (int length = maxWidth; length < maxLength-maxWidth; length+=3) {
			for (int width = 1; width < maxWidth-1; width+=maxWidth-3) {
				switch(direction){
				case 1:
					SetControlNodeTileState (ControlNode.TileState.Wall, x + width - maxWidth / 2, z + length - maxLength / 2);
					break;
				case 2:
					SetControlNodeTileState (ControlNode.TileState.Wall, x - length + maxLength / 2 + 1, z - width + maxWidth / 2);
					break;
				case 3:
					SetControlNodeTileState (ControlNode.TileState.Wall,  x - width + maxWidth / 2, z - length + maxLength / 2+1);
					break;
				case 0:
					SetControlNodeTileState (ControlNode.TileState.Wall, x + length - maxLength / 2 - 1, z - width + maxWidth / 2);
					break;
				}
			}
		}
		for (int length = 1; length < maxLength-1; length+=maxLength-3) {
			for (int width = maxLength; width < maxWidth-maxLength; width+=3) {
				switch(direction){
				case 1:
					SetControlNodeTileState (ControlNode.TileState.Wall, x + width - maxWidth / 2, z + length - maxLength / 2);
					break;
				case 2:
					SetControlNodeTileState (ControlNode.TileState.Wall, x - length + maxLength / 2, z - width + maxWidth / 2);
					break;
				case 3:
					SetControlNodeTileState (ControlNode.TileState.Wall,  x - width + maxWidth / 2, z - length + maxLength / 2);
					break;
				case 0:
					SetControlNodeTileState (ControlNode.TileState.Wall, x + length - maxLength / 2, z - width + maxWidth / 2);
					break;
				}
			}
		}
		return returnedNode;
	}

	void CreateRoom(ControlNode startNode,int sizeX,int sizeZ){
		int x = startNode.positionX;
		int z = startNode.positionZ;
		CreateRoom (x, z, sizeX, sizeZ);
	}

	void CreateRoom(int positionX, int positionZ,int sizeX, int sizeZ){
		int startPositionX = positionX - sizeX / 2;
		int startPositionZ = positionZ - sizeZ / 2;
		if (CheckPointValidity(startPositionX,startPositionZ)&&CheckPointValidity(startPositionX+sizeX,startPositionZ+sizeZ)){
			for (int x = 0; x < sizeX; x++) {
				for (int z = 0; z < sizeZ; z++) {
					if(x==0&&z==0||x==sizeX-1&&z==0||x==0&&z==sizeZ-1||x==sizeX-1&&z==sizeZ-1){if(controlNodes[startPositionX + x, startPositionZ + z].state !=ControlNode.TileState.Floor)
						SetControlNodeTileState (ControlNode.TileState.Wall, startPositionX + x, startPositionZ + z);
					}else{
						SetControlNodeTileState (ControlNode.TileState.Floor, startPositionX + x, startPositionZ + z);
					}

				}
			}
			for(int z=-1;z<=sizeZ+1;z+=sizeZ+1){
				for(int x=0;x<sizeX;x++){if(controlNodes[startPositionX + x, startPositionZ + z].state !=ControlNode.TileState.Floor)SetControlNodeTileState (ControlNode.TileState.Wall, startPositionX + x, startPositionZ+z);
				}
			}
			for (int x = -1; x < sizeX+1; x+=sizeX+1) {
				for (int z = 0; z < sizeZ; z++) {if(controlNodes[startPositionX + x, startPositionZ + z].state !=ControlNode.TileState.Floor)SetControlNodeTileState (ControlNode.TileState.Wall, startPositionX + x, startPositionZ + z);

				}
			}
		}
	}

	void SetControlNodeTileState (ControlNode.TileState state, int x, int z)
	{
		if (CheckPointValidity(x,z)) {

			if(state==ControlNode.TileState.Floor){
				controlNodes [x, z].state = state;
				controlNodes [x, z].GetComponent<Renderer> ().material= matFloor;
				controlNodes [x, z].occupied = true;
			}else if(state==ControlNode.TileState.Wall){
				controlNodes [x, z].state = state;
				controlNodes [x, z].GetComponent<Renderer> ().material= matWall;
				controlNodes [x, z].occupied = true;
			}else if(state==ControlNode.TileState.DekoTiles){
				controlNodes [x, z].state = state;
				controlNodes [x, z].GetComponent<Renderer> ().material = matDeko;
				controlNodes [x, z].occupied = true;
			}
		}
	}


	//----------------------------OLD Functions (deprecated)------------------------------
	Vector2 SetOneWalkwayX(ControlNode startNode,int halfLengthOfWay){
		Vector2 returnCoordinates;
		int x = startNode.positionX;
		int z = startNode.positionZ;
		returnCoordinates= SetOneWalkwayX (x, z, halfLengthOfWay);
		return returnCoordinates;
	}

	Vector2 SetOneWalkwayX (int x, int z, int halfLengthOfWay)
	{//Checks every 2nd! point! starts with positive. if (success) creates a path in that way, if false, it tries the other direction. returns startPointfor next path(DekoTile)
		Vector2 returnCoordinates = new Vector2 ();
		int increment = 1;
		int xWithOffset = x - 2;
		int lengthOfWay = halfLengthOfWay * 2;
		int widthMax = 5;

		for (int width = 0; width < widthMax; width++) {
			for (int length = 5; length < lengthOfWay; length += increment * 2) { 
				if (CheckPointOccupied (xWithOffset + width, z + length)) {
				} else {
					increment = -1;
					lengthOfWay = -lengthOfWay;
					for (int width2 = 0; width2 < widthMax; width2++) {
						for (int length2 = -5; length2 > lengthOfWay; length2 += increment * 2) {
							if (CheckPointOccupied (xWithOffset + width2, z + length2)) {
							} else {
								print ("broke");
								return returnCoordinates;
							}
						}
					}
					for (int width2 = 0; width2 < widthMax; width2++) {
						for (int length2 = 0; length2 > lengthOfWay; length2 += increment) {
							if (width2 == 0 || length2 == 0 || length2 == lengthOfWay + 1 || width2 == widthMax - 1) {
								if (controlNodes [xWithOffset + width2, z + length2].state != ControlNode.TileState.Floor) {
									SetControlNodeTileState (ControlNode.TileState.Wall, xWithOffset + width2, z + length2);
								}
							} else if (width2 == 2 && length2 == lengthOfWay + 3) {
								SetControlNodeTileState (ControlNode.TileState.DekoTiles, xWithOffset + width2, z + length2);
								returnCoordinates = new Vector2 (controlNodes [xWithOffset + width2, z + length2].positionX, controlNodes [xWithOffset + width2, z + length2].positionZ);
							} else {
								SetControlNodeTileState (ControlNode.TileState.Floor, xWithOffset + width2, z + length2);
							}
						}
					}
				}
			}
		}
		for (int width = 0; width < widthMax; width++) {
			for (int length = 0; length < lengthOfWay; length += increment) {
				if (width == 0 || length == 0 || length == lengthOfWay - 1 || width == widthMax - 1) {
					if (controlNodes [xWithOffset + width, z + length].state != ControlNode.TileState.Floor) {
						SetControlNodeTileState (ControlNode.TileState.Wall, xWithOffset + width, z + length);
					}
				} else if (width == 2 && length == lengthOfWay - 3) {
					SetControlNodeTileState (ControlNode.TileState.DekoTiles, xWithOffset + width, z + length);
					returnCoordinates = new Vector2 (controlNodes [xWithOffset + width, z + length].positionX, controlNodes [xWithOffset + width, z + length].positionZ);
				} else {
					SetControlNodeTileState (ControlNode.TileState.Floor, xWithOffset + width, z + length);

				}
			}
		}
		return returnCoordinates;
	}

	Vector2 SetOneWalkwayZ(ControlNode startNode,int halfLengthOfWay){
		Vector2 returnCoordinates;
		int x = startNode.positionX;
		int z = startNode.positionZ;
		returnCoordinates= SetOneWalkwayZ (x, z, halfLengthOfWay);
		return returnCoordinates;
	}

	Vector2 SetOneWalkwayZ (int x, int z, int halfLengthOfWay)
	{
		Vector2 returnCoordinates = new Vector2 ();
//		ControlNode returnNode = controlNodes [returnCoordinates.x, returnCoordinates.y];     Implement this further to return controlNode instead of Vector2
		int increment = 1;
		int zWithOffset = z - 2;
		int lengthOfWay = halfLengthOfWay * 2;
		int widthMax = 5;

		for (int length = 5; length < lengthOfWay; length += increment * 2) {
			for (int width = 0; width < widthMax; width++) {
				if (CheckPointOccupied (x + length, zWithOffset + width)) {
				} else {
					increment = -1;
					lengthOfWay = -lengthOfWay;
					for (int length2 = -5; length2 > lengthOfWay; length2 += increment * 2) {
						for (int width2 = 0; width2 < widthMax; width2++) {
//							Transform testCube = Instantiate<Transform> (testTile);
//							Vector3 position = new Vector3 (x + length2, 0, zWithOffset + width2);
//							testCube.position = position;
//							testCube.transform.parent = GameObject.Find("Control Nodes").transform;
//							print ("beforeBBBbrokeZ : length2:" + length2 + "  , width2 : " + width2);
//							print ("       x: " + x + "  zWithOffset : " + zWithOffset);
//							print (controlNodes [x + length2, zWithOffset + width2].occupied);
//							print (controlNodes [x + length2, zWithOffset + width2].state);
							if (CheckPointOccupied (x + length2, zWithOffset + width2)) {
							} else {
//								print (controlNodes [x + length2, zWithOffset + width2].occupied);
//								print (controlNodes [x + length2, zWithOffset + width2].state);
								print ("brokeZ : length2:" + length2 + "  , width2 : " + width2);
								print ("       x: " + x + "  zWithOffset : " + zWithOffset);
								return returnCoordinates;
							}
						}
					}
					for (int length2 = 0; length2 > lengthOfWay; length2 += increment) {
						for (int width2 = 0; width2 < widthMax; width2++) {
							if (width2 == 0 || length2 == 0 || length2 == lengthOfWay + 1 || width2 == widthMax - 1) {
								if (controlNodes [x + length2, zWithOffset + width2].state != ControlNode.TileState.Floor) {
									SetControlNodeTileState (ControlNode.TileState.Wall, x + length2, zWithOffset + width2);
								}
							} else if (width2 == 2 && length2 == lengthOfWay + 3) {
								SetControlNodeTileState (ControlNode.TileState.DekoTiles, x + length2, zWithOffset + width2);
								returnCoordinates = new Vector2 (controlNodes [x + length2, zWithOffset + width2].positionX, controlNodes [x + length2, zWithOffset + width2].positionZ);
							} else {
								SetControlNodeTileState (ControlNode.TileState.Floor, x + length2, zWithOffset + width2);
							}
						}
					}
				}
			}
		}
		for (int length = 0; length < lengthOfWay; length++) {
			for (int width = 0; width < widthMax; width++) {
				if (width == 0 || length == 0 || length == lengthOfWay - 1 || width == widthMax - 1) {
					if (controlNodes [x + length, zWithOffset + width].state != ControlNode.TileState.Floor) {
						SetControlNodeTileState (ControlNode.TileState.Wall, x + length, zWithOffset + width);
					}
				} else if (width == 2 && length == lengthOfWay - 3) {
					SetControlNodeTileState (ControlNode.TileState.DekoTiles, x + length, zWithOffset + width);
					returnCoordinates = new Vector2 (controlNodes [x + length, zWithOffset + width].positionX, controlNodes [x + length, zWithOffset + width].positionZ);
				} else {
					SetControlNodeTileState (ControlNode.TileState.Floor, x + length, zWithOffset + width);
				}
			}
		}
		return returnCoordinates;
	}

	Vector2 FindStartPoint(){ //finds a cell coordinate near the border of the map //Deprecated
		var startpoint=new Vector2();
		int tester = pRNG.Next (0, 4);
		if (tester == 0) {
			startpoint.x = 5;
			startpoint.y = pRNG.Next (5, worldSize - 6);
		}if(tester==1){
			startpoint.x = worldSize - 6;
			startpoint.y = pRNG.Next (5, worldSize - 6);
		}if(tester==2){
			startpoint.x=pRNG.Next (5, worldSize - 6);
			startpoint.y = 5;
		}if(tester==3){
			startpoint.x=pRNG.Next (5, worldSize - 6);
			startpoint.y = worldSize - 6;
		}
		return startpoint;
	}


}
