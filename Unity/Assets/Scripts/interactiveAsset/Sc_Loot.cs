using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Sc_Loot : MonoBehaviour
{
    [HideInInspector]
    public int ressourceGiven = 1;

    [HideInInspector]
    public Vector2 iniDirection;
    GameObject player;

    public enum moverType { toPlayer, Inactif, Stase }
    public moverType moverState;
    moverType iniMoverState;

    public enum lootType { Soul, Ingredient, Consummable }
    public lootType lootState;

    [HideIf("lootState", lootType.Soul)]
    public Sc_Item itemToLoot;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        iniMoverState = moverState;
    }

    public void WakeUp()
    {
        moverState = iniMoverState; ;

        if(lootState == lootType.Soul) transform.localScale = Vector3.one * ressourceGiven / 30;

        gameObject.SetActive(true);

        updateMovePattern(moverState);
    }
    public void AMimr()
    {
        gameObject.SetActive(false);
    }

    public void updateMovePattern(moverType newMoverState)
    {
        moverState = newMoverState;

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
        if (moverState != moverType.Stase)
        {
            if (other.tag == "Body")
            {
                GameObject character = other.gameObject.transform.parent.gameObject;
                if (character.tag == "Player")
                {
                    switch (lootState)
                    {
                        case (lootType.Soul):
                            character.GetComponent<Sc_inventoryManager>().GainSouls(ressourceGiven);
                            AMimr();
                            break;
                        case (lootType.Ingredient):
                            character.GetComponent<Sc_inventoryManager>().AddItem(itemToLoot, 1);
                            AMimr();
                            break;
                    }
                }
            }
        }
    }
}
