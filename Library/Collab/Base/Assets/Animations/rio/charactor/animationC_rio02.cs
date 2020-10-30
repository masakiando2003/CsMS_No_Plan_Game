using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationC_rio02 : MonoBehaviour {

    public Animator anim;

    public float attack2Time = 0.45f;

    private Vector3 startLocalScale;
    private Vector3 reverseLocalScale;

    //private bool isAttack = false;

	// Use this for initialization
	void Start () {
        startLocalScale = transform.localScale;
        reverseLocalScale = startLocalScale;
        reverseLocalScale.x *= -1;
    }
	
	// Update is called once per frame
	void Update ()
    {/*
        if (isAttack)
        {
            anim.SetBool("Running", false);
        }
        else */if (Input.GetAxis("Player2Horizontal") < 0)
        {
            anim.SetBool("Running", true);
            //transform.localScale = reverseLocalScale;
        }
        else if (Input.GetAxis("Player2Horizontal") > 0)
        {
            anim.SetBool("Running", true);
            //transform.localScale = startLocalScale;
        }
        else
        {
            anim.SetBool("Running", false);
        }

        if (Input.GetButtonDown("Range Attack"))
        {
            AttackingStart();
        }
    }

    void AttackingStart()
    {
        Debug.Log("AttackStart!");
        anim.SetBool("Attack", true);
        //StartCoroutine(AttackingStop(attack2Time));
    }
    /*
    IEnumerator AttackingStop(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        isAttack = false;
        anim.SetBool("Attack", false);
    }*/
}
