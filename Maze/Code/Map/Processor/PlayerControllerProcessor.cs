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
        public VelocityComponent VelocityComponent;
    }

   

    public class PlayerControllerProcessor : GameEntityProcessor<PlayerControllerComponent, PlayerControllerData>
    {
        

        public PlayerControllerProcessor() : base(typeof(VelocityComponent))
        {

        }

        protected override PlayerControllerData GenerateComponentData([NotNull] Entity entity, [NotNull] PlayerControllerComponent component)
        {
            var data = new PlayerControllerData();
            data.Controller = component;
            data.VelocityComponent = entity.Get<VelocityComponent>();
            return data;
        }

        public override void Update(GameTime time)
        {
            base.Update(time);
            var dir = Vector2.Zero;
            if (input.IsKeyDown(Keys.W)) dir.Y += 1;
            if (input.IsKeyDown(Keys.S)) dir.Y += -1;
            if (input.IsKeyDown(Keys.A)) dir.X += -1;
            if (input.IsKeyDown(Keys.D)) dir.X += 1;


            dir.Normalize();
            foreach (var data in ComponentDatas.Values)
            {
                data.VelocityComponent.Direction = dir;
            }
        }




        protected override bool IsAssociatedDataValid([NotNull] Entity entity, [NotNull] PlayerControllerComponent component, [NotNull] PlayerControllerData associatedData)
        {
            return associatedData.Controller == component &&               
                   associatedData.VelocityComponent == entity.Get<VelocityComponent>();
        }



    }
}
