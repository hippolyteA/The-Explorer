using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Cinemachine;
using UnityEngine;

public class Sc_Looker : MonoBehaviour
{

    public float delayDeforeLookAt = 0.8f;
    public float lookAtSensibility = 0.4f;
    public float lookAtSpeed;
    public float lookAtPower;

    //[HideInInspector]
    public bool isLooking;
    private bool lookingBack;

    private float iniOffset;

    //CinemachineComposer comp;//I named this variable comp for "Composer", you can name it however you like. This is the cinemachine component with all the aiming stuff on it
    CinemachineTransposer transposer;

    Sc_Mover mover;

    void Start()
    {
        mover = GetComponent<Sc_Mover>();

        transposer = mover.camGround.GetCinemachineComponent<CinemachineTransposer>();
        iniOffset = transposer.m_FollowOffset.y;
        lookAtPower += iniOffset;

    }

    public IEnumerator LookAt()
    {
        isLooking = true;

        float delay = 0;
        Sequence secDelay = DOTween.Sequence()
            .Append(DOTween.To(() => delay, x => delay = x, 1, delayDeforeLookAt));

        while (lookingBack)
        {
            yield return new WaitForSeconds(0.02f);
        }

        if (!lookingBack)
        {
            while (secDelay.active)
            {
                yield return new WaitForSeconds(0.02f);

                if (Mathf.Abs(mover.moveDir.y) <= lookAtSensibility || Mathf.Abs(mover.moveDir.x) != 0)
                {
                    isLooking = false;
                    secDelay.Kill();
                }
            }
        }
        else
        {
            lookingBack = false;
        }

        if (Mathf.Abs(mover.moveDir.y) >= lookAtSensibility && mover.moverState == Sc_Mover.moverType.ground) mover.enabled = false;
        while (isLooking)
        {
            yield return new WaitForSeconds(0.02f);

            if (Mathf.Abs(mover.moveDir.y) >= lookAtSensibility && mover.moverState == Sc_Mover.moverType.ground)
            {
                if (Mathf.Abs(transposer.m_FollowOffset.y) < lookAtPower)
                {
                    transposer.m_FollowOffset.y = transposer.m_FollowOffset.y + (mover.moveDir.y * lookAtSpeed / 100);
                }
            }
            else
            {
                isLooking = false;

                lookingBack = true;

                mover.enabled = true;

                //float sign = -Mathf.Sign(comp.m_TrackedObjectOffset.y - iniOffset);
                //while(!isLooking && Mathf.Abs(comp.m_TrackedObjectOffset.y - iniOffset) > 0.2f)
                //{
                //    yield return new WaitForSeconds(0.02f);
                //    comp.m_TrackedObjectOffset.y += 0.15f * sign;
                //}
                //lookingBack = false;
                //if(!isLooking) comp.m_TrackedObjectOffset.y = iniOffset;

                DOTween.To(() => transposer.m_FollowOffset.y, x => transposer.m_FollowOffset.y = x, iniOffset, 0.4f);
                yield return new WaitForSeconds(0.4f);
                lookingBack = false;

                //comp.m_TrackedObjectOffset.y = iniOffset;
            }
        }
        if(mover.moverState == Sc_Mover.moverType.ground) mover.enabled = true;
    }
}
