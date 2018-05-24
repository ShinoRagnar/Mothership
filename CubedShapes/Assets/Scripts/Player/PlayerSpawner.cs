using System.Collections;
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
        playerNode.parent = this.transform;
        playerNode.position = this.transform.position;
        

        //Player Placeholder
        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
        body.name = Organizer.NAME_PLAYER_GAMEOBJECT+Organizer.NAME_BODY;
        playerBody = body.transform;
        playerBody.parent = playerNode;
        playerBody.position = this.transform.position;
        playerBody.localScale += new Vector3(1, 1, 1);
        body.AddComponent<PlayerMovement>();
        body.AddComponent<CharacterController>();
        body.AddComponent<NavMeshObstacle>();
        ColliderOwner coPlay = body.AddComponent<ColliderOwner>();
        coPlay.owner = player;
        player.body = playerBody;

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
        cam.GetComponent<CameraMovement>().target = focus.transform;

        //Shield
        playerShield = Instantiate(o.P_FORCE_SHIELD, playerBody);
        playerShield.name = Organizer.NAME_PLAYER_GAMEOBJECT + Organizer.NAME_SHIELD;
        playerShield.localScale += new Vector3(1, 1, 1);
        playerShield.parent = playerBody;
        ColliderOwner coShield = playerShield.gameObject.AddComponent<ColliderOwner>();
        coShield.owner = player;

        //Layer
        Organizer.SetLayerOfThisAndChildren(Organizer.LAYER_PLAYER, playerNode.gameObject);
        Organizer.SetLayerOfThisAndChildren(Organizer.LAYER_SHIELDS, playerShield.gameObject);

        //Shows threat levels (For debug purposes)
        if (DevelopmentSettings.SHOW_THREAT_LEVELS) { 
            GameObject threatPreferred = GameObject.CreatePrimitive(PrimitiveType.Cube);
            threatPreferred.GetComponent<BoxCollider>().enabled = false;
            threatPreferred.transform.localScale = new Vector3(AISquad.DISTANCE_PREFERRED * 2, 0.5f, 0.5f);
            threatPreferred.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 3);
            threatPreferred.transform.parent = body.transform;
            Renderer rend = threatPreferred.GetComponent<Renderer>();
            rend.material.shader = Shader.Find("_Color");
            rend.material.SetColor("_Color", Color.green);
            rend.material.shader = Shader.Find("Specular");
            rend.material.SetColor("_SpecColor", Color.green);

            GameObject threatTooClose = GameObject.CreatePrimitive(PrimitiveType.Cube);
            threatTooClose.GetComponent<BoxCollider>().enabled = false;
            threatTooClose.transform.localScale = new Vector3(AISquad.DISTANCE_TOO_CLOSE * 2, 0.5f, 0.5f);
            threatTooClose.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 2.9f);
            threatTooClose.transform.parent = body.transform;
            Renderer rendToo = threatTooClose.GetComponent<Renderer>();
            rendToo.material.shader = Shader.Find("_Color");
            rendToo.material.SetColor("_Color", Color.red);
            rendToo.material.shader = Shader.Find("Specular");
            rendToo.material.SetColor("_SpecColor", Color.red);
        }
    }
}
