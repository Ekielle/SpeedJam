using System;

namespace CoolDawn.Player
{
    [Flags]
    public enum PlayerState
    {
        None = 0,
        Idle = 1 << 0,
        Walking = 1 << 1,
        Running = 1 << 2,
        Jumping = 1 << 3,
        Dashing = 1 << 4,
        Crouching = 1 << 5,
        Sliding = 1 << 6,
        Grounded = 1 << 7
    }
}