using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using static UnityEngine.InputSystem.InputAction;

public class Sc_Mover : MonoBehaviour
{
    Rigidbody2D rb;
    Sc_Jumper jumper;
    Sc_Jetpacker jetpacker;
    Sc_Looker looker;
    Sc_groundCheck groundCheck;
    [HideInInspector]
    public Vector2 moveDir;
    [HideInInspector]
    public Vector3 direction;

    public GameObject graphic;
    public GameObject checker;
    public CinemachineVirtualCamera camGround;
    public CinemachineVirtualCamera camAir;

    public enum moverType { ground, air, control, jetpack};
    public moverType moverState;

    public float walkSpeed;
    public float slowSpeed;
    public float maxGlobalSpeed;
    [HideInInspector]
    public float speed;

    [HideInInspector]
    public bool canTakeMarch;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jumper = GetComponent<Sc_Jumper>();
        jetpacker = GetComponent<Sc_Jetpacker>();
        looker = GetComponent<Sc_Looker>();
        groundCheck = checker.GetComponent<Sc_groundCheck>();

        GetComponent<Sc_playerInputHandler>().OnRightEvents.Add(RightInput);
        GetComponent<Sc_playerInputHandler>().OnLeftEvents.Add(LeftInput);
        GetComponent<Sc_playerInputHandler>().OnMoveEvents.Add(ActionAnalog);

        speed = walkSpeed;
        canTakeMarch = true;
    }

    private void FixedUpdate()
    {
        if (moveDir.y < -0.4f && moverState != moverType.control && moverState != moverType.jetpack && !jumper.isJumping)
        {
            direction = (((Vector2)transform.right * moveDir.x) + ((Vector2)transform.up * (moveDir.y/1.0f))) * (speed / 100);
        }
        else if(moverState == moverType.control)
        {
            direction = (Vector2)transform.up * (moveDir.y / 1.35f) * (speed / 160);
        }
        else
        {
            direction = ((Vector2)transform.right * moveDir.x) * (speed / 100);
            direction.y = 0;

            if (groundCheck.isOtherWall && groundCheck.isOtherGrounded)
            {
                direction += new Vector3(0, Mathf.Abs(direction.x) * (speed / 3));
            }
        }

        if (moverState != moverType.control)
        {
            if (groundCheck.enterGround)
            {
                moverState = moverType.ground;
            }
            else if (!groundCheck.isOtherGrounded)
            {
                if (jetpacker.usingJetpack)
                {
                    moverState = moverType.jetpack;
                }
                else
                {
                    moverState = moverType.air;
                }
            }
        }

        if (rb.velocity.magnitude < maxGlobalSpeed  && ( direction != Vector3.zero && Mathf.Sign(direction.x) == Mathf.Sign(rb.velocity.x) ) && Mathf.Sign(direction.x) == Mathf.Sign(rb.velocity.x))
        {
            rb.AddForce(direction);
        }
        else
        {
            if(rb.velocity.magnitude > maxGlobalSpeed) rb.velocity /= 1.02f;

            //take march
            if (direction != Vector3.zero && Mathf.Sign(direction.x) != Mathf.Sign(rb.velocity.x) && canTakeMarch)
            {
                rb.velocity = new Vector2(rb.velocity.x / 1.04f, rb.velocity.y);
            }
        }

        switch (moverState)
        {
            case moverType.ground:
                transform.position += direction;

                camGround.Priority = 10;
                camAir.Priority = 1;
                break;

            case moverType.air:
                transform.position += direction / 1f;

                camGround.Priority = 1;
                camAir.Priority = 10;
                break;

            case moverType.control:
                transform.position += direction;

                camGround.Priority = 1;
                camAir.Priority = 10;
                break;

            case moverType.jetpack:
                transform.position += direction / 1f;

                camGround.Priority = 1;
                camAir.Priority = 10;
                break;
        }
        FlipManager();
    }

    void FlipManager()
    {
        if (moveDir.x != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(moveDir.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private int lastContextRightValue = 0;
    private float right;
    public void RightInput(CallbackContext context)
    {
        float contextValue = context.ReadValue<float>();

        if (contextValue == 1 && lastContextRightValue == 0)
        {
            lastContextRightValue = 1;

            right = 1;
            moveDir.x = right + left;
        }
        else if (contextValue == 0 && lastContextRightValue == 1)
        {
            lastContextRightValue = 0;

            right = 0;
            moveDir.x = right + left;
        }
        //animManager.Moving(dir);
    }
    private int lastContextLeftValue = 0;
    private float left;
    public void LeftInput(CallbackContext context)
    {
        float contextValue = context.ReadValue<float>();

        if (contextValue == 1 && lastContextLeftValue == 0)
        {
            lastContextLeftValue = 1;

            left = -1;
            moveDir.x = right + left;
        }
        else if (contextValue == 0 && lastContextLeftValue == 1)
        {
            lastContextLeftValue = 0;

            left = 0;
            moveDir.x = right + left;
        }
        //animManager.Moving(dir);
    }

    private Vector2 ActionAnaValue;
    private bool InputTrigger;
    public void ActionAnalog(CallbackContext context)
    {
        Vector2 contextValue = context.ReadValue<Vector2>();

        if (contextValue.x >= 0.1f)
        {
            moveDir.x = contextValue.x;
        }
        else if (contextValue.x <= -0.1f)
        {
            moveDir.x = contextValue.x;
        }
        else
        {
            moveDir.x = 0;
        }
        if (contextValue.y >= 0.1f)
        {
            moveDir.y = contextValue.y;
        }
        else if (contextValue.y <= -0.1f)
        {
            moveDir.y = contextValue.y;
        }
        else
        {
            moveDir.y = 0;
        }
        //dir = contextValue;

        //LookAT
         if(!looker.isLooking) StartCoroutine(looker.LookAt());

        if ((Mathf.Abs(contextValue.x) + Mathf.Abs(contextValue.y)) >= 0.15f)
        {
            InputTrigger = true;
        }
        else
        {
            InputTrigger = false;
        }

        if (!context.performed)
        {
            //Debug.Log("Has stop.");
        }
        //Debug.Log("value : " + contextValue + "  |  isInputTrigger? : " + InputTrigger);
    }

}
