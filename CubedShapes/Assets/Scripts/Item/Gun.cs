using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Item {

    public Vector3 gunpoint;
    
    public Gun(string name, Transform item, Alignment align, Vector3 gunp)
    {
        this.itemName = name;
        this.prefab = item;
        this.alignment = align;
        this.gunpoint = gunp;
    }
    public void Shoot()
    {
        Debug.Log("Shooting");
    }
    public Gun Clone()
    {
        return new Gun(itemName, prefab, alignment, gunpoint);
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
