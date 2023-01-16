using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Input;
using Stride.Engine;
using Stride.Rendering.Compositing;
using Stride.Core;
using Stride.Rendering;
using Stride.Graphics;
using SharpDX.Direct3D11;
using Stride.Core.Annotations;

namespace Maze.Code.Render
{
    [DataContract]
    [Display("Forward Renderer EX")]
    public class LForwardRenderer : ForwardRenderer
    {
        public RenderStage VisionStage { get; set; }
        [NotNull]
        public VisionRenderer VisionRenderer = new VisionRenderer();

        protected override void CollectStages(RenderContext context)
        {
            base.CollectStages(context);
            if (VisionStage != null)
            {
                VisionStage.Output = new RenderOutputDescription(PixelFormat.R32_Float, PixelFormat.D24_UNorm_S8_UInt);
            }
        }

        protected override void CollectView(RenderContext context)
        {
            base.CollectView(context);
            if (VisionStage != null)
            {
                context.RenderView.RenderStages.Add(VisionStage);
            }
        }

        protected override void DrawView(RenderContext context, RenderDrawContext drawContext, int eyeIndex, int eyeCount)
        {
            //if(VisionStage != null)
            //{
            //    VisionRenderer?.DrawView(context, drawContext, VisionStage);
            //}
            base.DrawView(context, drawContext, eyeIndex, eyeCount);
        }

    }
}
