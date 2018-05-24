using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AIState
{
    Idle,
    Hunting,
    Fleeing
}

public class AISquad : MonoBehaviour {

    public static float DISTANCE_PREFERRED = 20;
    public static float DISTANCE_TOO_CLOSE = 10;

    public static float IDLE_REACTION_TIME = 0.5f;
    public static int NUMBER_OF_SQUAD_MEMBERS_TO_LOOK_WHEN_IDLE = 3;

    public Squad squad;
    public AIState state;
    private float timeSinceLastChange;
    public Faction enemyToThisSquad;

    public GameUnit target;

    public AISquad()
    {
        squad = new Squad();
        state = AIState.Idle;
        timeSinceLastChange = 0;
        enemyToThisSquad = Organizer.FACTION_PLAYER;
    }

    public GameUnit AddUnit(GameUnit gu)
    {
        if(gu.itemEquiper != null && gu.navMeshAgent != null && gu.body != null && gu.character != null && gu.animator != null)
        {
            squad.members.Add(gu);
        }
        else
        {
            Debug.Log("Character cannot be AI-Controlled, missing objects: " + gu.uniqueName);
        }
        return gu;
    }
	
	// Update is called once per frame
	void Update () {

        timeSinceLastChange += Time.deltaTime;

        if (state == AIState.Idle)
        {
            if(timeSinceLastChange > IDLE_REACTION_TIME)
            {
                timeSinceLastChange = 0;
                for(int i = 0; i < Mathf.Min(squad.members.Count, NUMBER_OF_SQUAD_MEMBERS_TO_LOOK_WHEN_IDLE); i++)
                {
                    GameUnit enemy = LookForEnemy(squad.members.RandomElement(), enemyToThisSquad);
                    if(enemy != null)
                    {
                        state = AIState.Hunting;
                        target = enemy;
                        squad.AssignReactionTimesToAllMembers(0.01f, 0.5f);
                        break;
                    }
                }
            }
        }
        if (state == AIState.Hunting)
        {
            if (IsTargetTooClose())
            {
                
                state = AIState.Fleeing;
                Dictionary<Ground, float> possibleMoves = GetPossibleMovesAtDistanceFromTarget(DISTANCE_PREFERRED);

                foreach (Ground p in possibleMoves.Keys)
                {
                    int reserves = squad.currentFormation.Move(p, possibleMoves[p], Squad.DEFAULT_SQUAD_SOLDIER_WIDTH);
                    Debug.Log("Fleeing to: " + p.obj.name + " but with: " + reserves + " reserves");
                    break;
                }
            }
            else { 
                squad.TellAllMembersToAimFor(target);
                squad.TurnAllMembersTowardsTarget(target);
                squad.TellMembersThatCanSeeToShoot(target);
            }
        }
        if(state == AIState.Fleeing)
        {
            squad.MoveAllSoldiers();
        }
	}

    protected GameUnit LookForEnemy(GameUnit looker, Faction lookForCharacterOfThisFaction)
    {
        ArrayList possibleTargets = GameUnit.unitsByFaction[lookForCharacterOfThisFaction];
        foreach (GameUnit possibleTarget in possibleTargets)
        {
            if (possibleTarget.body != null && looker.body != null)
            {
                if (looker.senses.CanSee(possibleTarget))
                {
                    return possibleTarget;
                }
                else if (looker.senses.CanHear(possibleTarget))
                {
                    return possibleTarget;
                }
            }
        }
        return null;
    }
    protected Dictionary<Ground, float> GetPossibleMovesAtDistanceFromTarget(float distance)
    {
        Dictionary<Ground, float> possibleMoves = new Dictionary<Ground, float>();
        // if (NavMeshAttachor.generated.ContainsKey(squad.currentFormation.currentlyOn))
        //{
        Ground currentGround = squad.currentFormation.currentlyOn; //NavMeshAttachor.generated[character.lastWalkedOn];
            if (CanMoveRightToPreferredDistance(currentGround))
            {
                possibleMoves.Add(currentGround, target.body.position.x + distance);
            }
            else if (CanMoveLeftToPreferredDistance(currentGround))
            {
                possibleMoves.Add(currentGround, target.body.position.x - distance);
            }

            Collider[] considerations = Physics.OverlapSphere(target.body.position, distance);

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
                                squad.currentFormation.GetFormationCenter().x > target.body.position.x
                                && link.x > target.body.position.x
                                && currentGround.startPointToEndPoint[link].x >= link.x
                                && currentGround.distances[link].ContainsKey(consideration)
                                )
                                ||
                                // Move left
                                (
                                squad.currentFormation.GetFormationCenter().x < target.body.position.x
                                && link.x < target.body.position.x
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
       // }
        return possibleMoves;
    }
    private bool CanMoveRightToPreferredDistance(Ground on)
    {
        return squad.currentFormation.GetFormationCenter().x > target.body.position.x && target.body.position.x + DISTANCE_PREFERRED < on.obj.position.x + on.obj.localScale.x / 2;
    }
    private bool CanMoveLeftToPreferredDistance(Ground on)
    {
        return squad.currentFormation.GetFormationCenter().x < target.body.position.x && target.body.position.x - DISTANCE_PREFERRED > on.obj.position.x - on.obj.localScale.x / 2;
    }
    protected bool IsTargetTooClose()
    {
        return Mathf.Abs(squad.currentFormation.GetFormationCenter().x - target.body.position.x) < DISTANCE_TOO_CLOSE
               &&
               Mathf.Abs(squad.currentFormation.GetFormationCenter().y - target.body.position.y) < DISTANCE_TOO_CLOSE
               ;
    }

}
