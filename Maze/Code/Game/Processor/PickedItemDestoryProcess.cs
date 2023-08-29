using System.Collections.Generic;
using Stride.Core.Annotations;
using Stride.Engine;
using Stride.Games;

namespace Maze.Code.Game
{
    public class PickedItemDestoryData
    {
        // public OwnerComponent Owner;
        // public DestoryComponent Destory;
    }

    public class PickedItemDestoryProcess : GameEntityProcessor<DestoryComponent, PickedItemDestoryData>
    {
        public PickedItemDestoryProcess() : base(typeof(OwnerComponent))
        {
            Order = ProcessorOrder.PickedItemDestory;
        }

        public override void Update(GameTime time)
        {
            base.Update(time);
            if(ComponentDatas.Count > 0)
            {
                var keys = new List<DestoryComponent>(ComponentDatas.Keys);
                keys.ForEach(componet => RemoveEntity(componet.Entity));
            }
        }

        protected override PickedItemDestoryData GenerateComponentData([NotNull] Entity entity, [NotNull] DestoryComponent component)
        {
            return new PickedItemDestoryData();
        }

        protected override bool IsAssociatedDataValid([NotNull] Entity entity, [NotNull] DestoryComponent component, [NotNull] PickedItemDestoryData associatedData)
        {
            return true;
        }
    }
}
