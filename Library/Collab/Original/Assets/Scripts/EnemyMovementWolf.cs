using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementWolf : MonoBehaviour {

    [SerializeField] float moveSpeed = 2f, defaultMoveSpeed = 2f;
    [SerializeField] float walkStairSpeed = 0f;
    [SerializeField] bool facingRight = true;
    [SerializeField] bool walkingStairs = true;

    Rigidbody2D myRigidbody;
    Animator myAnimator;

    // Use this for initialization
    void Start () {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = this.GetComponentInChildren<Animator>();
        Debug.Log("Animator Name: " + myAnimator.name);
    }
	
	// Update is called once per frame
	void Update () {
        if (IsFacingRight()) {
            myRigidbody.velocity = new Vector2(moveSpeed, walkStairSpeed);
            transform.localScale = new Vector2(1f, 1f);
        }
        else
        {
            myRigidbody.velocity = new Vector2(-moveSpeed, walkStairSpeed);
            transform.localScale = new Vector2(-1f, 1f);
        }

        if(myAnimator != null)
        {
            if (Mathf.Abs(walkStairSpeed) < 0.1)
            {
                myAnimator.SetBool("Walking", true);
            }
            else
            {
                myAnimator.SetBool("Walking", false);
            }
        }
    }

    private bool IsFacingRight()
    {
        return facingRight;
    }

    public void FlipFacingDirection(bool faceRight)
    {
        facingRight = (faceRight == true) ? false : true;
    }

    public bool GetFacingDirection()
    {
        return facingRight;
    }

    public void SetMoveSpeed(float speed)
    {
        this.moveSpeed = speed;
    }

    public void SetWalkingStairState(bool isWalkingStairs)
    {
        walkingStairs = isWalkingStairs;

        if(walkingStairs == true)
        {
            walkStairSpeed = 3f;
        }
        else
        {
            walkStairSpeed = 0f;
        }
    }

    public float GetDefaultMoveSpeed()
    {
        return defaultMoveSpeed;
    }

    public void SetMoveAnimation(bool isMoving)
    {
        //myAnimator.SetBool("Bouncing", isMoving);
    }
}
