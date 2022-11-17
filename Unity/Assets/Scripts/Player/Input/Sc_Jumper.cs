using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class Sc_Jumper : MonoBehaviour
{
    Rigidbody2D rb;
    Sc_Mover mover;
    Sc_Jetpacker jetpacker;
    Sc_Climber climber;

    Sc_groundCheck groundCheck;

    public float jumpPower;
    public float jumpTime;

    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    public bool canJump = true;
    public bool canJumpLock = false;
    public bool isJumping;
    public bool startedJumping;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mover = GetComponent<Sc_Mover>();
        jetpacker = GetComponent<Sc_Jetpacker>();
        climber = GetComponent<Sc_Climber>();

        groundCheck = mover.checker.GetComponent<Sc_groundCheck>();
        groundCheck.OnEnterGroundEvents.Add(canJumpTrue);
        groundCheck.OnExitGroundEvents.Add(canJumpFalse);


        GetComponent<Sc_playerInputHandler>().OnJumpEvents.Add(Action);
    }

    //make a better version?
    public void getCanJump()
    {
        canJump = ((groundCheck.isOtherGrounded||groundCheck.exitGround) && !jetpacker.usingJetpack && !climber.isClimbing);
    }
    void canJumpTrue()
    {
        canJump = true;
    }
    void canJumpFalse()
    {
        canJump = false;
    }

    IEnumerator Jump()
    {
        getCanJump();

        //if ((groundCheck.isOtherGrounded || groundCheck.isOtherWall) && canJump)
        if(!canJumpLock && canJump && mover.moverState != Sc_Mover.moverType.control)
        {
            isJumping = true;
            startedJumping = true;

            mover.moverState = Sc_Mover.moverType.air;
            rb.velocity = new Vector2(rb.velocity.x, 0);//?
            //rb.AddForce((Vector2)transform.up * (JumpPower + rb.velocity.magnitude*2.6f) * 12);

            float currentJumpTime = jumpTime;
            while(currentJumpTime > 0 && isJumping)
            {
                //if (currentJumpTime > jumpTime / 1.6f)
                //{
                //    rb.AddForce((Vector2)transform.up * jumpPower );
                //}
                //else
                //{
                //    rb.AddForce((Vector2)transform.up * jumpPower * ((currentJumpTime / jumpTime) + 0.5f));
                //}
                //rb.AddForce((Vector2)transform.up * (jumpPower * 1.0f));

                //if (rb.velocity.y < 0)
                //{
                //    rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
                //}
                //else if (rb.velocity.y > 0)
                //{
                //    rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
                //}

                rb.AddForce((Vector2)transform.up * jumpPower);
                rb.AddForce((Vector2)transform.up * jumpPower * (currentJumpTime / jumpTime));

                if (currentJumpTime < jumpTime * 0.92f) startedJumping = false;

                yield return new WaitForSeconds(0.02f);
                currentJumpTime -= 0.02f;
            }
        }
    }

    float lastContextValue;
    public void Action(CallbackContext context)
    {

         float gotContextValue = context.ReadValue<float>();
         if (gotContextValue == 1 && gotContextValue != lastContextValue)
         {
            lastContextValue = 1;

            StartCoroutine(Jump());
         }
         else if (gotContextValue == 0 && gotContextValue != lastContextValue)
         {
            lastContextValue = 0;

            isJumping = false;
         }        
    }
}
