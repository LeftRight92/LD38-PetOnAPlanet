using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	[SerializeField]
	private float dragSpeed = 1;
	[SerializeField]
	private float keyboardMultiplier;
	[SerializeField]
	private float slow = 0.5f;

	private const int CAMERA_MOUSE_BUTTON = 2;

	// Use this for initialization
	void Start () {
		
	}

	Vector3 lastPosition;
	float speed;
	// Update is called once per frame
	void Update () {
		float rotationAmount = 0;
		if(Input.GetKey(KeyCode.LeftArrow)) {
			Debug.Log("Hello");
			rotationAmount = dragSpeed * keyboardMultiplier;
		} else if(Input.GetKey(KeyCode.RightArrow)) {
			rotationAmount = -dragSpeed * keyboardMultiplier;
		} else {

			if(Input.GetMouseButtonDown(CAMERA_MOUSE_BUTTON)) {
				lastPosition = Input.mousePosition;
				return;
			}

			Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - lastPosition);
			Vector3 move = new Vector3(pos.x * dragSpeed, 0, pos.y * dragSpeed);

			if(!Input.GetMouseButton(CAMERA_MOUSE_BUTTON)) {
				if(Input.GetMouseButtonUp(CAMERA_MOUSE_BUTTON)) {
					speed = Mathf.Sign(move.x) * move.magnitude;
				}
				//speed = Mathf.Max(0, speed - (slow * Time.deltaTime));
				speed = Mathf.Sign(speed) * Mathf.Max(0, (Mathf.Abs(speed) - (slow * Time.deltaTime)));
				rotationAmount = speed;
				//return;
			} else if(rotationAmount == 0) {
				rotationAmount = Mathf.Sign(move.x) * move.magnitude;
			}
		}
		//transform.Translate(move, Space.World);
		transform.Rotate(new Vector3(0f, 0f, rotationAmount));

		lastPosition = Input.mousePosition;
	}
}
