using System.Collections.Generic;
using Stride.Core.Collections;
using Stride.Engine;

namespace Maze.Map
{
    public class Grid
    {
        private FastCollection<EntityComponent> elementComponents;
        public FastCollection<EntityComponent> ElementComponents
        {
            get => elementComponents;
        }
        
        //private int emptyCount = 0;

        public Grid()
        {
            elementComponents = new FastCollection<EntityComponent>(4);
        }
        
        public void Add(EntityComponent component)
        {
            elementComponents.Add(component);
        }

        public void Remove(EntityComponent component)
        {
            elementComponents.Remove(component);
        }
    }
}
