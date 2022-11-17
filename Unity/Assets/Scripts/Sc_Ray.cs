using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

//copier hollow knight crystal guardian
public class Sc_Ray : MonoBehaviour
{
    //[HideInInspector]
    public Sc_Munition munition;

    bool canDealDamage;

    private void Start()
    {
        WakeUp();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawRay(transform.position, transform.right * 20);
    }

    public void WakeUp()
    {
        transform.localScale = new Vector3(0.1f, 0.05f, 1f);

        switch (munition.characterState)
        {
            case (Sc_Munition.characterType.Player):
                GetComponent<SpriteRenderer>().color = new Color(.9f, .5f, .1f);
                break;

            case (Sc_Munition.characterType.Enemy):
                GetComponent<SpriteRenderer>().color = new Color(1f, .25f, .05f);
                break;
        }

        canDealDamage = false;

        gameObject.SetActive(true);


        GetComponent<SpriteRenderer>().DOFade(0.3f, 0);

        transform.DOScaleX(100, 5).SetEase(Ease.Linear);
        transform.DOLocalMove(transform.position + transform.right * 250, 5).SetEase(Ease.Linear);

        StartCoroutine(delayForceDealDamage());
    }

    public void aMimir()
    {
        this.StopAllCoroutines();
        DOTween.Kill(transform);
        munition.owner = null;
        gameObject.SetActive(false);
    }

    bool inDelayDealDamage;
    IEnumerator delayDealDamage()
    {
        inDelayDealDamage = true;
        yield return new WaitForSeconds(0.005f);
        DOTween.Kill(transform);

        yield return new WaitForSeconds(0.5f);
        GetComponent<SpriteRenderer>().DOFade(1f, 0);
        transform.DOScaleY(transform.localScale.y * 1.4f, 0.1f);
        yield return new WaitForSeconds(0.1f);

        canDealDamage = true;

        yield return new WaitForSeconds(munition.speed);
        inDelayDealDamage = false;

        aMimir();
    }
    IEnumerator delayForceDealDamage()
    {
        yield return new WaitForSeconds(0.5f);
        if (!inDelayDealDamage) StartCoroutine(delayDealDamage());
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case ("Ground"):
                //aMimir();
                //StartCoroutine(delayDealDamage());
                DOTween.Kill(transform);
                break;

            case ("Ground_Walkable"):
                //aMimir();
                //StartCoroutine(delayDealDamage());
                DOTween.Kill(transform);
                break;
        }
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        switch (other.tag)
        {
            case ("Body"):
                GameObject character = other.gameObject.transform.parent.gameObject;
 
                character.GetComponent<Sc_healthManager>().hitPoint = other.gameObject.GetComponent<Collider2D>().ClosestPoint(transform.position);

                if (character != munition.owner && canDealDamage == true)
                {
                    switch (character.tag)
                    {
                        case ("Player"):
                            character.GetComponent<Sc_healthManager>().takeDamage(munition.damage);
                            //aMimir();
                            break;

                        case ("Enemy"):
                            character.GetComponent<Sc_healthManager>().takeDamage(munition.damage);
                            character.GetComponent<Sc_healthManager>().takeHit();
                            //aMimir();
                            break;

                    }
                }
                break;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        switch (other.tag)
        {
            case ("MainCamera"):
                Vector3 screenPoint = other.gameObject.GetComponent<Camera>().WorldToViewportPoint(transform.position);
                bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < Screen.width && screenPoint.y > 0 && screenPoint.y < Screen.height;
                if (!onScreen)
                {
                    aMimir();
                }

                break;
        }
    }

    #region rotateAround

    public void startCoroutineRotateAround(Transform target, float speed)
    {
        StartCoroutine(rotateAround(target, speed));
    }
    IEnumerator rotateAround(Transform target, float speed)
    {
        StopCoroutine(delayForceDealDamage());
        yield return new WaitForSeconds(0.12f);
        DOTween.Kill(transform);
        StartCoroutine(delayForceDealDamage());

        while (gameObject.activeInHierarchy)
        {
            yield return new WaitForSeconds(0.01f);
            transform.RotateAround(new Vector3(0, 10, 0), new Vector3(0, 0, 10), 0.3f * speed);

        }
    }

    #endregion rotateAround
}
