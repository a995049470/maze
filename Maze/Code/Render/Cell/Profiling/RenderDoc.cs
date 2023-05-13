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
            var t1 = typeof(CellSpriteComponent).GetTypeInfo();
            var t2 = typeof(CellSpriteComponent).GetTypeInfo();
            Log.Info($"t1.Equals(t2) : {t1.Equals(t2)}");
            GameProfiler.EnableProfiling();
        }
    }
}
