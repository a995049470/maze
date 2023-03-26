using Stride.Animations;
using Stride.Core;
using Stride.Engine;
using Stride.Engine.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Code.Map
{
    [DefaultEntityComponentProcessor(typeof(PoseProcessor), ExecutionMode = ExecutionMode.Runtime)]
    public class PoseComponent : ScriptComponent
    {
        private readonly Dictionary<string, AnimationClip> animations;
        public Dictionary<string, AnimationClip> Animations
        {
            get { return animations; }
        }
        [DataMemberIgnore]
        public PlayingAnimation PlayingAnimation { get; private set; }
        [DataMemberIgnore]
        public AnimationBlender Blender { get; internal set; } = new AnimationBlender();

        private AnimationClip GetAnimationClip(string name)
        {
            if (!animations.TryGetValue(name, out var clip))
            {
                throw new Exception($"no animation:{name}");
            }
            return clip;
        }

        public TimeSpan CalculateAnimationCurrentTime(string name, float time)
        {
            var clip = GetAnimationClip(name);
            var currentTime = time - (int)(time / clip.Duration.TotalSeconds) * clip.Duration.TotalSeconds;
            return TimeSpan.FromSeconds(currentTime);
        }

        public void Play(string name, TimeSpan time)
        {
            if(PlayingAnimation?.Name != name)
            {        
                PlayingAnimation = new PlayingAnimation(name, GetAnimationClip(name));
            }
            PlayingAnimation.CurrentTime = time;
        }

        public PoseComponent()
        {
            animations = new Dictionary<string, AnimationClip>();
        }
    }
}
