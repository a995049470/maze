using Stride.Core.Diagnostics;
using Stride.Graphics;
using Stride.Rendering.Images;
using Stride.Rendering;

using Stride.Core.Mathematics;
using Stride.Core;
using System.ComponentModel;


namespace Maze.Code.Render
{
    [DataContract]
    public class CellRenderer
    {

        private static readonly ProfilingKey Cell = new ProfilingKey(new ProfilingKey("Compositing"), "Cell");
        private int viewportWidthWS = 8;
        private int viewportHeightWS = 6;
        private int dpm = 8;

        [DataMember(10)]
        public int ViewportWidthWS 
        { 
            get => viewportWidthWS; 
            set { isDitry |= viewportWidthWS == value; viewportWidthWS = value; } 
        }
        [DataMember(20)]
        public int ViewportHeightWS
        {
            get => viewportHeightWS;
            set { isDitry |= viewportHeightWS == value; viewportHeightWS = value; }
        }
        [DataMember(30)]
        public int DPM
        {
            get => dpm;
            set { isDitry |= dpm == value; dpm = value; }
        }

        [DataMember(40)]
        [DefaultValue(null)]
        public RenderStage TransmittanceStage;

        [DataMember(50)]
        [DefaultValue(null)]
        public RenderStage VisionStage;



        private bool isDitry = true;


        private RectangleF drawRect;
        private RectangleF safeRect;


        private Int2 curTexSize = new Int2(0, 0);
        private TextureBuffer pixelLightBuffer;
        private TextureBuffer brightnessBuffer;
        private Texture transmittanceTex;
        private Texture finalCellTexture;
        

        public float AirTransmittance = 0.99f;
        public int DiffusionCount = 32;
        private const int maxDiffusionCount = 128;

        public Color3 MyColor = new Color3(0.0f, 1.0f, 1.0f);


        private ImageEffectShader lightUpdateEffect;
        private ImageEffectShader lightDiffusionEffect;
        private bool updateCell = false;
        [DataMemberIgnore]
        public CellRenderView RenderView { get; private set; } = new CellRenderView();

        [DataMember(50)]
        public Texture testTexture;


        public CellRenderer()
        {
            lightUpdateEffect = new ImageEffectShader("LightUpdateEffect");
            lightDiffusionEffect = new ImageEffectShader("LightDiffusionEffect");
            pixelLightBuffer = new TextureBuffer();
            brightnessBuffer = new TextureBuffer();
        }

        public void Collect(RenderContext context)
        {
            var camera = context.GetCurrentCamera();
            //var viewDir = camera.Entity.Transform.LocalMatrix.Forward;
            //updateCell = viewDir.Z * camera.Entity.Transform.Position.Z < 0;
            updateCell = true;
            updateCell &= VisionStage != null && TransmittanceStage != null;
            if (!updateCell) return;
            //todo:计算摄像机视口y=0平面投影的正交包围盒
            var cameraPos = camera.Entity.Transform.Position;
            //var center = cameraPos - cameraPos.Z / viewDir.Z * viewDir;
            var width = 2 * ViewportWidthWS * DPM;
            var height = 2 * viewportHeightWS * DPM;
            if (isDitry || !(safeRect.Contains(cameraPos.X, cameraPos.Z)))
            {
                isDitry = false;
                drawRect = new RectangleF
                    (
                       cameraPos.X - ViewportWidthWS,
                       cameraPos.Z - ViewportHeightWS,
                       ViewportWidthWS * 2,
                       viewportHeightWS * 2
                    );
                safeRect = new RectangleF
                    (
                        cameraPos.X - ViewportWidthWS * 0.5f,
                        cameraPos.Z - ViewportHeightWS * 0.5f,
                        ViewportWidthWS,
                        viewportHeightWS
                    );
                //if(width != curTexSize.X || height != curTexSize.Y)
                {
                    curTexSize = new Int2(width, height);
                    //TODO:需要迁移数据...
                    CreateTextures(context.RenderSystem, width, height);
                }
            }
            
            RenderView.ViewSize = new Vector2(width, height);
            CellRenderView.UpdateRenderRectToRenderView(drawRect, RenderView);
            RenderView.RenderStages.Add(VisionStage);
            RenderView.RenderStages.Add(TransmittanceStage);
            RenderView.CellTexture = finalCellTexture;
            context.RenderSystem.Views.Add(RenderView);
            context.RenderView.RenderStages.Add(VisionStage);
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

        private void CreateTextures(RenderSystem renderSystem, int width, int height)
        {
            var format = PixelFormat.R16G16B16A16_Float;
            var flag = TextureFlags.RenderTarget | TextureFlags.ShaderResource;
            CreateTexture(renderSystem, ref pixelLightBuffer.BackTexture, width, height, format, flag);
            CreateTexture(renderSystem, ref pixelLightBuffer.CurrentTexture, width, height, format, flag);

            CreateTexture(renderSystem, ref transmittanceTex, width, height, format, flag);

            CreateTexture(renderSystem, ref brightnessBuffer.CurrentTexture, width, height, format, flag);
            CreateTexture(renderSystem, ref brightnessBuffer.BackTexture, width, height, format, flag);
            CreateTexture(renderSystem, ref finalCellTexture, width, height, format, flag);

        }

        public Texture GetLightTexture()
        {
            return brightnessBuffer.CurrentTexture;
        }



        public void DrawView(RenderContext context, RenderDrawContext drawContext)
        {
            drawContext.CommandList.ResourceBarrierTransition(finalCellTexture, GraphicsResourceState.RenderTarget);
            drawContext.CommandList.Clear(finalCellTexture, new Color4(1, 1, 0, 1));
            //if (!updateCell) return;
            
            var renderSystem = context.RenderSystem;

            using (drawContext.QueryManager.BeginProfile(Color.Yellow, Cell))
            {
                //渲染透光率
                using (drawContext.PushRenderTargetsAndRestore())
                {
                    //drawContext.CommandList.ResourceBarrierTransition(transmittanceTex, GraphicsResourceState.RenderTarget);
                    //drawContext.CommandList.Clear(transmittanceTex, Color4.White * AirTransmittance);
                    //drawContext.CommandList.SetRenderTarget(null, transmittanceTex);
                    //var viewPort = new Viewport(0, 0, transmittanceTex.Width, transmittanceTex.Height);
                    //drawContext.CommandList.SetViewport(viewPort);
                    //renderSystem.Draw(drawContext, RenderView, TransmittanceStage);

                    //渲染格子亮度       
                    drawContext.CommandList.ResourceBarrierTransition(pixelLightBuffer.CurrentTexture, GraphicsResourceState.RenderTarget);
                    drawContext.CommandList.Clear(pixelLightBuffer.CurrentTexture, Color4.Black);
                    drawContext.CommandList.SetRenderTarget(null, pixelLightBuffer.CurrentTexture);
                    var viewport = new Viewport(0, 0, pixelLightBuffer.CurrentTexture.Width, pixelLightBuffer.CurrentTexture.Height);
                    drawContext.CommandList.SetViewport(viewport);
                    renderSystem.Draw(drawContext, RenderView, VisionStage);


                    //先重新计算一下灯光
                    drawContext.CommandList.ResourceBarrierTransition(pixelLightBuffer.CurrentTexture, GraphicsResourceState.PixelShaderResource);
                    drawContext.CommandList.ResourceBarrierTransition(pixelLightBuffer.BackTexture, GraphicsResourceState.PixelShaderResource);

                    drawContext.CommandList.ResourceBarrierTransition(brightnessBuffer.CurrentTexture, GraphicsResourceState.PixelShaderResource);
                    drawContext.CommandList.ResourceBarrierTransition(brightnessBuffer.BackTexture, GraphicsResourceState.RenderTarget);


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

                        drawContext.CommandList.ResourceBarrierTransition(brightnessBuffer.CurrentTexture, GraphicsResourceState.PixelShaderResource);
                        drawContext.CommandList.ResourceBarrierTransition(brightnessBuffer.BackTexture, GraphicsResourceState.RenderTarget);

                        lightDiffusionEffect.SetInput(0, brightnessBuffer.CurrentTexture);
                        lightDiffusionEffect.SetInput(1, pixelLightBuffer.CurrentTexture);
                        lightDiffusionEffect.SetInput(2, transmittanceTex);
                        lightDiffusionEffect.SetOutput(brightnessBuffer.BackTexture);
                        lightDiffusionEffect.SetViewport(new Viewport(0, 0, brightnessBuffer.BackTexture.Width, brightnessBuffer.BackTexture.Height));
                        lightDiffusionEffect.Draw(drawContext);
                        brightnessBuffer.Swap();
                    }

                    pixelLightBuffer.Swap();
                }

            }

        }
    }
}