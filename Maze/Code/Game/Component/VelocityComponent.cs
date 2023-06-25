using Stride.Core;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Engine.Design;
using System.Collections;
using System.Collections.Generic;

namespace Maze.Code.Game
{

    [DefaultEntityComponentProcessor(typeof(MoveProcessor), ExecutionMode = ExecutionMode.Runtime)]
    public class VelocityComponent : ScriptComponent
    {
        [DataMemberIgnore]
        public Vector3 Direction;
        [DataMemberIgnore]
        public Vector3 FaceDirection = Vector3.UnitZ;
        public Vector3 TargetPos;
        public Vector3 LastTargetPos;
        public float Speed;
        public bool IsActive = true;

        public void UpdatePos(Vector3 currentPos)
        {
            TargetPos = currentPos;
            LastTargetPos = currentPos;
        }
    }

}
