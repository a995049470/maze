using Stride.Core;
using Stride.Engine;
using Stride.Engine.Design;

namespace Maze.Code.Game
{
    //生效后销毁
    [DataContract]
    [DefaultEntityComponentProcessor(typeof(PickedItemDestoryProcess), ExecutionMode = ExecutionMode.Runtime)]
    public class DestoryComponent : EntityComponent
    {

    }
}
