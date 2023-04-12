using Stride.Engine;
using Stride.Core;
using Stride.Engine.Design;

namespace Maze.Code.Render
{

    public enum CellFlag
    {
        Barrier,
        Light
    }

    [DataContract("CellModelComponent")]
    [Display("CellModel", Expand = ExpandRule.Once)]
    [DefaultEntityComponentProcessor(typeof(CellModelRenderProcessor))]
    [DisableSuperClassRenderProcessor]
    [ComponentOrder(11001)]
    [ComponentCategory("Model")]
    public class CellModelComponent : ModelComponent
    {
        [DataMember(50)]
        public CellFlag Flag = CellFlag.Barrier;
    }
}
