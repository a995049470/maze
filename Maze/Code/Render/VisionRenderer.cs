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

        private Texture cellTex;
        public int CellTexWidth = 1280;
        public int CellTexHeight = 720;

        private Texture transmittanceTex;
        public int TransmittanceTexWidth = 1280;
        public int TransmittanceTexHeight = 720;

        public Color3 MyColor = new Color3(0.0f, 1.0f, 1.0f);
        
        private Texture dest;
        private ImageEffectShader test;
        
        public VisionRenderer()
        {
            test = new ImageEffectShader("TestImageEffect");
        }
        
        public void CreateTexture(RenderSystem renderSystem, ref Texture tex, int width, int height, PixelFormat pixelFormat, TextureFlags flags)
        {
            bool isNeedCreate = true;
           
            if (tex != null)
            {
                isNeedCreate = tex.Width != width || tex.Height != height;
                if (isNeedCreate)
                {
                    tex.ReleaseData();
                    tex = null; 
                }
            }
            if (isNeedCreate)
            {
                tex = Texture.New2D(renderSystem.GraphicsDevice, width, height, pixelFormat, flags);
            }
        }

        private void CreateTextures(RenderSystem renderSystem)
        {
            CreateTexture(renderSystem, ref cellTex, CellTexWidth, CellTexHeight, PixelFormat.R11G11B10_Float, TextureFlags.RenderTarget | TextureFlags.ShaderResource);
            CreateTexture(renderSystem, ref transmittanceTex, TransmittanceTexWidth, TransmittanceTexHeight, PixelFormat.R11G11B10_Float, TextureFlags.RenderTarget | TextureFlags.ShaderResource);

        }



        

        

        public void DrawView(RenderContext context, RenderDrawContext drawContext, RenderStage visionStage, RenderStage transmittanceStage)
        {
            var renderSystem = context.RenderSystem;
            CreateTextures(renderSystem);
            drawContext.CommandList.ResourceBarrierTransition(cellTex, GraphicsResourceState.RenderTarget);
            using(drawContext.QueryManager.BeginProfile(Color.Yellow, Vision))

            using (drawContext.PushRenderTargetsAndRestore())
            {
                drawContext.CommandList.Clear(transmittanceTex, Color4.White);
                drawContext.CommandList.SetRenderTarget(null, transmittanceTex);
                var viewPort = new Viewport(0, 0, transmittanceTex.Width, transmittanceTex.Height);
                drawContext.CommandList.SetViewport(viewPort);
                renderSystem.Draw(drawContext, context.RenderView, transmittanceStage);
            }

            using (drawContext.PushRenderTargetsAndRestore())
            {
                drawContext.CommandList.Clear(cellTex, Color4.Black);
                drawContext.CommandList.SetRenderTarget(null, cellTex);
                var viewport = new Viewport(0, 0, cellTex.Width, cellTex.Height);
                drawContext.CommandList.SetViewport(viewport);               
                renderSystem.Draw(drawContext, context.RenderView, visionStage);                     
            }

        }
        
    }
}
