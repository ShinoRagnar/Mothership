using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    public Transform target;
    public Vector3 offset;

	// Update is called once per frame
	void Update () {   
        transform.position = target.position + Organizer.CAMERA_DISTANCE;
        transform.LookAt(target);
	}
}
