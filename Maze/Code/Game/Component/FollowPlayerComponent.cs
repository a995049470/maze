using Stride.Core;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Engine.Design;

namespace Maze.Code.Game
{
    [DefaultEntityComponentProcessor(typeof(FollowPlayerProcessor), ExecutionMode = ExecutionMode.Runtime)]
    [DataContract]
    public class FollowPlayerComponent : ScriptComponent
    {
        [DataMember(10)]
        public Vector3 TargetOffset;
        [DataMember(20)]
        public float Distance = 5;
        [DataMember(30)]
        public float SmoothTime = 1;
        [DataMember(40)]
        public float MaxSpeed = 20;
        [DataMemberIgnore]
        public Vector3 CurrentVelocity;
    }

}
