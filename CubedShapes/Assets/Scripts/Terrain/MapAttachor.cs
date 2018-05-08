using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapAttachor : MonoBehaviour {

    // Changeables
    public bool coreVisible;
    public string attachmentName;

    //Statics
    private static string NODE_CLONES           = "xClones";

    //Static Themes
    private static string SCIFI_WALL_YELLOW     = "WALL_YELLOW";
    private static string SCIFI_WALL_TALL_IN    = "WALL_TALL_IN";
    private static string SCIFI_WALL_TALL_OUT   = "WALL_TALL_OUT";
    private static string SCIFI_WALL_SIMPLE     = "WALL_SIMPLE";
    private static string SCIFI_WALL_DOUBLE     = "WALL_DOUBLE";
    private static string SCIFI_WALL_WATER_IN   = "WALL_WATER_IN";
    private static string SCIFI_WALL_WATER_OUT  = "WALL_WATER_OUT";
    private static string SCIFI_WALL_LOW_IN     = "WALL_LOW_IN";
    private static string SCIFI_WALL_LOW_OUT    = "WALL_LOW_OUT";
    private static string SCIFI_WALL_LOW_DOUBLE = "WALL_LOW_DOUBLE";


    //Static Theme Lists
    private static string[] SCIFI_WALL_VARIATIONS = new string[] {
        SCIFI_WALL_YELLOW,
        SCIFI_WALL_TALL_IN,
        SCIFI_WALL_TALL_OUT,
        SCIFI_WALL_SIMPLE,
        SCIFI_WALL_DOUBLE,
        SCIFI_WALL_WATER_IN,
        SCIFI_WALL_WATER_OUT,
        SCIFI_WALL_LOW_IN,
        SCIFI_WALL_LOW_OUT,
        SCIFI_WALL_LOW_DOUBLE
    };

    //Private
    private Level lev;
    private System.Collections.Generic.Dictionary<int,Transform> parts;
    private int partNumber = 0;
    private System.Collections.Generic.Dictionary<Transform, Alignment> alignments;
    private PrefabOrganizor po;

    // Use this for initialization
    void Start () {
        po = PrefabOrganizor.instance;
        lev = Level.instance;

        parts = new System.Collections.Generic.Dictionary<int, Transform>();
        alignments = new System.Collections.Generic.Dictionary<Transform, Alignment>();
        if(attachmentName == "")
        {
            attachmentName = "xAirVent";
        }
        if(attachmentName == "xAirVent")
        {
            AttachAirVent();
        }else if (attachmentName == "xScifiOne")
        {
            AttachScifiOne();
        }
        if (!coreVisible)
        {
            GetComponent<MeshRenderer>().enabled = false;
        }
    }
    private void AttachScifiOne()
    {
        
        int width = lev.rand.Next(4, 11);

        float tileZ = po.P_SFI_GROUND[1].localScale.z;
        float tileX = po.P_SFI_GROUND[1].localScale.x;

        float xStart = transform.position.x - transform.localScale.x / 2;
        float xStop  = transform.position.x + transform.localScale.x / 2;
        float yStart = transform.position.y + transform.localScale.y / 2;
        float zStart = 0;
        float xLength = transform.localScale.x;

        AttachScifiFloor(tileX, tileZ, xStart, xStop, yStart, zStart, width);
        AttachScifiWalls(tileX, tileZ, xStart, xStop, yStart-tileX*1.5f, zStart+ tileZ - (Mathf.Round(width / 2f)) * tileZ, width, xLength);
        

    }
    private void AttachScifiWalls(float tileX, float tileZ, float xStart, float xStop, float yStart, float zStart, int width, float xLength){

        System.Collections.Generic.Dictionary<int, Transform> corners = new System.Collections.Generic.Dictionary<int, Transform>();
        System.Collections.Generic.Dictionary<int, Transform> walls = new System.Collections.Generic.Dictionary<int, Transform>();
        string wallVariation = SCIFI_WALL_LOW_OUT; //SCIFI_WALL_VARIATIONS[lev.rand.Next(SCIFI_WALL_VARIATIONS.Length)];

        if (width > 3)
        {
            // Populate our lists
            if (wallVariation.Equals(SCIFI_WALL_YELLOW)){
                corners.Add(1, po.P_SFI_WALL_CORNER);
                corners.Add(2, po.P_SFI_WALL_CORNER_TWO);
            }else if (wallVariation.Equals(SCIFI_WALL_TALL_IN)){
                corners.Add(1, po.P_SFI_WALL_CORNER_TALL_IN);
            }else if (wallVariation.Equals(SCIFI_WALL_TALL_OUT)){
                corners.Add(1, po.P_SFI_WALL_CORNER_TALL_OUT);
            }else if (wallVariation.Equals(SCIFI_WALL_SIMPLE)){
                corners.Add(1, po.P_SFI_WALL_CORNER_DOUBLE_SIMPLE);
            }else if (wallVariation.Equals(SCIFI_WALL_DOUBLE)){
                corners.Add(1, po.P_SFI_WALL_CORNER_DOUBLE);
            }else if (wallVariation.Equals(SCIFI_WALL_WATER_IN)){
                corners.Add(1, po.P_SFI_WALL_CORNER_WATER_IN);
            }else if (wallVariation.Equals(SCIFI_WALL_WATER_OUT)){
                corners.Add(1, po.P_SFI_WALL_CORNER_WATER_OUT);
            }else if (wallVariation.Equals(SCIFI_WALL_LOW_IN)){
                corners.Add(1, po.P_SFI_WALL_CORNER_LOW_IN);
            }else if (wallVariation.Equals(SCIFI_WALL_LOW_OUT)){
                corners.Add(1, po.P_SFI_WALL_CORNER_LOW_OUT);
            }

            // Set up common alignments
            alignments.Add(corners[1], new Alignment(xStart + tileX, yStart, zStart, 0, 270, 0, -0.90f, -0.90f, -0.90f));
            alignments[corners[1]].AddAlignment(Alignment.AWAY_LEFT, xStart + tileX, yStart, zStart, 0, 0, 0, -0.90f, -0.90f, -0.90f);
            alignments[corners[1]].AddAlignment(Alignment.AWAY_RIGHT, xStart + xLength, yStart, zStart, 0, 90, 0, -0.90f, -0.90f, -0.90f);
            alignments[corners[1]].AddAlignment(Alignment.TOWARDS_RIGHT, xStart + xLength, yStart, zStart, 0, 180, 0, -0.90f, -0.90f, -0.90f);

            // Set up theme specific alignment
            if (wallVariation.Equals(SCIFI_WALL_YELLOW)) {
                alignments.Add(corners[2], alignments[corners[1]].Clone());
                alignments[corners[2]].RotateAllAlignments(0, 180, 0);
            }else if (wallVariation.Equals(SCIFI_WALL_TALL_OUT) || wallVariation.Equals(SCIFI_WALL_WATER_OUT)){
                alignments[corners[1]].MoveAllAlignments(0, 0.5f, 0);
            }else if (wallVariation.Equals(SCIFI_WALL_LOW_IN)){
                alignments[corners[1]].RotateAllAlignments(0, 180, 0);
                alignments[corners[1]].MoveAllAlignments(0, 0.5f, 0);
                alignments[corners[1]].ScaleAllAlignments(-0.8f, -0.8f, -0.8f);
            }else if (wallVariation.Equals(SCIFI_WALL_LOW_OUT)){
                alignments[corners[1]].MoveAllAlignments(-0.25f, 0.3f, -0.25f);
                alignments[corners[1]].ScaleAllAlignments(-0.75f, -0.75f, -0.75f);
                alignments[corners[1]].MoveAlignment(Alignment.AWAY_LEFT, 0, 0, 0.5f);
                alignments[corners[1]].MoveAlignment(Alignment.AWAY_RIGHT, 0.5f, 0, 0.5f);
                alignments[corners[1]].MoveAlignment(Alignment.TOWARDS_RIGHT, 0.5f, 0, 0);
            }else{
                alignments[corners[1]].RotateAllAlignments(0, 180, 0);
                alignments[corners[1]].MoveAllAlignments(0, 0.5f, 0);
            }


            // Attach our prefabs
            Transform curr = corners[lev.rand.Next(1, corners.Count + 1)];
            AttachPart(curr, alignments[curr].SetAlignment(Alignment.ORIGINAL), 0, 0, tileZ / 2);
            curr = corners[lev.rand.Next(1, corners.Count + 1)];
            AttachPart(curr, alignments[curr].SetAlignment(Alignment.AWAY_LEFT), 0, 0, (width - 1) * tileZ - tileZ / 2);
            curr = corners[lev.rand.Next(1, corners.Count + 1)];
            AttachPart(curr, alignments[curr].SetAlignment(Alignment.AWAY_RIGHT), -tileX, 0, (width - 1) * tileZ - tileZ / 2);
            curr = corners[lev.rand.Next(1, corners.Count + 1)];
            AttachPart(curr, alignments[curr].SetAlignment(Alignment.TOWARDS_RIGHT), -tileX, 0, tileZ / 2);
        }
    }

    private System.Collections.Generic.Dictionary<int, Transform> AttachScifiFloor(float tileX, float tileZ, float xStart, float xStop, float yStart, float zStart, int width){
        
        System.Collections.Generic.Dictionary<int, Transform> floorTile = new System.Collections.Generic.Dictionary<int, Transform>();

        for (int i = 1; i < po.P_SFI_GROUND.Length; i++)
        {
            floorTile.Add(i, po.P_SFI_GROUND[i]);
            alignments.Add(floorTile[i], new Alignment(xStart, yStart, zStart, 0, 0, 0, -0.95f, 0, -0.95f));
            alignments[floorTile[i]].AddDegreeVariation(Alignment.NINETY, 0, 90, 0);
            alignments[floorTile[i]].AddDegreeVariation(Alignment.ONEEIGHTY, 0, 180, 0);
            alignments[floorTile[i]].AddDegreeVariation(Alignment.TWOSEVENTY, 0, 270, 0);
        }

        for (float x = 0; x < transform.localScale.x; x += tileX)
        {
            for (int z = 0; z < width; z++)
            {
                Transform floor = floorTile[lev.rand.Next(1, floorTile.Count + 1)];
                float offset = -(Mathf.Round(width / 2f)) * tileZ + z * tileZ;
                if (width > 1)
                {
                    offset += tileZ;
                }
                
                AttachPart(floor, alignments[floor].SetRandomAlignment(lev.rand), tileX / 2 + x * tileX, 0.01f, offset);

            }
        }
        
        return floorTile;
        
    }

    private void AttachAirVent()
    {

        // Used Prefabs
        Transform block = po.P_AIR_BLOCK;
        Transform cap = po.P_AIR_CAP;
        Transform holder = po.P_AIR_HOLDER;
        Transform cable = po.P_AIR_CABLE;

        //Init
        float yPos = transform.position.y;
        float startXPos = transform.position.x - transform.localScale.x / 2;
        float endXPos = transform.position.x + transform.localScale.x / 2;
        //Scale
        float partsScaleY = 0.2f;
        float partsScaleZ = 0.8f;
        float hangerScaleY = 2.0f;
        //Shift
        float middleShiftY = transform.localScale.y / 2;
        float middleShiftZ = block.localScale.z / 2 * (1 + partsScaleZ);
        float middleShiftX = block.localScale.x;
        //Cable
        float cableOffset = middleShiftY * 8;

        // Align parts
        alignments.Add(block, new Alignment(0, -middleShiftY, -middleShiftZ + partsScaleY / 2, 0, 0, 0, 0, partsScaleY, partsScaleZ));
        alignments.Add(cap, new Alignment(0, -middleShiftY, -middleShiftZ + partsScaleY / 2, 0, 270, 0, partsScaleZ, partsScaleY, 0));
        alignments.Add(cable, new Alignment(middleShiftX - partsScaleY / 2 + partsScaleY / 8, cableOffset, -partsScaleY / 8 + partsScaleY / 16, 0, 0, 0, 0, hangerScaleY, partsScaleZ));
        alignments.Add(holder, new Alignment(middleShiftX, -middleShiftY, middleShiftZ - partsScaleY - partsScaleY / 4, 0, 0, 0, 0, partsScaleY, partsScaleZ));
        // Add end alignments
        alignments[cap].AddAlignment(Alignment.END, 0, -middleShiftY, middleShiftZ - partsScaleY / 2, 0, 90, 0, partsScaleZ, partsScaleY, 0);
        alignments[cable].AddAlignment(Alignment.END, -middleShiftX - partsScaleY / 2 + partsScaleY / 8, cableOffset, -partsScaleY / 8 + partsScaleY / 16, 0, 0, 0, 0, hangerScaleY, partsScaleZ);
        alignments[holder].AddAlignment(Alignment.END, -middleShiftX, -middleShiftY, middleShiftZ - partsScaleY - partsScaleY / 4, 0, 0, 0, 0, partsScaleY, partsScaleZ);

        // Attach parts
        float xPos;
        float yCable;
        for (xPos = startXPos; xPos < endXPos; xPos += middleShiftX)
        {
            if (xPos == startXPos)
            {
                AttachPart(cap, alignments[cap], xPos, yPos,0);
                AttachPart(holder, alignments[holder], xPos, yPos,0);
                for (yCable = yPos + middleShiftY;  yCable < lev.getTopY() - cableOffset || yPos+middleShiftY == yCable;  yCable += cable.localScale.y * hangerScaleY)
                {
                    AttachPart(cable, alignments[cable], xPos, yCable,0);
                }
            }
            AttachPart(block, alignments[block], xPos, yPos,0);
        }
        AttachPart(cap, alignments[cap].SetAlignment(Alignment.END), xPos, yPos,0);
        AttachPart(holder, alignments[holder].SetAlignment(Alignment.END), xPos, yPos,0);
        AttachPart(cable, alignments[cable].SetAlignment(Alignment.END), xPos, yPos,0);

        for(yCable = yPos + middleShiftY; yCable < lev.getTopY()- cableOffset || yPos + middleShiftY == yCable; yCable += cable.localScale.y * hangerScaleY)
        {
            AttachPart(cable, alignments[cable].SetAlignment(Alignment.END), xPos, yCable,0);
        }
        
    }



    public Transform AttachPart(Transform part, Alignment align, float xpos, float ypos, float zpos)
    {
        Transform ret = Instantiate(part, new Vector3(xpos+align.x,ypos+align.y, zpos+align.z), Quaternion.identity);
        
        MeshRenderer mr = ret.GetComponent<MeshRenderer>();
        if(mr != null)
        {
            mr.enabled = true;
        }
        MeshCollider mc = ret.GetComponent<MeshCollider>();
        if(mc != null)
        {
            mc.enabled = false;
        }
        if (!((align.rotX == 0) && (align.rotY == 0) && (align.rotZ == 0)))
        {
            ret.Rotate(new Vector3(align.rotX, align.rotY, align.rotZ));
        }
        ret.localScale += new Vector3(align.scaleX, align.scaleY, align.scaleZ);
        parts.Add(partNumber, ret);
        ret.parent = GameObject.Find(NODE_CLONES).transform;
        partNumber++;

        return ret;
    }


	// Update is called once per frame
	void Update () {
		
	}
}
