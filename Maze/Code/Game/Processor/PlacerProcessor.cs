
using Stride.Core.Annotations;
using Stride.Engine;
using Stride.Games;
using Stride.Physics;
using System;
using Stride.Core.Serialization;
using Stride.Core.Threading;

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
        public PlacerProcessor() : base(typeof(TransformComponent))
        {

        }

        private void PlaceItem(PlacerData data)
        {
            //没有正在放置的物体
            bool isHasRemainCount = data.Placer.MaxPlaceItemCount > data.Placer.CurrentPlaceItemCount;
            if(isHasRemainCount)
            {
                //检测能否放置
                var placePos = data.Transform.Position;
                placePos.X = MathF.Round(placePos.X);
                placePos.Y = 0;
                placePos.Z = MathF.Round(placePos.Z);
                var prefab = content.Load(data.Placer.ItemUrl);
                var entity = prefab.Instantiate()[0];
                entity.Transform.Position = placePos;
                sceneSystem.SceneInstance.RootScene.Entities.Add(entity);                
            }
        }

        public override void Update(GameTime time)
        {
            base.Update(time);

            Dispatcher.ForEach(ComponentDatas, kvp =>
            {
                var data = kvp.Value;
                if (data.Placer.IsReadyPlace)
                {
                    data.Placer.IsReadyPlace = false;
                    PlaceItem(data);
                }

            });
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
    }
}
