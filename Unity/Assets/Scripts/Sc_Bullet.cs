using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Bullet : MonoBehaviour
{
    //[HideInInspector]
    public Sc_Munition munition;

    bool canBeDeflect;

    /*
    private void Start()
    {
        Debug.Log("a");
        munition.WakeUp = WakeUp;
    }
    */


    public void WakeUp()
    {
        switch (munition.characterState)
        {
            case (Sc_Munition.characterType.Player):
                GetComponent<SpriteRenderer>().color = new Color(.9f, .5f, .1f);
                break;

            case (Sc_Munition.characterType.Enemy):
                GetComponent<SpriteRenderer>().color = new Color(1f, .25f, .05f);
                break;
        }

        munition.speed = Mathf.Abs(munition.speed);
        canBeDeflect = true;

        gameObject.SetActive(true);

        munition.rb.AddForce(transform.right * munition.speed * 100);

        StartCoroutine(delayStartTrigger());
    }

    public void aMimir()
    {
        StopAllCoroutines();
        munition.owner = null;
        gameObject.SetActive(false);
    }

    IEnumerator delayStartTrigger()
    {
        GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(0.2f);
        GetComponent<Collider2D>().enabled = true;
    }

    IEnumerator delayCanBeDeflectAgain()
    {
        yield return new WaitForSeconds(0.1f);
        canBeDeflect = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case ("Body"):
                GameObject character = other.gameObject.transform.parent.gameObject;

                character.GetComponent<Sc_healthManager>().hitPoint = other.gameObject.GetComponent<Collider2D>().ClosestPoint(transform.position);

                switch (character.tag)
                {
                    case ("Player"):
                        character.GetComponent<Sc_healthManager>().takeDamage(munition.damage);
                        aMimir();
                        break;

                    case ("Enemy"):
                        character.GetComponent<Sc_healthManager>().takeDamage(munition.damage);
                        character.GetComponent<Sc_healthManager>().takeHit();
                        aMimir();
                        break;

                }
                break;

            case ("Bullet"):
                Sc_Munition.characterType colBulletState = other.gameObject.GetComponent<Sc_Bullet>().munition.characterState;
                if(colBulletState != munition.characterState)
                {
                    //!!!Keep it or nah?!!!
                    //aMimir();
                }
                break;

            case ("Deflecter"):
                if (canBeDeflect)
                {
                    canBeDeflect = false;
                    StartCoroutine(delayCanBeDeflectAgain());

                    //transform.Rotate(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + 180), Space.Self);
                    munition.rb.velocity *= -1.4f;
                    if (munition.owner != null) munition.rb.velocity += (Vector2)(munition.owner.transform.position - transform.position).normalized;
                    munition.damage *= 3;
                }
                break;

            case ("Ground"):
                aMimir();
                break;

            case ("Ground_Walkable"):
                aMimir();
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
}
