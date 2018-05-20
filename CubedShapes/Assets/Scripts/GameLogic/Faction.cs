using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Faction{

    public string factionName;
    public System.Collections.Generic.Dictionary<string, Faction> hostileFactions;
    public System.Collections.Generic.Dictionary<string, Faction> alliedFactions;

    public Faction(string name)
    {
        this.factionName = name;
        hostileFactions = new System.Collections.Generic.Dictionary<string, Faction>();
        alliedFactions = new System.Collections.Generic.Dictionary<string, Faction>();
        GameUnit.unitsByFaction.Add(this, new ArrayList());

    }
    public Faction(string name, Faction hostileTo)
    {
        this.factionName = name;
        hostileFactions = new System.Collections.Generic.Dictionary<string, Faction>();
        alliedFactions = new System.Collections.Generic.Dictionary<string, Faction>();
        GameUnit.unitsByFaction.Add(this, new ArrayList());

        SetHostileTo(hostileTo);
    }
    public void SetHostileTo(Faction f)
    {
        Debug.Log("These factions are now hostile: "+this.factionName + " " + f.factionName);
        SetHostile(f);
        f.SetHostile(this);
    }
    protected void SetHostile(Faction f)
    {
        if (!hostileFactions.ContainsKey(f.factionName))
        {
            hostileFactions.Add(f.factionName, f);
        }
        if (IsAlliedTo(f))
        {
            alliedFactions.Remove(f.factionName);
        }
    }
    public bool IsHostileTo(Faction f)
    {
        if(f == this)
        {
            return false;
        }
        if (hostileFactions.ContainsKey(f.factionName) )
        {
            return true;
        }
        return false;

    }
    public bool IsAlliedTo(Faction f)
    {
        if (f == this)
        {
            return true;
        }
        if (alliedFactions.ContainsKey(f.factionName))
        {
            return true;
        }
        return false;
    }
    public bool IsNeutralTo(Faction f)
    {
        if (f == this)
        {
            return false;
        }
        if (!IsHostileTo(f) && !IsAlliedTo(f))
        {
            return true;
        }
        return false;
    }





}
