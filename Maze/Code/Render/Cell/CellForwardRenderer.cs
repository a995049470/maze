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
using Stride.Core.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Maze.Code.Render
{

    [DataContract]
    [Display("Cell Forward Renderer")]
    public class CellForwardRenderer : ForwardRenderer
    {
        [NotNull]
        public CellRenderer CellRenderer = new CellRenderer();
        //[DefaultValue(null)]
        //public RenderStage VisionStage;
        //[DefaultValue(null)]
        //public RenderStage TransmittanceStage;


        public CellForwardRenderer()
        { 
        }

        //protected override void CollectView(RenderContext context)
        //{
        //    base.CollectView(context);
        //    //CellRenderer?.Collect(context);
        //}

        protected override void CollectStages(RenderContext context)
        {
            base.CollectStages(context);
            //if (VisionStage != null)
            //{
            //    VisionStage.Output = new RenderOutputDescription(PixelFormat.R11G11B10_Float);
            //}
            //if (TransmittanceStage != null)
            //{
            //    TransmittanceStage.Output = new RenderOutputDescription(PixelFormat.R11G11B10_Float);
            //}
        }

        protected override void CollectView(RenderContext context)
        {
            base.CollectView(context);
            CellRenderer?.Collect(context);
            //if (VisionStage != null)
            //{
            //    context.RenderView.RenderStages.Add(VisionStage);
            //}
            //if (TransmittanceStage != null)
            //{
            //    context.RenderView.RenderStages.Add(TransmittanceStage);
            //}
        }


        protected override void DrawView(RenderContext context, RenderDrawContext drawContext, int eyeIndex, int eyeCount)
        {
            CellRenderer?.DrawView(context, drawContext);
            base.DrawView(context, drawContext, eyeIndex, eyeCount);
        }

    }
}
