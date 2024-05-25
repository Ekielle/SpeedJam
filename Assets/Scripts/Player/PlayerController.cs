using System;
using UnityEngine;

namespace CoolDawn.Player
{
    public class PlayerController : MonoBehaviour
    {
        private const float MovementSpeed = 5.0f;
        private const float DashCooldown = 1.0f;
        [SerializeField] private CharacterController2D characterController;
        
        private float _dashCooldownTimer;
        private int _dashLeft = 1; // TODO Handle player evolution
        private int _jumpLeft = 1; // TODO Handle player evolution
        private Vector2 _velocity;

        public PlayerStateManager StateManager { get; private set; }

        private void Awake()
        {
            StateManager = new PlayerStateManager();
        }

        private void Start()
        {
            InputManager.Instance.Jump += InputManager_OnJump;
            InputManager.Instance.Dash += InputManager_OnDash;
            InputManager.Instance.Crouch += InputManager_OnCrouch;
            InputManager.Instance.StopCrouch += InputManager_OnStopCrouch;
            InputManager.Instance.Walk += InputManager_OnWalk;
            InputManager.Instance.StopWalk += InputManager_OnStopWalk;
        }

        private void Update()
        {
            if (_dashCooldownTimer > 0)
            {
                float newCooldownTimer = _dashCooldownTimer -= Time.deltaTime;
                _dashCooldownTimer = Mathf.Max(newCooldownTimer, 0);
            }
        }

        private void FixedUpdate()
        {
            Move();
        }
        private void OnDestroy()
        {
            InputManager.Instance.Jump -= InputManager_OnJump;
            InputManager.Instance.Dash -= InputManager_OnDash;
            InputManager.Instance.Crouch -= InputManager_OnCrouch;
            InputManager.Instance.Walk -= InputManager_OnWalk;
        }

        private void Move()
        {
            float movementInput = InputManager.Instance.GetMovementH();
            characterController.Move(movementInput * MovementSpeed);
        }

        private bool CanDash()
        {
            return _dashLeft > 0;
        }

        private bool CanJump()
        {
            return _jumpLeft > 0;
        }

        private void ResetCooldowns()
        {
            _jumpLeft = 1; // TODO Handle player evolution
            _dashCooldownTimer = 0f;
            ResetDash();
        }

        private void ResetDash()
        {
            _dashLeft = 1; // TODO Handle player evolution
        }

        private void InputManager_OnJump(object sender, EventArgs e)
        {
            if (!CanJump()) return;
            characterController.Jump();
            StateManager.AddState(PlayerState.Jumping);
        }

        private void InputManager_OnDash(object sender, EventArgs e)
        {
            if (!CanDash()) return;
            StateManager.AddState(PlayerState.Dashing);
            _dashLeft--;
            if (StateManager.HasState(PlayerState.Grounded)) _dashCooldownTimer = DashCooldown;
        }

        private void InputManager_OnCrouch(object sender, EventArgs e)
        {
            StateManager.AddState(PlayerState.Crouching);
        }
        
        private void InputManager_OnStopCrouch(object sender, EventArgs e)
        {
            StateManager.RemoveState(PlayerState.Crouching);
        }

        private void InputManager_OnWalk(object sender, EventArgs e)
        {
            StateManager.AddState(PlayerState.Walking);
        }
        
        private void InputManager_OnStopWalk(object sender, EventArgs e)
        {
            StateManager.RemoveState(PlayerState.Walking);
        }
    }
}