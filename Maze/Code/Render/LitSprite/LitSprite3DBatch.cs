using SharpDX.Direct3D11;
using Stride.Graphics;
using Stride.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maze.Code.Render
{
    public class LitSprite3DBatch : Sprite3DBatch
    {
        
        private VisionRenderer visionRender;
        public LitSprite3DBatch(GraphicsDevice device, VisionRenderer renderer, int bufferElementCount = 1024, int batchCapacity = 64) : base(device, bufferElementCount, batchCapacity)
        {
            visionRender = renderer;
        }

        protected override void PrepareForRendering()
        {
            var lightTex = visionRender?.GetLightTexture();
            if(lightTex != null)
            {
                Parameters.Set(SpriteLightKeys.SpriteLightTexture, lightTex);
            }
            base.PrepareForRendering();
        }

    }
}
