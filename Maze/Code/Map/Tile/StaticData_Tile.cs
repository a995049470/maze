using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;

namespace Maze.Map
{
    public class StaticData_Tile : StaticData
    {
        public bool IsWalkable = true;
        public bool IsVisionBarrier = false;
    }
}
