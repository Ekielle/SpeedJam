using System;
using UnityEngine;

namespace CoolDawn
{
    public class FallingPlatformVisual : MonoBehaviour
    {
        [SerializeField] private FallingPlatform fallingPlatform;
        [SerializeField] private Animator animator;
        [SerializeField] private ParticleSystem particles;

        private void Awake()
        {
            particles.Stop();
        }

        private void Start()
        {
            fallingPlatform.Falling += FallingPlatform_OnFalling;
            fallingPlatform.StopFalling += FallingPlatform_OnStopFalling;
        }

        private void FallingPlatform_OnStopFalling(object sender, EventArgs e)
        {
            particles.Stop();
        }

        private void FallingPlatform_OnFalling(object sender, EventArgs e)
        {
            particles.Play();
            animator.SetTrigger("Fall");
        }
    }
}