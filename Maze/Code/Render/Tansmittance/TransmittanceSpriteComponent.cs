using Stride.Core;
using Stride.Engine;
using Stride.Engine.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Maze.Code.Render
{

    //透光组件
    [DataContract("TransmittanceSpriteComponent")]
    [Display("TransmittanceSprite", Expand = ExpandRule.Once)]
    [DefaultEntityComponentRenderer(typeof(TransmittanceSpriteRenderProcessor))]
    [DisableSuperClassRenderProcessor]
    [ComponentOrder(10002)]
    [ComponentCategory("Sprites")]
    public class TransmittanceSpriteComponent : SpriteComponent
    {

    }
}
