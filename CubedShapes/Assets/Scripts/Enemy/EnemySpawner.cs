using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour {

    private Organizer o;
    public int unitsToSpawn = 6;
    public float spawnDistance = 1f;

    private bool spawned;

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
                AISquad ai = this.gameObject.AddComponent<AISquad>();

                int reserves = ai.squad.currentFormation.ProjectFormationOn(
                    NavMeshAttachor.generated[hit.collider.transform],
                    hit.point.x,
                    spawnDistance,
                    unitsToSpawn
                    );

                foreach(Vector3 location in ai.squad.currentFormation.placements)
                {
                    ai.squad.currentFormation.Place(location,ai.AddUnit(SpawnEnemy(location)));
                }
                ai.squad.EquipAllMembersWith(o.GUN_STANDARD_RIFLE,HumanBodyBones.RightHand);
                ai.squad.EquipAllMembersWith(o.JETPACK_STANDARD, HumanBodyBones.UpperChest);
                

                Debug.Log("Units placed with: "+reserves+" reserves");

                spawned = true;
            }
        }
    }

    public GameUnit SpawnEnemy(Vector3 location)
    {
        //GameUnit
        GameUnit enemy = Organizer.ENEMY_SOLDIER_STANDARD.Clone();

        //Body
        Transform enemyBody = Instantiate(o.UNIT_ENEMY_SOLDIER, this.transform);
        enemyBody.position = location;
        enemyBody.gameObject.name = enemy.uniqueName;

        //Body components
        enemyBody.gameObject.AddComponent<ColliderOwner>();
        enemy.RegisterBodyAndCompontentsForAgent(enemyBody);

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

        return enemy;
    }
}
