using Stride.Core;
using Stride.Engine;

namespace Maze.Code.Game
{
    public class PlaceItemOwnerComponent : EntityComponent
    {
        [DataMemberIgnore]
        public PlacerComponent Placer;
    }

}
