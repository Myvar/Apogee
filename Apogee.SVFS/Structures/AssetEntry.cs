using System.IO;

namespace Apogee.SVFS.Structures
{
    public class AssetEntry : IStructure
    {
        public string Name { get; set; }
        public uint Processor { get; set; }
        public ulong Offset { get; set; }
        public ulong Size { get; set; }

        public AssetEntry()
        {
            
        }
        public AssetEntry(string name)
        {
            Name = name;
        }

        public void Read(BinaryReader reader)
        {
            Name = reader.ReadString();
            Processor = reader.ReadUInt32();
            Offset = reader.ReadUInt64();
            Size = reader.ReadUInt64();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(Name);
            writer.Write(Processor);
            writer.Write(Offset);
            writer.Write(Size);
        }
    }
}