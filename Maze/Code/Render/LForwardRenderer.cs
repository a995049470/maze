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

namespace Maze.Code.Render
{


    [DataContract]
    [Display("Forward Renderer EX")]
    public class LForwardRenderer : ForwardRenderer
    {
        public RenderStage VisionStage { get; set; }
        public RenderStage TransmittanceStage { get; set; }
        [NotNull]
        public VisionRenderer VisionRenderer = new VisionRenderer();
       




        public LForwardRenderer()
        { 
            
        }

        protected override void InitializeCore()
        {
            base.InitializeCore();
            Context.RenderSystem.RenderFeatures.CollectionChanged += OnRenderFeaturesChange;
            foreach (var renderFeature in Context.RenderSystem.RenderFeatures)
            {
                var e = new FastTrackingCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, renderFeature, null);
                OnRenderFeaturesChange(this, ref e);
            }
        }

        protected override void Destroy()
        {
            base.Destroy();
            Context.RenderSystem.RenderFeatures.CollectionChanged -= OnRenderFeaturesChange;
        }

        private void OnRenderFeaturesChange(object sender, ref FastTrackingCollectionChangedEventArgs e)
        {
            if(e.Item is ISpriteLitRenderFeature renderFeature)
            {
                switch (e.Action)
                {
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                        renderFeature.SetVisionRender(VisionRenderer);
                        break;
                            
                }
            }
        }

        protected override void CollectStages(RenderContext context)
        {
            base.CollectStages(context);
            if (VisionStage != null)
            {
                VisionStage.Output = new RenderOutputDescription(PixelFormat.R11G11B10_Float);
            }
            if(TransmittanceStage != null)
            {
                TransmittanceStage.Output = new RenderOutputDescription(PixelFormat.R11G11B10_Float);
            }
        }

        protected override void CollectView(RenderContext context)
        {
            base.CollectView(context);
            if (VisionStage != null)
            {
                context.RenderView.RenderStages.Add(VisionStage);
            }
            if(TransmittanceStage != null)
            {
                context.RenderView.RenderStages.Add(TransmittanceStage);
            }
        }

        protected override void DrawView(RenderContext context, RenderDrawContext drawContext, int eyeIndex, int eyeCount)
        {
            if(VisionStage != null && TransmittanceStage != null)
            {
                VisionRenderer?.DrawView(context, drawContext, VisionStage, TransmittanceStage);
            }
            base.DrawView(context, drawContext, eyeIndex, eyeCount);
        }

    }
}
