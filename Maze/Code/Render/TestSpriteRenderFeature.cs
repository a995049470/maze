using Stride.Graphics;
using Stride.Rendering.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maze.Code.Render
{
    public class TestSpriteRenderFeature : BaseSpriteRenderFeature<RenderSprite, Sprite3DBatch>
    {
        protected override string effectName => null;

        protected override string alphaCutoffEffectName => null;

        protected override Sprite3DBatch GetSprite3DBatch()
        {
            return new Sprite3DBatch(Context.GraphicsDevice);
        }

    }
}
