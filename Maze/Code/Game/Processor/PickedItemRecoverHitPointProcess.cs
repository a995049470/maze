using System.Threading;
using Stride.Core.Annotations;
using Stride.Core.Threading;
using Stride.Engine;
using Stride.Games;

namespace Maze.Code.Game
{
    public class PickedItemRecoverHitPointData
    {
        public OwnerComponent Owner;
        public RecoverHitPointComponent RecoverHitPoint;
    }

    public class PickedItemRecoverHitPointProcess : GameEntityProcessor<RecoverHitPointComponent, PickedItemRecoverHitPointData>
    {
        private PickerProcess pickerProcess;
        public PickedItemRecoverHitPointProcess() : base(typeof(OwnerComponent))
        {
            Order = ProcessorOrder.PickedItemTakeEffect;
        }

        protected override void OnSystemAdd()
        {
            base.OnSystemAdd();
            pickerProcess = pickerProcess ?? GetProcessor<PickerProcess>();
        }

        public override void Update(GameTime time)
        {
            base.Update(time);
            if(pickerProcess == null) return;
            Dispatcher.ForEach(ComponentDatas, kvp =>
            {
                var data = kvp.Value;
                var ownerId = data.Owner.Id;
                if(pickerProcess.PikerDic.TryGetValue(ownerId, out var picker))
                {
                    var hurt = picker.Entity.Get<HurtComponet>();
                    //回血
                    if(hurt != null)
                    {
                        Interlocked.Add(ref hurt.HurtValue, -data.RecoverHitPoint.Value);
                    }    
                }
            });
        }

        protected override PickedItemRecoverHitPointData GenerateComponentData([NotNull] Entity entity, [NotNull] RecoverHitPointComponent component)
        {
            return new PickedItemRecoverHitPointData()
            {
                Owner = entity.Get<OwnerComponent>(),
                RecoverHitPoint = component
            };
        }

        protected override bool IsAssociatedDataValid([NotNull] Entity entity, [NotNull] RecoverHitPointComponent component, [NotNull] PickedItemRecoverHitPointData associatedData)
        {
            return associatedData.RecoverHitPoint == component &&
                associatedData.Owner == entity.Get<OwnerComponent>();
        }

    }
}
