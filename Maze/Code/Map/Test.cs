using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;
using Stride.Core.Diagnostics;
using Stride.Rendering.Sprites;
using Stride.Graphics;

namespace Maze.Map
{
    public class Test : StartupScript
    {
        public int width;
        public int height;
        public string sheetUrl;
        public string saveUrl;
    
        public Int2 start;
        public Int2 step;

        public void TestLoadTile()
        {
            int count = width * height;
            for (int i = 0; i < count; i++)
            {
                var data = new StaticData_Tile();
                data.AssetUrl = sheetUrl;
                data.FrameIndex = i;
                var pos = start;
                pos.X += step.X * (i % width);
                pos.Y += step.Y * (i / width);
                var entity = new Entity();
                SceneSystem.SceneInstance.RootScene.Entities.Add(entity);
                var dynamicData = new DynamicData_Tile();
                dynamicData.Pos = pos;
                var tileComp = new TileComponent(data, dynamicData);
                entity.Add(tileComp);
                tileComp.Create();
            }
        }
        
        public override void Start()
        {
            TestLoadTile();

        }   

        
      

        
    }
}
