using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AIMode
{
    Scouting,
    Hunting
}
public class AIController : MonoBehaviour {

    public static System.Collections.Generic.Dictionary<AIController, Vector3> moveOrders = new System.Collections.Generic.Dictionary<AIController, Vector3>();



    private NavMeshAgent navAgent;
    private Camera mainCam;
    private Character character;
    private Animator anim;
    private ItemEquiper itemEquiper;
    private GameUnit self;
    private Organizer o;

    //AI
    AIMode currentMode;
    public GameUnit huntingTarget;
    private float reactionCycle = 0;


    // Use this for initialization
    private void Awake()
    {
        character = GetComponent<Character>();
        navAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        itemEquiper = GetComponent<ItemEquiper>();

    }

    void Start () {
        o = Organizer.instance;

        self = GetComponent<ColliderOwner>().owner;

        mainCam = GameObject.Find("MainCamera").GetComponent<Camera>();
        navAgent.updateRotation = false;

        Gun rifle = o.GUN_STANDARD_RIFLE.Clone();
        itemEquiper.EquipItem(rifle);
        rifle.Show(anim.GetBoneTransform(HumanBodyBones.RightHand));

        JetPack jet = o.JETPACK_STANDARD.Clone();
        itemEquiper.EquipItem(jet);
        jet.Show(anim.GetBoneTransform(HumanBodyBones.UpperChest));

        currentMode = AIMode.Scouting;
    }
    protected GameUnit LookForEnemy(Faction lookForCharacterOfThisFaction)
    {
        ArrayList possibleTargets = GameUnit.unitsByFaction[lookForCharacterOfThisFaction];
        foreach(GameUnit possibleTarget in possibleTargets)
        {
            if(possibleTarget.body != null && self.body != null)
            {
                if (self.senses.CanSee(possibleTarget))
                {
                    Debug.Log("I saw: " + possibleTarget.body);
                    return possibleTarget;
                }
                else if (self.senses.CanHear(possibleTarget))
                {
                    Debug.Log("I heard: " + possibleTarget.body);
                    return possibleTarget;
                }
            }
        }
        return null;

    }
    public void Hunt(GameUnit target)
    {
        Debug.Log("Hunting:" + target.uniqueName);
        huntingTarget = target;
        character.LookAt(target.body);
        character.rifling = true;
        currentMode = AIMode.Hunting;
        
    }
	
	// Update is called once per frame
	void Update () {

        reactionCycle += Time.deltaTime;

        if (currentMode == AIMode.Scouting)
        {
            if(reactionCycle > self.senses.reactionTime)
            {
                GameUnit target = LookForEnemy(Organizer.FACTION_PLAYER);
                if (target != null)
                {
                    Hunt(target);
                }
                else
                {
                    reactionCycle = 0;
                }
            }    
        }else if(currentMode == AIMode.Hunting)
        {
            if(reactionCycle > self.senses.reactionTime/4)
            {
                reactionCycle = 0;
                if (self.senses.CanSee(huntingTarget))
                {
                    character.shooting = true;
                }
                else
                {
                    character.shooting = false;
                }

            }
        }

        // Manual orders
        if (navAgent != null)
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

                    moveOrders.Remove(this);

                    foreach (Vector3 order in moveOrders.Values)
                    {
                        if(y == order.y && z == order.z && order.x-0.8 < x && order.x+0.8 > x){
                            x += 0.8f;
                        }
                    }
                    Vector3 destination = new Vector3(x, y, z);
                    navAgent.SetDestination(destination);
                    moveOrders.Add(this, destination);

                }
            }
        }

        if (character != null)
        {
            if (navAgent.remainingDistance > navAgent.stoppingDistance)
            {
                character.Move(navAgent.desiredVelocity, false, false);
            }
            else
            {
                moveOrders.Remove(this);
                character.Move(Vector3.zero, false, false);
            }
        }

    }



    /*if (Input.GetButton("AIRifleUp"))
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
    }*/
}
