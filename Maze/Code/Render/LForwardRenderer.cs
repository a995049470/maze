﻿using System;
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
        private IVisionSpriteRenderFeature visionSpriteRenderFeature;

        protected override void InitializeCore()
        {
            base.InitializeCore();
            Context.RenderSystem.RenderFeatures.CollectionChanged += OnRenderFeaturesChange;
        }

        protected override void Destroy()
        {
            base.Destroy();
            Context.RenderSystem.RenderFeatures.CollectionChanged -= OnRenderFeaturesChange;
        }

        private void OnRenderFeaturesChange(object sender, ref FastTrackingCollectionChangedEventArgs e)
        {
            if(e.Item is IVisionSpriteRenderFeature renderFeature)
            {
                switch (e.Action)
                {
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                        visionSpriteRenderFeature = renderFeature;
                        break;
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                        if(visionSpriteRenderFeature == renderFeature) visionSpriteRenderFeature = null;
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
