using System;

namespace Apogee.SVFS
{
    [Flags]
    public enum GroupFlags : byte
    {
        Basic = 1,
        Compressed = 2,
        Metadata = 4,
    }
}