using Stride.Core.Collections;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Engine.Design;
using Stride.Physics;

namespace Maze.Code.Game
{
    [DefaultEntityComponentProcessor(typeof(PlayerPlaceProcessor), ExecutionMode = ExecutionMode.Runtime)]
    [DefaultEntityComponentProcessor(typeof(PlacerProcessor), ExecutionMode = ExecutionMode.Runtime)]
    public class PlacerComponent : EntityComponent
    {
        public bool IsReadyPlace = false;
        public string AssetUrl = "Tile/dungeon";
        public int FrameIndex = 69;
        public BoxColliderShape SafeShape = new BoxColliderShape(true, Vector3.One);
        public Entity PlaceItem;
    }

}
