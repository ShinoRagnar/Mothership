using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUnit{

    protected static System.Collections.Generic.Dictionary<string, int> activeUnits = new System.Collections.Generic.Dictionary<string, int>();

    public Faction belongsToFaction;
    public float weight;
    public Health health;
    public string unitName;
    public string uniqueName;

    public GameUnit(string unitNameVal, Faction f, Health h,  float weightVal)
    {
        this.belongsToFaction = f;
        this.health = h;
        this.weight = weightVal;
        this.unitName = unitNameVal;
        if (activeUnits.ContainsKey(unitName))
        {
            activeUnits[unitName]++;
        }
        else { activeUnits.Add(unitName, 0); }
        uniqueName = unitName + activeUnits[unitName];
    }

    public GameUnit Clone()
    {
        return new GameUnit(unitName, belongsToFaction, health.Clone(), weight);
    }
}
