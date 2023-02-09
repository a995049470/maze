using Stride.Core;
using Stride.Graphics;
using Stride.Rendering.Sprites;
using Stride.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Stride.Core.Mathematics;

namespace Maze.Code.Render
{
    public abstract class BaseSpriteRenderFeature<TRenderSprite, USpriteBatch> : RootRenderFeature  where TRenderSprite : RenderSprite where USpriteBatch : Sprite3DBatch
    {
        private ThreadLocal<ThreadContext<USpriteBatch>> threadContext;

        private Dictionary<BlendModes, BlendStateDescription> blendModeToDescription = new Dictionary<BlendModes, BlendStateDescription>();
        private Dictionary<SpriteBlend, BlendModes> spriteBlendToBlendMode = new Dictionary<SpriteBlend, BlendModes>();

        public override Type SupportedRenderObjectType => typeof(TRenderSprite);
        
        protected abstract string effectName { get; }
        protected abstract string alphaCutoffEffectName { get; }

        private enum BlendModes
        {
            Default = 0,
            Additive = 1,
            Alpha = 2,
            NonPremultiplied = 3,
            NoColor = 4,
        }

        protected abstract USpriteBatch GetSprite3DBatch();

        protected override void InitializeCore()
        {
            base.InitializeCore();
 
            threadContext = new ThreadLocal<ThreadContext<USpriteBatch>>(() => new ThreadContext<USpriteBatch>(Context.GraphicsDevice, GetSprite3DBatch(), effectName, null, alphaCutoffEffectName), true);

            spriteBlendToBlendMode[SpriteBlend.None] = BlendModes.Default;
            spriteBlendToBlendMode[SpriteBlend.AdditiveBlend] = BlendModes.Additive;
            spriteBlendToBlendMode[SpriteBlend.NoColor] = BlendModes.NoColor;

            blendModeToDescription[BlendModes.Default] = BlendStates.Default;
            blendModeToDescription[BlendModes.Additive] = BlendStates.Additive;
            blendModeToDescription[BlendModes.Alpha] = BlendStates.AlphaBlend;
            blendModeToDescription[BlendModes.NonPremultiplied] = BlendStates.NonPremultiplied;
            blendModeToDescription[BlendModes.NoColor] = BlendStates.ColorDisabled;
        }

        protected override void Destroy()
        {
            if (threadContext == null)
            {
                return;
            }

            base.Destroy();

            foreach (var context in threadContext.Values)
            {
                context.Dispose();
            }
            threadContext.Dispose();
        }

        protected virtual void SpriteBatchDraw(ThreadContext<USpriteBatch> batchContext, TRenderSprite renderSprite, Sprite sprite, float projectedZ, ref RectangleF sourceRegion, Texture texture, ref Color4 color, ref Matrix worldMatrix, ref Vector2 spriteSize)
        {
            batchContext.SpriteBatch.Draw(texture, ref worldMatrix, ref sourceRegion, ref spriteSize, ref color, sprite.Orientation, renderSprite.Swizzle, projectedZ);
        }


        public override void Draw(RenderDrawContext context, RenderView renderView, RenderViewStage renderViewStage, int startIndex, int endIndex)
        {
            base.Draw(context, renderView, renderViewStage, startIndex, endIndex);

            var isMultisample = RenderSystem.RenderStages[renderViewStage.Index].Output.MultisampleCount != MultisampleCount.None;

            var batchContext = threadContext.Value;

            Matrix viewInverse;
            Matrix.Invert(ref renderView.View, out viewInverse);

            uint previousBatchState = uint.MaxValue;

            //TODO string comparison ...?
            var isPicking = RenderSystem.RenderStages[renderViewStage.Index].Name == "Picking";

            bool hasBegin = false;
            for (var index = startIndex; index < endIndex; index++)
            {
                var renderNodeReference = renderViewStage.SortedRenderNodes[index].RenderNode;
                var renderNode = GetRenderNode(renderNodeReference);

                var renderSprite = (TRenderSprite)renderNode.RenderObject;

                var sprite = renderSprite.Sprite;
                if (sprite == null)
                    continue;

                // TODO: this should probably be moved to Prepare()
                // Project the position
                // TODO: This could be done in a SIMD batch, but we need to figure-out how to plugin in with RenderMesh object
                var worldPosition = new Vector4(renderSprite.WorldMatrix.TranslationVector, 1.0f);

                Vector4 projectedPosition;
                Vector4.Transform(ref worldPosition, ref renderView.ViewProjection, out projectedPosition);
                var projectedZ = projectedPosition.Z / projectedPosition.W;

                BlendModes blendMode;
                EffectInstance currentEffect = batchContext.GetOrCreateEffect(RenderSystem.EffectSystem);
                if (isPicking)
                {
                    blendMode = BlendModes.Default;
                    currentEffect = batchContext.GetOrCreatePickingSpriteEffect(RenderSystem.EffectSystem);
                }
                else
                {
                    var spriteBlend = renderSprite.BlendMode;
                    if (spriteBlend == SpriteBlend.Auto)
                        spriteBlend = sprite.IsTransparent ? SpriteBlend.AlphaBlend : SpriteBlend.None;

                    if (spriteBlend == SpriteBlend.AlphaBlend)
                    {
                        blendMode = renderSprite.PremultipliedAlpha ? BlendModes.Alpha : BlendModes.NonPremultiplied;
                    }
                    else
                    {
                        blendMode = spriteBlendToBlendMode[spriteBlend];
                    }
                }

                // Check if the current blend state has changed in any way, if not
                // Note! It doesn't really matter in what order we build the bitmask, the result is not preserved anywhere except in this method
                var currentBatchState = (uint)blendMode;
                currentBatchState = (currentBatchState << 1) + (renderSprite.IgnoreDepth ? 1U : 0U);
                currentBatchState = (currentBatchState << 1) + (renderSprite.IsAlphaCutoff ? 1U : 0U);
                currentBatchState = (currentBatchState << 2) + ((uint)renderSprite.Sampler);

                if (previousBatchState != currentBatchState)
                {
                    var blendState = blendModeToDescription[blendMode];

                    if (renderSprite.IsAlphaCutoff)
                        currentEffect = batchContext.GetOrCreateAlphaCutoffSpriteEffect(RenderSystem.EffectSystem);

                    var depthStencilState = renderSprite.IgnoreDepth ? DepthStencilStates.None : DepthStencilStates.Default;

                    var samplerState = context.GraphicsDevice.SamplerStates.LinearClamp;
                    if (renderSprite.Sampler != SpriteSampler.LinearClamp)
                    {
                        switch (renderSprite.Sampler)
                        {
                            case SpriteSampler.PointClamp:
                                samplerState = context.GraphicsDevice.SamplerStates.PointClamp;
                                break;
                            case SpriteSampler.AnisotropicClamp:
                                samplerState = context.GraphicsDevice.SamplerStates.AnisotropicClamp;
                                break;
                        }
                    }

                    if (hasBegin)
                    {
                        batchContext.SpriteBatch.End();
                    }

                    var rasterizerState = RasterizerStates.CullNone;
                    if (isMultisample)
                    {
                        rasterizerState.MultisampleCount = RenderSystem.RenderStages[renderViewStage.Index].Output.MultisampleCount;
                        rasterizerState.MultisampleAntiAliasLine = true;
                    }

                    batchContext.SpriteBatch.Begin(context.GraphicsContext, renderView.ViewProjection, SpriteSortMode.Deferred, blendState, samplerState, depthStencilState, rasterizerState, currentEffect);
                    hasBegin = true;
                }
                previousBatchState = currentBatchState;

                var sourceRegion = sprite.Region;
                var texture = sprite.Texture;
                var color = renderSprite.Color;
                if (isPicking) // TODO move this code corresponding to picking out of the runtime code.
                {
                    var compId = RuntimeIdHelper.ToRuntimeId(renderSprite.Source);
                    color = new Color4(compId, 0.0f, 0.0f, 0.0f);
                }

                // skip the sprite if no texture is set.
                if (texture == null)
                    continue;

                // determine the element world matrix depending on the type of sprite
                var worldMatrix = renderSprite.WorldMatrix;
                if (renderSprite.SpriteType == SpriteType.Billboard)
                {
                    worldMatrix = viewInverse;

                    var worldMatrixRow1 = worldMatrix.Row1;
                    var worldMatrixRow2 = worldMatrix.Row2;

                    // remove scale of the camera
                    worldMatrixRow1 /= ((Vector3)viewInverse.Row1).Length();
                    worldMatrixRow2 /= ((Vector3)viewInverse.Row2).Length();

                    // set the scale of the object
                    worldMatrixRow1 *= ((Vector3)renderSprite.WorldMatrix.Row1).Length();
                    worldMatrixRow2 *= ((Vector3)renderSprite.WorldMatrix.Row2).Length();

                    worldMatrix.Row1 = worldMatrixRow1;
                    worldMatrix.Row2 = worldMatrixRow2;

                    // set the position
                    worldMatrix.TranslationVector = renderSprite.WorldMatrix.TranslationVector;

                    // set the rotation
                    var localRotationZ = renderSprite.RotationEulerZ;
                    if (localRotationZ != 0)
                        worldMatrix = Matrix.RotationZ(localRotationZ) * worldMatrix;
                }

                // calculate normalized position of the center of the sprite (takes into account the possible rotation of the image)
                var normalizedCenter = new Vector2(sprite.Center.X / sourceRegion.Width - 0.5f, 0.5f - sprite.Center.Y / sourceRegion.Height);
                if (sprite.Orientation == ImageOrientation.Rotated90)
                {
                    var oldCenterX = normalizedCenter.X;
                    normalizedCenter.X = -normalizedCenter.Y;
                    normalizedCenter.Y = oldCenterX;
                }
                // apply the offset due to the center of the sprite
                var centerOffset = Vector2.Modulate(normalizedCenter, sprite.Size);
                worldMatrix.M41 -= centerOffset.X * worldMatrix.M11 + centerOffset.Y * worldMatrix.M21;
                worldMatrix.M42 -= centerOffset.X * worldMatrix.M12 + centerOffset.Y * worldMatrix.M22;
                worldMatrix.M43 -= centerOffset.X * worldMatrix.M13 + centerOffset.Y * worldMatrix.M23;

                // adapt the source region to match what is expected at full resolution
                if (texture.ViewType == ViewType.Full && texture.ViewWidth != texture.FullQualitySize.Width)
                {
                    var fullQualitySize = texture.FullQualitySize;
                    var horizontalRatio = texture.ViewWidth / (float)fullQualitySize.Width;
                    var verticalRatio = texture.ViewHeight / (float)fullQualitySize.Height;
                    sourceRegion.X *= horizontalRatio;
                    sourceRegion.Width *= horizontalRatio;
                    sourceRegion.Y *= verticalRatio;
                    sourceRegion.Height *= verticalRatio;
                }

                // register resource usage.
                Context.StreamingManager?.StreamResources(texture);
                var spriteSize = sprite.Size;
                // draw the sprite
                SpriteBatchDraw(batchContext, renderSprite, sprite, projectedZ, ref sourceRegion, texture, ref color, ref worldMatrix, ref spriteSize);
            }

            if (hasBegin)
            {
                batchContext.SpriteBatch.End();
            }
        }

       
        

        protected class ThreadContext<TSprite3DBatch> : IDisposable where TSprite3DBatch : Sprite3DBatch
        {
            private bool isSrgb;
            private EffectInstance effect;
            private EffectInstance pickingEffect;
            private EffectInstance alphaCutoffEffect;

            private string effectName = "";
            private string pickingEffectName = "SpritePicking";
            private string alphaCutoffEffectName = "SpriteAlphaCutoffEffect";

            public TSprite3DBatch SpriteBatch { get; }

            public ThreadContext(GraphicsDevice device, TSprite3DBatch spriteBatch, string _effectName, string _pickingEffectName, string _alphaCutoffEffectName)
            {
                isSrgb = device.ColorSpace == ColorSpace.Gamma;
                SpriteBatch = spriteBatch;
                if (!string.IsNullOrEmpty(_effectName)) effectName = _effectName;
                if (!string.IsNullOrEmpty(_pickingEffectName)) pickingEffectName = _pickingEffectName;
                if (!string.IsNullOrEmpty(_alphaCutoffEffectName)) alphaCutoffEffectName = _alphaCutoffEffectName;
            }

            public EffectInstance GetOrCreateEffect(EffectSystem effectSystem)
            {
                if (string.IsNullOrEmpty(effectName)) return null;
                return null;
                if(effect == null)
                {
                    effect = new EffectInstance(effectSystem.LoadEffect(effectName).WaitForResult());
                    effect.Parameters.Set(SpriteBaseKeys.ColorIsSRgb, isSrgb);
                }
                return effect;
            }

            public EffectInstance GetOrCreatePickingSpriteEffect(EffectSystem effectSystem)
            {
                return pickingEffect ?? (pickingEffect = new EffectInstance(effectSystem.LoadEffect(pickingEffectName).WaitForResult()));
            }

            public EffectInstance GetOrCreateAlphaCutoffSpriteEffect(EffectSystem effectSystem)
            {
                if (alphaCutoffEffect != null)
                    return alphaCutoffEffect;

                alphaCutoffEffect = new EffectInstance(effectSystem.LoadEffect(alphaCutoffEffectName).WaitForResult());
                alphaCutoffEffect.Parameters.Set(SpriteBaseKeys.ColorIsSRgb, isSrgb);

                return alphaCutoffEffect;
            }

            public void Dispose()
            {
                SpriteBatch.Dispose();
            }
        }
    }
}
