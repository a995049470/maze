
using Stride.Core;
using Stride.Core.Annotations;
using Stride.Core.Collections;
using Stride.Engine;
using Stride.Games;

namespace Maze.Code.Game
{
    public class AutoMoveData
    {
        public AutoMoveControllerComponent Controller;
        public TransformComponent Transform;
    }

    public class AutoMoveProcessor : GameEntityProcessor<AutoMoveControllerComponent, AutoMoveData>
    {
        
        public AutoMoveProcessor() : base( typeof(TransformComponent))
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
                
            }
        }

        protected override AutoMoveData GenerateComponentData([NotNull] Entity entity, [NotNull] AutoMoveControllerComponent component)
        {
            var data = new AutoMoveData();
            data.Controller = component;
            data.Transform = entity.Get<TransformComponent>();
            return data;
        }

        

        protected override bool IsAssociatedDataValid([NotNull] Entity entity, [NotNull] AutoMoveControllerComponent component, [NotNull] AutoMoveData associatedData)
        {
            return associatedData.Controller == component;
        }

    }

    
}
