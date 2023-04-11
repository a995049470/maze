using Stride.Engine;
using Stride.Core;
using Stride.Engine.Design;

namespace Maze.Code.Render
{
    [DataContract("CellModelComponent")]
    [Display("CellModel", Expand = ExpandRule.Once)]
    [DisableSuperClassRenderProcessor]
    [ComponentOrder(11001)]
    [ComponentCategory("Model")]
    public class CellModelComponent : ModelComponent
    {
        

    }
}
