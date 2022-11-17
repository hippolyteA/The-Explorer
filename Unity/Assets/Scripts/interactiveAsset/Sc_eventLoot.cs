using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_eventLoot : MonoBehaviour
{
    public enum eventType { Chest, Ressource }
    public enum lootType { Soul, Ingredient }

    [System.Serializable]
    public class LootData
    {
        GameObject player;
        GameObject owner;
        Sc_inventoryManager playerInventoryManager;

        public List<GameObject> fruits = new List<GameObject>();
        private List<Vector3> fruitsIniPositions = new List<Vector3>();

        public eventType lootState;

        [ShowIf("lootState", eventType.Chest)]
        public float soulsGiven;

        [ShowIf("lootState", eventType.Ressource)]
        public lootType ressourceState;
        [ShowIf("lootState", eventType.Ressource)]
        public int maxRessources = 5;
        [ShowIf("lootState", eventType.Ressource), HideInEditorMode, DisableInPlayMode]
        public float currentRessources;
        [ShowIf("lootState", eventType.Ressource), DisableInPlayMode]
        public float respawnDelai = 100;

        public void ResetValue(GameObject Owner)
        {
            owner = Owner;
            player = GameObject.FindGameObjectWithTag("Player");
            playerInventoryManager = player.GetComponent<Sc_inventoryManager>();

            fruitsIniPositions = new List<Vector3>();
            for(int i = 0; i < fruits.Capacity; i++)
            {
                fruitsIniPositions.Add(fruits[i].transform.position);
            }
        }

        public void SetupAsset()
        {
            for(int i = 0; i < maxRessources; i++)
            {

            }
        }

        /*
        IEnumerator delaiRespawnLoot()
        {
            while (currentRessources < maxRessources)
            {
                yield return new WaitForSeconds(respawnDelai);

                for (int i = 0; i <= fruits.Capacity; i++)
                {
                    if (!fruits[i].activeInHierarchy)
                    {
                        fruits[i].transform.position = fruitsIniPositions[i];
                        fruits[i].GetComponent<Sc_Loot>().WakeUp();
                        currentRessources += 1;
                        break;
                    }
                }
            }
        }
        */
        bool indelaiRespawnLoot;
        public IEnumerator ActiveLoot()
        {
            currentRessources = 0;

            foreach (GameObject fruit in fruits)
            {
                fruit.GetComponent<Sc_Loot>().updateMovePattern(Sc_Loot.moverType.toPlayer);
                yield return new WaitForSeconds(Random.Range(0.01f, 0.6f));
            }

            //delaiRespawnLoot()
            if (!indelaiRespawnLoot)
            {
                indelaiRespawnLoot = true;

                while (currentRessources < maxRessources)
                {
                    yield return new WaitForSeconds(respawnDelai * Random.Range(0.8f, 1.2f));

                    for (int i = 0; i < fruits.Capacity; i++)
                    {
                        if (!fruits[i].activeInHierarchy)
                        {
                            fruits[i].transform.position = fruitsIniPositions[i];
                            fruits[i].GetComponent<Sc_Loot>().WakeUp();
                            currentRessources += 1;
                            break;
                        }
                    }
                }

                indelaiRespawnLoot = false;
            }
        }
    }
}
