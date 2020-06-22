using System.IO;

namespace Apogee.SVFS
{
    
    /// <summary>
    /// Can Be serialized to a stream
    /// </summary>
    public interface IStructure
    {
        void Read(BinaryReader reader);
        void Write(BinaryWriter writer);
    }
}