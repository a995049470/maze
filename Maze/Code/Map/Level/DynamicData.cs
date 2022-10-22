using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;

namespace Maze.Map
{
    public class DynamicData : IPosition
    {
        public Int2 Pos;
        
        public Int2 GetPosition()
        {
            return Pos;
        }

    }
}
