using Stride.Core.Annotations;
using Stride.Engine;

namespace Maze.Code.Game
{
    public class HitPointData
    {
        HitPointComponet HitPoint;
    }


    public class HitPointProcessor : GameEntityProcessor<HitPointComponet, HitPointData>
    {
        protected override HitPointData GenerateComponentData([NotNull] Entity entity, [NotNull] HitPointComponet component)
        {
            throw new System.NotImplementedException();
        }
    }
}
