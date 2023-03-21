using Stride.Core;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Engine.Design;
using System.Collections;
using System.Collections.Generic;

namespace Maze.Code.Map
{

    [DefaultEntityComponentProcessor(typeof(MoveProcessor))]
    public class VelocityComponent : ScriptComponent
    {
        [DataMemberIgnore]
        public Vector3 Direction;
        public float Speed;
        public bool IsActive = true;
    }

}
