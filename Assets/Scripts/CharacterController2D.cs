using System.Collections.Generic;
using UnityEngine;

namespace CoolDawn
{
    [RequireComponent(typeof(Collider2D))]
    public class CharacterController2D : MonoBehaviour
    {
        public bool IsGrounded { get; private set; }
        
        [SerializeField, Tooltip("Max speed, in units per second, that the character moves.")]
        float speed = 9;

        [SerializeField, Tooltip("Acceleration while grounded.")]
        float walkAcceleration = 75;

        [SerializeField, Tooltip("Acceleration while in the air.")]
        float airAcceleration = 30;

        [SerializeField, Tooltip("Deceleration applied when character is grounded and not attempting to move.")]
        float groundDeceleration = 70;

        [SerializeField, Tooltip("Max height the character will jump regardless of gravity")]
        float jumpHeight = 4;

        private Collider2D _collider;

        private Vector2 _velocity;
        private float _moveInput;
        
        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
        }

        private void FixedUpdate()
        {
            ApplyGravity();

            float acceleration = IsGrounded ? walkAcceleration : airAcceleration;
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

            IsGrounded = false;

            // Retrieve all colliders we have intersected after velocity has been applied.
            var hits = new List<Collider2D>();
            Physics2D.OverlapCollider(_collider, new ContactFilter2D(), hits);

            foreach (Collider2D hit in hits)
            {
                // Ignore our own collider.
                if (hit == _collider)
                    continue;

                ColliderDistance2D colliderDistance = hit.Distance(_collider);

                // Ensure that we are still overlapping this collider.
                // The overlap may no longer exist due to another intersected collider
                // pushing us out of this one.
                if (colliderDistance.isOverlapped)
                {
                    bool grounded = false;
                    // If we intersect an object beneath us, set grounded to true. 
                    if (Vector2.Angle(colliderDistance.normal, Vector2.up) < 90 && _velocity.y < 0)
                    {
                        grounded = true;
                    }
                    else
                    {
                        transform.Translate(colliderDistance.pointA - colliderDistance.pointB);
                    }
                    IsGrounded |= grounded;
                }
            }
            _moveInput = 0f;
        }
        
        private void ApplyGravity()
        {
            if (IsGrounded)
            {
                _velocity.y = 0;
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

        /// <summary>
        /// Set to true when the character intersects a collider beneath
        /// them in the previous frame.
        /// </summary>
    }
}