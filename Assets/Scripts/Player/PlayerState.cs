using System;

namespace CoolDawn.Player
{
    [Flags]
    public enum PlayerState
    {
        None = 0,
        Walking = 1 << 0,
        Running = 1 << 1,
        Jumping = 1 << 2,
        Dashing = 1 << 3,
        Crouching = 1 << 4,
        Sliding = 1 << 5,
        Grounded = 1 << 6
    }
}