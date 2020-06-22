using System.Collections.Generic;

namespace Apogee.AssetSystem
{
    public class RawAssetSource
    {
        public string File { get; set; }
        public List<string> Files { get; set; } = new List<string>();
        public string Type { get; set; }
    }
}