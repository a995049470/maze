
using Stride.Core.Annotations;
using Stride.Engine;

namespace Maze.Code.Map
{

    public class MapElementProcessor : EntityProcessor<MapElementComponent>
    {
        public MapElementProcessor() : base(typeof(TransformComponent))
        {

        }
        
        protected override void OnEntityComponentAdding(Entity entity, [NotNull] MapElementComponent component, [NotNull] MapElementComponent data)
        {
        
            var pos = component.Pos;
            Level.Instance.AddElement(pos.X, pos.Y, data);
        }

        protected override void OnEntityComponentRemoved(Entity entity, [NotNull] MapElementComponent component, [NotNull] MapElementComponent data)
        {
            base.OnEntityComponentRemoved(entity, component, data);
            var pos = component.Pos;
            Level.Instance.RemoveElement(pos.X, pos.Y, data);
        }


    }
}
