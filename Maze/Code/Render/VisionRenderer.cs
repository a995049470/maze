using Stride.Core.Mathematics;
using Stride.Core;
using Stride.Rendering;
using Stride.Graphics;
using Stride.Core.Diagnostics;
using Stride.Rendering.Images;

namespace Maze.Code.Render
{
    public class LightAltas
    {
        public Texture LastTexture;
        public Texture CurrentTexture;

        public void Swap()
        {
            var temp = LastTexture;
            LastTexture = CurrentTexture;
            CurrentTexture = temp;
        }
    }

    [DataContract]
    public class VisionRenderer
    {
        private static readonly ProfilingKey Vision = new ProfilingKey(new ProfilingKey("Compositing"), "Vision");

        private LightAltas lightAltas;
        public int LightTexWidth = 1280;
        public int LightTexHeight = 720;

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
            CreateTexture(renderSystem, ref lightAltas.LastTexture, LightTexWidth, LightTexHeight, PixelFormat.R11G11B10_Float, TextureFlags.RenderTarget | TextureFlags.ShaderResource);
            CreateTexture(renderSystem, ref lightAltas.CurrentTexture, LightTexWidth, LightTexHeight, PixelFormat.R11G11B10_Float, TextureFlags.RenderTarget | TextureFlags.ShaderResource);
            CreateTexture(renderSystem, ref transmittanceTex, TransmittanceTexWidth, TransmittanceTexHeight, PixelFormat.R11G11B10_Float, TextureFlags.RenderTarget | TextureFlags.ShaderResource);

        }
        

        public void DrawView(RenderContext context, RenderDrawContext drawContext, RenderStage visionStage, RenderStage transmittanceStage)
        {
            var renderSystem = context.RenderSystem;
            CreateTextures(renderSystem);
            
            using(drawContext.QueryManager.BeginProfile(Color.Yellow, Vision))

            //渲染透光率
            using (drawContext.PushRenderTargetsAndRestore())
            {
                drawContext.CommandList.ResourceBarrierTransition(transmittanceTex, GraphicsResourceState.RenderTarget);
                drawContext.CommandList.Clear(transmittanceTex, Color4.White);
                drawContext.CommandList.SetRenderTarget(null, transmittanceTex);
                var viewPort = new Viewport(0, 0, transmittanceTex.Width, transmittanceTex.Height);
                drawContext.CommandList.SetViewport(viewPort);
                renderSystem.Draw(drawContext, context.RenderView, transmittanceStage);
            }
            //渲染格子亮度
            using (drawContext.PushRenderTargetsAndRestore())
            {
                drawContext.CommandList.ResourceBarrierTransition(lightAltas.CurrentTexture, GraphicsResourceState.RenderTarget);
                drawContext.CommandList.Clear(lightAltas.CurrentTexture, Color4.Black);
                drawContext.CommandList.SetRenderTarget(null, lightAltas.CurrentTexture);
                var viewport = new Viewport(0, 0, lightAltas.CurrentTexture.Width, lightAltas.CurrentTexture.Height);
                drawContext.CommandList.SetViewport(viewport);               
                renderSystem.Draw(drawContext, context.RenderView, visionStage);                     
            }
            
            //先重新计算一下灯光
            

            //然后灯光扩散

            
        }
        
    }
}
