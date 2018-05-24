using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item {

    private static int itemCounter = 0;

    public ArrayList subItems = new ArrayList();

    public Transform visualItem;
    public Transform prefab;
    public Alignment alignment;

    public bool showing;
    public string itemName;

    protected ItemEquiper ie;
    public int itemNumber;

    //Never use this
    public Item() { }

    public Item(string itemn, Transform item, Alignment alig)
    {
        this.itemNumber = itemCounter;
        this.itemName = itemn;
        this.prefab = item;
        this.alignment = alig;
        itemCounter++;
    }
    public void AddEquipper(ItemEquiper iteme)
    {
        //Debug.Log("Adding equipper to:" + itemName);
        ie = iteme;
        foreach(Item child in subItems)
        {
            child.ie = iteme;
        }
    }

    public void Show(Transform parent)
    {
        if (ie != null)
        {
            ie.Materialize(this, parent);
        }
    }

    public Alignment GetAlignment()
    {
        return alignment;
    }
    public Item Clone()
    {
        return new Item(itemName, prefab, alignment);
    }
    public void Enable()
    {
        if (showing)
        {
            visualItem.gameObject.SetActive(true);
        }
        foreach (Item i in subItems)
        {
            i.Enable();
        }
    }
    public void Disable()
    {
        if (showing)
        {
            visualItem.gameObject.SetActive(false);
        }
        foreach (Item i in subItems)
        {
            i.Disable();
        }
    }
    public void ReEnable()
    {
        if(showing)
        {
            visualItem.gameObject.SetActive(false);
            visualItem.gameObject.SetActive(true);
        }
        foreach (Item i in subItems)
        {
            i.ReEnable();
        }
    }
}
