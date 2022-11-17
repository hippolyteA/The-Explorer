using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Sc_eventOffering : MonoBehaviour
{
    [System.Serializable]
    public class eventOfferingData
    {
        GameObject player;
        GameObject owner;

        Sc_inventoryManager playerInventoryManager;

        public GameObject door;

        public int soulsNeed;

        public void ResetValue(GameObject Owner)
        {
            owner = Owner;
            player = GameObject.FindGameObjectWithTag("Player");
            playerInventoryManager = player.GetComponent<Sc_inventoryManager>();
        }

        public void Offer()
        {
            if (playerInventoryManager.soulRessource >= soulsNeed)
            {
                playerInventoryManager.GainSouls(-soulsNeed);
                Event();
            }
        }

        public void Event()
        {
            door.SetActive(false);
        }
    }
}
