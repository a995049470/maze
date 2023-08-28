using Stride.Core;
using Stride.Engine;
using Stride.Engine.Design;

namespace Maze.Code.Game
{

    //具有被拾取的功能
    [DataContract]
    [DefaultEntityComponentProcessor(typeof(PickableProcessor), ExecutionMode = ExecutionMode.Runtime)]
    public class PickableComponent : EntityComponent
    {
        
    }

}
