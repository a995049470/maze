using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;
using Stride.Physics;

namespace Maze.Code.Game
{
    public class PhysicTest : SyncScript
    {
        

        public override void Start()
        {
            var s = this.GetSimulation();

        }

        public override void Update()
        {
            // Do stuff every new frame
        }
    }
}
