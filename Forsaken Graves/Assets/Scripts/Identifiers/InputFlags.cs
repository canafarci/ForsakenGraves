using System;

namespace ForsakenGraves.Identifiers
{
    [Flags]
    public enum InputFlags
    {
        Up = 1 << 0,
        Left = 1 << 1,
        Down = 1 << 2,
        Right = 1 << 3,
    }
}
