using System;
using UnityEngine;

namespace CoolDawn
{
    public class FallingPlatform : MonoBehaviour
    {
        public event EventHandler Falling;
        
        [SerializeField] private float fallDelay = 2.0f;
        [SerializeField] private float respawnDelay = 5.0f;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Trigger trigger;
        [SerializeField] private Collider2D platformCollider;
        
        private bool _isFalling;
        private float _respawnTimer;
        private Vector3 _initialPosition;
        
        private void Awake()
        {
            rb.isKinematic = true;
            _initialPosition = transform.position;
        }

        private void Start()
        {
            trigger.Triggered += Trigger_OnTriggered;
        }

        private void Update()
        {
            if (!_isFalling) return;
            
            _respawnTimer -= Time.deltaTime;
            if (_respawnTimer <= 0)
            {
                Respawn();
            }
        }
        
        private void Respawn()
        {
            rb.isKinematic = true;
            platformCollider.enabled = true;
            _isFalling = false;
            transform.position = _initialPosition;
            rb.velocity = Vector2.zero;
        }

        private void Trigger_OnTriggered(object sender, EventArgs e)
        {
            Falling?.Invoke(this, EventArgs.Empty);
            Invoke(nameof(Fall), fallDelay);
        }

        private void Fall()
        {
            rb.isKinematic = false;
            platformCollider.enabled = false;
            _isFalling = true;
            _respawnTimer = respawnDelay;
        }
        
        
    }
}