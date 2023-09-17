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

      

        public override void Update(GameTime time)
        {
            base.Update(time);
            pickerProcess = pickerProcess ?? GetProcessor<PickerProcess>();
            if (pickerProcess == null) return;
            foreach(var kvp in ComponentDatas)
            {
                var data = kvp.Value;
                var ownerId = data.Owner.OwnerId;
                if(pickerProcess.PikerDic.TryGetValue(ownerId, out var picker))
                {
                    var hurt = picker.Entity.Get<HurtComponet>();
                    //回血
                    if(hurt != null)
                    {
                        Interlocked.Add(ref hurt.HurtValue, -data.RecoverHitPoint.Value);
                    }    
                }
            };
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
