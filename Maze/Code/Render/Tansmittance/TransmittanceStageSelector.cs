using Stride.Rendering;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maze.Code.Render
{

    public class TransmittanceStageSelector : RenderStageSelector
    {
        [DefaultValue(RenderGroupMask.All)]
        public RenderGroupMask RenderGroup { get; set; } = RenderGroupMask.All;

        [DefaultValue(null)]
        public RenderStage TransmittanceRenderStage { get; set; }

        public string EffectName { get; set; }
        public override void Process(RenderObject renderObject)
        {           
            if (((RenderGroupMask)(1U << (int)renderObject.RenderGroup) & RenderGroup) != 0 && TransmittanceRenderStage != null)
            {
                renderObject.ActiveRenderStages[TransmittanceRenderStage.Index] = new ActiveRenderStage(EffectName);
            }
        }
    }
}
