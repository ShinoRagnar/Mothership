using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Organizer : MonoBehaviour {


    public static Organizer instance;

    // Air Vents
    public Transform P_AIR_BLOCK;
    public Transform P_AIR_CABLE;
    public Transform P_AIR_CAP;
    public Transform P_AIR_HOLDER;
    public Transform P_AIR_CORNER;
    public Transform P_AIR_WALL;

    // Base
    public Transform P_SFI_BASE;
    public Transform P_SFI_BASE_SPLICE;

    // Corners
    public Transform P_SFI_WALL_CORNER;
    public Transform P_SFI_WALL_CORNER_TWO;
    public Transform P_SFI_WALL_CORNER_TALL_OUT;
    public Transform P_SFI_WALL_CORNER_TALL_IN;
    public Transform P_SFI_WALL_CORNER_DOUBLE;
    public Transform P_SFI_WALL_CORNER_DOUBLE_SIMPLE;
    public Transform P_SFI_WALL_CORNER_WATER_IN;
    public Transform P_SFI_WALL_CORNER_WATER_OUT;
    public Transform P_SFI_WALL_CORNER_LOW_IN;
    public Transform P_SFI_WALL_CORNER_LOW_OUT;
    public Transform P_SFI_WALL_CORNER_LOW_DOUBLE;
    // Walls
    public Transform P_SFI_WALL_YELLOW;
    public Transform P_SFI_WALL_TALL_SIMPLE;
    public Transform P_SFI_WALL_TALL_GRID;
    public Transform P_SFI_WALL_DOUBLE;
    public Transform P_SFI_WALL_GRID_DOUBLE;
    public Transform P_SFI_WALL_SIMPLE_DOUBLE;
    public Transform P_SFI_WALL_LOW;
    public Transform P_SFI_WALL_LOW_DOUBLE;
    public Transform P_SFI_WALL_LOW_WINDOW;
    public Transform P_SFI_WALL_LOW_WINDOW_DOUBLE;


    // Rifles
    public Transform[] P_SFI_RIFLES;

    //Muzzles
    public Transform[] E_MUZZLE_FLASHES;


    //low lowdouble window windoudouble
    
    public Gun  GUN_STANDARD_RIFLE;
    public Item MUZZLE_STANDARD_RIFLE;


    public Transform[] P_SFI_GROUND;




    private System.Collections.Generic.Dictionary<string, Transform> prefabs;

    private void Awake()
    {
        instance = this;
        //DestroyImmediate(E_MUZZLE_FLASHES[0].Find("Distortion").gameObject,true);

        instance.MUZZLE_STANDARD_RIFLE = new Item("Muzzle Standard Rifle", E_MUZZLE_FLASHES[0], new Alignment(0.4f, 0, 0.2f, 180, 0, 0, 0, 0, 0));
       
        instance.GUN_STANDARD_RIFLE = new Gun(
            "Standard Rifle",
            P_SFI_RIFLES[0],
            new Alignment(0.0988f, 0, 0.03953f, 33.027f, -96.250f, -94.309f, -0.2f, -0.2f, -0.4f),
            new Vector3(2, 0, 0),
            MUZZLE_STANDARD_RIFLE
            );
    }

}
