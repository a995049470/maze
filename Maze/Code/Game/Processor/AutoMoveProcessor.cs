
using Stride.Core;
using Stride.Core.Annotations;
using Stride.Core.Collections;
using Stride.Core.Threading;
using Stride.Engine;
using Stride.Games;

namespace Maze.Code.Game
{
    public class AutoMoveData
    {
        public AutoMoveControllerComponent Controller;
        public VelocityComponent Velocity;
    }

    public class AutoMoveProcessor : GameEntityProcessor<AutoMoveControllerComponent, AutoMoveData>
    {
        
        public AutoMoveProcessor() : base( typeof(VelocityComponent))
        {
            
        }


        public override void Update(GameTime time)
        {
            base.Update(time);
            Dispatcher.ForEach(ComponentDatas, kvp =>
            {
                var velocity = kvp.Value.Velocity;
                bool isIdle = velocity.TargetPos == velocity.LastTargetPos;
                if(isIdle)
                {

                }
            });
        }

        

        protected override AutoMoveData GenerateComponentData([NotNull] Entity entity, [NotNull] AutoMoveControllerComponent component)
        {
            var data = new AutoMoveData()
            {
                Controller = component,
                Velocity = entity.Get<VelocityComponent>()
            };
            return data;
        }

        

        protected override bool IsAssociatedDataValid([NotNull] Entity entity, [NotNull] AutoMoveControllerComponent component, [NotNull] AutoMoveData associatedData)
        {
            return associatedData.Controller == component && associatedData.Velocity == entity.Get<VelocityComponent>() ;
        }

    }

    
}
