﻿// <auto-generated>
// Do not edit this file yourself!
//
// This code was generated by Stride Shader Mixin Code Generator.
// To generate it yourself, please install Stride.VisualStudio.Package .vsix
// and re-save the associated .sdfx.
// </auto-generated>

using System;
using Stride.Core;
using Stride.Rendering;
using Stride.Graphics;
using Stride.Shaders;
using Stride.Core.Mathematics;
using Buffer = Stride.Graphics.Buffer;

using Stride.Rendering.Data;
using Stride.Rendering.Materials;
namespace Stride.Rendering
{
    internal static partial class ShaderMixins
    {
        internal partial class StrideForwardShadingEffectUnlit  : IShaderMixinBuilder
        {
            public void Generate(ShaderMixinSource mixin, ShaderMixinContext context)
            {
                context.Mixin(mixin, "StrideEffectBase");
                ShaderSource extensionPixelStageSurfaceShaders = context.GetParam(MaterialKeys.PixelStageSurfaceShaders);
                if (extensionPixelStageSurfaceShaders != null)
                {
                    context.Mixin(mixin, "MaterialSurfacePixelStageCompositor");

                    {
                        var __mixinToCompose__ = (extensionPixelStageSurfaceShaders);
                        var __subMixin = new ShaderMixinSource();
                        context.PushComposition(mixin, "materialPixelStage", __subMixin);
                        context.Mixin(__subMixin, __mixinToCompose__);
                        context.PopComposition();
                    }

                    {
                        var __mixinToCompose__ = context.GetParam(MaterialKeys.PixelStageStreamInitializer);
                        var __subMixin = new ShaderMixinSource();
                        context.PushComposition(mixin, "streamInitializerPixelStage", __subMixin);
                        context.Mixin(__subMixin, __mixinToCompose__);
                        context.PopComposition();
                    }
                    ShaderSource extensionPixelStageSurfaceFilter = context.GetParam(MaterialKeys.PixelStageSurfaceFilter);
                    if (extensionPixelStageSurfaceFilter != null)
                    {
                        context.Mixin(mixin, (extensionPixelStageSurfaceFilter));
                    }
                    if (context.ChildEffectName == "GBuffer")
                    {
                        context.Mixin(mixin, "GBuffer");
                        return;
                    }
                }
                if (context.ChildEffectName == "ShadowMapCaster")
                {
                    context.Mixin(mixin, "ShadowMapCaster");
                    return;
                }
                if (context.ChildEffectName == "ShadowMapCasterParaboloid")
                {
                    context.Mixin(mixin, "ShadowMapCasterParaboloid");
                    return;
                }
                if (context.ChildEffectName == "ShadowMapCasterCubeMap")
                {
                    context.Mixin(mixin, "ShadowMapCasterCubeMap");
                    return;
                }
            }

            [ModuleInitializer]
            internal static void __Initialize__()

            {
                ShaderMixinManager.Register("StrideForwardShadingEffectUnlit", new StrideForwardShadingEffectUnlit());
            }
        }
    }
}
