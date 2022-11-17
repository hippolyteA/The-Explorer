using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

public class Sc_Jetpacker : MonoBehaviour
{
    Rigidbody2D rb;
    Sc_Mover mover;
    Sc_Climber climber;
    Sc_groundCheck groundCheck;

    public ParticleSystem vfxSmoke;
    public Image fuelBar;

    // all
    public float fuelUse;
    public float maxFuel;
    public float fuelRessource;
    //basic
    public float basicPower;
    //charge
    public float chargePower;
    public float chargeStack;
    public float chargeSpeed;

    public float maxSpeed;

    [HideInInspector]
    public bool usingJetpack;
    Vector2 iniGravity;

    public enum jetpackType {None, Basic, Charge, Drill, Aurore};
    public jetpackType jetpackEquip;
    public jetpackType jetpackA;
    public jetpackType jetpackB;
    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mover = GetComponent<Sc_Mover>();
        groundCheck = mover.checker.GetComponent<Sc_groundCheck>();

        groundCheck.OnEnterAnyEvents.Add(startDelayFuelCharge);
        GetComponent<Sc_playerInputHandler>().OnUseJetpackEvents.Add(ActionUseJetPackMain);
        GetComponent<Sc_playerInputHandler>().OnSwitchJetpackEvents.Add(ActionUseJetpackSecondaryContext);

        jetpackEquip = jetpackType.None;
        fuelRessource = maxFuel;

        iniGravity = Physics2D.gravity;
    }
    Vector3 getDirection()
    {
        Vector3 direction = (((Vector2)transform.right * mover.moveDir.x) + ((Vector2)transform.up * mover.moveDir.y)) * 12;
        if (direction == Vector3.zero) direction = Vector3.up * 12;
        return direction;
    }

    #region action
    public void inJetpack(jetpackType jetpack)
    {
        if (fuelRessource > 0)
        {
            jetpackEquip = jetpack;
            usingJetpack = true;

            if (mover.moveDir.y > -0.1f) rb.velocity /= 3;
            Physics2D.gravity = Vector2.zero;

            switch (jetpackEquip)
            {
                case (jetpackType.Basic):
                    break;
                case (jetpackType.Charge):
                    chargeStack = 0;
                    mover.enabled = false;
                    break;
            }
            StartCoroutine(useJetpack());
        }
    }
    public void outJetpack()
    {
        usingJetpack = false;

        switch (jetpackEquip)
        {
            case (jetpackType.Basic):
                jetpackEquip = jetpackType.None;
                startDelayFuelCharge();

                vfxSmoke.Stop();
                break;
            case (jetpackType.Charge):
                StartCoroutine(chargeJetpack());
                mover.enabled = true;
                break;
        }

        Physics2D.gravity = iniGravity;
    }

    public void switchJetpack()
    {
        if(jetpackEquip == jetpackA)
        {
            jetpackEquip = jetpackB;
        }
        else if(jetpackEquip == jetpackB)
        {
            jetpackEquip = jetpackA;
        }
    }
    #endregion action

    #region pre

    IEnumerator useJetpack()
    {
        if (usingJetpack && fuelRessource > 0)
        {
            switch (jetpackEquip)
            {
                case (jetpackType.Basic):
                    if (rb.velocity.magnitude < maxSpeed || (mover.moveDir.y < -0.2f && rb.velocity.magnitude < maxSpeed * 2.3f))
                    {
                        //push
                        Vector3 direction = getDirection();
                        rb.AddForce(direction * basicPower);

                        //vfx
                        var angle = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;
                        vfxSmoke.gameObject.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                        var emission = vfxSmoke.emission;
                        emission.rateOverTime = 35;
                        vfxSmoke.Play();
                    }
                    else
                    {
                        rb.velocity /= 1.02f;
                    }

                    fuelRessource -= fuelUse*0.7f;
                    break;
                case (jetpackType.Charge):
                    if(rb.velocity.magnitude > maxSpeed)
                    {
                        rb.velocity /= 1.02f;
                    }
                    else if(rb.velocity.magnitude > 0.0f)
                    {
                        rb.velocity /= 1.008f;
                    }
                    chargeStack += chargeSpeed;

                    fuelRessource -= fuelUse*2;
                    break;
            }

            fuelBar.fillAmount = fuelRessource / maxFuel;

            yield return new WaitForSeconds(0.005f);

            StartCoroutine(useJetpack());
        }
        else
        {
            outJetpack();
        }
    }

    void startDelayFuelCharge()
    {
        if(fuelRessource < maxFuel) StartCoroutine(DelayFuelCharge());
    }
    float delayCharge = 0.6f;
    IEnumerator DelayFuelCharge()
    {
        float delai = 0;
        DOTween.To(() => delai, x => delai = x, 1, delayCharge);

        while (delai < 1)
        {
            yield return new WaitForSeconds(0.02f);
            if (!groundCheck.isOtherGrounded || usingJetpack) delai = 1;
        }

        if (groundCheck.isOtherGrounded && !usingJetpack)
        {
            StartCoroutine(FuelCharge());
        }
        else
        {
            if (groundCheck.isOtherGrounded) StartCoroutine(DelayFuelCharge());
        }
    }
    IEnumerator FuelCharge()
    {
        if (!usingJetpack && fuelRessource < maxFuel)
        {
            fuelRessource += fuelUse * 5;

            fuelBar.fillAmount = fuelRessource / maxFuel;

            yield return new WaitForSeconds(0.02f);

            StartCoroutine(FuelCharge());

            if (fuelRessource > maxFuel) fuelRessource = maxFuel;
        }
    }
    #endregion pre

    #region post
    IEnumerator chargeJetpack()
    {
        yield return new WaitForSeconds(0.02f);
        float limit = maxSpeed * 2.3f;
        Vector3 direction = getDirection() * chargePower;
        //velocity
        //if (chargePower <= limit)
        //{
        //    direction *= chargePower;
        //}
        //else
        //{
        //    direction *= limit;
        //}
        if(rb.velocity.magnitude > limit)
        {
            direction = Vector3.zero;
        }
        //push
        rb.AddForce(direction);

        if (!usingJetpack)
        {
            chargeStack -= 2.0f;
            if (chargeStack > 0)
            {
                //vfx
                var angle = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;
                vfxSmoke.gameObject.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                var emission = vfxSmoke.emission;
                emission.rateOverTime = 100;
                vfxSmoke.Play();

                StartCoroutine(chargeJetpack());
            }
            else
            {
                jetpackEquip = jetpackType.None;
                startDelayFuelCharge();

                vfxSmoke.Stop();
            }
        }
    }

    #endregion post

    #region input
    float lastUseJetpackMainContextValue;
    public void ActionUseJetPackMain(CallbackContext context)
    {

        float gotContextValue = context.ReadValue<float>();
        if (gotContextValue == 1 && gotContextValue != lastUseJetpackMainContextValue && jetpackEquip == jetpackType.None)
        {
            lastUseJetpackMainContextValue = 1;

            inJetpack(jetpackA);
        }
        else if (gotContextValue == 0 && gotContextValue != lastUseJetpackMainContextValue)
        {
            lastUseJetpackMainContextValue = 0;

            if(usingJetpack) outJetpack();
        }
    }
    float lastUseJetpackSecondaryContextValue;
    public void ActionUseJetpackSecondaryContext(CallbackContext context)
    {
        float gotContextValue = context.ReadValue<float>();
        if (gotContextValue == 1 && gotContextValue != lastUseJetpackSecondaryContextValue && jetpackEquip == jetpackType.None)
        {
            lastUseJetpackSecondaryContextValue = 1;

            inJetpack(jetpackB);
        }
        else if (gotContextValue == 0 && gotContextValue != lastUseJetpackSecondaryContextValue)
        {
            lastUseJetpackSecondaryContextValue = 0;

            if (usingJetpack) outJetpack();
        }
    }
    #endregion input

}
