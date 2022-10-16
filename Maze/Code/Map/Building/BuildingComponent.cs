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
    public class BuildingComponent : BaseElementComponent<StaticData_Building, DynamicData_Building>
    {
        public BuildingComponent(StaticData_Building staticData, DynamicData_Building dynamicData) : base(staticData, dynamicData)
        {

        }
    }
}
