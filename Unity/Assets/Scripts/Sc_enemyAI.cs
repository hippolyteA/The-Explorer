using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine;

public class Sc_enemyAI : MonoBehaviour
{
    private GameObject player;
    
    private Sc_healthManager healthManager;

    private AIDestinationSetter destinationSetter;
    private AIPath path;
    private Sc_PoolsManager poolsManager;

    private SpriteRenderer eye;

    private Vector3 visorDirection;
    private Vector3 spawnPos;

    [HideInInspector]
    public bool stase;
    [HideInInspector]
    public bool isActive = true;

    public Sc_movementPattern.moveData moveData = new Sc_movementPattern.moveData();
    Sc_movementPattern.moveData iniMoveData;

    public Sc_shootPattern.shootData shootData = new Sc_shootPattern.shootData();

    //public int shootDamage;
    public int soulsGiven;
    public bool randomizeSouls = true;

    [HideInInspector]
    public bool canShoot;

    private void OnDrawGizmosSelected()
    {
        moveData.movementPattern.DrawPatternGizmo(moveData.targets, transform);

        getDirandPos();

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(spawnPos, .02f); 
    }

    void getDirandPos()
    {
        if(player==null) player = GameObject.FindGameObjectWithTag("Player");

        //visorDirection = (player.transform.position + Vector3.up * 0.15f) - transform.position;
        visorDirection = getVisorDirection();
        //spawnPos = visorDirection.normalized * 0.8f + transform.position;
        spawnPos = ((player.transform.position + Vector3.up * 0.0f) - transform.position).normalized * 0.8f + transform.position;
    }
    public Vector3 getVisorDirection()
    {
        return moveData.movementPattern.getVisorDir(moveData.targets[0], transform);
    }

    private IEnumerator Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        healthManager = GetComponent<Sc_healthManager>();

        moveData.movementPattern = Instantiate(moveData.movementPattern);
        moveData.resetValues(gameObject);
        moveData.movementPattern.SetupMovement(moveData);
        iniMoveData = moveData;

        inCamLimit = moveData.movementPattern.inCamLimit;
        playerDistanceLimit = moveData.movementPattern.playerDistanceLimit;

        shootData.resetValues(this.gameObject);

        destinationSetter = GetComponent<AIDestinationSetter>();
        path = GetComponent<AIPath>();

        poolsManager = GameObject.FindGameObjectWithTag("Pools").GetComponent<Sc_PoolsManager>();

        GetComponent<Sc_healthManager>().OnHitEvents.Add(OnHit);
        GetComponent<Sc_healthManager>().OnDeathEvents.Add(OnDeath);

        eye = transform.GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>();

        yield return new WaitForEndOfFrame();

        //wakeUp();
    }

    float inCamLimit;
    float playerDistanceLimit;

    private void FixedUpdate()
    {
        if (!stase)
        {
            moveData = moveData.movementPattern.Move(moveData);
            if (moveData.onScreen && moveData.inDistance)
            {
                if (canShoot) Shoot();
            }
        }
    }

    //
    public void wakeUp()
    {
        //isActive = true;

        //canShoot = true;

        healthManager.HealthReset();

        this.isActive = true;
    }
    public void aMimir()
    {
        //isActive = false;

        //if(healthManager != null) healthManager.HealthReset();

        GetComponent<Rigidbody2D>().gravityScale = 0;
        moveData = iniMoveData;

        //gameObject.GetComponent<Sc_healthManager>().takeDamage(9999);
        //transform.position = moveData.iniPos;
        //GetComponent<Rigidbody2D>().gravityScale = 1;
        //gameObject.SetActive(false);
    }

    public void Stase(bool setStase)
    {
        path = GetComponent<AIPath>();
        eye = transform.GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        healthManager = GetComponent<Sc_healthManager>();

        if (setStase)
        {
            stase = true;
            healthManager.isInvulnerable = true;

            path.enabled = true;
            eye.DOFade(0, 1);
        }
        else
        {
            stase = false;
            healthManager.isInvulnerable = false;
            
            path.enabled = false;
            eye.DOFade(1, 1);
        }
    }

    IEnumerator delayBetweenShoot()
    {
        canShoot = false;
        yield return new WaitForSeconds(1.75f);
        getDirandPos();

        moveData.eye.DOFade(0, 0.2f);
        yield return new WaitForSeconds(0.25f);
        moveData.eye.DOFade(1, 0);

        if (moveData.screenPoint.z > 0 && moveData.screenPoint.x > 0 && moveData.screenPoint.x < inCamLimit * 1.2f && moveData.screenPoint.y > 0 && moveData.screenPoint.y < inCamLimit * 1.2f) Shoot();
        canShoot = true;
    }

    void Shoot()
    {
        if (moveData.onScreen && moveData.inDistance)
        {
            canShoot = false;
            StartCoroutine(shootData.shootPattern.shoot(shootData));
        }
    }

    void OnHit()
    {
        if(moveData.movementPattern.PlayerInSightState == Sc_movementPattern.PlayerInSightType.Chase) path.canMove = true;
        if(stase) canShoot = true;
    }

    void OnDeath()
    {
        //spawn souls
        int ressource = soulsGiven;
        if(randomizeSouls) ressource = Mathf.CeilToInt((float)soulsGiven * (Random.Range(0.95f, 1.05f)));

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


        //DeadState
        this.isActive = false;
        this.StopAllCoroutines();
        canShoot = false;
        gameObject.tag = "Untagged";
        eye.DOFade(0, 0.5f);
        GetComponent<Rigidbody2D>().gravityScale = 1f;
        GetComponent<Rigidbody2D>().AddForce((transform.position - moveData.targets[0].transform.position).normalized * 1000);
        moveData.movementPattern.PlayerInSightState = Sc_movementPattern.PlayerInSightType.Stop;
    }
    public static Vector2 Rotate(Vector2 v, float delta)
    {
        return new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
        );
    }
}
