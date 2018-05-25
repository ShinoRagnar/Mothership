using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Buff {

    public Dictionary<string, float> stacks;

    public TextMeshProUGUI text;
    public float duration;
    public string buffName;
    public float currentDuration;
    //public int currentStacks;
    public Transform prefab;
    public Transform visibleIcon;
    public bool isDebuff;
    public bool isActive;

    public Buff(string buffnameVal, float durati, Transform icn, bool isDebuffVal)
    {
        this.buffName = buffnameVal;
        this.duration = durati;
        this.prefab = icn;
        this.isDebuff = isDebuffVal;
        this.isActive = true;
        this.currentDuration = 0;
        this.stacks = new Dictionary<string, float>();
    }
    public void DurationEnded()
    {
        this.isActive = false;
        this.stacks.Clear();
    }
    public void AddStack(string source)
    {
        this.currentDuration = 0;
        if (stacks.ContainsKey(source))
        {
            stacks[source] = 0;
        }
        else
        {
            stacks.Add(source, 0);
        }
        this.isActive = true;
    }


}
