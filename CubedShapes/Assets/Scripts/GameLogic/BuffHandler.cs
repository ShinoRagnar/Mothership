using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffHandler : MonoBehaviour {

    private System.Collections.Generic.Dictionary<string, Buff> activeBuffs;
    public GameUnit owner;


    public BuffHandler(GameUnit owningUnit)
    {
        this.owner = owningUnit;
    }

    // Use this for initialization
    void Start () {
        activeBuffs = new System.Collections.Generic.Dictionary<string, Buff>();
    }

    void AddBuff(Buff b)
    {
        activeBuffs.Add(b.buffName,b);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
