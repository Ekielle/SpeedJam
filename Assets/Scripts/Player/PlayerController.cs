using System;
using UnityEngine;

namespace CoolDawn.Player
{
    public class PlayerController : MonoBehaviour
    {
        private const float MovementSpeed = 5.0f;
        private const float DashCooldown = 1.0f;
        [SerializeField] private Transform feetPosition;
        //TODO Replace with a custom CharacterController, see https://roystan.net/articles/character-controller-2d/#:~:text=Character%20controllers%20are%20responsible%20for,with%20a%202D%20character%20controller.
        [SerializeField] private CharacterController characterController;
        
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
            CheckGroundState();
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
            ApplyGravity();
            float movementInput = InputManager.Instance.GetMovementH();
            characterController.Move(_velocity * Time.deltaTime);
            characterController.Move(new Vector2(movementInput * MovementSpeed, 0) * Time.deltaTime); // TODO Player can move accross coliders, should be prevented by the CharacterController
        }
        
        private void ApplyGravity()
        {
            if (StateManager.HasState(PlayerState.Grounded))
            {
                _velocity.y = 0;
            }
            else
            {
                _velocity.y += Physics.gravity.y * Time.deltaTime; // TODO Handle custom gravity (use biome gravity)
            }
            
            characterController.Move(_velocity * Time.deltaTime);
        }

        private bool CanDash()
        {
            return _dashLeft > 0;
        }

        private bool CanJump()
        {
            return _jumpLeft > 0;
        }

        private void CheckGroundState()
        {
            const float castRadius = 0.5f;
            const float castDistance = 0f;
            
            if (characterController.isGrounded || Physics2D.CircleCast(feetPosition.position, castRadius, Vector2.down, castDistance))
            {
                if (StateManager.HasState(PlayerState.Grounded)) return;
                StateManager.AddState(PlayerState.Grounded);
                ResetCooldowns();
            }
            else
            {
                Debug.Log("Not grounded");
                StateManager.RemoveState(PlayerState.Grounded);
            }
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