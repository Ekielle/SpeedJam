using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace CoolDawn
{
    [RequireComponent(typeof(Collider2D))]
    public class CharacterController2D : MonoBehaviour
    {
        public bool IsGrounded { get; private set; }
        
        [SerializeField, Tooltip("Max speed, in units per second, that the character moves.")]
        private float speed = 5;

        [SerializeField, Tooltip("Acceleration while grounded.")]
        private float groundedAcceleration = 30;

        [SerializeField, Tooltip("Acceleration while in the air.")]
        private float airAcceleration = 30;

        [SerializeField, Tooltip("Deceleration applied when character is grounded and not attempting to move.")]
        private float groundDeceleration = 100;

        [SerializeField, Tooltip("Deceleration applied when character is in the air and not attempting to move.")]
        float airDeceleration = 100;
        
        [SerializeField, Tooltip("Max height the character will jump regardless of gravity")]
        float jumpHeight = 4;

        [SerializeField]
        private Collider2D characterCollider;

        [SerializeField, Tooltip("Position from where we're checking if the character is grounded.")]
        private Transform[] groundChecks;
        
        private Vector2 _velocity;
        private float _moveInput;
        
        private void Awake()
        {
            characterCollider.enabled = false;
        }

        private void FixedUpdate()
        {
            CheckGrounded();
            ApplyGravity();

            float acceleration = IsGrounded ? groundedAcceleration : airAcceleration;
            float deceleration = IsGrounded ? groundDeceleration : 0;

            if (_moveInput != 0)
            {
                _velocity.x = Mathf.MoveTowards(_velocity.x, speed * _moveInput, acceleration * Time.fixedDeltaTime);
            }
            else
            {
                _velocity.x = Mathf.MoveTowards(_velocity.x, 0, deceleration * Time.fixedDeltaTime);
            }


            transform.Translate(_velocity * Time.fixedDeltaTime);

            // Retrieve all colliders we have intersected after velocity has been applied.
            var hits = new List<Collider2D>();
            Physics2D.OverlapCollider(characterCollider, new ContactFilter2D(), hits);

            foreach (Collider2D hit in hits)
            {
                // Ignore our own collider.
                if (hit == characterCollider)
                    continue;

                ColliderDistance2D colliderDistance = hit.Distance(characterCollider);

                // Ensure that we are still overlapping this collider.
                // The overlap may no longer exist due to another intersected collider
                // pushing us out of this one.
                if (colliderDistance.isOverlapped)
                {
                    transform.Translate(colliderDistance.pointA - colliderDistance.pointB);
                }
            }
            _moveInput = 0f;
        }

        private void CheckGrounded()
        {
            bool grounded = false;
            
            foreach(Transform groundCheck in groundChecks)
            {
                RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.05f);
                if (hit)
                {
                    grounded = true;
                }
            }
            
            IsGrounded = grounded;
        }
        
        private void ApplyGravity()
        {
            if (IsGrounded)
            {
                _velocity.y = Mathf.Max(0, _velocity.y);
            }
            else
            {
                _velocity.y += Physics2D.gravity.y * Time.fixedDeltaTime;
            }
        }

        public void Move(float motion)
        {
            _moveInput = motion;
        }

        public void Jump()
        {
            _velocity.y = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y));
        }
    }
}