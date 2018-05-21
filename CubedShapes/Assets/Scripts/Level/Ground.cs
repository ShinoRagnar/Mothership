using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground {

    public Transform obj;
    public System.Collections.Generic.Dictionary<Vector3, Ground> links;

    public Ground(Transform groundObject)
    {
        this.obj = groundObject;
        this.links = new System.Collections.Generic.Dictionary<Vector3, Ground>();
    }
    

}
