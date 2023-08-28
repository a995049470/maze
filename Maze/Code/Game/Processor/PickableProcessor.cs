using Stride.Core.Annotations;
using Stride.Core.Collections;
using Stride.Core.Mathematics;
using Stride.Engine;
using System;
using System.Collections.Generic;

namespace Maze.Code.Game
{
    public class PickableData 
    {
        public PickableComponent pickable;
    }
    public class PickableProcessor : GameEntityProcessor<PickableComponent, PickableData>
    {
        private Dictionary<Int2, FastCollection<PickableComponent>> pickableDic = new Dictionary<Int2, FastCollection<PickableComponent>>();//可能存在重叠的问题...
        
  
        private Int2 PositionToInt2(Vector3 position)
        {
            int x = (int)(position.X + 0.5);
            int z = (int)(position.Z + 0.5);
            return new Int2(x, z);
        }

        public bool TryGetPickableItemList(Vector3 position, out FastCollection<PickableComponent> list)
        {
            var id = PositionToInt2(position);
            var isGet = pickableDic.TryGetValue(id, out list);
            if(isGet) isGet &= list.Count > 0;
            return isGet;
        }

        protected override PickableData GenerateComponentData([NotNull] Entity entity, [NotNull] PickableComponent component)
        {
            return new PickableData()
            {
                pickable = component
            };
        }


        protected override bool IsAssociatedDataValid([NotNull] Entity entity, [NotNull] PickableComponent component, [NotNull] PickableData associatedData)
        {
            return associatedData.pickable == component;
        }

        protected override void OnEntityComponentAdding(Entity entity, [NotNull] PickableComponent component, [NotNull] PickableData data)
        {
            var id = PositionToInt2(entity.Transform.Position);
            if(!pickableDic.TryGetValue(id, out var list))
            {
                list = new FastCollection<PickableComponent>();
                pickableDic[id] = list;
            }
            list.Add(component);
        }

        protected override void OnEntityComponentRemoved(Entity entity, [NotNull] PickableComponent component, [NotNull] PickableData data)
        {
            var id = PositionToInt2(entity.Transform.Position);
            if(pickableDic.TryGetValue(id, out var list))
            {
               list.Remove(component); 
            }
        }

        
    }
}
