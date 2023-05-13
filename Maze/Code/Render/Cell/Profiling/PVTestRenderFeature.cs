using Stride.Core;
using Stride.Core.Mathematics;
using Stride.Core.Serialization;
using Stride.Core.Serialization.Contents;
using Stride.Core.Storage;
using Stride.Graphics;
using Stride.Rendering;
using System;


namespace Maze.Code.Profiling
{
    public class PVTestRenderFeature : SubRenderFeature
    {
        private LogicalGroupReference pvTestKey;
        [DataMember]
        public Color4 TestColor;
        [DataMember]
        public Texture TestTexture;

        

        protected override void InitializeCore()
        {
            base.InitializeCore();
            pvTestKey = ((RootEffectRenderFeature)RootRenderFeature).CreateViewLogicalGroup("PVTest");
            

        }

        public override void Prepare(RenderDrawContext context)
        {
            base.Prepare(context);
            foreach (var view in RenderSystem.Views)
            {
                var viewFeature = view.Features[RootRenderFeature.Index];
                ViewResourceGroupLayout firstViewLayout = null;
                if (viewFeature.Layouts.Count == 0) continue;
                foreach (var viewLayout in viewFeature.Layouts)
                {
                    // Only process view layouts in normal state
                    if (viewLayout.State != RenderEffectState.Normal)
                        continue;

                    var viewLighting = viewLayout.GetLogicalGroup(pvTestKey);
                    if (viewLighting.Hash != ObjectId.Empty)
                    {
                        firstViewLayout = viewLayout;
                        break;
                    }
                }
                if (firstViewLayout == null)
                    continue;
                var pvTestLayout = firstViewLayout.GetLogicalGroup(pvTestKey);
                var viewParameterLayout = new ParameterCollectionLayout();
                var viewParameters = new ParameterCollection();
                viewParameterLayout.ProcessLogicalGroup(firstViewLayout, ref pvTestLayout);
                viewParameters.UpdateLayout(viewParameterLayout);
                viewParameters.Set(PVTestKeys.PVTestColor, TestColor);

                foreach (var viewLayout in viewFeature.Layouts)
                {
                    // Only process view layouts in normal state
                    if (viewLayout.State != RenderEffectState.Normal)
                        continue;

                    var viewLighting = viewLayout.GetLogicalGroup(pvTestKey);
                    if (viewLighting.Hash == ObjectId.Empty)
                        continue;

                    if (viewLighting.Hash != pvTestLayout.Hash)
                        throw new InvalidOperationException("PerView Lighting layout differs between different RenderObject in the same RenderView");

                    var resourceGroup = viewLayout.Entries[view.Index].Resources;

                    // Update resources
                    resourceGroup.UpdateLogicalGroup(ref viewLighting, viewParameters);
                    if(TestTexture != null)
                        resourceGroup.DescriptorSet.SetShaderResourceView(viewLighting.DescriptorSlotStart, TestTexture);
                }
            }
        }

    }
}
