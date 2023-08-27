using Stride.Core.Annotations;
using Stride.Core.Threading;
using Stride.Engine;
using Stride.Games;

namespace Maze.Code.Game
{
    public class HitPointBarData
    {
        public HitPointBarComponent HitPointBar;
        public HitPointComponet HitPoint;
    }

    public class HitPointBarProcessor : GameEntityProcessor<HitPointBarComponent, HitPointBarData>
    {
        public HitPointBarProcessor() : base(typeof(HitPointComponet))
        {

        }

        public override void Update(GameTime time)
        {
            base.Update(time);
            Dispatcher.ForEach(ComponentDatas, kvp =>
            {
                var data = kvp.Value;
                var hitpoint = data.HitPoint;
                var bar = data.HitPointBar;
                bar.Refresh(hitpoint.GetCurrentHitPointPercent());
            });
        }

        protected override HitPointBarData GenerateComponentData([NotNull] Entity entity, [NotNull] HitPointBarComponent component)
        {
            return new HitPointBarData()
            {
                HitPointBar = component,
                HitPoint = entity.Get<HitPointComponet>()
            };
        }

        protected override bool IsAssociatedDataValid([NotNull] Entity entity, [NotNull] HitPointBarComponent component, [NotNull] HitPointBarData associatedData)
        {
            return associatedData.HitPointBar == component &&
                associatedData.HitPoint == entity.Get<HitPointComponet>();
        }

        protected override void OnEntityComponentAdding(Entity entity, [NotNull] HitPointBarComponent component, [NotNull] HitPointBarData data)
        {
            base.OnEntityComponentAdding(entity, component, data);
            bool isSuccess = component.TryBind();
            if (!isSuccess)
            {
                entity.Remove(component);
            }
        }


    }
}
