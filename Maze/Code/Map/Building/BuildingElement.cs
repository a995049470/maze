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
    public class BuildingElement : BaseElement<StaticData_Building, DynamicData_Building>
    {
        public BuildingElement(StaticData_Building staticData, DynamicData_Building dynamicData) : base(staticData, dynamicData)
        {

        }

        public override bool IsWalkable()
        {
            return StaticData.IsWalkable;
        }
    }
}
