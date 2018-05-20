using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUnit{

    protected static System.Collections.Generic.Dictionary<string, int> activeUnits = new System.Collections.Generic.Dictionary<string, int>();
    public static System.Collections.Generic.Dictionary<Faction, ArrayList> unitsByFaction = new Dictionary<Faction, ArrayList>();


    public Faction belongsToFaction;
    public float weight;
    public Health health;
    public Senses senses;
    public string unitName;
    public string uniqueName;

    public Transform body;


    public GameUnit(string unitNameVal, Faction f, Health h, Senses s, float weightVal)
    {
        this.belongsToFaction = f;
        this.health = h;
        this.senses = s;
        this.weight = weightVal;
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
        
        return new GameUnit(unitName, belongsToFaction, health.Clone(),senses.Clone(),  weight);
    }
}
