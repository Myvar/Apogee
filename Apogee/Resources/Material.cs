using Apogee.Core;

namespace Apogee.Resources
{
    public class Material
    {
        public float Ns { get; set; }
        public Vector3f Ka { get; set; } = new Vector3f(0, 0, 0);
        public Vector3f Kd { get; set; } = new Vector3f(0, 0, 0);
        public Vector3f Ks { get; set; } = new Vector3f(0, 0, 0);
        public float Ni { get; set; }
        public float d { get; set; }
        public int illum { get; set; }

        public void Apply(Shader s)
        {
            s.SetUniform("Ns", Ns);
            s.SetUniform("Ka", Ka);
            s.SetUniform("Kd", Kd);
            s.SetUniform("Ks", Ks);
            s.SetUniform("Ni", Ni);
            s.SetUniform("d", d);
            s.SetUniform("illum", illum);
        }
    }
}