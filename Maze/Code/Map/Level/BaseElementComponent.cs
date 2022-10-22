using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Graphics;
using Stride.Rendering;
using Stride.Rendering.Sprites;

namespace Maze.Map
{
    public interface IElement
    {
        void SetLevel(Level level);
        void Create();
        ScriptComponent GetComponent();
    }

    public class BaseElementComponent<T, V> : ScriptComponent, IElement where T : StaticData where V : DynamicData
    {
        public T StaticData;
        public V DynamicData;
        private const float gap = 0.1f;
        protected Level targetLevel;

        public BaseElementComponent(T staticData, V dynamicData)
        {
            StaticData = staticData;
            DynamicData = dynamicData;
        }

        protected float GetPosZ()
        {
            var z = gap * StaticData.Layer;
            return z;
        }

        public virtual void Create()
        {
            var sheet = Content.Load<SpriteSheet>(StaticData.AssetUrl);
            var spriteComponent = Entity.GetOrCreate<SpriteComponent>();
            spriteComponent.Sampler = SpriteSampler.PointClamp;
            if (spriteComponent.SpriteProvider is SpriteFromSheet spriteSheet)
            {
                spriteSheet.Sheet = sheet;
                spriteSheet.CurrentFrame = StaticData.FrameIndex;
            }

            Entity.Transform.Position = new Vector3(DynamicData.Pos.X, DynamicData.Pos.Y, GetPosZ());


        }


        public ScriptComponent GetComponent()
        {
            return this;
        }

        public void SetLevel(Level level)
        {
            targetLevel = level;
        }
    }
}
