using System;
using UnityEngine;

namespace CoolDawn
{
    public class FallingPlatformVisual : MonoBehaviour
    {
        [SerializeField] private FallingPlatform fallingPlatform;
        [SerializeField] private Animator animator;

        private void Start()
        {
            fallingPlatform.Falling += FallingPlatform_OnFalling;
        }

        private void FallingPlatform_OnFalling(object sender, EventArgs e)
        {
            animator.SetTrigger("Fall");
        }
    }
}