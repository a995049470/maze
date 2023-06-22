using Stride.Core;
using Stride.Graphics;
using Stride.Rendering;
using Stride.Rendering.Rendering.Images;

namespace Maze.Code.Render
{
    [DataContract]
    public class CellWarFog : BasePostProcessingEffect
    {
        private Texture GetCellTexture(RenderDrawContext context)
        {
            Texture cellTexture = null;
            var views = context.RenderContext.RenderSystem.Views;
            foreach (var view in views)
            {
                if (view is CellRenderView cellRenderView)
                {
                    cellTexture = cellRenderView.CellTexture;
                    break;
                }
            }
            return cellTexture;
        }

        
        public override bool IsVaild(Texture[] inputs, Texture output)
        {
            return true;
        }


        public override void Draw(RenderDrawContext context, Texture[] inputs, Texture output)
        {
            bool isVaild = IsVaild(inputs, output);
            if (isVaild)
            {
                var len = inputs?.Length ?? 0;
                for (int i = 0; i < len; i++)
                {
                    var input = inputs[i];
                    if (input != null) SetInput(i, input);
                }
                SetOutput(output);
                Draw(context);
            }
        }
    }
}
