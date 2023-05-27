using SharpFont;
using Stride.Core;
using Stride.Core.Annotations;
using Stride.Core.Collections;
using Stride.Core.Mathematics;
using Stride.Core.Threading;
using Stride.Engine;
using Stride.Games;
using Stride.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maze.Code.Game
{
    public class PlayerControllerData
    {
        public PlayerControllerComponent Controller;
        public VelocityComponent Velocity;
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
            data.Velocity = entity.Get<VelocityComponent>();
            return data;
        }

        public override void Update(GameTime time)
        {
            base.Update(time);
            var dir = Vector3.Zero;
            if (input.IsKeyDown(Keys.W)) dir.Z += 1;
            else if (input.IsKeyDown(Keys.S)) dir.Z -= 1;
            else if (input.IsKeyDown(Keys.A)) dir.X += 1;
            else if (input.IsKeyDown(Keys.D)) dir.X -= 1;
            if (dir == Vector3.Zero) return;

            var simulation = GetSimulation();
            Dispatcher.ForEach(ComponentDatas, kvp =>
            {
                
                var velocity = kvp.Value.Velocity;
                var isIdle = velocity.TargetPos == velocity.LastTargetPos;
            
                if(isIdle)
                {
                    var from = velocity.TargetPos;
                    var to = velocity.TargetPos + dir;
                    
                    if(simulation != null)
                    {
                        var hit = simulation.Raycast(from, to);
                        if(!hit.Succeeded)
                        {
                            velocity.TargetPos = to;
                            velocity.FaceDirection = dir;
                        }
                    }
                }
                else
                {
                    var isResetTarget = Vector3.Dot(velocity.TargetPos - velocity.LastTargetPos, dir) < -0.5f;
                    if(isResetTarget)
                    {
                        var temp = velocity.LastTargetPos;
                        velocity.LastTargetPos = velocity.TargetPos;
                        velocity.TargetPos = temp;
                        velocity.FaceDirection = dir;
                    }
                }
            });

            
        }




        protected override bool IsAssociatedDataValid([NotNull] Entity entity, [NotNull] PlayerControllerComponent component, [NotNull] PlayerControllerData associatedData)
        {
            return associatedData.Controller == component &&               
                   associatedData.Velocity == entity.Get<VelocityComponent>();
        }



    }
}
