using Stride.Core.Mathematics;
using Stride.Graphics;
using Stride.Rendering;

namespace Maze.Code.Render
{
    public class LSprite3DBatch : Sprite3DBatch
    {
        protected ValueParameter<float> cellValue;
        private EffectInstance defaultSpriteEffect;

        public LSprite3DBatch(GraphicsDevice device, int bufferElementCount = 1024, int batchCapacity = 64) : base(device, bufferElementCount, batchCapacity)
        {
        }

        protected override void PrepareParameters()
        {
            base.PrepareParameters();
        }

    }
}
