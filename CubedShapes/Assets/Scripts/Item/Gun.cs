using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Item {

    public static int RAYCASTS_WHEN_SHOOTING = 1;

    public Vector3 gunpoint;
    public Item muzzle;
    public Dictionary<Buff, float> transferringBuffs;
    
    public Gun(string name, Transform item, Alignment align, Vector3 gunp, Item muzzl)
    {
        this.itemName = name;
        this.prefab = item;
        this.alignment = align;
        this.gunpoint = gunp;
        this.muzzle = muzzl;
        this.transferringBuffs = new Dictionary<Buff, float>();
    }
    public void AddBuffToTransferOnShot(Buff b, float direction)
    {
        transferringBuffs.Add(b,direction);
    }

    public void ShootAt(GameUnit target)
    {
        if (showing) {

            RaycastHit hit = ie.owner.senses.TryToHit(muzzle.visualItem.transform.position, target, RAYCASTS_WHEN_SHOOTING);
            if(hit.collider != null)
            {
                
                //Hit shield
                Forge3D.Forcefield ffHit = hit.collider.transform.GetComponentInParent<Forge3D.Forcefield>();
                if (ffHit != null)
                {
                    float hitPower = Random.Range(-2f, 2f);
                    ffHit.OnHit(hit.point, hitPower);
                    //Debug.Log("Shooting at: " + Time.time);
                }
                //Transfer debuffs
                ColliderOwner colo = hit.collider.transform.GetComponent<ColliderOwner>();
                if(colo != null)
                {
                    if(colo.owner.buffHandler != null)
                    {
                        foreach (Buff buff in transferringBuffs.Keys)
                        {
                            float direction = transferringBuffs[buff];
                            //Transfer directional buffs in the correct direction
                            if(
                                (colo.owner.body.position.x < ie.owner.body.position.x && direction < 0)
                                ||
                                (colo.owner.body.position.x > ie.owner.body.position.x && direction > 0)
                                ||
                                direction == 0
                                )
                            {
                                colo.owner.buffHandler.AddBuff(ie.owner.uniqueName, buff);
                            }
                            
                        }
                    }
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
        Gun g = new Gun(itemName, prefab, alignment, gunpoint, muzzle.Clone());
        foreach(Buff b in transferringBuffs.Keys)
        {
            g.transferringBuffs.Add(b,transferringBuffs[b]);
        }
        return g;

    }

}
