
using Stride.Core.Annotations;
using Stride.Engine;
using Stride.Games;
using Stride.Physics;
using System;
using Stride.Core.Serialization;
using Stride.Core.Threading;
using System.Collections.Concurrent;
using Stride.Core.Mathematics;
using System.Collections.Generic;
using SharpFont;

namespace Maze.Code.Game
{
    public class PlacerData
    {
        public PlacerComponent Placer;
        public TransformComponent Transform;
    }

    public class PlacerProcessor : GameEntityProcessor<PlacerComponent, PlacerData>
    {
        private Simulation simulation;
        public Dictionary<Guid, PlacerComponent> PlacermDic = new Dictionary<Guid, PlacerComponent>();
        private BombProcessor bombProcessor;
        public PlacerProcessor() : base(typeof(TransformComponent))
        {

        }

        private void ClearLastPlaceData(PlacerData data)
        {
            //离开上次的放置点后，清除上次的放置位置
            if (Vector3.DistanceSquared(data.Transform.Position, data.Placer.LastPlacePos) > tiny)
            {
                data.Placer.LastPlacePos = PlacerComponent.InvalidPos;
            }
        }

        private void PlaceItem(PlacerData data, int frame)
        {
            //没有正在放置的物体
            int bombCount = GetBombCount(data);
            bool isHasRemainCount = data.Placer.MaxPlaceItemCount > bombCount;
            if(isHasRemainCount)
            {
                //检测能否放置
                var placePos = data.Transform.Position;
                placePos.X = MathF.Round(placePos.X);
                placePos.Y = 0;
                placePos.Z = MathF.Round(placePos.Z);
                if(Vector3.DistanceSquared(placePos, data.Placer.LastPlacePos) > tiny)
                {
                    var prefab = content.Load(data.Placer.ItemUrl);
                    var entity = prefab.Instantiate()[0];
                    entity.Transform.Position = placePos;
                    data.Placer.LastPlacePos = placePos;
                    cacheActions.Add(() =>
                    {
                        data.Placer.CurrentPlaceItemCount ++;
                        var owner = new OwnerComponent(frame, -1)
                        {
                            OwnerId = data.Transform.Id
                        };
                        entity.Add(owner);
                        sceneSystem.SceneInstance.RootScene.Entities.Add(entity);
                    });                  
                }
            }
        }

        public override void Update(GameTime time)
        {
            base.Update(time);
            bombProcessor = bombProcessor ?? GetProcessor<BombProcessor>();
            Dispatcher.ForEach(ComponentDatas, kvp =>
            {
                var data = kvp.Value;
                if (data.Placer.IsReadyPlace)
                {
                    data.Placer.IsReadyPlace = false;
                    PlaceItem(data, time.FrameCount);
                }
                else
                {
                    ClearLastPlaceData(data);
                }

            });

            InvokeCacheActions();

        }

        private int GetBombCount(PlacerData data)
        {
            int count = 0;
            count = data.Placer.CurrentPlaceItemCount;
            return count;
        }

        protected override PlacerData GenerateComponentData([NotNull] Entity entity, [NotNull] PlacerComponent component)
        {
            return new PlacerData()
            {
                Placer = component,
                Transform = entity.Transform,
            };
        }

        protected override bool IsAssociatedDataValid([NotNull] Entity entity, [NotNull] PlacerComponent component, [NotNull] PlacerData associatedData)
        {
            return associatedData.Placer == component && associatedData.Transform == entity.Transform;
        }

        protected override void OnEntityComponentAdding(Entity entity, [NotNull] PlacerComponent component, [NotNull] PlacerData data)
        {
            base.OnEntityComponentAdding(entity, component, data);
            PlacermDic[data.Transform.Id] = component;
        }

        protected override void OnEntityComponentRemoved(Entity entity, [NotNull] PlacerComponent component, [NotNull] PlacerData data)
        {
            base.OnEntityComponentRemoved(entity, component, data);
            PlacermDic.Remove(data.Transform.Id);
        }
    }
}
