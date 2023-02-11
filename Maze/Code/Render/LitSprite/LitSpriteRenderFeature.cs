using Stride.Rendering.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maze.Code.Render
{
    public interface ISpriteLitRenderFeature
    {
        void SetVisionRender(VisionRenderer renderer);
    }
    public class LitSpriteRenderFeature : BaseSpriteRenderFeature<RenderSprite, LitSprite3DBatch>, ISpriteLitRenderFeature
    {
        protected override string effectName => "LitSpriteEffect";

        protected override string alphaCutoffEffectName => "";
        private VisionRenderer visionRenderer;

        public void SetVisionRender(VisionRenderer renderer)
        {
            visionRenderer = renderer;
        }

        protected override LitSprite3DBatch GetSprite3DBatch()
        {
            return new LitSprite3DBatch(Context.GraphicsDevice, visionRenderer);
        }
    }
}
