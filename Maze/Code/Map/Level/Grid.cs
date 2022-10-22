using System.Collections.Generic;
using Stride.Core.Collections;
using Stride.Engine;

namespace Maze.Map
{
    public class Grid
    {
        private FastCollection<IElement> elements;
        public FastCollection<IElement> Elements
        {
            get => elements;
        }
        
        //private int emptyCount = 0;

        public Grid()
        {
            elements = new FastCollection<IElement>(4);
        }
        
        public void Add(IElement element)
        {
            elements.Add(element);
        }

        public void Remove(IElement element)
        {
            elements.Remove(element);
        }
    }
}
