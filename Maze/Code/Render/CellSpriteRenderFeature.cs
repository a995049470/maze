using Stride.Core.Mathematics;
using Stride.Graphics;
using Stride.Rendering.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maze.Code.Render
{
    public class CellSpriteRenderFeature : BaseSpriteRenderFeature<RenderCellSprite, CellSprite3DBatch>
    {
        protected override string effectName => "CellSpriteEffect";

        protected override string alphaCutoffEffectName => "";

        protected override CellSprite3DBatch GetSprite3DBatch()
        {
            return new CellSprite3DBatch(Context.GraphicsDevice);
        }

        protected override void SpriteBatchDraw(ThreadContext<CellSprite3DBatch> batchContext, RenderCellSprite renderSprite, Sprite sprite, float projectedZ, ref RectangleF sourceRegion, Texture texture, ref Color4 color, ref Matrix worldMatrix, ref Vector2 spriteSize)
        {
            batchContext.SpriteBatch.Draw(texture, renderSprite.CellValue, ref worldMatrix, ref sourceRegion, ref spriteSize, ref color, sprite.Orientation, renderSprite.Swizzle, projectedZ);
        }
    }
}
