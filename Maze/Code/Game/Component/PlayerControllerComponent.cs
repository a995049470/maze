using Stride.Engine;
using Stride.Engine.Design;

namespace Maze.Code.Game
{
    [DefaultEntityComponentProcessor(typeof(PlayerControllerProcessor), ExecutionMode = ExecutionMode.Runtime)]
    public class PlayerControllerComponent : ScriptComponent
    {

    }
}
