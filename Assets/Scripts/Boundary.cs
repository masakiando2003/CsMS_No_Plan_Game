using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundary : MonoBehaviour {
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            bool facingDirection = other.gameObject.GetComponent<EnemyMovement>().GetFacingDirection();
            other.gameObject.GetComponent<EnemyMovement>().FlipFacingDirection(facingDirection);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            bool facingDirection = other.gameObject.GetComponent<EnemyMovement>().GetFacingDirection();
            other.gameObject.GetComponent<EnemyMovement>().FlipFacingDirection(facingDirection);
        }
    }
}
