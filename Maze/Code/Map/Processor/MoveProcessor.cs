using Stride.Core;
using Stride.Core.Annotations;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Games;
using Stride.Physics;

namespace Maze.Code.Map
{

    public class MoveData
    {
        public VelocityComponent Velocity;
        public TransformComponent Transform;
    }

    public class MoveProcessor : GameEntityProcessor<VelocityComponent, MoveData>
    {
        private Simulation simulation;
        public MoveProcessor() :base(typeof(TransformComponent))
        {

        }

        public override void Update(GameTime time)
        {
            base.Update(time);

            simulation = simulation ?? GetSimulation();
            foreach (var data in ComponentDatas.Values)
            {
                if(data.Velocity.Direction != Vector2.Zero)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        var dir = Vector3.Zero;
                        dir[i] = data.Velocity.Direction[i];
                        var originPos = data.Transform.Position;
                        var targetPos = originPos + data.Velocity.Speed * (float)time.Elapsed.TotalSeconds * dir;
                        var scale = Vector3.One;
                        var rotation = Quaternion.Identity;
                        Matrix.Transformation(ref scale , ref rotation, ref originPos, out var from);
                        Matrix.Transformation(ref scale , ref rotation, ref targetPos, out var to);
                        var shape = new SphereColliderShape(true, 0.3f);
                        shape.LocalOffset = new Vector3(0, -0.05f, 0);
                        var hitReslut = simulation.ShapeSweep(shape, from, to);
                        if(!hitReslut.Succeeded)
                        {
                            data.Transform.Position = targetPos; 
                        }
                    }
                }

            }
        }

        protected override void OnSystemAdd()
        {
            base.OnSystemAdd();
            
        }

        protected override MoveData GenerateComponentData([NotNull] Entity entity, [NotNull] VelocityComponent component)
        {
            var data = new MoveData();
            data.Velocity = component;
            data.Transform = entity.Transform;
            return data;
        }

        protected override bool IsAssociatedDataValid([NotNull] Entity entity, [NotNull] VelocityComponent component, [NotNull] MoveData associatedData)
        {
            return associatedData.Velocity == component && associatedData.Transform == entity.Transform;
        }

    }
}
