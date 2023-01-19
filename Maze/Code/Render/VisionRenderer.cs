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
        public Texture BackTexture;
        public Texture CurrentTexture;

        public void Swap()
        {
            var temp = BackTexture;
            BackTexture = CurrentTexture;
            CurrentTexture = temp;
        }
    }

    [DataContract]
    public class VisionRenderer
    {
        private static readonly ProfilingKey Vision = new ProfilingKey(new ProfilingKey("Compositing"), "Vision");

        public int Width = 320;
        public int Height = 180;
        private LightAltas lightAltas;
        private LightAltas brightnessAltas;
        private Texture transmittanceTex;   

        public float AirTransmittance = 0.99f;
        public int DiffusionCount = 32;
        private const int maxDiffusionCount = 128;

        public Color3 MyColor = new Color3(0.0f, 1.0f, 1.0f);
        
        
        private ImageEffectShader lightUpdateEffect;
        private ImageEffectShader lightDiffusionEffect;
        


        public VisionRenderer()
        {
            lightUpdateEffect = new ImageEffectShader("LightUpdateEffect");
            lightDiffusionEffect = new ImageEffectShader("LightDiffusionEffect");
            lightAltas = new LightAltas();
            brightnessAltas = new LightAltas();
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
            CreateTexture(renderSystem, ref lightAltas.BackTexture, Width, Height, PixelFormat.R11G11B10_Float, TextureFlags.RenderTarget | TextureFlags.ShaderResource);
            CreateTexture(renderSystem, ref lightAltas.CurrentTexture, Width, Height, PixelFormat.R11G11B10_Float, TextureFlags.RenderTarget | TextureFlags.ShaderResource);
            CreateTexture(renderSystem, ref transmittanceTex, Width, Height, PixelFormat.R11G11B10_Float, TextureFlags.RenderTarget | TextureFlags.ShaderResource);
            CreateTexture(renderSystem, ref brightnessAltas.CurrentTexture, Width, Height, PixelFormat.R11G11B10_Float, TextureFlags.RenderTarget | TextureFlags.ShaderResource);
            CreateTexture(renderSystem, ref brightnessAltas.BackTexture, Width, Height, PixelFormat.R11G11B10_Float, TextureFlags.RenderTarget | TextureFlags.ShaderResource);
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
                drawContext.CommandList.Clear(transmittanceTex, Color4.White * AirTransmittance);
                drawContext.CommandList.SetRenderTarget(null, transmittanceTex);
                var viewPort = new Viewport(0, 0, transmittanceTex.Width, transmittanceTex.Height);
                drawContext.CommandList.SetViewport(viewPort);
                renderSystem.Draw(drawContext, context.RenderView, transmittanceStage);
            
                //渲染格子亮度       
                drawContext.CommandList.ResourceBarrierTransition(lightAltas.CurrentTexture, GraphicsResourceState.RenderTarget);
                drawContext.CommandList.Clear(lightAltas.CurrentTexture, Color4.Black);
                drawContext.CommandList.SetRenderTarget(null, lightAltas.CurrentTexture);
                var viewport = new Viewport(0, 0, lightAltas.CurrentTexture.Width, lightAltas.CurrentTexture.Height);
                drawContext.CommandList.SetViewport(viewport);               
                renderSystem.Draw(drawContext, context.RenderView, visionStage);


                //先重新计算一下灯光
                drawContext.CommandList.ResourceBarrierTransition(lightAltas.CurrentTexture, GraphicsResourceState.PixelShaderResource);
                drawContext.CommandList.ResourceBarrierTransition(lightAltas.BackTexture, GraphicsResourceState.PixelShaderResource);

                drawContext.CommandList.ResourceBarrierTransition(brightnessAltas.CurrentTexture, GraphicsResourceState.PixelShaderResource);
                drawContext.CommandList.ResourceBarrierTransition(brightnessAltas.BackTexture, GraphicsResourceState.RenderTarget);

                lightUpdateEffect.SetInput(0, brightnessAltas.CurrentTexture);
                lightUpdateEffect.SetInput(1, lightAltas.BackTexture);
                lightUpdateEffect.SetInput(2, lightAltas.CurrentTexture);
                lightUpdateEffect.SetOutput(brightnessAltas.BackTexture);
                lightUpdateEffect.SetViewport(new Viewport(0, 0, brightnessAltas.BackTexture.Width, brightnessAltas.BackTexture.Height));
                lightUpdateEffect.Draw(drawContext);
                brightnessAltas.Swap();

                //然后灯光扩散
                int diffusionCount = MathUtil.Clamp(DiffusionCount, 0, maxDiffusionCount);

                for (int i = 0; i < diffusionCount; i++)
                {
                    drawContext.CommandList.ResourceBarrierTransition(lightAltas.CurrentTexture, GraphicsResourceState.PixelShaderResource);
                    drawContext.CommandList.ResourceBarrierTransition(transmittanceTex, GraphicsResourceState.PixelShaderResource);

                    drawContext.CommandList.ResourceBarrierTransition(brightnessAltas.CurrentTexture, GraphicsResourceState.PixelShaderResource);
                    drawContext.CommandList.ResourceBarrierTransition(brightnessAltas.BackTexture, GraphicsResourceState.RenderTarget);

                    lightDiffusionEffect.SetInput(0, brightnessAltas.CurrentTexture);
                    lightDiffusionEffect.SetInput(1, lightAltas.CurrentTexture);
                    lightDiffusionEffect.SetInput(2, transmittanceTex);
                    lightDiffusionEffect.SetOutput(brightnessAltas.BackTexture);
                    lightDiffusionEffect.SetViewport(new Viewport(0, 0, brightnessAltas.BackTexture.Width, brightnessAltas.BackTexture.Height));
                    lightDiffusionEffect.Draw(drawContext);
                    brightnessAltas.Swap();
                }

                lightAltas.Swap();
                

            }
            
            

            

            
        }
        
    }
}
