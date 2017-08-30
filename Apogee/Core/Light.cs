namespace Apogee.Core
{
    public class Light
    {
        public Vector3f Position { get; set; } = new Vector3f(0, 0, 0);
        public Vector3f Ambient { get; set; } = new Vector3f(0.2f, 0.2f, 0.2f);
        public Vector3f Diffuse { get; set; } = new Vector3f(1, 1, 1);
        public Vector3f Specular { get; set; } = new Vector3f(1, 1, 1);

        public void SetUniform(Shader s, string name)
        {
            s.SetUniform(name + ".position", Position);
            s.SetUniform(name + ".ambient", Ambient);
            s.SetUniform(name + ".diffuse", Diffuse);
            s.SetUniform(name + ".specular", Specular);
        }
    }
}