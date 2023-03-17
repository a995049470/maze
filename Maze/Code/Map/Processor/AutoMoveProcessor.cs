
using Stride.Core;
using Stride.Core.Annotations;
using Stride.Core.Collections;
using Stride.Engine;
using Stride.Games;

namespace Maze.Code.Map
{
    public class AutoMoveData
    {
        public AutoMoveControllerComponent Controller;
        public MapElementComponent MapElement;
        public TransformComponent Transform;
    }

    public class AutoMoveProcessor : GameEntityProcessor<AutoMoveControllerComponent, AutoMoveData>
    {
        
        public AutoMoveProcessor() : base(typeof(MapElementComponent), typeof(TransformComponent))
        {
            
        }


        public override void Update(GameTime time)
        {
            base.Update(time);
            foreach (AutoMoveData data in ComponentDatas.Values)
            {
                AutoMove(time, data);
            }
        }

        private void AutoMove(GameTime time, AutoMoveData data)
        {
            bool isCanMove = data.Controller.MoveTimer.Run((float)time.Elapsed.TotalSeconds, game.UpdateTime.FrameCount);
            if(isCanMove)
            {
                var targetPos = data.Controller.GetNextMovePoint(data.MapElement.Pos);
                var isCurrentPoint = targetPos == data.MapElement.Pos;
                var isWalkable = levelManager.CurrentLevel.IsWalkable(targetPos);
                if(isWalkable)
                {
                    levelManager.CurrentLevel.ElementMove(data.MapElement.Pos, targetPos, data.MapElement);
                    data.MapElement.Pos = targetPos;

                    var entityPos = data.Transform.Position;
                    entityPos.X = targetPos.X;
                    entityPos.Y = targetPos.Y;
                    data.Transform.Position = entityPos;
                }
                else if (!isCurrentPoint && !isWalkable)
                {
                    data.Controller.ChangeMoveDir();
                }
            }
        }

        protected override AutoMoveData GenerateComponentData([NotNull] Entity entity, [NotNull] AutoMoveControllerComponent component)
        {
            var data = new AutoMoveData();
            data.Controller = component;
            data.MapElement = entity.Get<MapElementComponent>();
            data.Transform = entity.Get<TransformComponent>();
            return data;
        }

       

        protected override bool IsAssociatedDataValid([NotNull] Entity entity, [NotNull] AutoMoveControllerComponent component, [NotNull] AutoMoveData associatedData)
        {
            return associatedData.Controller == component;
        }

    }

    
}
