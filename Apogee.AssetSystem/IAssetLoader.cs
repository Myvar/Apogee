using Apogee.Core;

namespace Apogee.AssetSystem
{
    public interface IAssetLoader
    {
        IAsset LoadFromFile(RawAssetSource source);
    }
}