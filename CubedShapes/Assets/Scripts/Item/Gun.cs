using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Item {

    public Vector3 gunpoint;
    public Item muzzle;
    
    public Gun(string name, Transform item, Alignment align, Vector3 gunp, Item muzzl)
    {
        this.itemName = name;
        this.prefab = item;
        this.alignment = align;
        this.gunpoint = gunp;
        this.muzzle = muzzl;
    }
    public void Shoot()
    {
        if (showing) {
            if (!muzzle.showing)
            {
                ie.Materialize(muzzle, visualItem);
               // FPSParticleSystemScaler scaler = muzzle.visualItem.gameObject.AddComponent<FPSParticleSystemScaler>();
               // scaler.particlesScale = 4f;
            }
            else
            {
                muzzle.ReEnable();
            }
        }
        Debug.Log("Shooting");
    }
    public Gun Clone()
    {
        return new Gun(itemName, prefab, alignment, gunpoint, new Item(muzzle.itemName,muzzle.prefab,muzzle.alignment));
    }

}
