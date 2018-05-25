using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameUnit{

    protected static System.Collections.Generic.Dictionary<string, int> activeUnits = new System.Collections.Generic.Dictionary<string, int>();
    public static System.Collections.Generic.Dictionary<Faction, ArrayList> unitsByFaction = new Dictionary<Faction, ArrayList>();

    // Used by Agents
    public ItemEquiper itemEquiper;
    public Character character;
    public NavMeshAgent navMeshAgent;
    public Animator animator;
    public Rigidbody rigid;
    public CapsuleCollider collider;
    public CharacterLinkMover characterLinkMover;
   
    // Common
    public Faction belongsToFaction;
    public Health health;
    public Senses senses;
    public Transform body;

    // Not used?
    public float weight;
    public float headYOffset;

    //Names
    public string unitName;
    public string uniqueName;

    
    public GameUnit(string unitNameVal, Faction f, Health h, Senses s, float weightVal, float headYOffsetVal)
    {
        this.belongsToFaction = f;
        this.health = h;
        this.senses = s;
        this.weight = weightVal;
        this.headYOffset = headYOffsetVal;
        this.unitName = unitNameVal;
        if (activeUnits.ContainsKey(unitName))
        {
            activeUnits[unitName]++;
        }
        else { activeUnits.Add(unitName, 0); }
        uniqueName = unitName + activeUnits[unitName];
        unitsByFaction[f].Add(this);
        s.owner = this;
    }
    
    public GameUnit Clone()
    {
        
        return new GameUnit(unitName, belongsToFaction, health.Clone(),senses.Clone(),  weight, headYOffset);
    }

    public void RegisterBodyAndCompontentsForAgent(Transform bodyVal)
    {
        this.body = bodyVal;

        //Sets
        this.navMeshAgent = this.body.gameObject.AddComponent<NavMeshAgent>();
        this.navMeshAgent.stoppingDistance = 1f;
        this.navMeshAgent.speed = 7;
        this.itemEquiper = this.body.gameObject.AddComponent<ItemEquiper>();
        this.itemEquiper.owner = this;
        this.character = this.body.gameObject.AddComponent<Character>();
        this.character.owner = this;
        this.characterLinkMover = this.body.gameObject.AddComponent<CharacterLinkMover>();
        this.characterLinkMover.owner = this;

        //Gets (if it has them)
        this.animator = this.body.gameObject.GetComponent<Animator>();
        this.rigid = this.body.gameObject.GetComponent<Rigidbody>();
        this.collider = this.body.gameObject.GetComponent<CapsuleCollider>();
        
    }
}
