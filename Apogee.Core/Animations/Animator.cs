using System.Collections.Generic;
using Apogee.Core.Math;
using Apogee.Core.Resources;

namespace Apogee.Core.Animations
{
    public class Animator
    {
        public Animator(BasicMesh entity)
        {
            _entity = entity;
        }

        private BasicMesh _entity { get; set; }

        private Animation _currentAnimation;
        private float _animationTime = 0;


        public void DoAnimation(Animation animation)
        {
            _animationTime = 0;
            _currentAnimation = animation;
        }

        public void Update(float frametime)
        {
            if (_currentAnimation == null)
            {
                return;
            }

            IncreaseAnimationTime(frametime);

            var currentPose = CalculateCurrentAnimationPose();

            ApplyPoseToJoints(currentPose, _entity.RootJoint, new Matrix4F().InitIdentity());
        }

        private void IncreaseAnimationTime(float frametime)
        {
            _animationTime += frametime;
            if (_animationTime > _currentAnimation.Length)
            {
                _animationTime %= _currentAnimation.Length;
                // _animationTime = 0;
            }
        }


        private Dictionary<string, Matrix4F> CalculateCurrentAnimationPose()
        {
            var frames = GetPreviousAndNextFrames();
            var progression = CalculateProgression(frames[0], frames[1]);
            return InterpolatePoses(frames[0], frames[1], progression);
        }


        private void ApplyPoseToJoints(Dictionary<string, Matrix4F> currentPose, Joint joint, Matrix4F parentTransform)
        {
            var name = joint.Name;

            var currentLocalTransform = currentPose[name.Replace(".", "_")].Clone();
            var currentTransform = parentTransform.Clone() * currentLocalTransform.Clone();


            foreach (var childJoint in joint.Children)
            {
                ApplyPoseToJoints(currentPose, childJoint, currentTransform);
            }

            currentTransform = currentTransform * joint.InverseBindTransform.Clone();
            joint.AnimatedTransform = currentTransform.Clone();
        }

        private KeyFrame[] GetPreviousAndNextFrames()
        {
            var allFrames = _currentAnimation.KeyFrames;
            var previousFrame = allFrames[0];
            var nextFrame = allFrames[0];
            for (var i = 1; i < allFrames.Count; i++)
            {
                nextFrame = allFrames[i];
                if (nextFrame.TimeStamp > _animationTime)
                {
                    break;
                }

                previousFrame = allFrames[i];
            }

            return new[] {previousFrame, nextFrame};
        }

        private float CalculateProgression(KeyFrame previousFrame, KeyFrame nextFrame)
        {
            var totalTime = nextFrame.TimeStamp - previousFrame.TimeStamp;
            var currentTime = _animationTime - previousFrame.TimeStamp;
            return currentTime / totalTime;
        }


        private Dictionary<string, Matrix4F> InterpolatePoses(KeyFrame previousFrame, KeyFrame nextFrame,
            float progression)
        {

            var currentPose = new Dictionary<string, Matrix4F>();
            foreach (var jointName in previousFrame.Pose.Keys)
            {
                var previousTransform = previousFrame.Pose[jointName];
                var nextTransform = nextFrame.Pose[jointName];
                var currentTransform =
                    JointTransform.Interpolate(previousTransform, nextTransform, progression);
                currentPose[jointName] = currentTransform.GetLocalTransform();
            }

            return currentPose;
        }
    }
}