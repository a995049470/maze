using Stride.Core;
using Stride.Engine;

namespace Maze.Code.Map
{
    public class StateMachineComponent : ScriptComponent
    {
        [DataMemberIgnore]
        public UnitState CurrentState;

        public StateMachineComponent()
        {
            CurrentState = new UnitState(StateFlag.Idle, 0, -1);
        }

        public void TrySwitchState(StateFlag flag, float protecteTime, float stataTime)
        {
            if(CurrentState.IsCanSwtich() &&
               CurrentState.CurrentFlag != flag)
            {
                CurrentState = new UnitState(flag, protecteTime, stataTime);
            }
        }
    }

}
