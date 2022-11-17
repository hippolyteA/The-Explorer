using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class Sc_interactiveAsset : MonoBehaviour
{
    private GameObject player;
    Sc_playerInputHandler playerInputHandler;
    //Sc_healthManager playerHealthManager;
    Sc_Mover mover;
    Sc_groundCheck groundCheck;

    public enum interactiveAssetType { EventActivate, EventOffering, EventLoot, RespawnFlag, Merchant, EventSkillTree }
    public interactiveAssetType interactiveAssetState;

    [ShowIf("interactiveAssetState", interactiveAssetType.RespawnFlag)]
    public Sc_respawnFlag.respawnFlagData respawnFlagData = new Sc_respawnFlag.respawnFlagData();
    [ShowIf("interactiveAssetState", interactiveAssetType.EventActivate)]
    public Sc_eventActivate.eventOfferingData eventActivateData = new Sc_eventActivate.eventOfferingData();
    [ShowIf("interactiveAssetState", interactiveAssetType.EventOffering)]
    public Sc_eventOffering.eventOfferingData eventOfferingData = new Sc_eventOffering.eventOfferingData();
    [ShowIf("interactiveAssetState", interactiveAssetType.EventLoot)]
    public Sc_eventLoot.LootData eventLootData = new Sc_eventLoot.LootData();
    [ShowIf("interactiveAssetState", interactiveAssetType.EventLoot)]
    public Sc_eventSkillTree.SkillTreeData eventSkillTreeData = new Sc_eventSkillTree.SkillTreeData();


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerInputHandler = player.GetComponent<Sc_playerInputHandler>();
        //playerHealthManager = player.GetComponent<Sc_healthManager>();
        mover = player.GetComponent<Sc_Mover>();
        groundCheck = player.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Sc_groundCheck>();

        //
        //ResetInteractiveAssetScripts();
        respawnFlagData.ResetValue(gameObject);
        eventActivateData.ResetValue(gameObject);
        eventOfferingData.ResetValue(gameObject);
        eventLootData.ResetValue(gameObject);
        eventSkillTreeData.ResetValue(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Body")
        {
            GameObject character = other.gameObject.transform.parent.gameObject;
            if (character.tag == "Player")
            {
                playerInputHandler.OnInteractEvents.Add(ActionActive);

                switch (interactiveAssetState)
                {
                    case (interactiveAssetType.EventSkillTree):
                        eventSkillTreeData.showSkillTree(true);
                        break;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Body")
        {
            GameObject character = other.gameObject.transform.parent.gameObject;
            if (character.tag == "Player")
            {
                playerInputHandler.OnInteractEvents.Remove(ActionActive);

                switch (interactiveAssetState)
                {
                    case (interactiveAssetType.EventSkillTree):
                        eventSkillTreeData.showSkillTree(false);
                        break;
                }
            }
        }
    }

    void Activate()
    {
        if (mover.moverState == Sc_Mover.moverType.ground && groundCheck.isOtherGrounded)
        {
            switch (interactiveAssetState)
            {
                case (interactiveAssetType.EventActivate):
                    eventActivateData.Activate();
                    break;
                case (interactiveAssetType.RespawnFlag):
                    respawnFlagData.ActiveFlag();
                    break;
                case (interactiveAssetType.EventOffering):
                    eventOfferingData.Offer();
                    break;
                case (interactiveAssetType.EventLoot):
                    StartCoroutine(eventLootData.ActiveLoot());
                    break;
                case (interactiveAssetType.Merchant):
                    break;
                case (interactiveAssetType.EventSkillTree):
                    break;
            }
            Debug.Log("Active " + interactiveAssetState.ToString());
        }
    }

    #region input
    private float ActionActiveValue;
    private float lastActionActiveValue;
    public void ActionActive(CallbackContext context)
    {
        float contextValue = context.ReadValue<float>();

        if (contextValue == 1 && lastActionActiveValue == 0 && this.enabled)
        {
            lastActionActiveValue = 1;
            ActionActiveValue = 1;

            Activate();
        }
        else if (contextValue == 0 && lastActionActiveValue == 1)
        {
            lastActionActiveValue = 0;
            ActionActiveValue = 0;
        }
    }
    #endregion
}
