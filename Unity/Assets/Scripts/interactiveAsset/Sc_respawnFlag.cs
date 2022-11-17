using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class Sc_respawnFlag : MonoBehaviour
{
    /*
    private GameObject player;
    Sc_playerInputHandler playerInputHandler;
    Sc_healthManager playerHealthManager;
    Sc_Mover mover;
    Sc_groundCheck groundCheck;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerInputHandler = player.GetComponent<Sc_playerInputHandler>();
        playerHealthManager = player.GetComponent<Sc_healthManager>();
        mover = player.GetComponent<Sc_Mover>();
        groundCheck = player.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Sc_groundCheck>();
    }
    */

    [System.Serializable]
    public class respawnFlagData
    {
        GameObject player;
        GameObject owner;
        Sc_healthManager playerHealthManager;

        public void ResetValue(GameObject Owner)
        {
            owner = Owner;
            player = GameObject.FindGameObjectWithTag("Player");
            playerHealthManager = player.GetComponent<Sc_healthManager>();
        }

        public void ActiveFlag()
        {
            playerHealthManager.OnDeathEvents.Remove(RespawnAtFlag);
            playerHealthManager.OnDeathEvents.Add(RespawnAtFlag);         
        }
        void RespawnAtFlag()
        {
            player.transform.position = owner.transform.position;
        }
    }


    /*
    public void ResetValue(GameObject Owner)
    {
        owner = Owner;
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealthManager = player.GetComponent<Sc_healthManager>();
    }

    public void ActiveFlag()
    {
        //if (mover.moverState == Sc_Mover.moverType.ground && groundCheck.isOtherGrounded)
        //{
            playerHealthManager.OnDeathEvents.Remove(RespawnAtFlag);
            playerHealthManager.OnDeathEvents.Add(RespawnAtFlag);
        //}
    }
    void RespawnAtFlag()
    {
        player.transform.position = owner.transform.position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Body")
        {
            GameObject character = other.gameObject.transform.parent.gameObject;
            if (character.tag == "Player")
            {
                playerInputHandler.OnInteractEvents.Add(ActionActiveFlag);
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
                playerInputHandler.OnInteractEvents.Remove(ActionActiveFlag);
            }
        }
    }


    private float ActionActiveFlagValue;
    private float lastActionActiveFlagValue;
    public void ActionActiveFlag(CallbackContext context)
    {
        float contextValue = context.ReadValue<float>();

        if (contextValue == 1 && lastActionActiveFlagValue == 0 && this.enabled)
        {
            lastActionActiveFlagValue = 1;
            ActionActiveFlagValue = 1;

            ActiveFlag();
        }
        else if (contextValue == 0 && lastActionActiveFlagValue == 1)
        {
            lastActionActiveFlagValue = 0;
            ActionActiveFlagValue = 0;
        }
    }
    */
}
