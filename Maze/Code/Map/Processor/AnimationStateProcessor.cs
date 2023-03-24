using Stride.Engine;
using Stride.Games;

namespace Maze.Code.Map
{
    public class AnimationStateProcessor : StateMachineProcessor<AnimationComponent>
    {
        public override void Update(GameTime time)
        {
            base.Update(time);
            foreach (var data in ComponentDatas.Values)
            {
                var animation = data.Componet;
                
            }
        }
    }
}
