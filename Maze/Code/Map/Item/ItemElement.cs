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
    public class ItemElement : BaseElement<StaticData_Item, DynamicData_Item>
    {
        public ItemElement(StaticData_Item staticData, DynamicData_Item dynamicData) : base(staticData, dynamicData)
        {

        }

        public override bool IsWalkable()
        {
            return StaticData.IsWalkable;
        }
    }
}
