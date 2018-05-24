using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*public enum AIMode
{
    Scouting,
    Hunting,
    Relocating
}*/
public class AIController : MonoBehaviour {
    /*
        public static System.Collections.Generic.Dictionary<AIController, Vector3> moveOrders = new System.Collections.Generic.Dictionary<AIController, Vector3>();



        public NavMeshAgent navAgent;
        private Camera mainCam;
        public Character character;
        private Animator anim;
        public ItemEquiper itemEquiper;
        private GameUnit self;
        private Organizer o;

        //AI
        AIMode currentMode;
        public GameUnit huntingTarget;
        private float reactionCycle = 0;

        //Behaviour


        // Use this for initialization
        private void Awake()
        {
            anim = GetComponent<Animator>();
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
                        //Debug.Log("I saw: " + possibleTarget.body);
                        return possibleTarget;
                    }
                    else if (self.senses.CanHear(possibleTarget))
                    {
                        //Debug.Log("I heard: " + possibleTarget.body);
                        return possibleTarget;
                    }
                }
            }
            return null;

        }
        public void Hunt(GameUnit target)
        {
            //Debug.Log("Hunting:" + target.uniqueName+" at:"+Time.time);
            huntingTarget = target;
            character.LookAt(target);
            character.rifling = true;
            currentMode = AIMode.Hunting;

        }

        protected bool IsTargetTooClose()
        {
            return Mathf.Abs(self.body.position.x - huntingTarget.body.position.x) < DISTANCE_TOO_CLOSE
                   &&
                   Mathf.Abs(self.body.position.y - huntingTarget.body.position.y) < DISTANCE_TOO_CLOSE
                   ;
        }
        protected Dictionary<Ground, float> GetPossibleMovesAtDistanceFromTarget(float distance)
        {
            Dictionary<Ground, float> possibleMoves = new Dictionary<Ground, float>();
            if (NavMeshAttachor.generated.ContainsKey(character.lastWalkedOn))
            {
                Ground currentGround = NavMeshAttachor.generated[character.lastWalkedOn];
                if (CanMoveRightToPreferredDistance(currentGround))
                {
                    possibleMoves.Add(currentGround, huntingTarget.body.position.x + distance);
                }
                else if (CanMoveLeftToPreferredDistance(currentGround))
                {
                    possibleMoves.Add(currentGround, huntingTarget.body.position.x - distance);
                }

                Collider[] considerations = Physics.OverlapSphere(huntingTarget.body.position, distance);

                foreach (Collider c in considerations)
                {
                    if (NavMeshAttachor.generated.ContainsKey(c.transform))
                    {
                        Ground consideration = NavMeshAttachor.generated[c.transform];

                        foreach (Vector3 link in currentGround.links.Keys)
                        {
                            if (
                                //TODO: Should also take Y into consideration
                                //Move Right
                                    (
                                    self.body.position.x > huntingTarget.body.position.x
                                    && link.x > huntingTarget.body.position.x
                                    && currentGround.startPointToEndPoint[link].x >= link.x
                                    && currentGround.distances[link].ContainsKey(consideration)
                                    )
                                    ||
                                // Move left
                                    (
                                    self.body.position.x < huntingTarget.body.position.x
                                    && link.x < huntingTarget.body.position.x
                                    && currentGround.startPointToEndPoint[link].x <= link.x
                                    && currentGround.distances[link].ContainsKey(consideration)
                                    )
                                )
                            {
                                possibleMoves.Add(consideration, consideration.GetMidPoint().x);
                            }
                        }
                    }
                }
            }
            return possibleMoves;
        }
        private bool CanMoveRightToPreferredDistance(Ground on)
        {
            return self.body.position.x > huntingTarget.body.position.x && huntingTarget.body.position.x + DISTANCE_PREFERRED < on.obj.position.x + on.obj.localScale.x / 2;
        }
        private bool CanMoveLeftToPreferredDistance(Ground on)
        {
            return self.body.position.x < huntingTarget.body.position.x && huntingTarget.body.position.x - DISTANCE_PREFERRED > on.obj.position.x - on.obj.localScale.x / 2;
        }

        // Update is called once per frame
        void Update () {

            reactionCycle += Time.deltaTime;

            if (currentMode == AIMode.Scouting && reactionCycle > self.senses.GetReactionTime())
            {
                GameUnit target = LookForEnemy(Organizer.FACTION_PLAYER);
                if (target != null){
                    Hunt(target);
                }else{
                    reactionCycle = 0;
                }
            }else if (currentMode == AIMode.Hunting && reactionCycle > self.senses.GetReactionTime() / 4){
                if (IsTargetTooClose()){
                    Debug.Log("I need to fall back: "+Time.time);
                    currentMode = AIMode.Relocating;
                    character.shooting = false;
                }else{
                    if (self.senses.CanSee(huntingTarget)){
                        character.shooting = true;
                    }else{
                        character.shooting = false;
                    }
                    reactionCycle = 0;
                }
            }
            else if (currentMode == AIMode.Relocating && reactionCycle > self.senses.GetReactionTime() / 4 && character.IsGrounded() && character.lastWalkedOn != null)
            {
                reactionCycle = 0;
                if (IsTargetTooClose()) {
                    Dictionary<Ground, float> possibleMoves = GetPossibleMovesAtDistanceFromTarget(DISTANCE_PREFERRED);

                    foreach(Ground p in possibleMoves.Keys){
                        navAgent.SetDestination(new Vector3(possibleMoves[p], p.GetMidPoint().y));
                        //Debug.Log("Relocating to: " + p.obj.transform.gameObject.name);
                        break;
                    }

                }else{
                    currentMode = AIMode.Hunting;
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
                    if (currentMode == AIMode.Hunting)
                    {
                        character.FaceTarget(huntingTarget.body.position);
                    }
                }
            }

        }*/
}
