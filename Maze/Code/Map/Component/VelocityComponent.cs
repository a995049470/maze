using Stride.Core;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Engine.Design;
using System.Collections;
using System.Collections.Generic;

namespace Maze.Code.Map
{

    [DefaultEntityComponentProcessor(typeof(MoveProcessor), ExecutionMode = ExecutionMode.Runtime)]
    public class VelocityComponent : ScriptComponent
    {
        [DataMemberIgnore]
        public Vector3 Direction;
        [DataMemberIgnore]
        public Vector3 FaceDirection = Vector3.UnitZ;
        public float Speed;
        public bool IsActive = true;
    }

}
