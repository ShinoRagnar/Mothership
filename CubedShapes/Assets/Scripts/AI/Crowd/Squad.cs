using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Squad {

    public static Dictionary<Squad, string> activeSquads = new Dictionary<Squad, string>();
    public static string DEFAULT_SQUAD_NAME = "Alpha:";
   

    public List<GameUnit> members;
    public UnitFormation currentFormation;

    public string name;

    public Squad()
    {
        currentFormation = new UnitFormation(PlacementStrategy.MiddleAndOut);
        name = DEFAULT_SQUAD_NAME + (activeSquads.Count + 1);
        members = new List<GameUnit>();
        activeSquads.Add(this, name);
    }
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
                member.character.FaceTarget(target.body.transform.position);
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
            gu.itemEquiper.EquipItem(item.Clone()).Show(gu.animator.GetBoneTransform(placement));
        }
    }
    public void EquipAllMembersWith(JetPack item, HumanBodyBones placement)
    {
        foreach (GameUnit gu in members)
        {
            gu.itemEquiper.EquipItem(item.Clone()).Show(gu.animator.GetBoneTransform(placement));
        }
    }



}
