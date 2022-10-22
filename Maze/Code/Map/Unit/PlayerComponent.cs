using Stride.Core.Mathematics;
using Stride.Graphics;
using Stride.Rendering;
using Stride.Rendering.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maze.Map
{
    public interface IPlayer
    {
        void Move(int dx, int dy);
    }   

    public class PlayerComponent : UnitComponent, IPlayer
    {
        public PlayerComponent(StaticData_Unit staticData, DynamicData_Unit dynamicData) : base(staticData, dynamicData)
        {   

        }

        public void Move(int dx, int dy)
        {
            DynamicData.Pos.X += dx;
            DynamicData.Pos.Y += dy;
            Entity.Transform.Position = new Vector3(DynamicData.Pos.X, DynamicData.Pos.Y, GetPosZ());
        }
    }
}
