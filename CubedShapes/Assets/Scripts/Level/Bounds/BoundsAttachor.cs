using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundsAttachor : MonoBehaviour {

    private Level level;
    public GameObject water;

    public static float WATER_LEVEL = 10;

	// Use this for initialization
	void Start () {

        level = GetComponent<Level>();

        CreateBounds(level.getLeftX(), 0, 0, level.height);
        CreateBounds(level.getTopAndBottomX(), level.getTopY(), level.width, 0);
        CreateBounds(level.getTopAndBottomX(), level.getBottomY(), level.width, 0);
        CreateBounds(level.getRightX(), 0, 0, level.height);

        if (DevelopmentSettings.SHOW_OCEAN) { 
            if (water != null)
            {
                Transform t = water.GetComponent<Transform>();
                t.Translate(new Vector3(0, -level.height/2+ WATER_LEVEL, 0));
                MeshRenderer mr = water.GetComponent<MeshRenderer>();
                if (mr != null)
                {
                    mr.enabled = true;
                }
            }
        }

    }
    GameObject CreateBounds(float x, float y, float width, float height) {
        GameObject bound = GameObject.CreatePrimitive(PrimitiveType.Cube);
        bound.transform.position = new Vector3(x, y, 0);
        bound.transform.localScale += new Vector3(width, height, 0);
        //bound.GetComponent<MeshRenderer>
        return bound;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
