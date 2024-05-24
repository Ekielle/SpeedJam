using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    
    public EventHandler Jump;
    public EventHandler Dash;
    public EventHandler Crouch;
    public EventHandler Pause;
    public EventHandler Reload;
    public EventHandler Walk;
    
    private InputActions _inputActions;

    private void Awake()
    {
        Instance = this;
        _inputActions = new InputActions();
        _inputActions.Enable();
        
        _inputActions.Player.Jump.performed += Jump_OnPerformed;
        _inputActions.Player.Dash.performed += Dash_OnPerformed;
        _inputActions.Player.Crouch.performed += Crouch_OnPerformed;
        _inputActions.Player.Pause.performed += Pause_OnPerformed;
        _inputActions.Player.Reload.performed += Restart_OnPerformed;
        _inputActions.Player.Walk.performed += Walk_OnPerformed;
    }

    private void OnDestroy()
    {
        _inputActions.Player.Jump.performed -= Jump_OnPerformed;
        _inputActions.Player.Dash.performed -= Dash_OnPerformed;
        _inputActions.Player.Crouch.performed -= Crouch_OnPerformed;
        _inputActions.Player.Pause.performed -= Pause_OnPerformed;
        _inputActions.Player.Reload.performed -= Restart_OnPerformed;
        _inputActions.Player.Walk.performed -= Walk_OnPerformed;
        
        _inputActions.Dispose();
    }

    public Vector2 GetMovementVector()
    {
        return _inputActions.Player.Move.ReadValue<Vector2>();
    }
    
    private void Jump_OnPerformed(InputAction.CallbackContext ctx)
    {
        Jump?.Invoke(this, EventArgs.Empty);
    }
    
    private void Dash_OnPerformed(InputAction.CallbackContext ctx)
    {
        Dash?.Invoke(this, EventArgs.Empty);
    }
    
    private void Crouch_OnPerformed(InputAction.CallbackContext ctx)
    {
        Crouch?.Invoke(this, EventArgs.Empty);
    }
    
    private void Pause_OnPerformed(InputAction.CallbackContext ctx)
    {
        Pause?.Invoke(this, EventArgs.Empty);
    }
    
    private void Restart_OnPerformed(InputAction.CallbackContext ctx)
    {
        Reload?.Invoke(this, EventArgs.Empty);
    }
    
    private void Walk_OnPerformed(InputAction.CallbackContext ctx)
    {
        Walk?.Invoke(this, EventArgs.Empty);
    }
}
