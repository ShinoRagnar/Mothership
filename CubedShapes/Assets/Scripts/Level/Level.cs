using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour {

    public static Level instance;

    public System.Random rand;
    public float width;
    public float height;
    public float xOffset;

    private void Awake()
    {
        rand = new System.Random();
        instance = this;
    }

    // Use this for initialization
    void Start () {
		if(width == 0) { width = 50; };
        if(height == 0) { height = 50; };
        
	}
    public float getLeftX()
    {
        return xOffset;
    }
    public float getTopY()
    {
        return height / 2;
    }
    public float getBottomY()
    {
        return -height / 2;
    }
    public float getRightX()
    {
        return width + xOffset;
    }
    public float getTopAndBottomX()
    {
        return width / 2 + xOffset;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
