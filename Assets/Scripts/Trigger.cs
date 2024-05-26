using System;
using UnityEngine;

namespace CoolDawn
{
    [RequireComponent(typeof(Collider2D))]
    public class Trigger : MonoBehaviour
    {
        public EventHandler Triggered;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;
            Triggered?.Invoke(this, EventArgs.Empty);
        }
    }
}