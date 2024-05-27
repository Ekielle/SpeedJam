using System;
using UnityEngine;

namespace CoolDawn.Player
{
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] private PlayerController playerController;
        [SerializeField] private Animator animator;
        private static readonly int Dash = Animator.StringToHash("Dash");
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int Crouch = Animator.StringToHash("Crouch");
        private static readonly int Grab = Animator.StringToHash("Grab");

        private void Start()
        {
            playerController.Dashing += Player_Dashing;
            playerController.Crouching += Player_Crouching;
            playerController.Grabbing += Player_Grabbing;
        }

        private void Update()
        {
            animator.SetFloat(Speed, Mathf.Abs(playerController.LastMoveInput));
        }

        private void Player_Dashing(object sender, EventArgs e)
        {
            animator.SetTrigger(Dash);
        }
        
        private void Player_Crouching(object sender, bool isCrouching)
        {
            animator.SetBool(Crouch, isCrouching);
        }

        private void Player_Grabbing(object sender, bool isGrabbing)
        {
            animator.SetBool(Crouch, isGrabbing);
        }
    }
}