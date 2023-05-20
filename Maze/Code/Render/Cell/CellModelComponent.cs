using Stride.Engine;
using Stride.Core;
using Stride.Engine.Design;
using System;

namespace Maze.Code.Render
{

    [Flags]
    public enum CellFlag
    {
        None = 0,
        Barrier = 1,
        Light = 2,
    }

    [DataContract("CellModelComponent")]
    [Display("CellModel", Expand = ExpandRule.Once)]
    [DefaultEntityComponentRenderer(typeof(CellModelRenderProcessor))]
    [DisableSuperClassRenderProcessor]
    [ComponentOrder(11001)]
    [ComponentCategory("CellModel")]
    public class CellModelComponent : ModelComponent
    {
        [DataMember(50)]
        public CellFlag Flag = CellFlag.Barrier;
    }
}
