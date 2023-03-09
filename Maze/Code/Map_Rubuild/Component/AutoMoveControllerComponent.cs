using Maze.Map;
using Stride.Core.Mathematics;
using Stride.Engine;

namespace Maze.Code.Map
{
    public class AutoMoveControllerComponent : EntityComponent
    {
        public Int2[] WayPoints;
        public int NextPointIndex = 1;
        public int MoveDir = 1;
        public CycleFlag Flag;
        public bool IsAutoMove = false;
    }



}
