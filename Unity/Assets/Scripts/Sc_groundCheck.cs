using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_groundCheck : MonoBehaviour
{
    public bool isOtherGrounded;
    public bool enterGround;
    public bool exitGround;
    public bool isOtherWall;
    public bool isOtherMarch;
    public bool isOtherLadder;
    //public bool enterWall;
    public bool isColGrounded;
    public bool isColWall;

    [HideInInspector]
    public Vector3 LadderPos;

    public delegate void CheckEvents();
    public List<CheckEvents> OnEnterAnyEvents = new List<CheckEvents>();
    public List<CheckEvents> OnEnterGroundEvents = new List<CheckEvents>();
    public List<CheckEvents> OnEnterWallEvents = new List<CheckEvents>();
    public List<CheckEvents> OnExitGroundEvents = new List<CheckEvents>();
    public List<CheckEvents> OnExitWallEvents = new List<CheckEvents>();

    RaycastHit2D hitWall;
    RaycastHit2D hitMarch;
    RaycastHit2D hitGround;

    private void OnDrawGizmosSelected()
    {
        checkHitGround();
        checkHitWall();

        //ground
        if (hitGround && hitGround.collider.tag == "Ground")
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, Vector3.up * -0.08f);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, Vector3.up * -0.08f);
        }

        //wall
        if (hitWall && hitWall.collider.tag == "Ground")
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, Vector3.right * Mathf.Sign(transform.root.localScale.x) * 0.08f);
        }
        else
        {
             Gizmos.color = Color.green;
             Gizmos.DrawRay(transform.position, Vector3.right * Mathf.Sign(transform.root.localScale.x) * 0.08f);
        }
    }

    void checkHitGround()
    {
        hitGround = Physics2D.Raycast(transform.position, Vector3.up * -1f, 0.08f);
    }
    void checkHitWall()
    {
        hitWall = Physics2D.Raycast(transform.position, Vector3.right * Mathf.Sign(transform.root.localScale.x), 0.08f);
    }
    void checkLegHitWall()
    {
        hitMarch = Physics2D.Raycast(transform.position, Vector3.right * Mathf.Sign(transform.root.localScale.x), 0.08f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Ground" || other.gameObject.tag == "Ground_Walkable")
        {
            Events(OnEnterAnyEvents);

            checkHitGround();
            if (hitGround)
            {
                enterGround = true;

                Events(OnEnterGroundEvents);
                this.StopAllCoroutines();
                StartCoroutine(DelayEnterGround());
            }
            checkHitWall();
            if (hitWall)
            {
                //enterWall = true;

                Events(OnEnterWallEvents);
                //StartCoroutine(DelayEnterWall());
            }
        }
    }
    IEnumerator DelayEnterGround()
    {
        yield return new WaitForSeconds(0.025f);
        enterGround = false;
    }
    IEnumerator DelayExitGround()
    {
        exitGround = true;
        yield return new WaitForSeconds(0.08f);
        exitGround = false;
    }
    //IEnumerator DelayEnterWall()
    //{
    //    yield return new WaitForSeconds(0.025f);
    //    enterWall = false;
    //}
    //public void forceEnterGround()
    //{
    //    Events(OnEnterAnyEvents);

    //    enterGround = true;
    //    Events(OnEnterGroundEvents);
    //    StartCoroutine(DelayEnterGround());

    //    isOtherGrounded = true;
    //}
    //public void ForceExitGround()
    //{

    //}

    void OnTriggerStay2D(Collider2D other)
    {
        switch (other.gameObject.tag)
        {
            case ("Ground"):
                checkHitGround();
                if (hitGround)
                {
                    isOtherGrounded = true;
                }
                checkHitWall();
                if (hitWall)
                {
                    if (!isOtherWall) Events(OnEnterWallEvents);
                    isOtherWall = true;
                    //isOtherMarch = false;
                }
                if (hitMarch)
                {
                    isOtherMarch = true;
                }
                break;

            case ("Ground_Walkable"):
                checkHitGround();
                if (hitGround)
                {
                    isOtherGrounded = true;
                }
                if (hitMarch)
                {
                    isOtherMarch = true;
                }

                break;

            case ("Ladder"):
                if (!isOtherWall) Events(OnEnterWallEvents);
                isOtherWall = true;
                isOtherLadder = true;
                LadderPos = other.transform.position;
                break;
        }
    }
    void OnTriggerExit2D(Collider2D other)
    { 
        switch (other.gameObject.tag)
        {
            case ("Ground"):

                if (isOtherGrounded == true)
                {
                    isOtherGrounded = false;
                    enterGround = false;

                    StartCoroutine(DelayExitGround());
                    Events(OnExitGroundEvents);
                }
                if (isOtherWall == true)
                {
                    isOtherWall = false;

                    Events(OnExitWallEvents);
                }
                if(isOtherMarch == true)
                {
                    isOtherMarch = false;
                }
    
                break;

            case ("Ground_Walkable"):

                if (isOtherGrounded == true)
                {
                    isOtherGrounded = false;
                    enterGround = false;

                    StartCoroutine(DelayExitGround());
                    Events(OnExitGroundEvents);
                }
                if (isOtherMarch == true)
                {
                    isOtherMarch = false;
                }

                break;

            case ("Ladder"):
                isOtherLadder = false;
                isOtherWall = false;
                Events(OnExitWallEvents);

                break;
        }
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.tag == "Ground")
            isColGrounded = true;
        if (col.gameObject.tag == "Wall")
            isColWall = true;
    }
    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.tag == "Ground")
            isColGrounded = false;
        if (col.gameObject.tag == "Wall")
            isColWall = false;
    }



    private void Events(List<CheckEvents> events)
    {
         for (int i = 0; i < events.Count; i++)
         {
             events[i]();
         }      
    }
}

