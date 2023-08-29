using System;
using System.Collections.Generic;
using Stride.Core.Annotations;
using Stride.Core.Threading;
using Stride.Engine;
using Stride.Games;

namespace Maze.Code.Game
{
    public class PickerData
    {
        public PickerComponent Picker;
    }

    public class PickerProcess : GameEntityProcessor<PickerComponent, PickerData>
    {

        public Dictionary<Guid, PickerComponent> PikerDic = new Dictionary<Guid, PickerComponent>();
        private PickableProcessor pickableProcessor;

        public PickerProcess() : base()
        {
            Order = ProcessorOrder.Pick;
        }

        protected override PickerData GenerateComponentData([NotNull] Entity entity, [NotNull] PickerComponent component)
        {
            return new PickerData()
            {
                Picker = component
            };
        }

       

        public override void Update(GameTime time)
        {
            base.Update(time);
            pickableProcessor = pickableProcessor ?? GetProcessor<PickableProcessor>();
            if (pickableProcessor == null) return;
            var currentFrame = time.FrameCount;
            //TODO:控制更新的频率
            //一格不允许同时存在两个角色
            //应该不可能出现两个人捡一个东西把??
            Dispatcher.ForEach(ComponentDatas, kvp =>
            {
                var data = kvp.Value;
                var picker = kvp.Key;;
                var position = data.Picker.Entity.Transform.Position;
                bool isPick = pickableProcessor.TryGetPickableItemList(position, out var list);
                if(isPick)
                {
                    foreach (var pickable in list)
                    {
                        var entity = pickable.Entity;
                        var owner = entity.Get<OwnerComponent>();
                        if(owner == null)
                        {
                            owner = new OwnerComponent(currentFrame, 1)
                            {
                                OwnerId = picker.Id,
                            };
                            entity.Add(owner);
                        }
                    }
                }
            });
        }

        protected override bool IsAssociatedDataValid([NotNull] Entity entity, [NotNull] PickerComponent component, [NotNull] PickerData associatedData)
        {
            return associatedData.Picker == component;
        }

        protected override void OnEntityComponentAdding(Entity entity, [NotNull] PickerComponent component, [NotNull] PickerData data)
        {
            PikerDic[component.Id] = component;
        }
    

        protected override void OnEntityComponentRemoved(Entity entity, [NotNull] PickerComponent component, [NotNull] PickerData data)
        {
            PikerDic.Remove(component.Id);
        }


        
    }
}
