using System;
using UnityEngine;

namespace CoolDawn.Player
{
    public class PlayerController : MonoBehaviour
    {
        
        public event EventHandler Dashing;
        public event EventHandler<bool> Crouching;

        public event EventHandler<bool> Grabbing;
        
        private const float MovementSpeed = 1.0f;
        private const float GroundDashCooldown = 1.0f;
        [SerializeField] private CharacterController2D characterController;
        [SerializeField] private Transform visual;
        [SerializeField] private bool airJumpEnabled = true;
        [SerializeField] private int airDashCount = 1;
        
        private float _groundDashCooldownTimer;
        private int _airDashLeft;
        private int _airJumpLeft;
        private Vector2 _velocity;

        public PlayerStateManager StateManager { get; private set; }
        public float LastMoveInput { get; private set; }

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
            InputManager.Instance.Reload += InputManager_Respawn;
            characterController.GroundStateChanged += CharacterController_OnGrounded;
            characterController.GrabWallStateChanged += CharacterController_OnWallGrabbed;
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
        }

        private void Move()
        {
            float movementInput = InputManager.Instance.GetMovementH();
            LastMoveInput = movementInput;
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

        private bool CanAirJump()
        {
            return (airJumpEnabled && _airJumpLeft > 0) || StateManager.HasState(PlayerState.WallGrabbing);
        }

        private void ResetAirCooldowns()
        {
            _airJumpLeft = 1; 
            _airDashLeft = airDashCount;
        }

        private void InputManager_OnJump(object sender, EventArgs e)
        {
            if (!CanAirJump() && !StateManager.HasState(PlayerState.Grounded)) return;
            
            characterController.Jump();
            
            if(!StateManager.HasState(PlayerState.Grounded))
            {
                _airJumpLeft--;
            }
            
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
            Dashing?.Invoke(this, EventArgs.Empty);
        }

        private void InputManager_OnCrouch(object sender, EventArgs e)
        {
            StateManager.AddState(PlayerState.Crouching);
            characterController.Crouch();
            Crouching?.Invoke(this, true);
        }
        
        private void InputManager_OnStopCrouch(object sender, EventArgs e)
        {
            StateManager.RemoveState(PlayerState.Crouching);
            characterController.StopCrouch();
            Crouching?.Invoke(this, false);
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
        
        private void CharacterController_OnWallGrabbed(object sender, bool isGrabbingWall)
        {
            if (isGrabbingWall)
            {
                StateManager.AddState(PlayerState.WallGrabbing);
                Grabbing?.Invoke(this, true);
            }
            else
            {
                StateManager.RemoveState(PlayerState.WallGrabbing);
                Grabbing?.Invoke(this, false);
            }
        }

        private void InputManager_Respawn(object sender, EventArgs e)
        {
            Respawn();
        }

        public void Respawn()
        {
            Vector2 target = CheckpointManager.Instance.GetCurrentCheckpointPosition();
            characterController.Teleport(target);
        }
    }
}