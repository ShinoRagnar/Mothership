﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Squad {

    public static Dictionary<Squad, string> activeSquads = new Dictionary<Squad, string>();
    public static string DEFAULT_SQUAD_NAME = "Alpha:";
    public static float DEFAULT_SQUAD_SOLDIER_WIDTH = 1f;
   

    public List<GameUnit> members;
    public UnitFormation currentFormation;

    public string name;

    public Vector2 GetXRangeOfMembers()
    {
        float left = 0;
        float right = 0;
        bool first = true;

        foreach(GameUnit mem in members)
        {
            if(first || mem.body.position.x < left)
            {
                left = mem.body.position.x;
            }
            if (first || mem.body.position.x > right)
            {
                right = mem.body.position.x;
            }
            first = false;
        }
        return new Vector2(left, right);
    }

    public Squad()
    {
        currentFormation = new UnitFormation(PlacementStrategy.MiddleAndOut);
        name = DEFAULT_SQUAD_NAME + (activeSquads.Count + 1);
        members = new List<GameUnit>();
        activeSquads.Add(this, name);
    }
    public bool UpdateCharacterMove(GameUnit target, float preferredDistance, float insideLastStand)
    {
        bool moved = false;

        foreach (GameUnit member in members)
        {
            if(IsTargetTooClose(target, member.body.position.x, member.body.position.y, insideLastStand)){
                member.navMeshAgent.SetDestination(member.body.position);

            }else if (IsTargetTooClose(target, member.body.position.x, member.body.position.y, preferredDistance))
            {
                member.navMeshAgent.SetDestination(currentFormation.GetMoveFor(member));
            }
            else
            {
                member.navMeshAgent.SetDestination(member.body.position);
            }

            if (DevelopmentSettings.SHOW_DESTINATIONS)
            {
                Vector3 direction = member.navMeshAgent.destination - member.body.position;
                Debug.DrawRay(member.body.position, direction, Color.green);
            }

            if (member.navMeshAgent.remainingDistance > member.navMeshAgent.stoppingDistance)
            {
                member.character.Move(member.navMeshAgent.desiredVelocity);
                moved = true;
            }
            else
            {
                member.character.Move(Vector3.zero);
            }
        }
        return moved;

    }
    /*public bool MoveAllSoldiers()
    {
        bool finished = true;
        foreach (GameUnit member in members)
        {
            if (member.character.ShouldAct())
            {
                member.character.gunState = GunState.Idle;


                if (! (Vector3.Distance(member.body.position,member.navMeshAgent.destination) < 1f))
                {
                    finished = false;
                }
                member.character.UpdateAnimatorState();
            }
            else { finished = false; }
        }
        return finished;
    }*/
    public void AssignReactionTimesToAllMembers(float min, float max)
    {
        System.Random rand = Level.instance.rand;
        foreach (GameUnit member in members)
        {
            float num = (max - min) * (float) rand.NextDouble() + min;
            member.character.reactionTime = num;
            member.character.currentReactionCycle = 0;
        }
    }

    public void TellAllMembersToAimFor(GameUnit target)
    {
        foreach (GameUnit member in members)
        {
            if (member.character.ShouldAct()) { 
                member.character.armState = ArmState.Aiming;
                member.character.LookAt(target);
                member.character.UpdateAnimatorState();
            }
        }
    }
    public void TurnAllMembersTowardsTarget(GameUnit target)
    {
        foreach (GameUnit member in members)
        {
            if (member.character.ShouldAct())
            {

                //Don't turn when moving
                if (! (member.navMeshAgent.remainingDistance > member.navMeshAgent.stoppingDistance))
                {
                    member.character.FaceTarget(target.body.transform.position);
                }
            }
        }
    }
    public void TellMembersThatCanSeeToShoot(GameUnit target)
    {
        foreach (GameUnit member in members)
        {

            if (member.character.ShouldAct())
            {
                if (member.senses.CanSee(target))
                {
                    member.character.gunState = GunState.Shooting;
                }
                else
                {
                    member.character.gunState = GunState.Idle;
                }
                member.character.UpdateAnimatorState();
            }
            //gu.character.FaceTarget(target.body.transform.position);
        }
    }
    public void EquipAllMembersWith(Gun item, HumanBodyBones placement)
    {
        foreach(GameUnit gu in members)
        {
            Gun g = item.Clone();
            gu.itemEquiper.EquipItem(g);
            g.Show(gu.animator.GetBoneTransform(placement));
        }
    }
    public void EquipAllMembersWith(JetPack item, HumanBodyBones placement)
    {
        foreach (GameUnit gu in members)
        {
            JetPack jet = item.Clone();
            gu.itemEquiper.EquipItem(jet);
            jet.Show(gu.animator.GetBoneTransform(placement));
        }
    }
    public bool IsTargetTooClose(GameUnit targ, float x, float y, float dist)
    {
        return IsTargetTooClose(targ, x, x, y, y, dist);
    }

    public bool IsTargetTooClose(GameUnit targ, float xMin, float xMax, float yMin, float yMax, float dist)
    {
        return (
                    (targ.body.position.x + dist > xMin && targ.body.position.x < xMin)
                    ||
                    (targ.body.position.x - dist < xMax && targ.body.position.x > xMax)
                )
               &&
               (
                    (targ.body.position.y + dist > yMin && targ.body.position.y < yMin)
                    ||
                    (targ.body.position.y - dist < yMax && targ.body.position.y > yMax)
                );
    }

}
