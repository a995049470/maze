using Stride.Core.Annotations;
using Stride.Core.Threading;
using Stride.Engine;
using Stride.Games;

namespace Maze.Code.Game
{
    public class HurtData
    {
        public HitPointComponet HitPoint;
        public HurtComponet Hurt;
    }

    public class HurtProcessor : GameEntityProcessor<HurtComponet, HurtData>
    {
        public HurtProcessor() : base(typeof(HitPointComponet))
        {

        }

        public override void Update(GameTime time)
        {
            base.Update(time);
            Dispatcher.ForEach(ComponentDatas, kvp =>
            {
                var data = kvp.Value;
                if(data.Hurt.HurtValue > 0)
                {
                    data.HitPoint.CurrentHp -= data.Hurt.HurtValue;
                    //log.Info($"case {data.Hurt.HurtValue} point damage, remain {data.HitPoint.CurrentHp} hitpoint");
                    data.Hurt.HurtValue = 0;
                    //Test:死亡表现
                    if(data.HitPoint.CurrentHp <= 0)
                    {                   
                        //使用移除测试死亡表现...
                        cacheActions.Add(() =>
                        {
                            //log.Info("die....");
                            RemoveEntity(data.HitPoint.Entity);
                        });
                    }
                }
            });
            InvokeCacheActions();
        }

        protected override HurtData GenerateComponentData([NotNull] Entity entity, [NotNull] HurtComponet component)
        {
            return new HurtData()
            {
                Hurt = component,
                HitPoint = entity.Get<HitPointComponet>()
            };
        }

        protected override bool IsAssociatedDataValid([NotNull] Entity entity, [NotNull] HurtComponet component, [NotNull] HurtData associatedData)
        {
            return associatedData.Hurt == component &&
                associatedData.HitPoint == entity.Get<HitPointComponet>();
        }
    }
}
