using System;

namespace CoolDawn.Player
{
    public class PlayerStateManager
    {
        public EventHandler StateChanged;

        private PlayerState CurrentState { get; set; } = PlayerState.Idle;

        public void AddState(PlayerState state)
        {
            CurrentState |= state;
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        public void RemoveState(PlayerState state)
        {
            CurrentState &= ~state;
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool HasState(PlayerState state)
        {
            return (CurrentState & state) == state;
        }
    }
}