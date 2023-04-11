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

    public class CellSpriteRenderFeature : BaseSpriteRenderFeature<RenderCellSprite, Sprite3DBatch>
    {
        protected override string effectName => "CellSpriteEffect";

        protected override string alphaCutoffEffectName => "";

        protected override Sprite3DBatch GetSprite3DBatch()
        {
            return new Sprite3DBatch(Context.GraphicsDevice);
        }

        
    }
}
