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
    public void ShootAt(GameUnit target)
    {
        if (showing) {

            RaycastHit hit = ie.equippedUnit.senses.TryToHit(muzzle.visualItem.transform.position, target, Senses.MAX_RAY_CASTS_WHEN_TRY_TO_SEE);
            if(hit.collider != null)
            {
                Forge3D.Forcefield ffHit = hit.collider.transform.GetComponentInParent<Forge3D.Forcefield>();
                if (ffHit != null)
                {
                    float hitPower = Random.Range(-2f, 2f);
                    ffHit.OnHit(hit.point, hitPower);
                    //Debug.Log("Shooting at: " + Time.time);
                }
            }
            /*RaycastHit hit;
            Vector3 fromPosition = muzzle.visualItem.transform.position;
            Vector3 toPosition = target.transform.position;
            Vector3 direction = toPosition - fromPosition;

            // Casts a ray against colliders in the scene
            if (Physics.Raycast(fromPosition, direction, out hit))
            {
                Forge3D.Forcefield ffHit = hit.collider.transform.GetComponentInParent<Forge3D.Forcefield>();
                //Debug.DrawRay(fromPosition, direction, Color.green);
                // Generate random hit power value and call Force Field script if successful
                if (ffHit != null)
                {
                    float hitPower = Random.Range(-2f, 2f);
                    ffHit.OnHit(hit.point, hitPower);
                }
            }*/
            muzzle.ReEnable();
        }
    }
    public new Gun Clone()
    {
        return new Gun(itemName, prefab, alignment, gunpoint, muzzle.Clone());//new Item(muzzle.itemName,muzzle.prefab,muzzle.alignment));
    }

}
