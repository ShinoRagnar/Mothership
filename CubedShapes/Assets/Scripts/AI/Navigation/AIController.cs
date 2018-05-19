using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour {

    public static System.Collections.Generic.Dictionary<AIController, Vector3> orders = new System.Collections.Generic.Dictionary<AIController, Vector3>();

    private NavMeshAgent meshAgent;
    private Camera mainCam;
    private Character character;
    private Animator anim;
    private Transform player;
    private ItemEquiper itemEquiper;

    bool rifling;
    bool shooting;

    private Organizer o;

    // Use this for initialization
    private void Awake()
    {
        character = GetComponent<Character>();
        meshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        itemEquiper = GetComponent<ItemEquiper>();
    }

    void Start () {
        o = Organizer.instance;

        mainCam = GameObject.Find("MainCamera").GetComponent<Camera>();
        meshAgent.updateRotation = false;

        Gun rifle = o.GUN_STANDARD_RIFLE.Clone();
        itemEquiper.EquipItem(rifle);
        rifle.Show(anim.GetBoneTransform(HumanBodyBones.RightHand));


        //player = GameObject.Find("Player").transform;
        //character.LookAt(player);

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
