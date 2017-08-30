using System.Collections.Generic;
using System.IO;
using Apogee.Core;
using Apogee.Resources;
using ImageSharp;

namespace Apogee.Terrain
{
    public class TerrainModel
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public int MaxOffset { get; set; } = 1;
        public int MinOffset { get; set; } = -1;


        public Model Model { get; set; }
        public Image<Rgba32> Tile { get; set; }

        private Shader Shader { get; set; }

        public Transform Transform { get; set; } = new Transform();
        
        private Light Light { get; set; } = new Light();
        public Material Material { get; set; }

        public void Draw(Camera c)
        {
            Shader.Apply();
            Shader.Update(Transform.GetTranformation(), c.GetProjection() * Transform.GetTranformation());

            Light.Position = c.Position;
            
            
            Shader.SetUniform("l_Direction", c.Forward);
            Shader.SetUniform("viewPos", c.Position);
            Light.SetUniform(Shader, "light");
            Model.Draw();
            
        }

        public TerrainModel(Image<Rgba32> tile)
        {
            Tile = tile;
            Width = tile.Width;
            Height = tile.Height;

            Generate();
        }

        public TerrainModel(string img)
        {
            using (var fl = File.OpenRead(img))
            using (var tile = Image.Load(fl))
            {
                Tile = tile;
                Width = tile.Width;
                Height = tile.Height;
                Generate();
            }
        }

        private float Normalize(float min, float max, float x) => ((x + 0.00001f) - min) / (max - min);
        private int Normalize(int min, int max, int x) => (int)(((x + 0.00001f) - min) / (max - min));
        

        private uint CalualteIndexBaceOnCoords(int x, int y)
        {
            return (uint) (x + (y * Width));
        }

        private Vector3f GetMapVector(int x, int y)
        {
            var px = Tile[x, y];
            return new Vector3f(
               x +  Normalize(MinOffset, MaxOffset, px.R - 127),
               0 + Normalize(MinOffset, MaxOffset, px.G- 127), 
               y + Normalize(MinOffset, MaxOffset, px.B- 127)
                );
        }

        public void Generate()
        {
            Shader = new Shader("./Shaders/terain");

            Material = new Material()
            {
                Kd = new Vector3f(34 / 255, 62 / 255, 10 / 255),
                Ns = 0,
                Ka = new Vector3f(1.000000f, 1.000000f, 1.000000f),
                Ks = new Vector3f(0.500000f, 0.500000f, 0.500000f),
                Ni = 1,
                d = 1,
            };
            
            Model?.Dispose();
            Model = new Model();

            var indecieData = new List<uint>();
            var vertexData = new List<Vertex>();


            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    var vert = new Vertex(
                        GetMapVector(x, y)
                    );
                    vertexData.Add(vert);
                }
            }

            for (int x = 0; x < Width - 1; x++)
            {
                for (int y = 0; y < Height - 1; y++)
                {
                    indecieData.Add(CalualteIndexBaceOnCoords(x, y));
                    indecieData.Add(CalualteIndexBaceOnCoords(x + 1, y));
                    indecieData.Add(CalualteIndexBaceOnCoords(x, y + 1));


                    indecieData.Add(CalualteIndexBaceOnCoords(x + 1, y + 1));

                    vertexData.Add(vertexData[(int) CalualteIndexBaceOnCoords(x, y + 1)]);
                    indecieData.Add((uint) vertexData.Count - 1);

                    vertexData.Add(vertexData[(int) CalualteIndexBaceOnCoords(x + 1, y)]);
                    indecieData.Add((uint) vertexData.Count - 1);
                }
            }

            Model.Load(vertexData.ToArray(), new List<List<uint>>() {indecieData});
        }
    }
}