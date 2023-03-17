using Stride.Core;
using Stride.Graphics;
using Stride.Rendering.Rendering.Images;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maze.Code.Render
{
    [DataContract]
    public class PixelStyle : BasePostProcessingEffect
    {
        public PixelStyle() : base("PixelStyleEffect")
        {

        }
        public override bool IsVaild(Texture[] inputs, Texture output)
        {
            bool isVaild = inputs?.Length > 0 && inputs[0] != null && output != null;
            return isVaild;
        }
    }
}
