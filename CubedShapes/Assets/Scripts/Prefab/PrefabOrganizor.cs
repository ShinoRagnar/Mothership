using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabOrganizor : MonoBehaviour {


    public static PrefabOrganizor instance;

    public Transform P_AIR_BLOCK;
    public Transform P_AIR_CABLE;
    public Transform P_AIR_CAP;
    public Transform P_AIR_HOLDER;

    public Transform P_AIR_CORNER;

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




    //low lowdouble window windoudouble




    public Transform[] P_SFI_GROUND;


    private System.Collections.Generic.Dictionary<string, Transform> prefabs;

    private void Awake()
    {
        instance = this;
    }

}
