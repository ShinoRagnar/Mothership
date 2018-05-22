using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEquiper : MonoBehaviour {

    public System.Collections.Generic.Dictionary<string, Item> equipped;
    public Character equippedCharacter;
    public GameUnit equippedUnit;

    public void Awake()
    {
        equipped = new System.Collections.Generic.Dictionary<string, Item>();
    }
    public void Materialize(Item i, Transform parent)
    {
        //Debug.Log(i.itemName);
        if (i.prefab != null && !i.showing)
        {
            i.visualItem = Instantiate(i.prefab, parent);
            i.visualItem.Rotate(new Vector3(i.alignment.rotX, i.alignment.rotY, i.alignment.rotZ));
            
            i.visualItem.localScale += new Vector3(i.alignment.scaleX, i.alignment.scaleY, i.alignment.scaleZ);
            i.visualItem.position += new Vector3(i.alignment.x, i.alignment.y, i.alignment.z);
            i.showing = true;
            if(i is Gun)
            {
                Materialize(((Gun)i).muzzle, i.visualItem);
                ((Gun)i).muzzle.visualItem.gameObject.SetActive(false);
            }
        }
    }

    public void EquipItem(Item i)
    {
        equipped.Add(i.itemName, i);
        i.AddEquipper(this);
        if(equippedCharacter != null)
        {
            equippedCharacter.UpdateWithEquippedItems();
        }
    }
	
	
}
