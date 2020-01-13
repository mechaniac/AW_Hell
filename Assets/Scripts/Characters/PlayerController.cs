using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	float startSpeed = 3.0f;
	float speed;
	float maxSpeed = 5.0f;
	float stepSpeed = 1.0f;
	float jumpSpeed = 5.0f;
	float gravity = 20.0f;
	float rotateSpeed = 3.0f;
	private Vector3 moveDirection = Vector3.zero;
	private Vector3 mainCursorDirection = Vector3.zero;
	private Vector3 cursorDirection = Vector3.zero;
	public Transform cursorTransform;
	public Transform mainCursorTransform;
	public Transform forwardTiltTransform;
	public Transform characterRotate;

	public Transform target;
	Transform chest;

	float runTimer;
	float currentVelocity2DFloat;
	float velocity2DAbs;
	float runHeight;

	public Transform characterTilt;
	Quaternion maxForwardTilt;
	Quaternion maxSideTilt;

	float velocityAbs;
	float directionAngleOffset;

	Quaternion mainDirection;

	bool leftJoystickUsed = false;
	bool rightJoystickUsed = false;

	public Animator animator;
	CharacterController controller;

	void Start () {
		controller = GetComponent<CharacterController> ();
		mainDirection = Quaternion.identity;
		chest = animator.GetBoneTransform (HumanBodyBones.Spine);
	}
	
	void Update () {
		GetInput ();

		if(controller.isGrounded){
			moveDirection = mainCursorDirection;
			moveDirection *= speed;
			if (Input.GetButtonDown ("B")) {
				moveDirection.y = jumpSpeed;
				animator.CrossFade ("jump_01", .1f);
			}
		}
//		print (velocityAbs);

		moveDirection.y -= gravity * Time.deltaTime;

		controller.Move (moveDirection * Time.deltaTime);

		velocityAbs = controller.velocity.magnitude;
		currentVelocity2DFloat = (controller.velocity.x + controller.velocity.z) / 2;
		velocity2DAbs = Mathf.Abs (currentVelocity2DFloat);


		cursorTransform.transform.localPosition = transform.InverseTransformDirection (cursorDirection.normalized);
		mainCursorTransform.transform.localPosition = transform.InverseTransformDirection (mainCursorDirection.normalized);

//		animator.Play ("idle");
		if(Input.GetButtonDown("A")){
			animator.CrossFadeInFixedTime ("attack_01",.1f);
			if(animator.GetCurrentAnimatorStateInfo(0).IsName("attack_01")) {
				animator.CrossFadeInFixedTime ("idle_01", .5f);
			}
//			animator.Play ("run_lft");
		}
//		if 
		if(rightJoystickUsed){
			RotateToPoint (cursorDirection,8);
		}else if(leftJoystickUsed){
			RotateToPoint (mainCursorDirection,4);
		}
		if(leftJoystickUsed||rightJoystickUsed&& velocityAbs>0){
			runTimer += Time.deltaTime * ( velocityAbs);
//			print ("vel : " + velocityAbs);
//			print ("sin : " + runTimer);
		} else if(velocityAbs == 0){
			runTimer = 0;
		}
		if(rightJoystickUsed){
			speed = Mathf.Lerp(speed,stepSpeed,Time.deltaTime*5);
		}else{speed = Mathf.Lerp(speed, maxSpeed, Time.deltaTime);}
		if(!rightJoystickUsed&&!leftJoystickUsed){
			speed = startSpeed;
		}
//		print (Mathf.Sin (Time.time));
		float stepByTime = Mathf.Sin (runTimer*6);
//		print (stepByTime);
		if(velocityAbs>0&&controller.isGrounded){
//			animator.Play ("run_rght");
			if (stepByTime > 0){
				animator.CrossFade ("run_lft",5f);

			} else{
				animator.CrossFade ("run_rght",5f);

			}

		}

		runHeight = stepByTime;
//		print (runHeight);
		if (!float.IsNaN(runHeight)) {
//			print ("runheight   " + runHeight);
			RunUpDownMotion (runHeight);
			}
		directionAngleOffset = Vector3.Angle(transform.localRotation * Vector3.forward,mainCursorDirection);  // calcualtes the angle between two vectors
//		print ("angle = " + directionAngleOffset);
//		print ("cursor = " + mainCursorDirection);
//		if(velocityAbs > 3){
////			print ("here");
//			LeanForward (15,3);
//		}
//		if(velocityAbs< 3){
////			print ("herertt");
//			LeanForward (0,12);
//		}
		characterTilt.rotation = LeanToPoint (mainCursorDirection);

	}

	void LateUpdate(){
		if(Input.GetKeyDown(KeyCode.X)){
			//			print ("yes");
//			print(animator.bodyPosition);

			animator.bodyPosition = animator.bodyPosition + new Vector3 (0, 11, 0);
			Vector3 leftHandposition = animator.bodyPosition + new Vector3 (1, 0, 3);
			animator.SetIKPosition (AvatarIKGoal.LeftHand, leftHandposition);
//			print(animator.bodyPosition);

		}
//		chest.position += new Vector3(0,1,0);



	}

	void GetInput(){
		cursorDirection = new Vector3 (Input.GetAxisRaw ("HorizontalCursor"), 0, Input.GetAxisRaw ("VerticalCursor"));
		if (cursorDirection.x != 0 || cursorDirection.z != 0){
			rightJoystickUsed = true;
		} else{rightJoystickUsed = false;}
		mainCursorDirection = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical"));
		if (mainCursorDirection.x != 0 || mainCursorDirection.z != 0){
			leftJoystickUsed = true;
		} else{leftJoystickUsed = false;}
	}


	void RotateToPoint(Vector3 inputDirection, float turnSpeed){
		if(inputDirection!= Vector3.zero){
			mainDirection = Quaternion.LookRotation (inputDirection);
		}
		characterRotate.transform.localRotation = Quaternion.Lerp(characterRotate.transform.localRotation,mainDirection,turnSpeed * Time.deltaTime);
	}


	void LeanForward(float angle, float leanSpeed){
		float setAngle = (angle / speed)*velocityAbs*4;
		if (setAngle < 0)
			setAngle = 0;
		maxForwardTilt.eulerAngles = new Vector3 (setAngle, 0, 0);
		forwardTiltTransform.transform.localRotation = Quaternion.Lerp (forwardTiltTransform.transform.localRotation, maxForwardTilt, leanSpeed * Time.deltaTime);

		forwardTiltTransform.transform.localPosition = Vector3.Lerp (forwardTiltTransform.transform.localPosition, new Vector3 (0, 0, -setAngle / 50),leanSpeed* Time.deltaTime);
	}


	void TiltSideWays(float tiltSpeed){
		characterTilt.transform.localRotation = Quaternion.Lerp (characterTilt.transform.localRotation, maxSideTilt, tiltSpeed * Time.deltaTime);
	}


	Quaternion LeanToPoint(Vector3 point){
		Vector3 lookAtPoint =  new Vector3 (point.z, 0, -point.x);
		Quaternion returnRotation = Quaternion.Euler (lookAtPoint*velocityAbs);
		Quaternion finalRotation = returnRotation * returnRotation;
//		returnRotation.eulerAngles = new Vector3 (0, 0, 0);
		return finalRotation;
	}

	void RunUpDownMotion(float heightOffset){
//		print ("heightoff   "+ heightOffset);
		heightOffset = Mathf.Abs ((heightOffset/10 ));
		Vector3 upDownMotion = new Vector3 (0, heightOffset, 0);
		characterTilt.transform.localPosition = upDownMotion;
	}
}
