using Stride.Core;
using Stride.Engine;
using Stride.Engine.Design;

namespace Maze.Code.Game
{
    [DefaultEntityComponentProcessor(typeof(PoseStateProcessor), ExecutionMode = ExecutionMode.Runtime)]
    [DefaultEntityComponentProcessor(typeof(VelocityStateProcessor), ExecutionMode = ExecutionMode.Runtime)]
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
               CurrentState.Flag != flag)
            {
                CurrentState = new UnitState(flag, protecteTime, stataTime);
            }
        }
        
        public void Run(float time)
        {
            CurrentState.Run(time);
        }
    }

}
