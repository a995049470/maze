using Stride.Core;
using Stride.Rendering;
using System.ComponentModel;

namespace Maze.Code.Render
{
    [DataContract]
    public class VisionStageSelector : RenderStageSelector
    {
        [DefaultValue(RenderGroupMask.All)]
        public RenderGroupMask RenderGroup { get; set; } = RenderGroupMask.All;

        [DefaultValue(null)]
        public RenderStage VisionRenderStage { get; set; }

        public string EffectName { get; set; }
        public override void Process(RenderObject renderObject)
        {
            if (((RenderGroupMask)(1U << (int)renderObject.RenderGroup) & RenderGroup) != 0 && VisionRenderStage != null)
            {
                renderObject.ActiveRenderStages[VisionRenderStage.Index] = new ActiveRenderStage(EffectName);
            }
        }
    }
}
