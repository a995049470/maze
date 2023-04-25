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
            if (((RenderGroupMask)(1U << (int)renderObject.RenderGroup) & RenderGroup) != 0 && renderObject is CellRenderMesh cellRenderMesh)
            {
                if(TransmittanceStage != null && cellRenderMesh.Flag == CellFlag.Barrier)
                {
                    renderObject.ActiveRenderStages[TransmittanceStage.Index] = new ActiveRenderStage(EffectName);
                }
                else if(VisionStage != null && cellRenderMesh.Flag == CellFlag.Light)
                {
                    renderObject.ActiveRenderStages[VisionStage.Index] = new ActiveRenderStage(EffectName);
                }
            }
        }
    }
}
