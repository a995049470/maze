using Stride.Animations;
using Stride.Engine;
using Stride.Games;
using System.Threading.Tasks;
using System;

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
