using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Engine.Design;

namespace Maze.Code.Map
{
    [DefaultEntityComponentProcessor(typeof(MapElementProcessor))]
    public class MapElementComponent : EntityComponent
    {
        public bool IsWalkable;
        public Int2 Pos;
        public int Layer;
    }
}
