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
        private Texture spriteLightTexture;   
        public LitSprite3DBatch(GraphicsDevice device, int bufferElementCount = 1024, int batchCapacity = 64) : base(device, bufferElementCount, batchCapacity)
        {
        }

        protected override void PrepareForRendering()
        {
            if(spriteLightTexture != null)
            {
                Parameters.Set(SpriteLightKeys.SpriteLightTexture, spriteLightTexture);
            }
            base.PrepareForRendering();
        }

    }
}
