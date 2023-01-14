using Microsoft.VisualBasic.Logging;
using Stride.Core.Mathematics;
using Stride.Graphics;
using Stride.Rendering;
using System;

namespace Maze.Code.Render
{
    public class CellSprite3DBatch : Sprite3DBatch
    {
        public class DrawParameter
        {
            public Texture Texture;
            public float CellValue;
        }


        protected ValueParameter<float>? cellValueUpdater;
        private EffectInstance defaultSpriteEffect;
        private DrawParameter[] drawParameters;



        public CellSprite3DBatch(GraphicsDevice device, int bufferElementCount = 1024, int batchCapacity = 64) : base(device, bufferElementCount, batchCapacity)
        {
            drawParameters = new DrawParameter[batchCapacity];
        }

        public void Draw(Texture texture, float cellValue, ref Matrix worldMatrix, ref RectangleF sourceRectangle, ref Vector2 elementSize, ref Color4 color,
                         ImageOrientation imageOrientation = ImageOrientation.AsIs, SwizzleMode swizzle = SwizzleMode.None, float? depth = null)
        {
            // Check that texture is not null
            if (texture == null)
                throw new ArgumentNullException("texture");

            // Skip items with null size
            if (elementSize.LengthSquared() < MathUtil.ZeroTolerance)
                return;

            // Calculate the information needed to draw.
            var drawInfo = new Sprite3DDrawInfo
            {
                Source =
                {
                    X = sourceRectangle.X / texture.ViewWidth,
                    Y = sourceRectangle.Y / texture.ViewHeight,
                    Width = sourceRectangle.Width / texture.ViewWidth,
                    Height = sourceRectangle.Height / texture.ViewHeight,
                },
                ColorScale = color,
                ColorAdd = new Color4(0, 0, 0, 0),
                Swizzle = swizzle,
            };


            var matrix = worldMatrix;
            matrix.M11 *= elementSize.X;
            matrix.M12 *= elementSize.X;
            matrix.M13 *= elementSize.X;
            matrix.M21 *= elementSize.Y;
            matrix.M22 *= elementSize.Y;
            matrix.M23 *= elementSize.Y;

            Vector4.Transform(ref vector4UnitX, ref matrix, out drawInfo.UnitXWorld);
            Vector4.Transform(ref vector4UnitY, ref matrix, out drawInfo.UnitYWorld);

            // rotate origin and unit axis if need.
            var leftTopCorner = new Vector4(-0.5f, 0.5f, 0, 1);
            if (imageOrientation == ImageOrientation.Rotated90)
            {
                var unitX = drawInfo.UnitXWorld;
                drawInfo.UnitXWorld = -drawInfo.UnitYWorld;
                drawInfo.UnitYWorld = unitX;
                leftTopCorner = new Vector4(-0.5f, -0.5f, 0, 1);
            }
            Vector4.Transform(ref leftTopCorner, ref matrix, out drawInfo.LeftTopCornerWorld);

            float depthSprite;
            if (depth.HasValue)
            {
                depthSprite = depth.Value;
            }
            else
            {
                Vector4 projectedPosition;
                var worldPosition = new Vector4(worldMatrix.TranslationVector, 1.0f);
                Vector4.Transform(ref worldPosition, ref transformationMatrix, out projectedPosition);
                depthSprite = projectedPosition.Z / projectedPosition.W;
            }

            var elementInfo = new ElementInfo(StaticQuadBufferInfo.VertexByElement, StaticQuadBufferInfo.IndicesByElement, in drawInfo, depthSprite);
            var drawParameter = new DrawParameter();
            drawParameter.Texture = texture;
            drawParameter.CellValue = cellValue;
            Draw(drawParameter, in elementInfo);
        }


        protected void Draw(DrawParameter parameter, in ElementInfo elementInfo)
        {
            // Make sure that Begin was called
            CheckBeginHasBeenCalled("draw");

            // Resize the buffer of SpriteInfo
            if (drawsQueueCount >= drawsQueue.Length)
            {
                Array.Resize(ref drawsQueue, drawsQueue.Length * 2);
            }

            // set the info required to draw the image
            drawsQueue[drawsQueueCount] = elementInfo;

            // If we are in immediate mode, render the sprite directly
            if (sortMode == SpriteSortMode.Immediate)
            {
                DrawBatchPerTexture(parameter, drawsQueue, 0, 1);
            }
            else
            {
                if (drawParameters.Length < drawsQueue.Length)
                {
                    Array.Resize(ref drawParameters, drawsQueue.Length);
                }
                drawParameters[drawsQueueCount] = parameter;
                drawsQueueCount++;
            }
        }

        protected void DrawBatchPerTexture(DrawParameter parameter, ElementInfo[] sprites, int offset, int count)
        {
            // Sets the texture for this sprite effect.
            // Use an optimized version in order to avoid to reapply the sprite effect here just to change texture
            // We are calling the PixelShaderStage directly. We assume that the texture is on slot 0 as it is
            // setup in the original BasicEffect.fx shader.
            if (textureUpdater.HasValue)
                Effect.Parameters.Set(textureUpdater.Value, parameter.Texture);
            if (cellValueUpdater.HasValue)
                Effect.Parameters.Set(cellValueUpdater.Value, parameter.CellValue);
            Effect.Apply(GraphicsContext);

            // Draw the batch of sprites
            DrawBatchPerTextureAndPass(sprites, offset, count);
        }

        protected override void PrepareParameters()
        {
            base.PrepareParameters();
            cellValueUpdater = null;
            if (Effect.Effect.HasParameter(CellSpriteKeys.CellValue))
                cellValueUpdater = Effect.Parameters.GetAccessor(CellSpriteKeys.CellValue);
            
        }

        protected override void FlushBatch()
        {
            ElementInfo[] spriteQueueForBatch;

            // If Deferred, then sprites are displayed in the same order they arrived
            if (sortMode == SpriteSortMode.Deferred)
            {
                spriteQueueForBatch = drawsQueue;
            }
            else
            {
                // Else Sort all sprites according to their sprite order mode.
                SortSprites();
                spriteQueueForBatch = sortedDraws;
            }

            // Iterate on all sprites and group batch per texture.
            int offset = 0;
            DrawParameter previousDrawParameter = null;
            for (int i = 0; i < drawsQueueCount; i++)
            {
                DrawParameter drawParameter;

                if (sortMode == SpriteSortMode.Deferred)
                {
                    drawParameter = drawParameters[i];
                }
                else
                {
                    // Copy ordered sprites to the queue to batch
                    int index = sortIndices[i];
                    spriteQueueForBatch[i] = drawsQueue[index];

                    // Get the texture indirectly
                    drawParameter = drawParameters[index];
                }

                if (previousDrawParameter == null ||
                    drawParameter.Texture != previousDrawParameter.Texture ||
                    drawParameter.CellValue != previousDrawParameter.CellValue)
                {
                    if (i > offset)
                    {
                        DrawBatchPerTexture(drawParameter, spriteQueueForBatch, offset, i - offset);
                    }

                    offset = i;
                    previousDrawParameter = drawParameter;
                }
            }

            // Draw the last batch
            DrawBatchPerTexture(previousDrawParameter, spriteQueueForBatch, offset, drawsQueueCount - offset);

            // Reset the queue.
            Array.Clear(drawParameters, 0, drawsQueueCount);
            drawsQueueCount = 0;

            // When sorting is disabled, we persist mSortedSprites data from one batch to the next, to avoid
            // unnecessary work in GrowSortedSprites. But we never reuse these when sorting, because re-sorting
            // previously sorted items gives unstable ordering if some sprites have identical sort keys.
            if (sortMode != SpriteSortMode.Deferred)
            {
                Array.Clear(sortedDraws, 0, sortedDraws.Length);
            }
        }


    }
}
