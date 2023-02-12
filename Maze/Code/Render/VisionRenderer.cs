using Stride.Core.Mathematics;
using Stride.Core;
using Stride.Rendering;
using Stride.Graphics;
using Stride.Core.Diagnostics;
using Stride.Rendering.Images;

namespace Maze.Code.Render
{
    public class TextureBuffer
    {
        public Texture BackTexture;
        public Texture CurrentTexture;

        private Texture[] textures = new Texture[2];

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
        private TextureBuffer pixelLightBuffer;
        private TextureBuffer brightnessBuffer;
        private TextureBuffer lightDirBuffer;
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
            pixelLightBuffer = new TextureBuffer();
            brightnessBuffer = new TextureBuffer();
            lightDirBuffer = new TextureBuffer();
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
            var format = PixelFormat.R16G16B16A16_Float;
            var flag = TextureFlags.RenderTarget | TextureFlags.ShaderResource;
            var dirFormat = PixelFormat.R32G32_Float;
            CreateTexture(renderSystem, ref pixelLightBuffer.BackTexture, Width, Height, format, flag);
            CreateTexture(renderSystem, ref pixelLightBuffer.CurrentTexture, Width, Height, format, flag);
            
            CreateTexture(renderSystem, ref transmittanceTex, Width, Height, format, flag);
            
            CreateTexture(renderSystem, ref brightnessBuffer.CurrentTexture, Width, Height, format, flag);
            CreateTexture(renderSystem, ref brightnessBuffer.BackTexture, Width, Height, format, flag);

            CreateTexture(renderSystem, ref lightDirBuffer.CurrentTexture, Width, Height, dirFormat, flag);
            CreateTexture(renderSystem, ref lightDirBuffer.BackTexture, Width, Height, dirFormat, flag);
        }
        
        public Texture GetLightTexture()
        {
            return brightnessBuffer.CurrentTexture;
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
                drawContext.CommandList.ResourceBarrierTransition(pixelLightBuffer.CurrentTexture, GraphicsResourceState.RenderTarget);
                drawContext.CommandList.Clear(pixelLightBuffer.CurrentTexture, Color4.Black);
                drawContext.CommandList.SetRenderTarget(null, pixelLightBuffer.CurrentTexture);
                var viewport = new Viewport(0, 0, pixelLightBuffer.CurrentTexture.Width, pixelLightBuffer.CurrentTexture.Height);
                drawContext.CommandList.SetViewport(viewport);               
                renderSystem.Draw(drawContext, context.RenderView, visionStage);


                //先重新计算一下灯光
                drawContext.CommandList.ResourceBarrierTransition(pixelLightBuffer.CurrentTexture, GraphicsResourceState.PixelShaderResource);
                drawContext.CommandList.ResourceBarrierTransition(pixelLightBuffer.BackTexture, GraphicsResourceState.PixelShaderResource);
                drawContext.CommandList.ResourceBarrierTransition(lightDirBuffer.CurrentTexture, GraphicsResourceState.PixelShaderResource);

                drawContext.CommandList.ResourceBarrierTransition(brightnessBuffer.CurrentTexture, GraphicsResourceState.PixelShaderResource);
                drawContext.CommandList.ResourceBarrierTransition(brightnessBuffer.BackTexture, GraphicsResourceState.RenderTarget);
                drawContext.CommandList.ResourceBarrierTransition(lightDirBuffer.BackTexture, GraphicsResourceState.RenderTarget);

                lightUpdateEffect.SetInput(0, brightnessBuffer.CurrentTexture);
                lightUpdateEffect.SetInput(1, pixelLightBuffer.BackTexture);
                lightUpdateEffect.SetInput(2, pixelLightBuffer.CurrentTexture);
                lightUpdateEffect.SetOutput(brightnessBuffer.BackTexture);
                lightUpdateEffect.SetViewport(new Viewport(0, 0, brightnessBuffer.BackTexture.Width, brightnessBuffer.BackTexture.Height));
                lightUpdateEffect.Draw(drawContext);
                brightnessBuffer.Swap();

                //然后灯光扩散
                int diffusionCount = MathUtil.Clamp(DiffusionCount, 0, maxDiffusionCount);

                for (int i = 0; i < diffusionCount; i++)
                {
                    drawContext.CommandList.ResourceBarrierTransition(pixelLightBuffer.CurrentTexture, GraphicsResourceState.PixelShaderResource);
                    drawContext.CommandList.ResourceBarrierTransition(transmittanceTex, GraphicsResourceState.PixelShaderResource);
                    drawContext.CommandList.ResourceBarrierTransition(lightDirBuffer.CurrentTexture, GraphicsResourceState.PixelShaderResource);

                    drawContext.CommandList.ResourceBarrierTransition(brightnessBuffer.CurrentTexture, GraphicsResourceState.PixelShaderResource);
                    drawContext.CommandList.ResourceBarrierTransition(brightnessBuffer.BackTexture, GraphicsResourceState.RenderTarget);
                    drawContext.CommandList.ResourceBarrierTransition(lightDirBuffer.BackTexture, GraphicsResourceState.RenderTarget);

                    lightDiffusionEffect.SetInput(0, brightnessBuffer.CurrentTexture);
                    lightDiffusionEffect.SetInput(1, pixelLightBuffer.CurrentTexture);
                    lightDiffusionEffect.SetInput(2, transmittanceTex);
                    lightDiffusionEffect.SetInput(3, lightDirBuffer.CurrentTexture);
                    lightDiffusionEffect.SetOutput(brightnessBuffer.BackTexture, lightDirBuffer.BackTexture);
                    lightDiffusionEffect.SetViewport(new Viewport(0, 0, brightnessBuffer.BackTexture.Width, brightnessBuffer.BackTexture.Height));
                    lightDiffusionEffect.Draw(drawContext);
                    brightnessBuffer.Swap();
                    lightDirBuffer.Swap();
                }

                pixelLightBuffer.Swap();
                

            }                                       
        }
        
    }
}
