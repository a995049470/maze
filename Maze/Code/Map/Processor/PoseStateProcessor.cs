﻿using Stride.Games;

namespace Maze.Code.Map
{
    public class PoseStateProcessor : StateMachineProcessor<PoseComponent>
    {
        public override void Update(GameTime time)
        {
            base.Update(time);
            foreach (var data in ComponentDatas.Values)
            {
                data.StateMachine.CurrentState.Run((float)time.Elapsed.TotalMilliseconds);
                var animationName = data.StateMachine.CurrentState.Flag.ToString();
                var animationTime = data.Componet.CalculateAnimationCurrentTime(animationName, data.StateMachine.CurrentState.CurrentTime);
                data.Componet.Play(animationName, animationTime);
            }
        }
    }
}
