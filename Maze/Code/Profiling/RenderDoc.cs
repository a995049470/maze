using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;
using Stride.Engine.Design;
using Maze.Code.Render;
using System.Reflection;

namespace Maze.Code.Profiling
{
    public class RenderDoc : StartupScript
    {
        // Declared public member fields and properties will show in the game studio

        public override void Start()
        {
            var cellSpriteType = typeof(CellSpriteComponent).GetTypeInfo();
            var spriteType = typeof(SpriteComponent).GetTypeInfo();
            Log.Info($"cellSpriteType.Equals(spriteType) : {cellSpriteType.Equals(spriteType)}");
            Log.Info($"spriteType.Equals(cellSpriteType) : {spriteType.Equals(cellSpriteType)}");
            GameProfiler.EnableProfiling();
        }
    }
}
