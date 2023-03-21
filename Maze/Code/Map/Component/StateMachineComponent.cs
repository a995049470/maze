using Stride.Core;
using Stride.Engine;

namespace Maze.Code.Map
{
    public class StateMachineComponent : ScriptComponent
    {
        [DataMemberIgnore]
        public UnitState CurrentState;
    }

}
