using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerOld : MonoBehaviour
{

	CharacterController controller;
	public Transform aimCursor;
	public Transform cursorPoint;
	public Transform mainDirectionCursor;
	public Transform characterRotate;
	public Transform characterTilt;
	public Transform characterTilt2;
	public Transform forwardTiltTransform;
	public float startSpeed = 1;
	public float maxSpeed = 2;
	public float minSpeed = 1;
	float speed = 2;
	float offset;
	float deltaTime = 0;
	float deltaOffset = 1;
	float forwardAngle = 0;
	Vector3 cursorPosition;
	Vector3[] lastCursorPositions;
	bool inputFrom1stJoyStick = false;
	bool inputFrom2ndJoystick = false;
	bool JoystickMode = true;
	Vector3 mainPosition;
	Vector3[] lastMainPositions;
	bool characterInertiaCoroutineIsRunning = false;
	bool isJumping = false;
	MeshRenderer aimCursorMeshRender;
	MeshRenderer mainCursorMeshRenderer;

	public float gravity = 462;

	Rigidbody rBody;

	void Start ()
	{
		FirstControlllerAttemptStart ();
	}


	void Update (){
		FirstControllerAttempt ();
	{



	}
	float cursorCounter;
//	void CursorSwitch (float inputX, float inputZ){
//
//		float inputXAbs = Mathf.Abs (inputX);
//		float inputZAbs = Mathf.Abs (inputZ);
//		float timeOffset = 0.8f;
//		if (cursorCounter == null)
//			cursorCounter = 0;
//		if(inputX==0&&inputZ==0){
//			cursorCounter = 0;
//		}
//		if(JoystickMode== false){
//			if ((inputXAbs + inputZAbs) > 1.2) {
//				cursorCounter += 4*Time.deltaTime;
////				print ("false " + cursorCounter);
//			}
//			if(inputXAbs==1||inputZAbs ==1){
//				cursorCounter += 4*Time.deltaTime;
//			}
//		}
//
//		if(cursorCounter >=timeOffset){
//			JoystickMode = true;
//			cursorCounter = 0;
//		}
////		print (JoystickMode);
//		if(JoystickMode==true){
//			if ((inputXAbs + inputZAbs) < 0.8) {
//				print ((inputXAbs + inputZAbs));
////				print ("axis = "+ Input.GetAxisRaw ("VerticalCursor"));
//				if(inputX!=0||inputZ!=0){
//					cursorCounter -= Time.deltaTime;
//				}
////				print (Input.GetAxisRaw("HorizontalCursor"));
//
//
//			}
//			if(cursorCounter<=-timeOffset){
//				JoystickMode = false;
//				cursorCounter = 0;
////				print ("setFalse");
//			}
//		}
//
//
//	}

//	IEnumerator CharacterInertiaMovement(float currentVelocity){ // ----- works, but not using it
//		StopAllCoroutines ();
//		characterInertiaCoroutineIsRunning = true;
//		print ("On");
////		Vector3 direction = lastCursorPosition + lastMainPosition;
//		for(float f = 1; f> 0;f-=.01f){
//			controller.SimpleMove (direction*f);
//			yield return new WaitForSeconds (.1f);
//		}
//		characterInertiaCoroutineIsRunning = false;
//
//	}

//	IEnumerator MoveCharacter(){
//		Vector3 direction = new Vector3 (0, 1, 0);
//		for(float f = 1;f>0;f-=Time.deltaTime){
//			print ("core");
////			controller.
//			controller.Move (direction );
//			yield return new WaitForSeconds (Time.deltaTime);
//		}
//	}
//	IEnumerator JumpCharacter(){
//		Vector3 direction = new Vector3 (0, 1, 0);
//		for(float f = .1f;f>0;f-=Time.deltaTime){
//			print ("core");
//
//			controller.Move (direction );
//			yield return new WaitForSeconds (Time.deltaTime);
//		}
	}


	void FirstControlllerAttemptStart(){
		cursorPosition = new Vector3 (0, 0, 1);
		controller = GetComponent<CharacterController> ();
		mainPosition = new Vector3 (0, 0,1);
		mainDirectionCursor.transform.localPosition = mainPosition.normalized * 2;

		lastCursorPositions = new Vector3[10];
		lastMainPositions = new Vector3[10];
		aimCursorMeshRender = aimCursor.GetComponent<MeshRenderer> ();
		mainCursorMeshRenderer = mainDirectionCursor.GetComponent<MeshRenderer> ();
		rBody = GetComponent<Rigidbody> ();
	}

	void FirstControllerAttempt (){
		//------------------- Getting inputs ---------------------------

		//----------1st AXIS -----------
		float inputX = Input.GetAxis ("Horizontal");
		float inputZ = Input.GetAxis ("Vertical");
		if (inputX == 0 && inputZ == 0) {
			inputFrom1stJoyStick = false;
			mainCursorMeshRenderer.enabled = false;
		} else {
			inputFrom1stJoyStick = true;
			mainCursorMeshRenderer.enabled = true;
		}
		mainPosition.x = inputX;
		mainPosition.z = inputZ;

		//------storing inputs------

		Vector3[] lastMainpositionsStorage = new Vector3[lastMainPositions.Length];
		for(int i = 1; i < lastMainPositions.Length;i++){
			lastMainpositionsStorage [i] = lastMainPositions [i - 1];
		}
		lastMainPositions = lastMainpositionsStorage;
		lastMainPositions [0] = mainPosition;

//		if(Input.GetButtonDown("A")){
//			Vector3 jump = new Vector3 (0, 7, 0);
//			mainPosition += jump;
//		}
//		if(mainPosition.y > 0){
//			mainPosition.y -= gravity ;
//
//		}
		controller.Move (mainPosition * Time.deltaTime * speed);

		//----------2nd AXIS -----------

		float inputX2nd = Input.GetAxis ("HorizontalCursor");
		float inputZ2nd = Input.GetAxis ("VerticalCursor");

		if(inputX2nd == 0 && inputZ2nd == 0){
			inputFrom2ndJoystick = false;
			aimCursorMeshRender.enabled = false; 

		}

		if (inputX2nd != 0 || inputZ2nd != 0) {
			aimCursorMeshRender.enabled = true;

			cursorPosition.x = inputX2nd;
			cursorPosition.z = inputZ2nd;
		}
		Vector3 inputMousecontroller = new Vector3 (inputX2nd, 0, inputZ2nd);

		if(JoystickMode==true){
			aimCursor.transform.localPosition = cursorPosition.normalized * 1.5f;
			cursorPoint.transform.localPosition = aimCursor.transform.localPosition;
		}else{
			cursorPoint.transform.localPosition += inputMousecontroller/17;
			aimCursor.transform.localPosition = cursorPoint.transform.localPosition.normalized * 1.5f;
		}


		//----------------------- CHARACTER ORIENTATION ----------------------------------
		Quaternion characterRotation = Quaternion.LookRotation (mainDirectionCursor.transform.localPosition);
		Quaternion aimRotation = Quaternion.LookRotation (aimCursor.transform.localPosition);

		if (inputX != 0 || inputZ != 0) {
			mainDirectionCursor.transform.localPosition = mainPosition.normalized * 2;
			if(inputX2nd==0&&inputZ2nd==0){
				characterRotate.transform.localRotation = Quaternion.Lerp (characterRotate.transform.rotation, characterRotation, 5 * Time.deltaTime);
				offset = characterRotate.transform.localRotation.eulerAngles.y - characterRotation.eulerAngles.y;
				inputFrom2ndJoystick = false;
			}

		}
		float currentVelocity2DFloat = (controller.velocity.x + controller.velocity.z) / 2;
		float velocity2DAbs = Mathf.Abs (currentVelocity2DFloat);


		if(inputX2nd != 0 || inputZ2nd != 0){
			characterRotate.transform.localRotation = Quaternion.Lerp (characterRotate.transform.rotation, aimRotation, 6* Time.deltaTime);
			offset = characterRotate.transform.localRotation.eulerAngles.y - aimRotation.eulerAngles.y;

			inputFrom2ndJoystick=true;
		}
		float direction = 1;
		float offsetAbs = Mathf.Abs (offset);

		if(offset < offsetAbs){
			direction = -1;
		}
		//--------------  TILT  -----------------
		Quaternion maxTilt = Quaternion.identity;
		maxTilt.eulerAngles = new Vector3 (0,0, direction*20);

		if(offsetAbs>2&&!inputFrom2ndJoystick&&velocity2DAbs>1&&offsetAbs<40){
			characterTilt2.transform.localRotation =  Quaternion.Lerp(characterTilt2.transform.localRotation,maxTilt,velocity2DAbs*Time.deltaTime);;
		}else{
			characterTilt2.transform.localRotation =  Quaternion.Lerp(characterTilt2.transform.localRotation,Quaternion.identity,velocity2DAbs* Time.deltaTime);;
		}
		if(velocity2DAbs <= .1f){
			characterTilt2.transform.localRotation = Quaternion.Lerp(characterTilt2.transform.localRotation,Quaternion.identity,10* Time.deltaTime);
		}
		//---------------  FORWARDTILT ---------------------------
		float maxForwardAngle = 10;
		if (velocity2DAbs > .1f&&offsetAbs <20){forwardAngle = Mathf.Lerp (forwardAngle, maxForwardAngle,4* Time.deltaTime);}else{forwardAngle = Mathf.Lerp (forwardAngle, 0,6* Time.deltaTime);}
		Quaternion forwardTilt = Quaternion.identity;
		forwardTilt.eulerAngles = new Vector3 (forwardAngle*velocity2DAbs, 0, 0);

		forwardTiltTransform.localRotation = forwardTilt;

		if(Time.time>3){
			if (!inputFrom1stJoyStick && forwardAngle > 2&&!inputFrom2ndJoystick&&controller.isGrounded) {
				Vector3 appliedDirection = new Vector3 ();
				for (int i = 0; i < lastMainPositions.Length; i++) {
					appliedDirection += lastMainPositions[i]/lastMainPositions.Length;
				}
				print (appliedDirection);
				controller.transform.position += (appliedDirection.normalized * forwardAngle)/50;

			}
		}



		if(!inputFrom2ndJoystick&&speed<maxSpeed){
			speed = maxSpeed;
		}
		if(inputFrom2ndJoystick&&speed>minSpeed){
			speed = minSpeed;

		}
		if(velocity2DAbs == 0){
			speed = startSpeed;
		}

		//		if(currentVelocityFloat != 0&&!inputFrom1stJoyStick&&!inputFrom2ndJoystick){
		////			print ("1yes");
		//			if(!characterInertiaCoroutineIsRunning&&!isJumping){
		//				print("starting Cor")
		//				StartCoroutine ("CharacterInertiaMovement", velocityAbs);
		//
		//			}
		////			CharacterInertiaMovement (velocityAbs);
		////			print (velocityAbs);
		//
		//		}


		//		if (Input.GetButtonDown ("A")&&!isJumping) {
		////			isJumping = true;
		//			Vector3 jump = transform.TransformDirection (Vector3.up) * 126;
		////			controller.Move (jump * Time.deltaTime);
		////			isJumping = false;
		////			controller.attachedRigidbody.velocity = jump;
		////			rBody.velocity = jump;
		////			controller.Mo
		////			StartCoroutine(MoveCharacter());
		//
		//		}
	}
}
