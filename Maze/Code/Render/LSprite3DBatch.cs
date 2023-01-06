using Stride.Core.Mathematics;
using Stride.Graphics;
using Stride.Rendering;

namespace Maze.Code.Render
{
    public class LSprite3DBatch : Sprite3DBatch
    {
        protected class DrawParameter
        {
            public Texture Texture;
            public float CellValue;
        }


        public static readonly ValueParameterKey<float> CellValue = ParameterKeys.NewValue<float>();

        protected ValueParameter<float>? cellValue;
        private EffectInstance defaultSpriteEffect;

        public LSprite3DBatch(GraphicsDevice device, int bufferElementCount = 1024, int batchCapacity = 64) : base(device, bufferElementCount, batchCapacity)
        {

        }

        protected void Draw(DrawParameter parameter, in ElementInfo elementInfo)
        {

        }

        protected override void PrepareParameters()
        {
            base.PrepareParameters();
            cellValue = null;
            if (Effect.Effect.HasParameter(CellValue))
                cellValue = Effect.Parameters.GetAccessor(CellValue);
        }

        
    }
}
