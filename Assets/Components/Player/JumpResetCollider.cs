using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpResetCollider : MonoBehaviour
{
    [SerializeField] public PlayerMovement playerMovement;

    private void OnTriggerEnter (Collider other)
    {
        if (other.gameObject.layer == 9)
            playerMovement.CanJump = true;
    }

    private void OnTriggerExit (Collider other)
    {
        if (other.gameObject.layer == 9)
            playerMovement.CanJump = false;
    }
}
