using Stride.Rendering;
using System;
using System.ComponentModel;

namespace Maze.Code.Render
{
    public class CellStageSelector : RenderStageSelector
    {
        [DefaultValue(RenderGroupMask.All)]
        public RenderGroupMask RenderGroup { get; set; } = RenderGroupMask.All;
        [DefaultValue(null)]
        public RenderStage TransmittanceStage;
        [DefaultValue(null)]
        public RenderStage VisionStage;
        public string EffectName;

        public override void Process(RenderObject renderObject)
        {
            if (((RenderGroupMask)(1U << (int)renderObject.RenderGroup) & RenderGroup) != 0 && renderObject is RenderMesh renderMesh)
            {
                if(TransmittanceStage != null && (renderMesh.ModelFlag & ModelFlag.CellBarrier) != 0)
                {
                    renderObject.ActiveRenderStages[TransmittanceStage.Index] = new ActiveRenderStage(EffectName);
                }
                else if(VisionStage != null && (renderMesh.ModelFlag & ModelFlag.CellLight) != 0)
                {
                    renderObject.ActiveRenderStages[VisionStage.Index] = new ActiveRenderStage(EffectName);
                }
            }
        }
    }
}
