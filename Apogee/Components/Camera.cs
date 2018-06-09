using Apogee.Core;
using OpenTK.Input;

namespace Apogee.Components
{
    public class Camera
    {
        public static Vec3 yAcis = new Vec3(0, 1, 0);

        public Vec3 Position;
        public Vec3 Forward;
        public Vec3 Up;

        private float zNear;
        private float zFar;
        private float Width;
        private float Height;
        private float fov;
        private float Sensitivity = 1;


        public Camera(float znear, float zfar, float width, float height, float Fov, float sensitivity)
        {
            Position = new Vec3(0, 0, 0);
            Forward = new Vec3(0, 0, 1);
            Up = new Vec3(0, 1, 0);

            Up.Normalize();
            Forward.Normalize();

            zNear = znear;
            zFar = zfar;
            Width = width;
            Height = height;
            fov = Fov;
            //Sensitivity = sensitivity;
        }

        public bool CanMove { get; set; } = true;

        public void Resize(float znear, float zfar, float width, float height, float Fov)
        {
            zNear = znear;
            zFar = zfar;
            Width = width;
            Height = height;
            fov = Fov;
        }

        public Mat4 GetProjection()
        {
            var projectionMatric = new Mat4().InitProjection(fov, Width, Height, zNear, zFar);
            var camerRotation = new Mat4().InitCamera(Forward, Up);
            var camraTranslation = new Mat4().InitTranslation(-Position.X, -Position.Y, -Position.Z);
            return projectionMatric.Mul(camerRotation.Mul(camraTranslation));
        }

        public Mat4 GetProjectionSkyBox()
        {
            var projectionMatric = new Mat4().InitProjection(fov, Width, Height, zNear, zFar);
            var camerRotation = new Mat4().InitCamera(Forward, Up);
            return projectionMatric.Mul(camerRotation);
        }

        public Mat4 GetWorld()
        {
            var projectionMatric = new Mat4().InitProjection(fov, Width, Height, zNear, zFar);
            var camerRotation = new Mat4().InitCamera(Forward, Up);
            var camraTranslation = new Mat4().InitTranslation(-Position.X, -Position.Y, -Position.Z);
            return camerRotation.Mul(camraTranslation);
        }

        public Vec3 GetLeft()
        {
            var left = Forward.Cross(Up);
            left.Normalize();
            return left;
        }

        public void RotateY(float angle)
        {
            Vec3 Haxis = yAcis.Cross(Forward);
            Haxis.Normalize();

            Forward.Rotate(angle, yAcis);
            Forward.Normalize();

            Up = Forward.Cross(Haxis);
            Up.Normalize();
        }

        public void RotateX(float angle)
        {
            Vec3 Haxis = yAcis.Cross(Forward);
            Haxis.Normalize();

            Forward.Rotate(angle, Haxis);
            Forward.Normalize();

            Up = Forward.Cross(Haxis);
            Up.Normalize();
        }

        public Vec3 GetRight()
        {
            var right = Up.Cross(Forward);
            right.Normalize();
            return right;
        }

        public void Move(Vec3 dir, float amt)
        {
            Position = Position + dir * amt;
        }

        private bool MouseLocker = false;


        public void Input(Input i, double delta)
        {
            if (!CanMove) return;

            float movAmt = (float) (i.IsKeyDown(Key.ShiftLeft) ? 4 : 10 * delta);
            if (MouseLocker)
            {
                if (i.IsKeyDown(Key.W))
                {
                    Move(Forward, movAmt);
                }

                if (i.IsKeyDown(Key.A))
                {
                    Move(GetLeft(), movAmt);
                }

                if (i.IsKeyDown(Key.S))
                {
                    Move(Forward, -movAmt);
                }

                if (i.IsKeyDown(Key.D))
                {
                    Move(GetRight(), movAmt);
                }
            }

            if (MouseLocker != (Mouse.GetState().RightButton == ButtonState.Pressed))
            {
                i.CenterMouse();
                MouseLocker = Mouse.GetState().RightButton == ButtonState.Pressed;
                i.MouseVisible(!MouseLocker);
                return;
            }

            MouseLocker = Mouse.GetState().RightButton == ButtonState.Pressed;

            i.MouseVisible(!MouseLocker);

            if (MouseLocker)
            {
                Vec2 deltaPos = i.GetMouseDelta();

                bool rotY = deltaPos.X != 0;
                bool rotX = deltaPos.Y != 0;

                if (rotY)
                {
                    RotateY((float) OpenTK.MathHelper.DegreesToRadians(deltaPos.X * Sensitivity));
                }

                if (rotX)
                {
                    RotateX((float) OpenTK.MathHelper.DegreesToRadians(deltaPos.Y * Sensitivity));
                }


                i.CenterMouse();
            }
        }
    }
}