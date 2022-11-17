using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Sc_Munition : MonoBehaviour
{
    public bool isTest;

    public float speed;
    //public float lifeTime;

    [HideInInspector]
    public int damage;

    [HideInInspector]
    public GameObject owner;

    [HideInInspector]
    public Rigidbody2D rb;

    [HideInInspector]
    public enum characterType { Player, Enemy };
    public characterType characterState;

    [System.Serializable]
    public class MunitionEvent : UnityEvent { }
    public MunitionEvent WakeUp;


    // test
    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        if (isTest)
        {
            WakeUp.Invoke();
        }
    }

    private void OnValidate()
    {
        rb = GetComponent<Rigidbody2D>();
    }
}