using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuffHandler : MonoBehaviour {

    private System.Collections.Generic.Dictionary<string, Buff> buffs;
    public float activeBuffs;
    public GameUnit owner;
    public static float BUFF_DISTANCE = 55;
    public static float BUFF_START_DISTANCE = 30;
    public GameObject debuffPanel;
    private Organizer o;

    // Use this for initialization
    void Start () {
        o = Organizer.instance;
        if (buffs == null)
        {
            buffs = new System.Collections.Generic.Dictionary<string, Buff>();
            debuffPanel = GameObject.Find(Organizer.NAME_DEBUFF_PANEL);
            activeBuffs = 0;
        }
    }

    /*public void Test()
    {
        Buff b = new Buff("Slowed Left", 2, Organizer.instance.UI_BUFF_ARROW_LEFT, true);
        Buff c = new Buff("Slowed Left2", 4, Organizer.instance.UI_BUFF_ARROW_LEFT, true);

        AddBuff(b);
        AddBuff(c);
    }*/

    public void AddBuff(string source, Buff b)
    {
        if (buffs.ContainsKey(b.buffName))
        {
            
        }
        else
        {
            buffs.Add(b.buffName, b);
            if (owner.isPlayer)
            {
                if (b.isDebuff)
                {
                    b.visibleIcon = Instantiate(b.prefab, debuffPanel.transform);
                }
            }
        }

        buffs[b.buffName].AddStack(source);
        RefreshActiveBuffsCounter();
        
        if (owner.isPlayer)
        {
            RecalculateBuffPositions();
        }

    }
    public void RefreshActiveBuffsCounter()
    {
        activeBuffs = 0;
        foreach (Buff bob in buffs.Values)
        {
            if (bob.isActive)
            {
                activeBuffs++;
            }
        }
    }

    public void RecalculateBuffPositions()
    {
        float posX = 0;
        if (owner.isPlayer)
        {
            foreach (Buff buf in buffs.Values)
            {
                if (buf.isActive)
                {
                    RectTransform rt = buf.visibleIcon.GetComponent<RectTransform>();

                    if (rt != null)
                    {
                        rt.localPosition = new Vector3(BUFF_START_DISTANCE + posX * BUFF_DISTANCE, rt.localPosition.y, rt.localPosition.z);
                        posX++;
                    }
                }
            }
        }
    }
	public void UpdateBuff(Buff b)
    {
        if(b.isActive && !b.visibleIcon.transform.gameObject.activeSelf)
        {
            b.visibleIcon.transform.gameObject.SetActive(true);
        }
        if(b.text == null)
        {
            b.text = b.visibleIcon.GetComponentInChildren<TextMeshProUGUI>();
        }
        b.text.text = ""+(b.stacks.Count);
    }

	// Update is called once per frame
	void Update () {
        if(activeBuffs > 0) { 
		    foreach(Buff buf in buffs.Values)
            {
                //Debug.Log("updatin");
                if (buf.isActive) {
                
                    buf.currentDuration += Time.deltaTime;
                    if(buf.currentDuration > buf.duration)
                    {
                        buf.DurationEnded();
                        activeBuffs--;
                        if (owner.isPlayer) {
                            buf.visibleIcon.gameObject.SetActive(false);
                            RecalculateBuffPositions();
                        }
                    }
                    else
                    {
                        UpdateBuff(buf);
                    }
                }
            }

        }
    }
}
