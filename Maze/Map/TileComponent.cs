using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Graphics;
using Stride.Rendering;
using Stride.Rendering.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maze.Map
{
    public class TileComponent : ScriptComponent
    {
        private StaticData_Tile staticData;
        private DynamicData_Tile dynamicData;
        private bool isHasEntity = false;


        public TileComponent(StaticData_Tile _staticData, Vector2 pos)
        {
            staticData = _staticData;
            dynamicData = new DynamicData_Tile();
            dynamicData.Pos = pos;
        }

        public void Create()
        {
            var sheet = Content.Load<SpriteSheet>(staticData.AssetPath);
            var spriteComponent = Entity.GetOrCreate<SpriteComponent>();
            spriteComponent.Sampler = SpriteSampler.PointClamp;
            if(spriteComponent.SpriteProvider is SpriteFromSheet spriteSheet)
            {
                spriteSheet.Sheet = sheet;
                spriteSheet.CurrentFrame = staticData.FrameIndex;
            }
            Entity.Transform.Position = new Vector3(dynamicData.Pos, 0);
        }

    }
}
