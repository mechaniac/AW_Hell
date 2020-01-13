using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderElement : MonoBehaviour {

	public Vector3 startPosition;
	public Vector3 endPosition;

//	BoxCollider collider;

	public void Initialize(ControlNode startNode, ControlNode diagonalNode, int worldSize){
		startPosition.x = startNode.positionX - worldSize/2;
		startPosition.y = 0;
		startPosition.z = startNode.positionZ -worldSize/2;

		endPosition.x = diagonalNode.positionX - worldSize/2;
		endPosition.y = 0;
		endPosition.z = diagonalNode.positionZ - worldSize/2;

		this.GetComponent<BoxCollider>().center = startPosition-endPosition;
	}
		

}
