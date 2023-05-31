using Stride.Core;
using Stride.Core.Storage;
using Stride.Rendering;
using System;
using Stride.Graphics;
using Stride.Core.Mathematics;

namespace Maze.Code.Render
{
    public class CellLitRenderFeature : SubRenderFeature
    {
        private LogicalGroupReference cellLitKey;
        private static Texture whiteTex;


        protected override void InitializeCore()
        {
            base.InitializeCore();
            cellLitKey = ((RootEffectRenderFeature)RootRenderFeature).CreateViewLogicalGroup("CellLighting");
        }

        private void InitDefalutTexture(RenderDrawContext context)
        {
            if (whiteTex is null)
            {
                var device = context.RenderContext.RenderSystem.GraphicsDevice;
                whiteTex = Texture.New2D(device, 1, 1, PixelFormat.B8G8R8A8_UNorm_SRgb, TextureFlags.ShaderResource | TextureFlags.RenderTarget);
                context.CommandList.ResourceBarrierTransition(whiteTex, GraphicsResourceState.RenderTarget);
                context.CommandList.Clear(whiteTex, Color4.White);
            }
        }

        public override void Prepare(RenderDrawContext context)
        {
            base.Prepare(context);
            InitDefalutTexture(context);
            CellRenderView targetView = null;

            foreach (var view in RenderSystem.Views)
            {
                if (view is CellRenderView cellRenderView)
                {
                    targetView = cellRenderView;
                    break;
                }
            }
            //if (targetView == null) return;
            foreach (var view in RenderSystem.Views)
            {
                if (view is CellRenderView) continue;
                var viewFeature = view.Features[RootRenderFeature.Index];
                ViewResourceGroupLayout firstViewLayout = null;
                if (viewFeature.Layouts.Count == 0) continue;
                foreach (var viewLayout in viewFeature.Layouts)
                {
                    // Only process view layouts in normal state
                    if (viewLayout.State != RenderEffectState.Normal)
                        continue;

                    var viewLighting = viewLayout.GetLogicalGroup(cellLitKey);
                    if (viewLighting.Hash != ObjectId.Empty)
                    {
                        firstViewLayout = viewLayout;
                        break;
                    }
                }
                if (firstViewLayout == null)
                    continue;
                var pvTestLayout = firstViewLayout.GetLogicalGroup(cellLitKey);
                var viewParameterLayout = new ParameterCollectionLayout();
                var viewParameters = new ParameterCollection();
                viewParameterLayout.ProcessLogicalGroup(firstViewLayout, ref pvTestLayout);
                viewParameters.UpdateLayout(viewParameterLayout);

                viewParameters.Set(CellLightingKeys.CellViewProjectionMatrix, targetView?.ViewProjection ?? Matrix.Identity);

                foreach (var viewLayout in viewFeature.Layouts)
                {
                    // Only process view layouts in normal state
                    if (viewLayout.State != RenderEffectState.Normal)
                        continue;

                    var viewLighting = viewLayout.GetLogicalGroup(cellLitKey);
                    if (viewLighting.Hash == ObjectId.Empty)
                        continue;

                    if (viewLighting.Hash != pvTestLayout.Hash)
                        throw new InvalidOperationException("PerView Lighting layout differs between different RenderObject in the same RenderView");

                    var resourceGroup = viewLayout.Entries[view.Index].Resources;

                    // Update resources
                    resourceGroup.UpdateLogicalGroup(ref viewLighting, viewParameters);

                    if (targetView?.CellTexture != null)
                        resourceGroup.DescriptorSet.SetShaderResourceView(viewLighting.DescriptorSlotStart, targetView.CellTexture);
                    else
                        resourceGroup.DescriptorSet.SetShaderResourceView(viewLighting.DescriptorSlotStart, whiteTex);


                }
            }
        }

    }
}
