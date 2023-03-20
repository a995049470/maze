using Stride.Core;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Engine.Design;

namespace Maze.Code.Map
{
    [DefaultEntityComponentProcessor(typeof(MoveProcessor))]
    public class VelocityComponent : ScriptComponent
    {
        [DataMemberIgnore]
        public Vector3 Direction;
        public float Speed;
    }

}
