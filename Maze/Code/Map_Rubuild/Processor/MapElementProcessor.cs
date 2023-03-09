using Maze.Map;
using Stride.Core.Annotations;
using Stride.Engine;

namespace Maze.Code.Map
{
    public class MapElementData
    {
        public MapElementComponent element;
    }

    public class MapElementProcessor : EntityProcessor<MapElementComponent, MapElementData>
    {
        public MapElementProcessor()
        {

        }
        protected override MapElementData GenerateComponentData([NotNull] Entity entity, [NotNull] MapElementComponent component)
        {
            return new MapElementData() { element = component };
        }

        protected override void OnEntityComponentAdding(Entity entity, [NotNull] MapElementComponent component, [NotNull] MapElementData data)
        {
            base.OnEntityComponentAdding(entity, component, data);
            var pos = data.element.Pos;
            Level.Instance.AddElement(pos.X, pos.Y, data);
        }

        protected override void OnEntityComponentRemoved(Entity entity, [NotNull] MapElementComponent component, [NotNull] MapElementData data)
        {
            base.OnEntityComponentRemoved(entity, component, data);
            var pos = data.element.Pos;
            Level.Instance.RemoveElement(pos.X, pos.Y, data);
        }

        protected override bool IsAssociatedDataValid([NotNull] Entity entity, [NotNull] MapElementComponent component, [NotNull] MapElementData associatedData)
        {
            return associatedData.element == component;
        }
    }
}
