using System;
using Apogee.Core;
using Apogee.Ecs;

namespace Apogee.Components
{
    public class Transform : Component
    {
        public Guid PerentTransform { get; set; }

        public Vec3 Translation { get; set; } = new Vec3(0);
        public Vec3 Rotation { get; set; } = new Vec3(0);
        public Vec3 Scale { get; set; } = new Vec3(1);


        public Mat4 GetTranformation()
        {
            var perent = GameEngine.Container.GetComponent<Transform>(PerentTransform);

            var trans = new Mat4().InitTranslation(
                Translation.X + perent.Translation.X,
                Translation.Y + perent.Translation.Y,
                Translation.Z + perent.Translation.Z);

            var rot = new Mat4().InitRotation(
                Rotation.X + perent.Rotation.X,
                Rotation.Y + perent.Rotation.Y,
                Rotation.Z + perent.Rotation.Z);

            var scal = new Mat4().InitScale(
                Scale.X + (perent.Scale.X - 1),
                Scale.Y + (perent.Scale.Y - 1),
                Scale.Z + (perent.Scale.Z - 1));

            return trans * rot * scal;
        }
    }
}