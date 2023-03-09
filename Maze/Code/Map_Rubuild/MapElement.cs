using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Engine;

namespace Maze.Code.Map
{
    public class MapElement : EntityComponent
    {
        public bool IsWalkable;
        public Int2 Pos;
        public int Layer;
    }
}
