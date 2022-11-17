using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class Sc_Itemer : MonoBehaviour
{
    public Image chargeBar;
    GameObject chargeBarBack;

    Sc_inventoryManager inventoryManager;
    Sc_Mover mover;
    Sc_Jumper jumper;
    Sc_Neter neter;
    Sc_Jetpacker jetpacker;
    Sc_Shooter shooter;
    Sc_groundCheck groundCheck;

    bool usingItem;
    Sequence chargeSec;

    // Start is called before the first frame update
    void Start()
    {
        chargeBarBack = chargeBar.gameObject.transform.parent.gameObject;
        chargeBarBack.SetActive(false);

        inventoryManager = GetComponent<Sc_inventoryManager>();
        mover = GetComponent<Sc_Mover>();
        jumper = GetComponent<Sc_Jumper>();
        neter = GetComponent<Sc_Neter>();
        jetpacker = GetComponent<Sc_Jetpacker>();
        shooter = GetComponent<Sc_Shooter>();
        groundCheck = mover.checker.GetComponent<Sc_groundCheck>();

        GetComponent<Sc_playerInputHandler>().OnItemAEvents.Add(ActionUseItemA);
        GetComponent<Sc_playerInputHandler>().OnItemBEvents.Add(ActionUseItemB);
    }

    #region items
    public void inItem()
    {
        if (!usingItem && groundCheck.isOtherGrounded && !jetpacker.usingJetpack)
        {
            usingItem = true;

            mover.speed = mover.slowSpeed;
            shooter.enabled = false;
            shooter.WeaponGraphic.SetActive(false);

            Sc_Potion potion = null;
            if (inventoryManager.inventory.consummableSlot[0, 0].Item is Sc_Potion) potion = (Sc_Potion)inventoryManager.inventory.consummableSlot[0, 0].Item;
            if (potion != null) StartCoroutine(usePotion(potion, true));
        }
    }
    public void outItem()
    {
        usingItem = false;

        mover.speed = mover.walkSpeed;
        shooter.enabled = true;

        DOTween.Kill(this);
        chargeSec.Kill();
        chargeBarBack.SetActive(false);
    }
    
    public IEnumerator usePotion(Sc_Potion potion, bool firstDrink)
    {
        chargeBar.fillAmount = 0;
        chargeBarBack.SetActive(true);

        float delay = potion.ConsumeTime;
        if (!firstDrink) delay /= 1.7f;
 
        chargeSec = DOTween.Sequence()
           .Append(chargeBar.DOFillAmount(1, delay).SetEase(Ease.Linear));

        while (chargeSec.IsPlaying())
        {
            if (!groundCheck.isOtherGrounded || jetpacker.usingJetpack)
            {
                outItem();
            }
            yield return new WaitForEndOfFrame();
        }

        chargeBarBack.SetActive(false);

        // fake heal potion for proto
        GetComponent<Sc_healthManager>().takeHeal(potion.heal, false);

        if (usingItem)
        {
            StartCoroutine(usePotion(potion, false));
        }
    }
    
    #endregion items

    #region input
    float lastUseItemAContextValue;
    public void ActionUseItemA(CallbackContext context)
    {
        float gotContextValue = context.ReadValue<float>();
        if (gotContextValue == 1 && gotContextValue != lastUseItemAContextValue)
        {
            lastUseItemAContextValue = 1;

            inItem();
        }
        else if (gotContextValue == 0 && gotContextValue != lastUseItemAContextValue)
        {
            lastUseItemAContextValue = 0;

            outItem();
        }
    }
    float lastUseItemBContextValue;
    public void ActionUseItemB(CallbackContext context)
    {
        float gotContextValue = context.ReadValue<float>();
        if (gotContextValue == 1 && gotContextValue != lastUseItemAContextValue)
        {
            lastUseItemAContextValue = 1;
        }
        else if (gotContextValue == 0 && gotContextValue != lastUseItemAContextValue)
        {
            lastUseItemAContextValue = 0;
        }
    }
    #endregion input
    
}
