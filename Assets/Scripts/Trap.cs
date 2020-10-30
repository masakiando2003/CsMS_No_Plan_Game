using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour {

    [SerializeField] int trapIndex = 0;
    [SerializeField] float fallSpeed = 0.2f;
    [SerializeField] bool trapIsHit = false;

    // Cached component references
    Rigidbody2D myRigidbody;

    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myRigidbody.velocity = new Vector2(0, 0);
    }

    public void StartTrigger(int trapIndex)
    {
        if (this.trapIndex == trapIndex)
        {
            GetComponent<Rigidbody2D>().gravityScale = 1;
            GetComponent<Rigidbody2D>().velocity = new Vector2(myRigidbody.velocity.x, fallSpeed);
        }
    }

    public void IsHit()
    {
        trapIsHit = true;
    }

    public bool GetIsHitState()
    {
        return trapIsHit;
    }
}
