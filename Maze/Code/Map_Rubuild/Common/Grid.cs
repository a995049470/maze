using System.Collections.Generic;
using Maze.Code.Map;
using Stride.Core.Collections;
using Stride.Engine;

namespace Maze.Code.Map
{
    public class Grid
    {
        private FastCollection<MapElementComponent> elements;
        public FastCollection<MapElementComponent> Elements
        {
            get => elements;
        }

        //private int emptyCount = 0;

        public Grid()
        {
            elements = new FastCollection<MapElementComponent>(4);
        }

        public void Add(MapElementComponent element)
        {
            elements.Add(element);
        }

        public void Remove(MapElementComponent element)
        {
            elements.Remove(element);
        }

        public bool IsWalkable()
        {
            bool isWalkable = true;
            foreach (var element in elements)
            {
                isWalkable &= element.IsWalkable;
                if (!isWalkable) break;
            }
            return isWalkable;
        }
    }
}
