using Stride.Core.Annotations;
using Stride.Engine;
using Stride.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maze.Code.Render
{
    public class TransmittanceSpritInfo
    {
        public bool Active;
        public RenderTransmittanceSprite sprite;
    }
    public class TransmittanceSpriteRenderProcessor : EntityProcessor<TansmittanceSpriteComponent, TransmittanceSpritInfo>, IEntityComponentRenderProcessor
    {
        public VisibilityGroup VisibilityGroup { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        protected override TransmittanceSpritInfo GenerateComponentData([NotNull] Entity entity, [NotNull] TansmittanceSpriteComponent component)
        {
            throw new NotImplementedException();
        }
    }
}
