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
    public class AnimationTest : SyncScript
    {
        
        public override void Start()
        {
            Entity.Get<AnimationComponent>().Play("Idle");
            
        }

        public override void Update()
        {
        }
    }
}
