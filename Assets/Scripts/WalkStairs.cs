using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkStairs : MonoBehaviour {

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("Walk Stair Tag: " + other.gameObject.tag);
        switch (other.gameObject.tag)
        {
            case "Adventurer":
            case "Magician":
            case "Player":
                other.gameObject.GetComponent<Player>().SetWalkingStairState(true);
                break;
            case "Enemy":
                other.gameObject.GetComponent<EnemyMovement>().SetWalkingStairState(true);
                break;
            default:
                break;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        switch (other.gameObject.tag)
        {
            case "Adventurer":
            case "Magician":
            case "Player":
                other.gameObject.GetComponent<Player>().SetWalkingStairState(false);
                break;
            case "Enemy":
                other.gameObject.GetComponent<EnemyMovement>().SetWalkingStairState(false);
                break;
            default:
                break;
        }
    }
}
