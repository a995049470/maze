using Stride.Core.Mathematics;
using Stride.Core;
using Stride.Rendering;
using Stride.Graphics;
using Stride.Core.Diagnostics;
using Stride.Rendering.Images;

namespace Maze.Code.Render
{
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
                isNeedCreate = visionTex.Width != width || visionTex.Height != height;
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
                var viewport = new Viewport(0, 0, visionTex.Width, visionTex.Height);
                drawContext.CommandList.SetViewport(viewport);               
                renderSystem.Draw(drawContext, context.RenderView, visionStage);
               
                //drawContext.CommandList.ResourceBarrierTransition(visionTex, GraphicsResourceState.PixelShaderResource);
                //test.Parameters.Set(TestImageEffectKeys.MyColor, MyColor);
                //test.SetInput(visionTex);
                //test.SetOutput(dest);
                //test.Draw(drawContext);
            }
        }
        
    }
}
