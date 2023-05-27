
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Engine.Design;
using Stride.Games;
using System;

namespace Maze.Code.Game
{

    [DefaultEntityComponentProcessor(typeof(AutoMoveProcessor), ExecutionMode = ExecutionMode.Runtime)]
    public class AutoMoveControllerComponent : EntityComponent
    {
        public Int2[] WayPoints;
        public int NextPointIndex = 1;
        public int MoveDir = 1;
        public CycleFlag Flag;
        public bool IsAutoMove = false;
        public Timer MoveTimer;

        public Int2 GetNextMovePoint(Int2 pos)
        {
            //TODO:可能需要检查路径的合法性
            var nextWayPoint = WayPoints[NextPointIndex];
            if (pos == nextWayPoint)
            {
                bool isEndPoint = NextPointIndex == WayPoints.Length - 1 || NextPointIndex == 0;
                if (isEndPoint && Flag == CycleFlag.PingPong)
                {
                    MoveDir *= -1;
                }
                MoveNextPointIndex();
                nextWayPoint = WayPoints[NextPointIndex];
            }
            var offset = nextWayPoint - pos;
            offset.X /= Math.Max(1, Math.Abs(offset.X));
            offset.Y /= Math.Max(1, Math.Abs(offset.Y));
            return pos + offset;
        }

        private void MoveNextPointIndex()
        {
            NextPointIndex = (NextPointIndex + MoveDir + WayPoints.Length) % WayPoints.Length;
        }


        public void ChangeMoveDir()
        {
            MoveDir *= -1;
            MoveNextPointIndex();
        }

    }

    



}
