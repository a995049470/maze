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
    public class UnitComponent : BaseElementComponent<StaticData_Unit, DynamicData_Unit>
    {
        public UnitComponent(StaticData_Unit staticData, DynamicData_Unit dynamicData) : base(staticData, dynamicData)
        {

        }
    }
}
