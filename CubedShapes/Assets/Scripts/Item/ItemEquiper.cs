using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEquiper : MonoBehaviour {

    public System.Collections.Generic.Dictionary<string, Item> equipped;

    public void Awake()
    {
        equipped = new System.Collections.Generic.Dictionary<string, Item>();
    }

    public void EquipItem(Item i, Transform parent)
    {
        if (i.prefab != null && !i.showing)
        {
            equipped.Add(i.itemName, i);
            Transform visualItem = Instantiate(i.prefab, parent);
            visualItem.position += new Vector3(i.alignment.x, i.alignment.y, i.alignment.z);
            visualItem.Rotate(new Vector3(i.alignment.rotX, i.alignment.rotY, i.alignment.rotZ));
            visualItem.localScale += new Vector3(i.alignment.scaleX, i.alignment.scaleY, i.alignment.scaleZ);
            i.showing = true;
        }

    }
	
	
}
