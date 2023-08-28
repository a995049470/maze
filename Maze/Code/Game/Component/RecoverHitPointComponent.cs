using Stride.Core;
using Stride.Engine;
using Stride.Engine.Design;

namespace Maze.Code.Game
{
    [DataContract]
    [DefaultEntityComponentProcessor(typeof(PickedItemRecoverHitPointProcess), ExecutionMode = ExecutionMode.Runtime)]
    public class RecoverHitPointComponent : EntityComponent
    {
        //必须大于0
        public int Value;
    }
}
