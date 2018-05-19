﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerSpawner : MonoBehaviour {

    private Transform playerNode;
    private Organizer o;
    private GameUnit player;
    private Transform playerBody;
    private Transform playerShield;
    private Transform focusTransform;
    private FocusMovement cameraFocus;
    private Camera cam;



	// Use this for initialization
	void Start () {
        o = Organizer.instance;
        player = Organizer.PLAYER_STANDARD_SETUP.Clone();

        //GameObject
        playerNode = new GameObject(Organizer.NAME_PLAYER_GAMEOBJECT).transform;
        playerNode.position = this.transform.position;

        //Player Placeholder
        GameObject play = GameObject.CreatePrimitive(PrimitiveType.Cube);
        play.name = Organizer.NAME_PLAYER_GAMEOBJECT+Organizer.NAME_BODY;
        playerBody = play.transform;
        playerBody.parent = playerNode;
        playerBody.position = this.transform.position;
        playerBody.localScale += new Vector3(1, 1, 1);
        play.AddComponent<PlayerMovement>();
        play.AddComponent<CharacterController>();
        ColliderOwner coPlay = play.AddComponent<ColliderOwner>();

        //Focus
        GameObject focus = GameObject.CreatePrimitive(PrimitiveType.Cube);
        focus.GetComponent<BoxCollider>().enabled = false;
        focus.name = Organizer.NAME_MAIN_FOCUS;
        focus.transform.position = this.transform.position;
        focus.transform.localScale += new Vector3(10, 10, -0.9f);
        focusTransform = focus.transform;
        cameraFocus = focus.AddComponent<FocusMovement>();
        cameraFocus.playerBody = this.playerBody;
        focusTransform.parent = playerNode;

        //Camera
        cam = GameObject.Find(Organizer.NAME_MAIN_CAMERA).GetComponent<Camera>();
        cam.gameObject.transform.position = new Vector3(
                    this.transform.position.x + Organizer.CAMERA_DISTANCE.x, 
                    this.transform.position.y + Organizer.CAMERA_DISTANCE.y, 
                    this.transform.position.z + Organizer.CAMERA_DISTANCE.z);
        cam.GetComponent<CameraMovement>().target = focus.transform;
        
 
        //Shield
        playerShield = Instantiate(o.P_FORCE_SHIELD, playerBody);
        playerShield.name = Organizer.NAME_PLAYER_GAMEOBJECT + Organizer.NAME_SHIELD;
        playerShield.localScale += new Vector3(1, 1, 1);
        playerShield.parent = playerBody;
        ColliderOwner coShield = playerShield.gameObject.AddComponent<ColliderOwner>();
        coShield.owner = player;
        
    }
}
