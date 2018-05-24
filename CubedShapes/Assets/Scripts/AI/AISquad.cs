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
            squad.TellAllMembersToAimFor(target);
            squad.TurnAllMembersTowardsTarget(target);
            squad.TellMembersThatCanSeeToShoot(target);
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
}
