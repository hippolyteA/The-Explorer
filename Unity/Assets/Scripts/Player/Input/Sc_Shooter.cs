using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

public class Sc_Shooter : MonoBehaviour
{
    //public Camera cam;
    public GameObject WeaponGraphic;
    public Image visor;
    public Image munitions;
    public Image delai;
    private Vector2 munSizeDelta;
    public RectTransform use;
    public GameObject bullet;
    Rigidbody2D rb;
    Sc_groundCheck groundCheck;
    Sc_PoolsManager poolsManager;
    //private IEnumerator corRecharge;

    public float rechargeDelay;
    public int maxMunition;
    public int currentMunition;

    public enum weapons { Shotgun, Revolver };
    [HideInInspector]
    public weapons weaponsEquip;
    [System.Serializable]
    public class weaponData
    {
        public Sprite graphic;

        public int shootDamage;
        public float recolPower;
        public int useMunition;
        public bool focusOnGround;
    }
    public weaponData weaponShotGun;
    public weaponData weaponRevolver;

    public weaponData currentWeapon = new weaponData();

    [HideInInspector]
    public Vector2 visorPoint;
    [HideInInspector]
    public Vector3 visorDirection;
    Vector3 spawnPos;

    Transform target;

    private IEnumerator corRecharge;
    private Sequence naviAlpha;

    private void OnDrawGizmosSelected()
    {
        getDirandPos();

        Gizmos.color = Color.blue;
        //Gizmos.DrawCube(dir, Vector3.one * .02f);
        Gizmos.DrawCube(visorDirection + transform.position, Vector3.one * .02f);
        Gizmos.DrawRay(transform.position, visorDirection);

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(spawnPos, .02f);
    }

    private void FixedUpdate()
    {
        getDirandPos();

        target = lookForEnemyWithThickRaycast(spawnPos, visorDirection, 50);
    }

    void getDirandPos()
    {
        visorDirection = VisorValue;
        spawnPos = visorDirection.normalized * 0.25f + transform.position;

        if (visorDirection != Vector3.zero && this.enabled)
        {
            /*
            //visor.rectTransform.position = spawnPos;
            Vector3 naviTarget = target.position + Vector3.up * 0.4f - Vector3.right * 0.36f;
            if(visor.rectTransform.position != naviTarget)
            {
                visor.rectTransform.position = Vector2.MoveTowards(visor.rectTransform.position, naviTarget, 0.15f);

                if(!naviAlpha.IsPlaying())
                {
                    naviAlpha = DOTween.Sequence()
                        .Append(visor.DOFade(1, 0.35f));
                }
            }
            */
            WeaponGraphic.SetActive(true);

            SetRectTransform();
        }
        else
        {
            /*
            //visor.rectTransform.position = spawnPos - Vector3.up * 1000;
            visor.rectTransform.position = Vector2.MoveTowards(visor.rectTransform.position, transform.position, 0.1f);
            if (!naviAlpha.IsPlaying())
            {
                naviAlpha = DOTween.Sequence()
                    .Append(visor.DOFade(0, 0.14f));
            }
            */
            WeaponGraphic.SetActive(false);

            SetRectTransform();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        groundCheck = GetComponent<Sc_Mover>().checker.GetComponent<Sc_groundCheck>();
        poolsManager = GameObject.FindGameObjectWithTag("Pools").GetComponent<Sc_PoolsManager>();

        corRecharge = Recharge();
        naviAlpha = DOTween.Sequence();

        GetComponent<Sc_playerInputHandler>().OnVisorEvents.Add(VisorAnalog);
        GetComponent<Sc_playerInputHandler>().OnUseWeaponEvents.Add(ActionShootShotgun);
        GetComponent<Sc_playerInputHandler>().OnSwitchWeaponEvents.Add(ActionShootRevolver);

        munSizeDelta = use.parent.GetComponent<RectTransform>().sizeDelta;

        visor.DOFade( 0, 0);

        setWeaponData(weapons.Shotgun);
        currentMunition = maxMunition;

    }

    public void Shoot(weapons weapon)
    {
        if(weapon != weaponsEquip) Switch();

        if (visorDirection != Vector3.zero && (currentMunition - currentWeapon.useMunition) >= 0)
        {
            float recolMultiplicateur = 20;
            if (groundCheck.isOtherGrounded && visorDirection.y >= -0.6f)
            {
                if (currentWeapon.focusOnGround)
                {
                    recolMultiplicateur /= 5;
                }
                else recolMultiplicateur /= 2.4f;
            }
            Vector2 dir = (((Vector2)transform.right * visorDirection.x) + ((Vector2)transform.up * visorDirection.y)).normalized * -currentWeapon.recolPower * recolMultiplicateur;
            rb.AddForce(dir);

            if (currentMunition == maxMunition)
            {
                StopCoroutine(corRecharge);
                corRecharge = Recharge();
                StartCoroutine(corRecharge);
            }
            currentMunition -= currentWeapon.useMunition;
            munitions.fillAmount -= currentWeapon.useMunition / (float)maxMunition;

            SetRectTransform();

            Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * visorDirection;
            if (target != null)
            {
                rotatedVectorToTarget += Quaternion.Euler(0, 0, 90) * ((target.position - spawnPos)*2);
            }

            //get and active a bullet
            GameObject bullet = poolsManager.ballToSpawn(Sc_PoolsManager.poolType.Bullet);
            bullet.transform.position = spawnPos;
            bullet.transform.rotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);
            bullet.GetComponent<Sc_Munition>().characterState = Sc_Munition.characterType.Player;
            bullet.GetComponent<Sc_Munition>().damage = currentWeapon.shootDamage;
            bullet.GetComponent<Sc_Munition>().owner = gameObject;
            bullet.GetComponent<Sc_Munition>().WakeUp.Invoke();
        }
    }

    void SetRectTransform()
    {
        if ((currentMunition - currentWeapon.useMunition) >= 0 && visorDirection != Vector3.zero)
        {
            use.gameObject.GetComponent<Image>().fillAmount = 1;

            use.SetRight(munSizeDelta.x * (1 - munitions.fillAmount));
            //use.SetLeft(munSizeDelta.x * (1 - ((float)currentWeapon.useMunition / (float)currentMunition)));
            use.SetLeft((munSizeDelta.x / maxMunition) * (currentMunition - currentWeapon.useMunition));
        }
        else
        {
            use.gameObject.GetComponent<Image>().fillAmount = 0;
        }
    }

    public LayerMask layer;
    public Transform lookForEnemyWithThickRaycast(Vector2 startWorldPos, Vector2 direction, float visibilityThickness)
    {
        if (visibilityThickness == 0) return null; //aim assist disabled

        int[] castOrder = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21 }; //{ 2, 1, 3, 0, 4 };
        int numberOfRays = castOrder.Length;
        const float minDistanceAway = 0.32f; //don't find results closer than this
        const float castDistance = 10f;
        const float flareOutAngle = 65f;

        Transform Target = target;
        if (Target != null)
        {
            if (Vector2.Angle(direction, target.position) > flareOutAngle / 2 || Vector2.Distance(transform.position, target.position) > castDistance) Target = null;
        }

        foreach (int i in castOrder)
        {
            //Vector2 perpDirection = Vector2.Perpendicular(direction);
            //float perpDistance = -visibilityThickness * 0.5f + i * visibilityThickness /(numberOfRays-1);
            //Vector2 startPos = perpDirection * perpDistance + startWorldPos;
            Vector2 startPos = spawnPos;

            float angleOffset = -flareOutAngle * 0.5f + i * flareOutAngle / (numberOfRays - 1);
            //Vector2 flaredDirection = direction.Rotate(angleOffset);
            Vector2 flaredDirection = rotateVector2(direction.normalized, angleOffset);

            RaycastHit2D hit = Physics2D.Raycast(startPos, flaredDirection, castDistance, layer);//, obstaclesEnemyExplosiveMask);
            float castDistanceHit = hit.distance;
            if (castDistanceHit == 0) castDistanceHit = castDistance;
            Debug.DrawRay(startPos, flaredDirection * castDistanceHit, Color.yellow, Time.deltaTime);

            if (hit && hit.collider.tag == "Body")// && isInTargetLayer(hit.collider.gameObject.layer))
            {
                if (hit.collider.transform.parent.tag == "Enemy")
                {
                    //make sure it's in range
                    float distanceAwaySqr = ((Vector2)hit.transform.position - startWorldPos).sqrMagnitude;
                    if (distanceAwaySqr > minDistanceAway * minDistanceAway)
                    {
                        Debug.DrawRay(startPos, flaredDirection * castDistanceHit, Color.red, Time.deltaTime);
                        //look if closest target
                        if (Target != null)
                        {
                            float oldTargetDist = (Target.position - transform.position).magnitude;
                            float newTargetDist = (hit.transform.position - transform.position).magnitude;

                            if (newTargetDist < oldTargetDist) Target = hit.transform;
                        }
                        else
                        {
                            Target = hit.transform;
                        }
                    }
                }
            }
        }

        return Target;
    }
    public Vector2 rotateVector2(Vector2 v, float angle)
    {
        return Quaternion.Euler(0, 0, angle) * v;
    }

    private Sequence secRecharge;
    private Sequence secFill;
    IEnumerator Recharge()
    {
        delai.fillAmount = 0;
        secFill = DOTween.Sequence()
            .Append(delai.DOFillAmount(1, rechargeDelay).SetEase(Ease.Linear));

        yield return new WaitForSeconds(rechargeDelay);
        currentMunition = maxMunition;
        munitions.fillAmount = 1;
        delai.fillAmount = 0;
    }

    public void Switch()
    {
        switch (weaponsEquip)
        {
            case weapons.Shotgun:
                weaponsEquip = weapons.Revolver;
                setWeaponData(weapons.Revolver);
                break;
            case weapons.Revolver:
                weaponsEquip = weapons.Shotgun;
                setWeaponData(weapons.Shotgun);
                break;
        }

        SetRectTransform();
    }

    public void setWeaponData(weapons data)
    {
        switch (data)
        {
            case weapons.Shotgun:
                WeaponGraphic.GetComponent<SpriteRenderer>().sprite = weaponShotGun.graphic;

                currentWeapon.shootDamage = weaponShotGun.shootDamage;
                currentWeapon.useMunition = weaponShotGun.useMunition;
                currentWeapon.recolPower = weaponShotGun.recolPower;
                currentWeapon.focusOnGround = weaponShotGun.focusOnGround;
                break;

            case weapons.Revolver:
                WeaponGraphic.GetComponent<SpriteRenderer>().sprite = weaponRevolver.graphic;

                currentWeapon.shootDamage = weaponRevolver.shootDamage;
                currentWeapon.useMunition = weaponRevolver.useMunition;
                currentWeapon.recolPower = weaponRevolver.recolPower;
                currentWeapon.focusOnGround = weaponRevolver.focusOnGround;
                break;
        }
    }

    #region playerInput
    private Vector3 VisorValue;
    private bool isVisorTrigger;
    public void VisorAnalog(CallbackContext context)
    {
        Vector2 contextValue = context.ReadValue<Vector2>();

        if (contextValue.x >= 0.1f)
        {
            VisorValue.x = contextValue.x;
        }
        else
        {
            //ActionAnaValue.x = 0;
        }
        if (contextValue.x <= -0.1f)
        {
            VisorValue.y = Mathf.Abs(contextValue.x);
        }
        else
        {
            //ActionAnaValue.y = 0;
        }
        VisorValue = contextValue;

        if ((Mathf.Abs(contextValue.x) + Mathf.Abs(contextValue.y)) >= 0.15f)
        {
            isVisorTrigger = true;
        }
        else
        {
            isVisorTrigger = false;
        }

        if (!context.performed)
        {
            //Debug.Log("Has stop.");
        }
        //visorPoint = -(transform.position - cam.ScreenToWorldPoint(contextValue));
        visorPoint = VisorValue;
    }

    private float ActionShootShotgunValue;
    private float lastActionShootShotgunValue;
    private bool isActionShootShotgunTrigger;
    public void ActionShootShotgun(CallbackContext context)
    {
        float contextValue = context.ReadValue<float>();

        if (contextValue == 1 && lastActionShootShotgunValue == 0 && this.enabled)
        {
            lastActionShootShotgunValue = 1;
            ActionShootShotgunValue = 1;

            Shoot(weapons.Shotgun);
        }
        else if (contextValue == 0 && lastActionShootShotgunValue == 1)
        {
            lastActionShootShotgunValue = 0;
            ActionShootShotgunValue = 0;
        }
    }

    private float ActionShootRevolverValue;
    private float lastActionShootRevolverValue;
    private bool isActionShootRevolverTrigger;
    public void ActionShootRevolver(CallbackContext context)
    {
        float contextValue = context.ReadValue<float>();

        if (contextValue == 1 && lastActionShootRevolverValue == 0)
        {
            lastActionShootRevolverValue = 1;
            ActionShootRevolverValue = 1;

            Shoot(weapons.Revolver);
        }
        else if (contextValue == 0 && lastActionShootRevolverValue == 1)
        {
            lastActionShootRevolverValue = 0;
            ActionShootRevolverValue = 0;
        }
    }
    #endregion playerInput
}