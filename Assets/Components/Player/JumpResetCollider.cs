using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlockGame.Components.Player
{
    public class JumpResetCollider : MonoBehaviour
    {
        [SerializeField] public PlayerMovement playerMovement;

        private void Update ()
        {
            // ONLY BECAUSE IT DOESN'T WORK PROPERLY YET!
            playerMovement.CanJump = true;
        }

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
}