
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
        public Dictionary<Guid, TransformComponent> PlacerTransformDic = new Dictionary<Guid, TransformComponent>();
        private BombProcessor bombProcessor;
        public PlacerProcessor() : base(typeof(TransformComponent))
        {

        }

        private void PlaceItem(PlacerData data)
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
                        var owner = new PlaceItemOwnerComponent()
                        {
                            OwnerId = data.Transform.Id
                        };
                        entity.Add(owner);
                        sceneSystem.SceneInstance.RootScene.Entities.Add(entity);
                        //var cs = entity.Components;
                        //var str = "";
                        //foreach (var item in cs)
                        //{
                        //    str += "  " + item.ToString();
                        //}
                        //log.Info(str);
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
                    PlaceItem(data);
                }

            });

            InvokeCacheActions();

        }

        private int GetBombCount(PlacerData data)
        {
            int count = 0;
            count = bombProcessor?.GetOwnerBombCount(data.Transform.Id) ?? 0;
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
            PlacerTransformDic[data.Transform.Id] = data.Transform;
        }

        protected override void OnEntityComponentRemoved(Entity entity, [NotNull] PlacerComponent component, [NotNull] PlacerData data)
        {
            base.OnEntityComponentRemoved(entity, component, data);
            PlacerTransformDic.Remove(data.Transform.Id);
        }
    }
}
