using Stride.Engine;
using Stride.Engine.Design;

namespace Maze.Code.Map
{
    [DefaultEntityComponentProcessor(typeof(PlayerControllerProcessor), ExecutionMode = ExecutionMode.Runtime)]
    public class PlayerControllerComponent : ScriptComponent
    {

    }
}
