using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Organizer : MonoBehaviour {

    //SELF
    public static Organizer instance;
    //Camera
    public static Vector3 CAMERA_DISTANCE = new Vector3(0, 5, -20);

    // ---------------------- STRINGS
    //GameObjects
    public static string NAME_MAIN_CAMERA = "MainCamera";
    public static string NAME_MAIN_FOCUS = "CameraFocus";
    public static string NAME_PLAYER_GAMEOBJECT = "Player";
    //String parts
    public static string NAME_SHIELD = " Shield";
    public static string NAME_BODY = " Body";
    //Layers
    public static string LAYER_SHIELDS = "Shields";
    public static string LAYER_GROUND = "Ground";
    public static string LAYER_PLAYER = "Player";
    public static string LAYER_ENEMY = "Enemy";
    public static string LAYER_NO_INTERACTION = "No Interaction";

    // ---------------------- PREFAB
    //Enemy
    public Transform UNIT_ENEMY_SOLDIER;

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

    //Visor
    //public Transform P_VISOR_STANDARD_ORANGE;



    //Ground Tiles
    public Transform[] P_SFI_GROUND;

    //Shield
    public Transform P_FORCE_SHIELD;

    // Rifles
    public Transform[] P_SFI_RIFLES;


    // ---------------------- EFFECTS
    //Muzzles
    public Transform[] E_MUZZLE_FLASHES;

    //Jet beams
    public Transform[] E_JET_BEAMS;


    // ----------------------- MISC
    //Beams 
    public Item JET_BEAMER_STANDARD_LEFT;
    public Item JET_BEAMER_STANDARD_RIGHT;
    public JetPack JETPACK_STANDARD;

    //Guns
    public Gun  GUN_STANDARD_RIFLE;
    public Item MUZZLE_STANDARD_RIFLE;

    //Visors
    //public Visor STANDARD_SOLDIER_VISOR;


    //Factions
    public static Faction FACTION_PLAYER = new Faction("Player Faction");
    public static Faction FACTION_ENEMY = new Faction("Enemy Faction");

    //Soldiers
    public static Health ENEMY_SOLDIER_STANDARD_HEALTH = new Health(100, 200, 0, 1);
    public static GameUnit ENEMY_SOLDIER_STANDARD = new GameUnit("Standard Enemy Soldier ", FACTION_ENEMY, ENEMY_SOLDIER_STANDARD_HEALTH, 100);

    //Player
    public static Health PLAYER_STANDARD_HEALTH = new Health(1000, 2000, 1, 10);
    public static GameUnit PLAYER_STANDARD_SETUP = new GameUnit("Player", FACTION_PLAYER, PLAYER_STANDARD_HEALTH, 1000);






    private System.Collections.Generic.Dictionary<string, Transform> prefabs;

    public static void SetLayerOfThisAndChildren(string layer, GameObject go)
    {
        go.layer = LayerMask.NameToLayer(layer);
        foreach(Transform child in go.transform)
        {
            SetLayerOfThisAndChildren(layer, child.gameObject);
        }
    }


    private void Awake()
    {
        instance = this;
        //DestroyImmediate(E_MUZZLE_FLASHES[0].Find("Distortion").gameObject,true);
        CreateGameLogicObjects();
        CreateItems();


    }
    private void CreateGameLogicObjects()
    {
       
    }

    private void CreateItems()
    {
        instance.MUZZLE_STANDARD_RIFLE = new Item("Muzzle Standard Rifle", E_MUZZLE_FLASHES[0], new Alignment(0.4f, 0, 0.2f, 180, 0, 0, 0, 0, 0));

        instance.GUN_STANDARD_RIFLE = new Gun(
            "Standard Rifle",
            P_SFI_RIFLES[0],
            new Alignment(0.0988f, 0, 0.03953f, 33.027f, -96.250f, -94.309f, -0.2f, -0.2f, -0.4f),
            new Vector3(2, 0, 0),
            MUZZLE_STANDARD_RIFLE
            );
        instance.JET_BEAMER_STANDARD_LEFT = new Item("Jet Beamer Soldier Standard Left", E_JET_BEAMS[1], new Alignment(-0.1f, 0, -0.2f, 171.32f, -116.13f, -84.89f, -0.6f, -0.6f, -0.4f));
        instance.JET_BEAMER_STANDARD_RIGHT = new Item("Jet Beamer Soldier Standard Right", E_JET_BEAMS[1], new Alignment(0.1f, 0, -0.2f, -5, 121, 11, -0.6f, -0.6f, -0.4f));
        JETPACK_STANDARD = new JetPack("Jetpack Soldier Standard", instance.JET_BEAMER_STANDARD_LEFT, instance.JET_BEAMER_STANDARD_RIGHT);

        //instance.STANDARD_SOLDIER_VISOR = new Visor("Standard Visor Orange", P_VISOR_STANDARD_ORANGE, new Alignment(0, 0, 0, 90, 0, 0, 0, 0, 0));

    }
}
