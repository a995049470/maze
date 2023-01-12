using Stride.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core;
using Stride.Engine.Design;

namespace Maze.Code.Render
{
    [DataContract("CellSpriteComponent")]
    [Display("CellSprite", Expand = ExpandRule.Once)]
    [DefaultEntityComponentRenderer(typeof(CellSpriteRenderProcessor))]
    [ComponentOrder(10001)]
    [ComponentCategory("Sprites")]
    public class CellSpriteComponent : SpriteComponent
    {
        public float CellValue;
    }
}
