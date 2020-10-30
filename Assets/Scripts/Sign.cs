using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sign : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Input.GetButton("Hint"))
        {
            ShowHint();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        HideHint();
    }

    private void ShowHint()
    {
        Debug.Log("Sign is Hit!!!");
    }

    private void HideHint()
    {
        Debug.Log("Sign is not hit...");
    }
}
