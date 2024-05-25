using System;
using UnityEngine;

namespace CoolDawn.Player
{
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] private PlayerController playerController;

        private void Start()
        {
            playerController.StateManager.StateChanged += PlayerStateManager_OnStateChanged;
        }

        private void OnDestroy()
        {
            playerController.StateManager.StateChanged -= PlayerStateManager_OnStateChanged;
        }

        private void PlayerStateManager_OnStateChanged(object sender, EventArgs e)
        {
            if (playerController.StateManager.HasState(PlayerState.Idle))
                Debug.Log("Idle");
            else if (playerController.StateManager.HasState(PlayerState.Walking))
                Debug.Log("Walking");
            else if (playerController.StateManager.HasState(PlayerState.Running))
                Debug.Log("Running");
            else if (playerController.StateManager.HasState(PlayerState.Jumping))
                Debug.Log("Jumping");
            else if (playerController.StateManager.HasState(PlayerState.Dashing))
                Debug.Log("Dashing");
            else if (playerController.StateManager.HasState(PlayerState.Crouching))
                Debug.Log("Crouching");
            else if (playerController.StateManager.HasState(PlayerState.Sliding))
                Debug.Log("Sliding");
            else if (playerController.StateManager.HasState(PlayerState.Grounded)) Debug.Log("Grounded");
        }
    }
}