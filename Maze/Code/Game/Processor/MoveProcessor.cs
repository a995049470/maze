using Stride.Core;
using Stride.Core.Annotations;
using Stride.Core.Mathematics;
using Stride.Core.Threading;
using Stride.Engine;
using Stride.Games;
using Stride.Physics;
using System;

namespace Maze.Code.Game
{

    public class MoveData
    {
        public VelocityComponent Velocity;
        public TransformComponent Transform;
    }

    public class MoveProcessor : GameEntityProcessor<VelocityComponent, MoveData>
    {
        private Simulation simulation;
        private const float rotateSpeed = 4 * MathF.PI;
        public MoveProcessor() : base(typeof(TransformComponent))
        {

        }

        public override void Update(GameTime time)
        {
            base.Update(time);

            float delatTime = (float)time.Elapsed.TotalSeconds;
            Dispatcher.ForEach(ComponentDatas, kvp =>
            {
                var data = kvp.Value;
                var velocity = data.Velocity;
                if (velocity.FaceDirection != Vector3.Zero)
                {
                    var dir = velocity.FaceDirection;
                    //?????? 为啥forward是反的........
                    var forward = -data.Transform.LocalMatrix.Forward;
                    forward.Normalize();
                    var fDotD = MathUtil.Clamp(Vector3.Dot(forward, dir), -1, 1);
                    var angle = MathF.Acos(fDotD);
                    var rotateAngle = rotateSpeed * delatTime;
                    if (angle < rotateAngle)
                    {
                        data.Transform.Rotation = Quaternion.LookRotation(dir, Vector3.UnitY);
                        velocity.FaceDirection = Vector3.Zero;
                    }
                    else
                    {
                        rotateAngle = MathF.Min(angle, rotateSpeed * delatTime);
                        var rotateDir = Vector3.Cross(forward, dir).Y > 0 ? 1 : -1;
                        data.Transform.Rotation = Quaternion.RotationY(rotateAngle * rotateDir) * data.Transform.Rotation;
                    }
                }

                var direction = velocity.TargetPos - velocity.LastTargetPos;

                if (direction != Vector3.Zero)
                {
                    direction.Normalize();
                    var originPos = data.Transform.Position;
                    var targetPos = Vector3.Zero;
                    var isArrive = Vector3.Distance(originPos, velocity.TargetPos) < velocity.Speed * delatTime;
                    if(isArrive)
                    {
                        targetPos = velocity.TargetPos;
                        velocity.LastTargetPos = velocity.TargetPos;
                    }
                    else
                    {
                        targetPos = originPos + velocity.Speed * delatTime * direction;
                    }
                    data.Transform.Position = targetPos;
                }
            });
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
