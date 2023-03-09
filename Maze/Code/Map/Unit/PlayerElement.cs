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

    public class PlayerElement : UnitElement, IPlayer
    {
        public PlayerElement(StaticData_Unit staticData, DynamicData_Unit dynamicData) : base(staticData, dynamicData)
        {   

        }
        
        public void Move(int dx, int dy)
        {
            //保证每帧调用一次...
            bool isCanMove = DynamicData.MoveTimer.Run(CurrentLevel.DeltaTime, CurrentLevel.FrameCount);
            var targetPos = DynamicData.Pos + new Int2(dx, dy);
            var isWalkable = CurrentLevel.IsWalkable(targetPos);
            if(isWalkable)
            {
                //CurrentLevel.ElementMove(DynamicData.Pos, targetPos, this);
                DynamicData.Pos = targetPos;
                Entity.Transform.Position = new Vector3(DynamicData.Pos.X, DynamicData.Pos.Y, GetPosZ());
            }
        }
    }
}
