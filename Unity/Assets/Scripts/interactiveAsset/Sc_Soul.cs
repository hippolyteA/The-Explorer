using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Sc_Soul : MonoBehaviour
{
    [HideInInspector]
    public int soulGiven;

    [HideInInspector]
    public Vector2 iniDirection;
    GameObject player;

    public enum moverType { toPlayer, Inactif};
    public moverType moverState;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void WakeUp()
    {
        transform.localScale = Vector3.one * soulGiven / 30;

        gameObject.SetActive(true);

        switch (moverState)
        {
            case (moverType.toPlayer):
                GetComponent<Rigidbody2D>().AddForce(iniDirection);
                StartCoroutine(goToPlayer());
                break;
            case (moverType.Inactif):
                break;
        }
    }
    public void AMimr()
    {
        gameObject.SetActive(false);
    }

    IEnumerator goToPlayer()
    {
        yield return new WaitForSeconds(Random.Range(0.1f, 0.55f));

        float maxDistanceDelta = 0;
        DOTween.To(() => maxDistanceDelta, x => maxDistanceDelta = x, 0.15f, Random.Range(3f, 5.5f)).SetEase(Ease.Linear);

        while (this.enabled)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, maxDistanceDelta);
            yield return new WaitForSeconds(0.01f);
            //yield return new WaitForEndOfFrame();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Body")
        {
            GameObject character = other.gameObject.transform.parent.gameObject;
            if(character.tag == "Player")
            {
                character.GetComponent<Sc_inventoryManager>().GainSouls(soulGiven);
                AMimr();
            }
        }
    }
}
