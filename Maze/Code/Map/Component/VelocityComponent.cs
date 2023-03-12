using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Engine.Design;

namespace Maze.Code.Map
{
    [DefaultEntityComponentProcessor(typeof(MoveProcessor))]
    public class VelocityComponent : EntityComponent
    {
        public Vector2 Direction;
        public float Speed;
    }

}
