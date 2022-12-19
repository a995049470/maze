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

namespace Maze.Code.Render
{
    public class VisionRenderer
    {
        private Texture visionTex;

        public void Set(RenderSystem renderSystem, int width, int height)
        {
            bool isNeedCreate = true;
            if (visionTex != null)
            {
                isNeedCreate = visionTex.Description.Width != width || visionTex.Description.Height != height;
                if (isNeedCreate)
                {
                    visionTex.ReleaseData();
                    visionTex = null;
                }
            }
            if (isNeedCreate)
            {
                visionTex = Texture.New2D(renderSystem.GraphicsDevice, width, height, PixelFormat.R32_Float, TextureFlags.RenderTarget | TextureFlags.ShaderResource);
            }
        }
    }

    [Display("Forward Renderer(扩展)")]
    public class ForwardRendererEx : ForwardRenderer
    {
        public RenderStage VisionStage { get; set; }
        private VisionRenderer visionRenderer = new VisionRenderer();

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

    }
}
