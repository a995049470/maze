
using Stride.Core.Annotations;
using Stride.Core.Threading;
using Stride.Engine;
using Stride.Games;
using System;

namespace Maze.Code.Game
{
    public class FollowPlayerData
    {
        public FollowPlayerComponent FollowPlayer;
        public TransformComponent Transform;
    }

    public class FollowPlayerProcessor : GameEntityProcessor<FollowPlayerComponent, FollowPlayerData>
    {
        public FollowPlayerProcessor() : base(typeof(TransformComponent))
        {

        }

        public override void Update(GameTime time)
        {
            base.Update(time);
            float delatTime = (float)time.Elapsed.TotalSeconds;
            var target = GetProcessor<PlayerControllerProcessor>()?.Player;
            if(target != null)
            {
                Dispatcher.ForEach(ComponentDatas, kvp =>
                {
                    var data = kvp.Value;
                    var viewDir = data.Transform.WorldMatrix.Forward;
                    var targetPos = target.Position + data.FollowPlayer.TargetOffset - viewDir * data.FollowPlayer.Distance;
                    var currentPos = data.Transform.Position;
                    currentPos = MathHelper.SmoothDamp(currentPos, targetPos, ref data.FollowPlayer.CurrentVelocity, data.FollowPlayer.SmoothTime, data.FollowPlayer.MaxSpeed, delatTime);
                    data.Transform.Position = currentPos;
                });
            }
        }



        protected override FollowPlayerData GenerateComponentData([NotNull] Entity entity, [NotNull] FollowPlayerComponent component)
        {
            return new FollowPlayerData()
            {
                Transform = entity.Transform,
                FollowPlayer = component
            };
        }

        protected override bool IsAssociatedDataValid([NotNull] Entity entity, [NotNull] FollowPlayerComponent component, [NotNull] FollowPlayerData associatedData)
        {
            return associatedData.Transform == entity.Transform && associatedData.FollowPlayer == component;
        }
    }
}
