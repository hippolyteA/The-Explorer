using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Range : MonoBehaviour
{
    [HideInInspector]
    bool isActive;
    [HideInInspector]
    GameObject AI;
    Sc_enemyAI enemyAI;

    private void Start()
    {
        AI = transform.parent.GetChild(0).gameObject;
        enemyAI = AI.GetComponent<Sc_enemyAI>();

        aMimir();
    }

    public void WakeUp()
    {
        if (enemyAI.isActive)
        {
            isActive = true;
            //print("wakeup");

            AI.SetActive(true);
            enemyAI.canShoot = true;
            //enemyAI.wakeUp();
        }
    }
    public void aMimir()
    {
        isActive = false;
        //print("amimir");

        //enemyAI.aMimir();
        AI.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "MainCamera")
        {
            if (!isActive) WakeUp();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "MainCamera")
        {
            Vector3 screenPoint = collision.gameObject.GetComponent<Camera>().WorldToViewportPoint(transform.position);
            bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < Screen.width && screenPoint.y > 0 && screenPoint.y < Screen.height;

            if (isActive && !onScreen) aMimir();
        }
    }
}
