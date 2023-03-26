using Stride.Games;

namespace Maze.Code.Map
{
    public class VelocityStateProcessor : StateMachineProcessor<VelocityComponent>
    {
        public override void Update(GameTime time)
        {
            base.Update(time);
            foreach (var data in ComponentDatas.Values)
            {
                var flag = (data.Componet.Direction * data.Componet.Speed).Length() > 0 ? StateFlag.Walk : StateFlag.Idle;
                data.StateMachine.TrySwitchState(flag, 0, -1);
            }
        }
    }
}
