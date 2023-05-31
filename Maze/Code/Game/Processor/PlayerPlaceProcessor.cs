using Stride.Core.Annotations;
using Stride.Engine;
using Stride.Games;

namespace Maze.Code.Game
{
    public class PlayerPlaceData
    {
        public PlayerControllerComponent Controller;
        public PlacerComponent Placer;
    }

    public class PlayerPlaceProcessor : GameEntityProcessor<PlacerComponent, PlayerPlaceData>
    {
        public PlayerPlaceProcessor() : base(typeof(PlayerControllerComponent))
        {

        }

        public override void Update(GameTime time)
        {
            base.Update(time);
            if(input.IsKeyPressed(Stride.Input.Keys.J))
            {
                foreach (var data in ComponentDatas.Values)
                {
                    data.Placer.IsReadyPlace = true;
                }
            }
        }

        protected override PlayerPlaceData GenerateComponentData([NotNull] Entity entity, [NotNull] PlacerComponent component)
        {
            var data = new PlayerPlaceData();
            data.Controller = entity.Get<PlayerControllerComponent>();
            data.Placer = component;
            return data;
        }

        protected override bool IsAssociatedDataValid([NotNull] Entity entity, [NotNull] PlacerComponent component, [NotNull] PlayerPlaceData associatedData)
        {
            return associatedData.Controller == entity.Get<PlayerControllerComponent>() &&
                   associatedData.Placer == component;
        }
    }
}
