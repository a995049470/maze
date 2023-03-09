using System.Collections.Generic;
using Maze.Code.Map;
using Stride.Core.Collections;
using Stride.Engine;

namespace Maze.Map
{
    public class Grid
    {
        private FastCollection<MapElementData> elements;
        public FastCollection<MapElementData> Elements
        {
            get => elements;
        }
        
        //private int emptyCount = 0;

        public Grid()
        {
            elements = new FastCollection<MapElementData>(4);
        }
        
        public void Add(MapElementData element)
        {
            elements.Add(element);
        }

        public void Remove(MapElementData element)
        {
            elements.Remove(element);
        }
        
        public bool IsWalkable()
        {
            bool isWalkable = true;
            foreach (var element in elements)
            {
                isWalkable &= element.element.IsWalkable;
                if(!isWalkable) break;
            }
            return isWalkable;
        }
    }
}
