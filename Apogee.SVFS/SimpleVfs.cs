using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Apogee.AssetSystem;
using Apogee.Core;
using Apogee.SVFS.Structures;
using K4os.Compression.LZ4;
using K4os.Compression.LZ4.Streams;

namespace Apogee.SVFS
{
    public class SimpleVfs
    {
        /*
         * Goals
         * Compression
         * Live reloading (with imbeded provessor)
         * Multithreading
         */

        private Header _header = new Header();
        private GroupIndex _groupIndex = new GroupIndex();
        private Dictionary<int, AssetIndex> _assetIndex = new Dictionary<int, AssetIndex>();
        private Dictionary<AssetEntry, Stream> _assetsData = new Dictionary<AssetEntry, Stream>();

        private int _activeGroupIndex = 0;
        private bool _readMode = false;
        private AssetIndex _readAsset;
        private BinaryReader _readStream;
        public List<string> Groups => _groupIndex.Select(x => x.Name).ToList();

        public T LoadAsset<T>(string name) where T : IAsset
        {
            return default(T);
        }

        public List<AssetEntry> Assets
        {
            get
            {
                if (_readMode)
                {
                    return _readAsset.Select(x => x).ToList();
                }
                else
                {
                    return _assetIndex[_activeGroupIndex].Select(x => x).ToList();
                }
            }
        }

        public void ScopeGroup(string name)
        {
            if (_readMode)
            {
                _readAsset = new AssetIndex();
                var g = _groupIndex.First(x => x.Name == name);
                _readStream.BaseStream.Position = (long) g.Offset;
                _readAsset.Read(_readStream);
            }

            if (_groupIndex.Count == 0)
            {
                _groupIndex.Add(new GroupEntry()
                {
                    Count = 0,
                    Flags = GroupFlags.Compressed,
                    Name = name,
                    Offset = 0
                });
                _assetIndex.Add(0, new AssetIndex());
                _activeGroupIndex = 0;
            }
            else
            {
                if (_groupIndex.All(x => x.Name != name))
                {
                    _groupIndex.Add(new GroupEntry()
                    {
                        Count = 0,
                        Flags = GroupFlags.Compressed,
                        Name = name,
                        Offset = 0
                    });
                }

                _activeGroupIndex = _groupIndex.IndexOf(_groupIndex.First(x => x.Name == name));
                _assetIndex.Add(_activeGroupIndex, new AssetIndex());
            }
        }

        public void AddAsset(AssetEntry e, Stream data)
        {
            var index = _assetIndex[_activeGroupIndex];
            index.Add(e);
            _assetsData.Add(e, data);
        }

        public string ReadAllText(AssetEntry e)
        {
            if (_groupIndex[_activeGroupIndex].Flags.HasFlag(GroupFlags.Compressed))
            {
                _readStream.BaseStream.Position = (long) e.Offset;

                using (var mem = new MemoryStream())
                using (var decoder = LZ4Stream.Decode(_readStream.BaseStream, null, true))
                {
                    decoder.BlockCopyTo(mem, e.Size);

                    return Encoding.ASCII.GetString(mem.ToArray());
                }
            }
            else
            {
                _readStream.BaseStream.Position = (long) e.Offset;
                using (var mem = new MemoryStream((int) e.Size)) 
                {
                    _readStream.BaseStream.BlockCopyTo(mem, e.Size);
                    return Encoding.ASCII.GetString(mem.ToArray());
                }
            }
        }

        public Stream OpenRead(AssetEntry e)
        {
            if (_groupIndex[_activeGroupIndex].Flags.HasFlag(GroupFlags.Compressed))
            {
                _readStream.BaseStream.Position = (long) e.Offset;

                var mem = new MemoryStream();
                using (var decoder = LZ4Stream.Decode(_readStream.BaseStream, null, true))
                {
                    decoder.BlockCopyTo(mem, e.Size);

                    mem.Position = 0;
                    return mem;
                }
            }
            else
            {
                _readStream.BaseStream.Position = (long) e.Offset;
                var mem = new MemoryStream((int) e.Size);
                {
                    _readStream.BaseStream.BlockCopyTo(mem, e.Size);
                    mem.Position = 0;
                    return mem;
                }
            }
        }

        public void Read(Stream s)
        {
            var r = new BinaryReader(s);
            _readStream = r;
            _readMode = true;

            _header.Read(r);
            s.Position = _header.GroupIndexOffset;
            _groupIndex.Read(r);
        }
        

        public void Write(Stream s)
        {
            using (var w = new BinaryWriter(s, Encoding.Default, true))
            {
                _header.Write(w);


                foreach (var (idx, e) in _assetIndex)
                {
                    foreach (var asset in e)
                    {
                        var data = _assetsData[asset];

                        if (_groupIndex[_activeGroupIndex].Flags.HasFlag(GroupFlags.Compressed))
                        {
                            asset.Offset = (ulong) s.Position;
                            using (var enc = LZ4Stream.Encode(s, new LZ4EncoderSettings()
                            {
                                CompressionLevel = LZ4Level.L00_FAST
                            }, true))
                            {
                                data.CopyTo(enc);
                            }

                            asset.Size = (ulong) data.Length;
                        }
                        else
                        {
                            asset.Offset = (ulong) s.Position;
                            asset.Size = (ulong) data.Length;
                            data.CopyTo(s);
                        }
                    }

                    _groupIndex[idx].Offset = (ulong) s.Position;
                    
                    e.Write(w);
                }

                _header.GroupIndexOffset = (uint) s.Position;
                s.Position = 0;
                _header.Write(w);

                s.Position = _header.GroupIndexOffset;

                _groupIndex.Write(w);
            }
        }
    }
}