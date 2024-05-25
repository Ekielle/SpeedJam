﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoolDawn
{
    [RequireComponent(typeof(Collider2D))]
    public class CharacterController2D : MonoBehaviour
    {
        public bool IsGrounded { get; private set; }
        public EventHandler<bool> GroundStateChanged;

        [SerializeField, Tooltip("Max speed, in units per second, that the character moves.")]
        private float speed = 5;

        [SerializeField, Tooltip("Acceleration while grounded.")]
        private float groundedAcceleration = 20;

        [SerializeField, Tooltip("Acceleration while in the air.")]
        private float airAcceleration = 10;

        [SerializeField, Tooltip("Deceleration applied when character is grounded and not attempting to move.")]
        private float groundDeceleration = 70;

        [SerializeField, Tooltip("Deceleration applied when character is in the air and not attempting to move.")]
        float airDeceleration = 10;

        [SerializeField, Tooltip("Max height the character will jump regardless of gravity")]
        float jumpHeight = 1;

        [SerializeField, Tooltip("Force applied to the character when dashing.")]
        float dashForce = 5;

        [SerializeField, Tooltip("Acceleration applied to the character when dashing.")]
        float dashAcceleration = 30;

        [SerializeField] private Collider2D characterCollider;

        [SerializeField, Tooltip("Position from where we're checking if the character is grounded.")]
        private Transform[] groundChecks;

        private Vector2 _velocity;
        private float _moveInput;

        private void FixedUpdate()
        {
            CheckGrounded();
            ApplyGravity();

            float acceleration = IsGrounded ? groundedAcceleration : airAcceleration;
            float deceleration = IsGrounded ? groundDeceleration : airDeceleration;

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

            foreach (Transform groundCheck in groundChecks)
            {
                LayerMask mask = LayerMask.GetMask("Terrain");
                RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.05f, mask);
                if (hit)
                {
                    grounded = true;
                }
            }

            if (!IsGrounded && grounded)
            {
                GroundStateChanged?.Invoke(this, true);
            }

            if (IsGrounded && !grounded)
            {
                GroundStateChanged?.Invoke(this, false);
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

        public void Dash(Vector2 direction)
        {
            Vector2 dashUpRight = new Vector2(0.5f, 0.5f);
            Vector2 dashUpLeft = new Vector2(-0.5f, 0.5f);
            Vector2 dashLeft = new Vector2(-1, 0);
            Vector2 dashRight = new Vector2(1, 0);
            Vector2 dashDownRight = new Vector2(0.5f, -0.5f);
            Vector2 dashDownLeft = new Vector2(-0.5f, -0.5f);

            bool right = direction.x > 0f;
            bool left = direction.x < -0f;
            bool up = direction.y > 0.4f;
            bool down = direction.y < -0.4f;

            Vector2 dashDirection = Vector2.zero;
            if (up)
            {
                if (right)
                {
                    dashDirection = dashUpRight;
                }
                else if (left)
                {
                    dashDirection = dashUpLeft;
                }
            }
            else if (down)
            {
                if (right)
                {
                    dashDirection = dashDownRight;
                }
                else if (left)
                {
                    dashDirection = dashDownLeft;
                }
            }
            else
            {
                if (right)
                {
                    dashDirection = dashRight;
                }
                else if (left)
                {
                    dashDirection = dashLeft;
                }
            }

            _velocity = dashDirection * dashForce;
        }
    }
}