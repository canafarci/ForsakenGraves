using System;

namespace ForsakenGraves.Identifiers
{
    [Flags]
    public enum InputFlags
    {
        Forward = 1 << 0,
        Left = 1 << 1,
        Back = 1 << 2,
        Right = 1 << 3,
        Jump = 1 << 4
    }
}
