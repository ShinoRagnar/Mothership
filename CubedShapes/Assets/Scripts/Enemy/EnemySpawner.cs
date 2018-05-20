using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    private Organizer o;
    bool spawned;

    // Use this for initialization
    void Start () {
        o = Organizer.instance;


    }
    private void Update()
    {
        if (!spawned)
        {
            SpawnEnemy();
        }
    }
    public void SpawnEnemy()
    {
        //GameUnit
        GameUnit enemy = Organizer.ENEMY_SOLDIER_STANDARD.Clone();

        //Body
        Transform enemyBody = Instantiate(o.UNIT_ENEMY_SOLDIER, this.transform);
        enemyBody.position = this.transform.position;
        enemyBody.gameObject.name = enemy.uniqueName;
        ColliderOwner co = enemyBody.gameObject.AddComponent<ColliderOwner>();
        co.owner = enemy;
        enemy.body = enemyBody;

        //Shield
        Transform enemyShield = Instantiate(o.P_FORCE_SHIELD, enemyBody);
        enemyShield.name = enemy.uniqueName + Organizer.NAME_SHIELD;
        enemyShield.position = this.transform.position;
        enemyShield.localScale += new Vector3(1, 1, 1);
        enemyShield.Translate(new Vector3(0, 1, 0));
        ColliderOwner coShield = enemyShield.gameObject.AddComponent<ColliderOwner>();
        coShield.owner = enemy;

        //Layer
        Organizer.SetLayerOfThisAndChildren(Organizer.LAYER_ENEMY, enemyBody.gameObject);
        Organizer.SetLayerOfThisAndChildren(Organizer.LAYER_SHIELDS, enemyShield.gameObject);

        //Bool
        spawned = true;



    }
}
