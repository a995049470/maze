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
    public class TileComponent : BaseElementComponent<StaticData_Tile, DynamicData_Tile>
    {
        public TileComponent(StaticData_Tile staticData, DynamicData_Tile dynamicData) : base(staticData, dynamicData)
        {

        }
    }
}
