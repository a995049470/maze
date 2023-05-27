// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Stride.Graphics;
using Stride.Rendering;
using System.ComponentModel;

namespace Maze.Code.Rendering
{
    /// <summary>
    /// Pipeline processor for <see cref="RenderMesh"/> with materials. It will set blend and depth-stencil state for transparent objects, and properly set culling according to material and negative scaling.
    /// </summary>
    public class CellMeshPipelineProcessor : PipelineProcessor
    {

        [DefaultValue(null)]
        public RenderStage TransmittanceStage;

        [DefaultValue(null)]
        public RenderStage VisionStage;

        public override void Process(RenderNodeReference renderNodeReference, ref RenderNode renderNode, RenderObject renderObject, PipelineStateDescription pipelineState)
        {
            var output = renderNode.RenderStage.Output;
            var isMultisample = output.MultisampleCount != MultisampleCount.None;
            var renderMesh = (RenderMesh)renderObject;
            
            // Make object in transparent stage use AlphaBlend and DepthRead
            if (renderNode.RenderStage == TransmittanceStage)
            {
                pipelineState.DepthStencilState = new DepthStencilStateDescription(false, false);
                pipelineState.BlendState = BlendStates.AlphaBlend;
                pipelineState.DepthStencilState = DepthStencilStates.Default;
                if (isMultisample)
                    pipelineState.BlendState.AlphaToCoverageEnable = renderMesh.MaterialPass.AlphaToCoverage ?? true;
            }
            else if (renderNode.RenderStage == VisionStage)
            {
                pipelineState.DepthStencilState = new DepthStencilStateDescription(false, false);
                pipelineState.BlendState = BlendStates.Additive;
                pipelineState.DepthStencilState = DepthStencilStates.Default;
                if (isMultisample)
                    pipelineState.BlendState.AlphaToCoverageEnable = renderMesh.MaterialPass.AlphaToCoverage ?? true;
            }



            
        }
    }
}
