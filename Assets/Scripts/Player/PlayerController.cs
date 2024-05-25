using System;
using UnityEngine;

namespace CoolDawn.Player
{
    public class PlayerController : MonoBehaviour
    {
        private const float MovementSpeed = 1.0f;
        private const float GroundDashCooldown = 1.0f;
        [SerializeField] private CharacterController2D characterController;
        [SerializeField] private Transform visual;
        
        private float _groundDashCooldownTimer;
        private int _airDashLeft = 1; // TODO Handle player evolution
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
            characterController.GroundStateChanged += CharacterController_OnGrounded;
        }

        private void Update()
        {
            if (_groundDashCooldownTimer > 0)
            {
                float newCooldownTimer = _groundDashCooldownTimer -= Time.deltaTime;
                _groundDashCooldownTimer = Mathf.Max(newCooldownTimer, 0);
            }
            
            if (StateManager.HasState(PlayerState.Grounded))
            {
                ResetAirCooldowns();
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
            
            if(movementInput > 0)
            {
                visual.localScale = Vector3.one;
            }
            else if(movementInput < 0)
            {
                visual.localScale = new Vector3(-1, 1, 1);
            }
        }

        private bool CanAirDash()
        {
            return _airDashLeft > 0;
        }
        
        private bool CanGroundDash()
        {
            return _groundDashCooldownTimer <= 0;
        }

        private bool CanJump()
        {
            return _jumpLeft > 0;
        }

        private void ResetAirCooldowns()
        {
            _jumpLeft = 1; // TODO Handle player evolution
            _airDashLeft = 1; // TODO Handle player evolution
        }

        private void InputManager_OnJump(object sender, EventArgs e)
        {
            if (!CanJump()) return;
            
            characterController.Jump();
            
            _jumpLeft--;
            StateManager.AddState(PlayerState.Jumping);
        }

        private void InputManager_OnDash(object sender, EventArgs e)
        {
            
            if (!StateManager.HasState(PlayerState.Grounded) && !CanAirDash()) return;
            if (StateManager.HasState(PlayerState.Grounded) && !CanGroundDash()) return;

            characterController.Dash(InputManager.Instance.GetMovementH());

            if (StateManager.HasState(PlayerState.Grounded))
            {
                _groundDashCooldownTimer = GroundDashCooldown;
            }
            else
            {
                _airDashLeft--;
            }
            StateManager.AddState(PlayerState.Dashing);
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
        
        private void CharacterController_OnGrounded(object sender, bool isGrounded)
        {
            if (isGrounded)
            {
                StateManager.AddState(PlayerState.Grounded);
            }
            else
            {
                StateManager.RemoveState(PlayerState.Grounded);
            }
        }
    }
}