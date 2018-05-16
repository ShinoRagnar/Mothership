using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item {


    public Transform visualItem;
    public bool showing;
    public string itemName;
    public Transform prefab;
    public Alignment alignment;

    protected ItemEquiper ie;

    public Item() { }
    public Item(string itemn, Transform item, Alignment alig)
    {
        this.itemName = itemn;
        this.prefab = item;
        this.alignment = alig;
    }
    public void AddEquipper(ItemEquiper iteme)
    {
        ie = iteme;
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

	public void ReEnable()
    {
        if(showing)
        {
            visualItem.gameObject.SetActive(false);
            visualItem.gameObject.SetActive(true);
        }
    }
}
