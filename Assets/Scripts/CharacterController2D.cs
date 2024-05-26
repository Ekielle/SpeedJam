using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoolDawn
{
    [RequireComponent(typeof(Collider2D))]
    public class CharacterController2D : MonoBehaviour
    {
        public EventHandler<bool> GroundStateChanged;

        [SerializeField, Tooltip("Max speed, in units per second, that the character moves.")]
        private float speed = 5;

        [SerializeField, Tooltip("Acceleration while grounded.")]
        private float groundedAcceleration = 20;

        [SerializeField, Tooltip("Acceleration while in the air.")]
        private float airAcceleration = 10;

        [SerializeField, Tooltip("Deceleration applied when character is grounded and not attempting to move.")]
        private float groundDeceleration = 70;
        
        [SerializeField]
        private float crouchAcceleration = 3;
        
        [SerializeField]
        private float crouchDeceleration = 10;

        [SerializeField, Tooltip("Deceleration applied when character is in the air and not attempting to move.")]
        float airDeceleration = 10;

        [SerializeField, Tooltip("Max height the character will jump regardless of gravity")]
        float jumpHeight = 1;

        [SerializeField, Tooltip("Force applied to the character when dashing.")]
        float dashForce = 5;
        
        [SerializeField] private Collider2D characterCollider;
        [SerializeField] private Collider2D crouchCollider;

        [SerializeField, Tooltip("Position from where we're checking if the character is grounded.")]
        private Transform[] groundChecks;
        
        [SerializeField, Tooltip("Position from where we're checking if the character is grounded and crouched.")]
        private Transform[] crouchGroundChecks;

        [SerializeField, Tooltip("Position from where we're checking if the character touched the ceiling.")]
        private Transform[] ceilingChecks;
        
        [SerializeField]
        private float crouchHeight = 0.2f;
        
        private bool _isGrounded;
        private bool _isCrouched;
        private Vector2 _velocity;
        private float _moveInput;
        private float _lastMoveInput;

        private void Start()
        {
            characterCollider.enabled = true;
            crouchCollider.enabled = false;
        }

        private void FixedUpdate()
        {
            CheckGrounded();
            CheckCeiling();
            ApplyGravity();

            float acceleration;
            float deceleration;
            
            if (_isCrouched)
            {
                acceleration = crouchAcceleration;
                deceleration = crouchDeceleration;
            }
            else if (_isGrounded)
            {
                acceleration = groundedAcceleration;
                deceleration = groundDeceleration;
            }
            else
            {
                acceleration = airAcceleration;
                deceleration = airDeceleration;
            }

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
            Collider2D _collider = _isCrouched ? crouchCollider : characterCollider;
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
                    if (Vector2.Dot(colliderDistance.normal, Vector2.up) > 0.5f)
                        continue;
                    transform.Translate(colliderDistance.pointA - colliderDistance.pointB);
                }
            }

            _moveInput = 0f;
        }

        private void CheckGrounded()
        {
            bool grounded = false;
            
            var checks = _isCrouched ? crouchGroundChecks : groundChecks;
            
            foreach (Transform groundCheck in checks)
            {
                LayerMask mask = LayerMask.GetMask("Terrain");
                RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.05f, mask);
                if (hit)
                {
                    grounded = true;
                }
            }

            if (!_isGrounded && grounded)
            {
                GroundStateChanged?.Invoke(this, true);
            }

            if (_isGrounded && !grounded)
            {
                GroundStateChanged?.Invoke(this, false);
            }

            _isGrounded = grounded;
        }
        
        private void CheckCeiling()
        {
            foreach (Transform ceilingCheck in ceilingChecks)
            {
                LayerMask mask = LayerMask.GetMask("Terrain");
                RaycastHit2D hit = Physics2D.Raycast(ceilingCheck.position, Vector2.up, 0.05f, mask);
                if (hit)
                {
                    _velocity.y = -1;
                }
            }
        }

        private void ApplyGravity()
        {
            if (_isGrounded)
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
            if(_moveInput != 0) _lastMoveInput = motion;
        }

        public void Jump()
        {
            _velocity.y = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y));
        }

        public void Dash(float direction)
        {
            Vector2 dashDirection = direction is 0f ? new Vector2(_lastMoveInput, 0).normalized : new Vector2(direction, 0).normalized;

            _velocity = dashDirection * dashForce;
        }
        
        public void Crouch()
        {
            _isCrouched = true;
            transform.Translate(0, -crouchHeight, 0);
            characterCollider.enabled = false;
            crouchCollider.enabled = true;
        }
        
        public void StopCrouch()
        {
            _isCrouched = false;
            transform.Translate(0, crouchHeight, 0);
            characterCollider.enabled = true;
            crouchCollider.enabled = false;
        }
    }
}