using Stride.Core;
using Stride.Engine;
using Stride.Engine.Design;

namespace Maze.Code.Game
{

    //具有捡取道具的功能
    [DataContract]
    [DefaultEntityComponentProcessor(typeof(PickerProcess), ExecutionMode = ExecutionMode.Runtime)]
    public class PickerComponent : EntityComponent
    {
        
    }

}
