using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

public class EnemyAI : MonoBehaviour {

    public static System.Collections.Generic.Dictionary<EnemyAI, Vector3> orders = new System.Collections.Generic.Dictionary<EnemyAI, Vector3>();

    private NavMeshAgent meshAgent;
    private Camera mainCam;
    public ThirdPersonCharacter character;
    public Transform player;

    bool rifling;
    bool shooting;

    PrefabOrganizor po;
    Transform rifle;

	// Use this for initialization
	void Start () {
        po = PrefabOrganizor.instance;
        character = GetComponent<ThirdPersonCharacter>();
        meshAgent = GetComponent<NavMeshAgent>();
        mainCam = GameObject.Find("MainCamera").GetComponent<Camera>();
        meshAgent.updateRotation = false;

        character.Equip(po.P_SFI_RIFLES[0]
            ,new Vector3(0.0988f,0,0.03953f)
            ,new Vector3(33.027f,-96.250f,-94.309f)
            ,new Vector3(-0.2f,-0.2f,-0.4f));

        player = GameObject.Find("Player").transform;
        character.LookAt(player);

	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButton("AIRifleUp"))
        {
            if (!rifling)
            {
                rifling = true;
                character.rifling = rifling;
                Debug.Log("rifling: " + rifling);
                //character.Equip();
            }
        }
        if (Input.GetButton("AIRifleDown"))
        {
            if (rifling)
            {
                rifling = false;
                shooting = false;
                character.shooting = shooting;
                character.rifling = rifling;
                //Debug.Log("rifling: " + rifling);
            }
        }
        if (Input.GetButton("AIRifleShoot"))
        {
            if (!shooting)
            {
                shooting = true;
                character.shooting = shooting;
                //Debug.Log("Shooting: " + shooting);
            }
        }
        if (Input.GetButton("AIRifleStopShoot"))
        {
            if (shooting)
            {
                shooting = false;
                character.shooting = shooting;
                //Debug.Log("Shooting: " + shooting);
            }
        }
        if (meshAgent != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit))
                {
                    float x = hit.point.x;
                    float y = hit.point.y;
                    float z = hit.point.z;

                    orders.Remove(this);

                    foreach (Vector3 order in orders.Values)
                    {
                        if(y == order.y && z == order.z && order.x-0.8 < x && order.x+0.8 > x){
                            x += 0.8f;
                        }
                    }
                    Vector3 destination = new Vector3(x, y, z);
                    meshAgent.SetDestination(destination);
                    orders.Add(this, destination);

                }
            }
            if (character != null)
            {
                if (meshAgent.remainingDistance > meshAgent.stoppingDistance)
                {
                    character.Move(meshAgent.desiredVelocity,false,false);
                }
                else
                {
                    character.Move(Vector3.zero, false, false);
                }
            }
        }

	}
}
