using System.Threading;
using Apogee.Engine.Core;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

namespace Apogee.Core
{
    public class Camera
    {
        public static Vector3f yAcis = new Vector3f(0, 1, 0);

        public Vector3f Position;
        public Vector3f Forward;
        public Vector3f Up;

        private float zNear;
        private float zFar;
        private float Width;
        private float Height;
        private float fov;
        private float Sensitivity = 1;


        public Camera(float znear, float zfar, float width, float height, float Fov, float sensitivity)
        {
            Position = new Vector3f(0, 0, 0);
            Forward = new Vector3f(0, 0, 1);
            Up = new Vector3f(0, 1, 0);

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

        public Matrix4f GetProjection()
        {
            var projectionMatric = new Matrix4f().InitProjection(fov, Width, Height, zNear, zFar);
            var camerRotation = new Matrix4f().InitCamera(Forward, Up);
            var camraTranslation = new Matrix4f().InitTranslation(-Position.X, -Position.Y, -Position.Z);
            return projectionMatric.Mul(camerRotation.Mul(camraTranslation));
        }

        public Matrix4f GetProjectionSkyBox()
        {
            var projectionMatric = new Matrix4f().InitProjection(fov, Width, Height, zNear, zFar);
            var camerRotation = new Matrix4f().InitCamera(Forward, Up);
            return projectionMatric.Mul(camerRotation);
        }

        public Matrix4f GetWorld()
        {
            var projectionMatric = new Matrix4f().InitProjection(fov, Width, Height, zNear, zFar);
            var camerRotation = new Matrix4f().InitCamera(Forward, Up);
            var camraTranslation = new Matrix4f().InitTranslation(-Position.X, -Position.Y, -Position.Z);
            return camerRotation.Mul(camraTranslation);
        }

        public Vector3f GetLeft()
        {
            var left = Forward.Cross(Up);
            left.Normalize();
            return left;
        }

        public void RotateY(float angle)
        {
            Vector3f Haxis = yAcis.Cross(Forward);
            Haxis.Normalize();

            Forward.Rotate(angle, yAcis);
            Forward.Normalize();

            Up = Forward.Cross(Haxis);
            Up.Normalize();
        }

        public void RotateX(float angle)
        {
            Vector3f Haxis = yAcis.Cross(Forward);
            Haxis.Normalize();

            Forward.Rotate(angle, Haxis);
            Forward.Normalize();

            Up = Forward.Cross(Haxis);
            Up.Normalize();
        }

        public Vector3f GetRight()
        {
            var right = Up.Cross(Forward);
            right.Normalize();
            return right;
        }

        public void Move(Vector3f dir, float amt)
        {
            Position = Position.Add(dir.Mul(amt));
        }

        private bool MouseLocker = false;


        public void Input(Input i, double delta)
        {
           if(!CanMove) return;

            float movAmt = (float) (i.IsKeyDown(Key.ShiftLeft) ? 11 : 10 * delta);

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
                Vector2f deltaPos = i.GetMouseDelta();

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