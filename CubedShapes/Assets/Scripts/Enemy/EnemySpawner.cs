using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour {

    private Organizer o;
    public int unitsToSpawn = 6;
    public float spawnDistance = 1f;
    bool spawned;

    // Use this for initialization
    void Start () {
        o = Organizer.instance;
    }
    private void Update()
    {
        if (!spawned) { 
            RaycastHit hit = Senses.SeeGroundBelow(this.transform.position);

            if(hit.collider != null)
            {
                UnitFormation formation = new UnitFormation(PlacementStrategy.MiddleAndOut);
                int reserves = formation.ProjectFormationOn(NavMeshAttachor.generated[hit.collider.transform],
                    hit.point.x,
                    spawnDistance,
                    unitsToSpawn
                    );
                /*ArrayList placement = UnitFormation.ProjectFormationOn(
                    PlacementStrategy.MiddleAndOut,
                    NavMeshAttachor.generated[hit.collider.transform],
                    hit.point.x,
                    spawnDistance,
                    unitsToSpawn
                    );*/

                foreach(Vector3 location in formation.placedUnits)
                {
                    SpawnEnemy(location);
                }
                Debug.Log("Units placed with: "+reserves+" reserves");
                //groundBelow = hit.collider.transform;
                //firstSpawnX = hit.point.x;
                spawned = true;
            }
        }
    }

    public void SpawnEnemy(Vector3 location)
    {
        //GameUnit
        GameUnit enemy = Organizer.ENEMY_SOLDIER_STANDARD.Clone();

        //Body
        Transform enemyBody = Instantiate(o.UNIT_ENEMY_SOLDIER, this.transform);
        enemyBody.position = location;
        enemyBody.gameObject.name = enemy.uniqueName;
        ColliderOwner co = enemyBody.gameObject.AddComponent<ColliderOwner>();
        co.owner = enemy;
        enemy.body = enemyBody;

        //Body components
        ItemEquiper ie = enemyBody.gameObject.AddComponent<ItemEquiper>();
        NavMeshAgent nma = enemyBody.gameObject.AddComponent<NavMeshAgent>();
        AIController aic = enemyBody.gameObject.AddComponent<AIController>();
        Character c = enemyBody.gameObject.AddComponent<Character>();

        //Inner variables
        c.navAgent = nma;
        c.itemEquiper = ie;
        ie.equippedCharacter = c;
        ie.equippedUnit = enemy;
        aic.character = c;
        aic.navAgent = nma;
        aic.itemEquiper = ie;


        //Shield
        Transform enemyShield = Instantiate(o.P_FORCE_SHIELD, enemyBody);
        enemyShield.name = enemy.uniqueName + Organizer.NAME_SHIELD;
        enemyShield.position = location;
        enemyShield.localScale += new Vector3(1, 1, 1);
        enemyShield.Translate(new Vector3(0, 1, 0));
        ColliderOwner coShield = enemyShield.gameObject.AddComponent<ColliderOwner>();
        coShield.owner = enemy;

        //Layer
        Organizer.SetLayerOfThisAndChildren(Organizer.LAYER_ENEMY, enemyBody.gameObject);
        Organizer.SetLayerOfThisAndChildren(Organizer.LAYER_SHIELDS, enemyShield.gameObject);
    }
}
