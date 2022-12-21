using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;
using Stride.Rendering.Compositing;
using Stride.Core;
using Stride.Rendering;
using Stride.Graphics;
using SharpDX.Direct3D11;
using Stride.Core.Diagnostics;
using System.ComponentModel;
using Stride.Rendering.Images;
using Stride.Core.Annotations;

namespace Maze.Code.Render
{
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

    [DataContract]
    public class VisionRenderer
    {
        private static readonly ProfilingKey Vision = new ProfilingKey(new ProfilingKey("Compositing"), "Vision");

        private Texture visionTex;
        private Texture dest;
        private ImageEffectShader test;
        public int Width = 1280;
        public int Height = 720;
        public Color3 MyColor = new Color3(0.0f, 1.0f, 1.0f);
        
        
        public VisionRenderer()
        {
            test = new ImageEffectShader("TestImageEffect");
        }
        
        public void CreateVisionTex(RenderSystem renderSystem, int width, int height)
        {
            bool isNeedCreate = true;
            if (visionTex != null)
            {
                isNeedCreate = visionTex.Description.Width != width || visionTex.Description.Height != height;
                if (isNeedCreate)
                {
                    visionTex.ReleaseData();
                    visionTex = null;
                    dest.ReleaseData();
                    dest = null;
                }
            }
            if (isNeedCreate)
            {
                visionTex = Texture.New2D(renderSystem.GraphicsDevice, width, height, PixelFormat.R32_Float, TextureFlags.RenderTarget | TextureFlags.ShaderResource);
                dest = Texture.New2D(renderSystem.GraphicsDevice, width, height, PixelFormat.R16G16B16A16_Float, TextureFlags.RenderTarget | TextureFlags.ShaderResource);
            }
        }

        public void DrawView(RenderContext context, RenderDrawContext drawContext, RenderStage visionStage)
        {
            var renderSystem = context.RenderSystem;
            CreateVisionTex(renderSystem, Width, Height);
            drawContext.CommandList.ResourceBarrierTransition(visionTex, GraphicsResourceState.RenderTarget);
            using(drawContext.QueryManager.BeginProfile(Color.Yellow, Vision))
            using(drawContext.PushRenderTargetsAndRestore())
            {
                drawContext.CommandList.Clear(visionTex, Color4.Black);
                drawContext.CommandList.SetRenderTarget(null, visionTex);
                renderSystem.Draw(drawContext, context.RenderView, visionStage);

                drawContext.CommandList.ResourceBarrierTransition(visionTex, GraphicsResourceState.PixelShaderResource);
                test.Parameters.Set(TestImageEffectKeys.MyColor, MyColor);
                test.SetInput(visionTex);
                test.SetOutput(dest);
                test.Draw(drawContext);
            }
        }
        
    }

    [Display("Forward Renderer EX")]
    public class ForwardRendererEx : ForwardRenderer
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
            if(VisionStage != null)
            {
                VisionRenderer?.DrawView(context, drawContext, VisionStage);
            }
            base.DrawView(context, drawContext, eyeIndex, eyeCount);
        }

    }
}
