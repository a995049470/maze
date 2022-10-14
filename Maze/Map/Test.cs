using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;

namespace Maze.Map
{
    public class Test : StartupScript
    {
        public int width;
        public int height;
        public string sheetUrl;
        public Vector2 start;
        public Vector2 step;

        public override void Start()
        {
            int count = width * height;
            for (int i = 0; i < count; i++)
            {
                var data = new StaticData_Tile();
                data.AssetPath = sheetUrl;
                data.FrameIndex = i;
                var pos = start;
                pos.X += step.X * (i % width);
                pos.Y += step.Y * (i / width);
                var entity = new Entity();      
                SceneSystem.SceneInstance.RootScene.Entities.Add(entity);
                var tileComp = new TileComponent(data, pos);
                entity.Add(tileComp);
                tileComp.Create();
            }
            
        }
    }
}
