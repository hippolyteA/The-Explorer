using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class Sc_Neter : MonoBehaviour
{
    Vector2 currentVelocity;
    Rigidbody2D rb;

    Sc_Mover mover;
    Sc_Jumper jumper;
    Sc_Jetpacker jetpacker;
    Sc_Shooter shooter;

    public GameObject graphic;
    public GameObject deflecter;

    [HideInInspector]
    public bool canUseNet = true;

    public float durationVulnerability;
    public float durationInvincibility;
    public float multiplyVelocity;
    public float minMagnitude;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mover = GetComponent<Sc_Mover>();
        jumper = GetComponent<Sc_Jumper>();
        jetpacker = GetComponent<Sc_Jetpacker>();
        shooter = GetComponent<Sc_Shooter>();

        deflecter.SetActive(false);

        GetComponent<Sc_playerInputHandler>().OnNetEvents.Add(ActionInput);
    }

    public IEnumerator useNet()
    {
        if (canUseNet)
        {
            canUseNet = false;
            deflecter.SetActive(true);

            GetComponent<Sc_playerInputHandler>().canInput = false;
            mover.enabled = false;
            jetpacker.outJetpack();
            yield return new WaitForEndOfFrame();
            rb.gravityScale = 0;
            currentVelocity = rb.velocity;
            /*
            if (currentVelocity.magnitude < minMagnitude) currentVelocity = currentVelocity.normalized * minMagnitude;
            yield return new WaitForEndOfFrame();
            DOTween.To(() => rb.velocity, x => rb.velocity = x, currentVelocity * multiplyVelocity, duration/1.2f).SetEase(Ease.OutCubic);
            //rb.velocity /= 1.8f;

            graphic.transform.DORotate(new Vector3(graphic.transform.eulerAngles.x, graphic.transform.eulerAngles.y, graphic.transform.eulerAngles.z + 180), duration / 2).SetEase(Ease.Linear)
                .OnComplete(() => graphic.transform.DORotate(new Vector3(graphic.transform.eulerAngles.x, graphic.transform.eulerAngles.y, graphic.transform.eulerAngles.z + 180), duration / 2).SetEase(Ease.Linear));

            //yield return new WaitForSeconds(duration/2);
            yield return new WaitForEndOfFrame();
            //rb.AddForce(currentVelocity/1.8f);

            */
            //
            Vector2 dir = (currentVelocity/(currentVelocity.magnitude*0.65f)) + (Vector2)mover.direction;
            //print(dir);
            //Vector2 currentPos = (Vector2)transform.position;
            yield return new WaitForEndOfFrame();
            //Vector2 dir = ((Vector2)transform.position - currentPos).normalized;

            DOTween.To(() => rb.velocity, x => rb.velocity = x, rb.velocity / 1.3f, 0.08f).SetEase(Ease.OutCubic);
            yield return new WaitForSeconds(0.08f);
            DOTween.To(() => rb.velocity, x => rb.velocity = x, dir * multiplyVelocity, durationInvincibility + durationVulnerability).SetEase(Ease.OutCubic);
            graphic.transform.DORotate(new Vector3(graphic.transform.eulerAngles.x, graphic.transform.eulerAngles.y, graphic.transform.eulerAngles.z + 180), durationInvincibility/2).SetEase(Ease.Linear)
                .OnComplete(() => graphic.transform.DORotate(new Vector3(graphic.transform.eulerAngles.x, graphic.transform.eulerAngles.y, graphic.transform.eulerAngles.z + 180), durationInvincibility/2).SetEase(Ease.Linear));

            yield return new WaitForSeconds(durationInvincibility);
            deflecter.SetActive(false);
            yield return new WaitForSeconds(durationVulnerability);
            //

            //yield return new WaitForSeconds(duration + delai);

            rb.gravityScale = 1;
            GetComponent<Sc_playerInputHandler>().canInput = true;
            lastActionValue = 0;
            mover.enabled = true;
            deflecter.SetActive(false);

            yield return new WaitForSeconds(durationVulnerability + durationInvincibility);

            graphic.GetComponent<SpriteRenderer>().DOColor(Color.black, 0.1f)
                .OnComplete(() => graphic.GetComponent<SpriteRenderer>().DOColor(Color.white, 0.1f));

            canUseNet = true;
        }
    }


    private float lastActionValue;
    private bool isActionTrigger;
    public void ActionInput(CallbackContext context)
    {
        float contextValue = context.ReadValue<float>();

        if (contextValue == 1 && lastActionValue != contextValue)
        {
            lastActionValue = 1;

            StartCoroutine(useNet());
        }
        else if (contextValue == 0 && lastActionValue != contextValue)
        {
            lastActionValue = 0;
        }
    }
}
