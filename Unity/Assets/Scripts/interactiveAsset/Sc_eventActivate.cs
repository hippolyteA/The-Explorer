using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_eventActivate : MonoBehaviour
{
    [System.Serializable]
    public class eventOfferingData
    {
        GameObject owner;

        public List<Sc_enemyAI> enemiesToActivate = new List<Sc_enemyAI>();

        public void ResetValue(GameObject Owner)
        {
            owner = Owner;

            foreach (Sc_enemyAI enemy in enemiesToActivate)
            {
                enemy.Stase(true);
            }
        }

        public void Activate()
        {
            Event();
        }

        public void Event()
        {
            foreach(Sc_enemyAI enemy in enemiesToActivate)
            {
                enemy.Stase(false);
            }
        }
    }
}
