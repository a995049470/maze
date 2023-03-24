using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;

namespace Maze.Code.Profiling
{
    public class UpdateTest : SyncScript
    {
        // Declared public member fields and properties will show in the game studio

        public override void Start()
        {
            // Initialization of the script.
        }

        public override void Update()
        {
            var pos = Entity.Transform.Position;
            pos.Z += 5 * 0.2f;
            Entity.Transform.Position = pos;
        }
    }
}
