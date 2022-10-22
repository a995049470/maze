using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Graphics;
using Stride.Rendering;
using Stride.Rendering.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maze.Map
{
    public class TileElement : BaseElement<StaticData_Tile, DynamicData_Tile>
    {
        public TileElement(StaticData_Tile staticData, DynamicData_Tile dynamicData) : base(staticData, dynamicData)
        {

        }

        public override bool IsWalkable()
        {
            return StaticData.IsWalkable;
        }
    }
}
