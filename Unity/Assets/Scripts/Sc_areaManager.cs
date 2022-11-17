using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_areaManager : MonoBehaviour
{
    [HideInInspector]
    public bool isActive;
    [HideInInspector]
    public bool isPlayerInArea;

    [HideInInspector]
    GameObject player;
    [HideInInspector]
    Camera cam;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        aMimir();
    }

    public void WakeUp()
    {
        isActive = true;
        //print("wakeup");

        foreach(Transform tr in transform)
        {
            tr.gameObject.SetActive(true);
        }
    }
    public void aMimir()
    {
        isActive = false;
        //print("amimir");

        foreach (Transform tr in transform)
        {
            if(tr.gameObject.name != "PNJ") tr.gameObject.SetActive(false);
        }
    }

    IEnumerator DelayBeforeAMimir()
    {
        yield return new WaitForSeconds(2);

        if (!isPlayerInArea)
        {
            aMimir();
        }
        else
        {
            Vector3 screenPoint = cam.WorldToViewportPoint(player.transform.position);
            bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < Screen.width*2 && screenPoint.y > 0 && screenPoint.y < Screen.height*2;
            if(!onScreen)
            {
                aMimir();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "MainCamera")
        {
            if (!isActive) WakeUp();


            isPlayerInArea = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "MainCamera")
        {
            if (isActive) StartCoroutine(DelayBeforeAMimir());

            isPlayerInArea = false;
        }
    }
}
