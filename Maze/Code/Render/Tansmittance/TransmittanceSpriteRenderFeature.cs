using Stride.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maze.Code.Render
{
    public class TransmittanceSpriteRenderFeature : BaseSpriteRenderFeature<RenderTransmittanceSprite, Sprite3DBatch>
    {
        protected override string effectName => "CellSpriteRenderFeature";

        protected override string alphaCutoffEffectName => "";

        protected override Sprite3DBatch GetSprite3DBatch()
        {
            return new Sprite3DBatch(Context.GraphicsDevice);
        }
    }
}
