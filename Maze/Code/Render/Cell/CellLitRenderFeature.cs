using Stride.Core;
using Stride.Core.Storage;
using Stride.Rendering;
using System;
using Stride.Graphics;

namespace Maze.Code.Render
{
    public class CellLitRenderFeature : SubRenderFeature
    {
        private LogicalGroupReference cellLitKey;
        [DataMember]
        public Texture TestTexture;

        protected override void InitializeCore()
        {
            base.InitializeCore();
            cellLitKey = ((RootEffectRenderFeature)RootRenderFeature).CreateViewLogicalGroup("CellLit");
        }

        public override void Prepare(RenderDrawContext context)
        {
            base.Prepare(context);
            CellRenderView targetView = null;
            foreach (var view in RenderSystem.Views)
            {
                if(view is CellRenderView cellRenderView)
                {
                    targetView = cellRenderView;
                    break;
                }
            }
            if (targetView == null) return;
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

                viewParameters.Set(CellLitKeys.CellViewProjectionMatrix, targetView.ViewProjection);

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
                    //if(targetView.CellTexture != null)
                    if(TestTexture != null)
                        resourceGroup.DescriptorSet.SetShaderResourceView(viewLighting.DescriptorSlotStart, TestTexture);
                }
            }
        }

    }
}
