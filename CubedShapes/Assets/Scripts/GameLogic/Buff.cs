using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Buff {

    public float duration;
    public string buffName;
    public float currentDuration;
    public float currentStacks;
    public Sprite icon;
    public bool isDebuff;

    public Buff(string buffname, float durati, Sprite icn, bool isDebuffVal)
    {
        this.buffName = buffname;
        this.duration = durati;
        this.icon = icn;
        this.isDebuff = isDebuffVal;
    }


}
