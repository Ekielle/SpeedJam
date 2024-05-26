using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace CoolDawn
{
    public class FallingPlatform : MonoBehaviour
    {
        public event EventHandler Falling;
        
        [SerializeField] private float fallDelay = 2.0f;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Trigger trigger;

        private void Awake()
        {
            rb.isKinematic = true;
        }

        private void Start()
        {
            trigger.Triggered += Trigger_OnTriggered;
        }
        
        private void Trigger_OnTriggered(object sender, EventArgs e)
        {
            Falling?.Invoke(this, EventArgs.Empty);
            Invoke(nameof(Fall), fallDelay);
        }

        private void Fall()
        {
            rb.isKinematic = false;
        }
        
        
    }
}