using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Graphics;
using Stride.Rendering;
using Stride.Rendering.Sprites;

namespace Maze.Map
{
    public class BaseElementComponent<T,V> : ScriptComponent where T : StaticData where V : DynamicData
    {
        public T StaticData;
        public V DynamicData;

        public BaseElementComponent(T staticData, V dynamicData)
        {
            StaticData = staticData;
            DynamicData = dynamicData;
        }

        public void Create()
        {
            var sheet = Content.Load<SpriteSheet>(StaticData.AssetUrl);
            var spriteComponent = Entity.GetOrCreate<SpriteComponent>();
            spriteComponent.Sampler = SpriteSampler.PointClamp;
            if (spriteComponent.SpriteProvider is SpriteFromSheet spriteSheet)
            {
                spriteSheet.Sheet = sheet;
                spriteSheet.CurrentFrame = StaticData.FrameIndex;
            }
            Entity.Transform.Position = new Vector3(DynamicData.Pos.X, DynamicData.Pos.Y, 0);
            
            
        }
    }
}
