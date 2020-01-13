using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlNode : MonoBehaviour {


	public int positionX;
	public int positionZ;
	public Vector2 iD;

	public bool occupied;
	public bool set;
	public enum TileState {Floor, Bridge, Wall, RoomCorner, BridgeAnchor,Empty,DekoTiles,CornerRoomCorners,CornerRoomMiddles};
	public TileState state;
	public int lastUsedDirection;


}
