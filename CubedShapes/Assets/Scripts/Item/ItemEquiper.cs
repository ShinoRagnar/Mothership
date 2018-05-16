using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEquiper : MonoBehaviour {

    public System.Collections.Generic.Dictionary<string, Item> equipped;

    public void Awake()
    {
        equipped = new System.Collections.Generic.Dictionary<string, Item>();
    }
    public void Materialize(Item i, Transform parent)
    {
        if (i.prefab != null && !i.showing)
        {
            i.visualItem = Instantiate(i.prefab, parent);
            i.visualItem.position += new Vector3(i.alignment.x, i.alignment.y, i.alignment.z);
            i.visualItem.Rotate(new Vector3(i.alignment.rotX, i.alignment.rotY, i.alignment.rotZ));
            i.visualItem.localScale += new Vector3(i.alignment.scaleX, i.alignment.scaleY, i.alignment.scaleZ);
            i.showing = true;
        }
    }

    public void EquipItem(Item i)
    {
        equipped.Add(i.itemName, i);
        i.AddEquipper(this);
    }
	
	
}
