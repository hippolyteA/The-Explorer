using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static UnityEngine.InputSystem.InputAction;

public class Sc_Climber : MonoBehaviour
{
    Rigidbody2D rb;
    Sc_groundCheck groundCheck;
    Sc_Mover mover;
    Sc_Jumper jumper;

    public bool canClimb;
    public bool isClimbing;

    Tween seqVelocity;
    Tween seqMove;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mover = GetComponent<Sc_Mover>();
        jumper = GetComponent<Sc_Jumper>();

        groundCheck = mover.checker.GetComponent<Sc_groundCheck>();

        seqVelocity = DOTween.Sequence();
        seqMove = DOTween.Sequence();

        groundCheck.OnEnterWallEvents.Add(canClimbTrue);
        groundCheck.OnExitWallEvents.Add(canClimbFalse);
        GetComponent<Sc_playerInputHandler>().OnJumpEvents.Add(Action);
    }

    //make a better version?
    void canClimbTrue()
    {
        jumper.getCanJump();
        if (!jumper.canJump && !jumper.startedJumping && groundCheck.isOtherWall)
        {
            canClimb = true;
        }
        else
        {
            canClimb = false;
        }

        //rb.gravityScale = 0.4f;

        //jumper.canJump = false;
    }
    void canClimbFalse()
    {
        canClimb = false;

        //rb.gravityScale = 1f;

        //jumper.canJump = true;
        stopClimbing();      
    }

    void startClimbing()
    {
        isClimbing = true;

        mover.moverState = Sc_Mover.moverType.control;
        jumper.canJump = true;
        
        if (groundCheck.isOtherLadder)
        {
            seqMove = DOTween.Sequence()
                .Append(transform.DOMoveX(groundCheck.LadderPos.x, 0.12f));
            seqVelocity = DOTween.Sequence()
                .Append(DOTween.To(() => rb.velocity, x => rb.velocity = x, Vector2.zero, 0.28f).SetEase(Ease.Linear));
        }
        else
        {
            seqVelocity = DOTween.Sequence()
               .Append(DOTween.To(() => rb.velocity, x => rb.velocity = x, Vector2.zero, 0.8f).SetEase(Ease.Linear));
        }
        
        rb.gravityScale = 0;
        //rb.velocity = Vector2.zero;
    }

    void stopClimbing()
    {
        isClimbing = false;

        if (seqVelocity.IsPlaying()) seqVelocity.Kill();
        if (seqMove.IsPlaying()) seqMove.Kill();
        
        rb.gravityScale = 1f;
        mover.moverState = Sc_Mover.moverType.air;
    }


    float lastContextValue;
    public void Action(CallbackContext context)
    {
        canClimbTrue();
        if (canClimb)
        {
            float gotContextValue = context.ReadValue<float>();
            if (gotContextValue == 1 && gotContextValue != lastContextValue)
            {
                lastContextValue = 1;

                startClimbing();
                StartCoroutine(storeInput()); //?
            }
            else if (gotContextValue == 0 && gotContextValue != lastContextValue)
            {
                lastContextValue = 0;

                if (isClimbing) stopClimbing();
            }
        }
        else
        {
            float gotContextValue = context.ReadValue<float>();
            if (gotContextValue == 1 && gotContextValue != lastContextValue)
            {
                lastContextValue = 1;

                StartCoroutine(storeInput());
            }
            else if (gotContextValue == 0 && gotContextValue != lastContextValue)
            {
                lastContextValue = 0;

                if (isClimbing) stopClimbing();
            }
        }
    }

    IEnumerator storeInput()
    {
        while(lastContextValue == 1) //&& !canClimb)
        {
            yield return new WaitForSeconds(0.02f);
            canClimbTrue();
            if(lastContextValue == 1 && canClimb && !isClimbing)
            {
                startClimbing();
            }
        }
    }
}
