using SharpFont;
using Stride.Core;
using Stride.Core.Annotations;
using Stride.Core.Collections;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Games;
using Stride.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maze.Code.Map
{
    public class PlayerControllerData
    {
        public PlayerControllerComponent Controller;
        public MapElementComponent MapElement;
        public TransformComponent Transform;
    }

    public class PlayerControllerProcessor : GameEntityProcessor<PlayerControllerComponent, PlayerControllerData>
    {
        FastCollection<PlayerControllerData> datas = new FastCollection<PlayerControllerData>();

        public PlayerControllerProcessor() : base(typeof(MapElementComponent), typeof(TransformComponent))
        {

        }

        protected override PlayerControllerData GenerateComponentData([NotNull] Entity entity, [NotNull] PlayerControllerComponent component)
        {
            var data = new PlayerControllerData();
            data.Transform = entity.Get<TransformComponent>();
            data.Controller = component;
            data.MapElement = entity.Get<MapElementComponent>();
            return data;
        }

        public override void Update(GameTime time)
        {
            base.Update(time);
            var dir = Int2.Zero;
            if (input.IsKeyPressed(Keys.W)) dir.Y = 1;
            else if (input.IsKeyPressed(Keys.S)) dir.Y = -1;
            else if (input.IsKeyPressed(Keys.A)) dir.X = -1;
            else if (input.IsKeyPressed(Keys.D)) dir.X = 1;


            if (dir != Int2.Zero)
            {
                foreach (var data in datas)
                {
                    var originPos = data.MapElement.Pos;
                    var targetPos = originPos + dir;
                    var isWalkable = levelManager.CurrentLevel.IsWalkable(targetPos);
                    if(isWalkable)
                    {
                        data.MapElement.Pos = targetPos;
                        levelManager.CurrentLevel.ElementMove(originPos, targetPos, data.MapElement);
                        data.Transform.Position = new Vector3(targetPos.X, targetPos.Y, data.Transform.Position.Z);
                    }
                }
            }

        }


        protected override void OnEntityComponentAdding(Entity entity, [NotNull] PlayerControllerComponent component, [NotNull] PlayerControllerData data)
        {
            base.OnEntityComponentAdding(entity, component, data);
            datas.Add(data);
        }

        protected override void OnEntityComponentRemoved(Entity entity, [NotNull] PlayerControllerComponent component, [NotNull] PlayerControllerData data)
        {
            base.OnEntityComponentRemoved(entity, component, data);
            datas.Remove(data);
        }

        protected override bool IsAssociatedDataValid([NotNull] Entity entity, [NotNull] PlayerControllerComponent component, [NotNull] PlayerControllerData associatedData)
        {
            return associatedData.Controller == component && 
                   associatedData.Transform == entity.Get<TransformComponent>() &&
                   associatedData.MapElement == entity.Get<MapElementComponent>();
        }



    }
}
