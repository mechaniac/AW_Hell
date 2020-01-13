using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

	public Transform playerPosition;
	public float smoothTime = 0.3f;
	private Vector3 velocity = Vector3.zero;

	void Update(){
		Vector3 position = new Vector3(playerPosition.transform.position.x,0,playerPosition.transform.position.z);
		transform.position = Vector3.SmoothDamp (transform.position, position, ref velocity, smoothTime);
	}
}
