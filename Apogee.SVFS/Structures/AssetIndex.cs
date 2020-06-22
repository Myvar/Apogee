using System.Collections.Generic;
using System.IO;

namespace Apogee.SVFS.Structures
{
    public class AssetIndex : List<AssetEntry>, IStructure
    {
        public void Read(BinaryReader reader)
        {
            var count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                var x = new AssetEntry();
                x.Read(reader);
                Add(x);
            }
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(Count);
            foreach (var x in this)
            {
                x.Write(writer);
            }
        }
    }
}