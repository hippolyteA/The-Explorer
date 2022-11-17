using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Sc_butterflyAI : MonoBehaviour
{
    private GameObject player;

    private AIDestinationSetter destinationSetter;
    private AIPath path;
    private Sc_PoolsManager poolsManager;

    private Vector3 visorDirection;
    //private Vector3 spawnPos;

    public Sc_movementPattern.moveData moveData = new Sc_movementPattern.moveData();

    //public int shootDamage;
    public int soulGiven;

    //[HideInInspector]
    //public bool canShoot;

    private void OnDrawGizmosSelected()
    {
        moveData.movementPattern.DrawPatternGizmo(moveData.targets, transform);

        getDirandPos();

        //Gizmos.color = Color.cyan;
        //Gizmos.DrawSphere(spawnPos, .02f);
    }

    void getDirandPos()
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("Player");

        //visorDirection = (player.transform.position + Vector3.up * 0.15f) - transform.position;
        visorDirection = moveData.movementPattern.getVisorDir(moveData.targets[0], transform);
        //spawnPos = visorDirection.normalized * 0.8f + transform.position;
        //spawnPos = ((player.transform.position + Vector3.up * 0.15f) - transform.position).normalized * 0.8f + transform.position;
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        moveData.resetValues(gameObject);
        moveData.movementPattern.SetupMovement(moveData);

        inCamLimit = moveData.movementPattern.inCamLimit;
        playerDistanceLimit = moveData.movementPattern.playerDistanceLimit;

        destinationSetter = GetComponent<AIDestinationSetter>();
        path = GetComponent<AIPath>();

        poolsManager = GameObject.FindGameObjectWithTag("Pools").GetComponent<Sc_PoolsManager>();

        //GetComponent<Sc_healthManager>().OnHitEvents.Add(OnHit);
        //GetComponent<Sc_healthManager>().OnDeathEvents.Add(OnDeath);

        //anShoot = true;
    }

    float inCamLimit;
    float playerDistanceLimit;

    private void FixedUpdate()
    {
        moveData = moveData.movementPattern.Move(moveData);
    }

    void OnHit()
    {
        //if (movementPattern.PlayerInSightState == Sc_movementPattern.PlayerInSightType.Chase) path.canMove = true;
    }

    void OnDeath()
    {
        //spawn souls
        int ressource = Mathf.CeilToInt((float)soulGiven * (Random.Range(0.95f, 1.05f)));

        while (ressource > 0)
        {
            int soulRessource = Random.Range(1, 10);
            ressource -= soulRessource;
            if (ressource < 0) soulRessource += ressource;

            GameObject goSoul = poolsManager.ballToSpawn(Sc_PoolsManager.poolType.Soul);
            goSoul.transform.position = transform.position;

            Sc_Loot soul = goSoul.GetComponent<Sc_Loot>();
            soul.ressourceGiven = soulRessource;
            soul.moverState = Sc_Loot.moverType.toPlayer;
            soul.iniDirection = Rotate(Random.Range(150, 220) * ((transform.position - moveData.targets[0].transform.position).normalized), Random.Range(Mathf.Deg2Rad * -30, Mathf.Deg2Rad * 30));
            soul.WakeUp();
        }

        gameObject.SetActive(false);
    }
    public static Vector2 Rotate(Vector2 v, float delta)
    {
        return new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
        );
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        switch (col.tag)
        {
            case ("Bullet"):

                break;

            case ("Deflecter"):
                OnDeath();
                break;

        }
    }
}
