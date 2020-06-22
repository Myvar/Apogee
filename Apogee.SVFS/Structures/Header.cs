using System.Diagnostics;
using System.IO;

namespace Apogee.SVFS.Structures
{
    public class Header : IStructure
    {
        public uint MagicNumber => 0xAF77;
        public uint GroupIndexOffset { get; set; }

        public void Read(BinaryReader reader)
        {
            var mNum = reader.ReadUInt32();
            Debug.Assert(mNum == MagicNumber, "Failed to pass Magic Number test.");
            GroupIndexOffset = reader.ReadUInt32();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(MagicNumber);
            writer.Write(GroupIndexOffset);
        }
    }
}