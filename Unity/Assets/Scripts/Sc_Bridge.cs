using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

//pas convaincu de la feature pour l'instant
public class Sc_Bridge : MonoBehaviour
{
    Sc_playerInputHandler playerInputHandler;
    Sc_Jumper jumper;
    Sc_Mover mover;
    public Collider2D playerBody;

    Rigidbody2D rb;

    public BoxCollider2D bridgeCol;
    BoxCollider2D bridgeOther;


    private void OnDrawGizmosSelected()
    {
        if (playerBody == null)
        {
            GameObject[] bodys = GameObject.FindGameObjectsWithTag("Body");
            foreach (GameObject go in bodys)
            {
                GameObject character = go.transform.parent.gameObject;
                if (character.tag == "Player")
                {
                    playerBody = go.GetComponent<BoxCollider2D>();
                    break;
                }
            }
        }
        if (bridgeCol == null || bridgeOther == null)
        {
            BoxCollider2D[] cols = gameObject.GetComponents<BoxCollider2D>();
            foreach (BoxCollider2D col in cols)
            {
                if (col.isTrigger) bridgeOther = col;
                else bridgeCol = col;
            }

            bridgeOther.offset = new Vector2(0, -5);
            bridgeOther.size = bridgeCol.size;
        }
    }

    private void Start()
    {
        playerInputHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<Sc_playerInputHandler>();
        jumper = GameObject.FindGameObjectWithTag("Player").GetComponent<Sc_Jumper>();
        mover = GameObject.FindGameObjectWithTag("Player").GetComponent<Sc_Mover>();

        if (playerBody == null)
        {
            GameObject[] bodys = GameObject.FindGameObjectsWithTag("Body");
            foreach (GameObject go in bodys)
            {
                GameObject character = go.transform.parent.gameObject;
                if (character.tag == "Player")
                {
                    playerBody = go.GetComponent<BoxCollider2D>();
                    break;
                }
            }
        }
        if (bridgeCol == null || bridgeOther == null)
        {
            BoxCollider2D[] cols = gameObject.GetComponents<BoxCollider2D>();
            foreach (BoxCollider2D col in cols)
            {
                if (col.isTrigger) bridgeOther = col;
                else bridgeCol = col;
            }

            bridgeOther.offset = new Vector2(0, -5);
            bridgeOther.size = bridgeCol.size;
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            playerInputHandler.OnJumpEvents.Add(ActionPassThrough);
            playerInputHandler.OnMoveEvents.Add(ActionAnalog);

            Vector2 contextValue = playerInputHandler.handlerValues.MoveVector2;

            if (contextValue.y <= -sensibility)
            {
                ActionAnaValue.y = contextValue.y;

                jumper.canJump = false;
                jumper.canJump = true;
                mover.canTakeMarch = false;
            }
            else
            {
                ActionAnaValue.y = 0;

                jumper.canJump = true;
                jumper.canJumpLock = false;
                mover.canTakeMarch = true;
            }

        }
    }
    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            playerInputHandler.OnJumpEvents.Remove(ActionPassThrough);
            playerInputHandler.OnMoveEvents.Remove(ActionAnalog);
            lastActionPassThroughValue = 0;
            jumper.canJump = true;
            jumper.canJumpLock = false;
            mover.canTakeMarch = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Body")
        {
            GameObject character = other.gameObject.transform.parent.gameObject;
            if (character.tag == "Player")
            {
                Physics2D.IgnoreCollision(bridgeCol, other, true);

                jumper.canJumpLock = true;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Body")
        {
            GameObject character = other.gameObject.transform.parent.gameObject;
            if (character.tag == "Player")
            {
                Physics2D.IgnoreCollision(bridgeCol, other, false);

                jumper.canJumpLock = false;
            }
        }
    }


    void PassThroughBridge()
    {
        if (ActionAnaValue.y != 0)
        {
            Physics2D.IgnoreCollision(bridgeCol, playerBody, true);

            jumper.canJumpLock = true;
        }
    }

    private float ActionPassThroughValue;
    private float lastActionPassThroughValue;
    public void ActionPassThrough(CallbackContext context)
    {
        float contextValue = context.ReadValue<float>();
        if (contextValue == 1 && lastActionPassThroughValue == 0 && this.enabled)
        {
            lastActionPassThroughValue = 1;
            ActionPassThroughValue = 1;
            PassThroughBridge();
        }
        else if (contextValue == 0 && lastActionPassThroughValue == 1)
        {
            lastActionPassThroughValue = 0;
            ActionPassThroughValue = 0;
        }
    }

    private Vector2 ActionAnaValue;
    private float sensibility = 0.6f;
    public void ActionAnalog(CallbackContext context)
    {
        Vector2 contextValue = context.ReadValue<Vector2>();

        if (contextValue.y <= -sensibility)
        {
            ActionAnaValue.y = contextValue.y;

            jumper.canJump = false;
            jumper.canJumpLock = true;
        }
        else
        {
            ActionAnaValue.y = 0;

            jumper.canJump = true;
            jumper.canJumpLock = false;
            mover.canTakeMarch = true;
        }


        if (!context.performed)
        {
            //Debug.Log("Has stop.");
        }
        //Debug.Log("value : " + contextValue + "  |  isInputTrigger? : " + InputTrigger);
    }
}
