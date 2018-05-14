using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item {


    public Transform visualItem;
    public bool showing;
    public string itemName;
    public Transform prefab;
    public Alignment alignment;

    public Item() { }
    public Item(string itemn, Transform item, Alignment alig)
    {
        this.itemName = itemn;
        this.prefab = item;
        this.alignment = alig;
    }
    public Alignment GetAlignment()
    {
        return alignment;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
