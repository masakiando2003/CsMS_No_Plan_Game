using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraBoundary : MonoBehaviour {

    [SerializeField] bool player2PosResetAfterBattle = false;

    // Keep Player2 in Follow Player Camera Area
    // But Player1 must be alive first
    private void OnTriggerExit2D(Collider2D other)
    {
        if(GameObject.Find("Player1") != null)
        {
            KeepPlayer2InCamera(other);
        }
    }

    private static void KeepPlayer2InCamera(Collider2D other)
    {
        if (other.gameObject.name == "Player2")
        {
            Transform followPlayerCameraBoundaryPos = GameObject.Find("Follow Player Camera Boundary").GetComponent<Transform>().transform;
            Transform player1Pos = GameObject.Find("Player1").GetComponent<Transform>().transform;
            GameObject player2 = GameObject.Find("Player2") as GameObject;
            player2.GetComponent<Transform>().position = new Vector2(followPlayerCameraBoundaryPos.transform.position.x - 7.0f, player2.transform.position.y + 1f);

        }
    }
    
    public void ResetPlayer2PositionAfterBattle()
    {
        Transform followPlayerCameraBoundaryPos = GameObject.Find("Follow Player Camera Boundary").GetComponent<Transform>().transform;
        Transform player1Pos = GameObject.Find("Player1").GetComponent<Transform>().transform;
        GameObject player2 = GameObject.Find("Player2") as GameObject;
        if(player2 != null)
        {
            player2.GetComponent<Transform>().position = new Vector2(player1Pos.transform.position.x - 1.0f, player1Pos.position.y + 1f);
        }
    }
    
    public void SetResetPlayerPosFlag(bool resestFlag)
    {
        player2PosResetAfterBattle = resestFlag;
    }

    public bool GetResetPlayerPosFlag()
    {
        return player2PosResetAfterBattle;
    }
}
