using Stride.Core.Mathematics;
using Stride.Graphics;
using Stride.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maze.Code.Render
{
    public class VisionSprite3DBatch : Sprite3DBatch
    {
        public VisionSprite3DBatch(GraphicsDevice device, int bufferElementCount = 1024, int batchCapacity = 64) : base(device, bufferElementCount, batchCapacity)
        {

        }

        
    }
}
