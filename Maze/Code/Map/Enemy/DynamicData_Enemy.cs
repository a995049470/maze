using Stride.Core.Mathematics;
using System;
namespace Maze.Map
{
    public enum CycleFlag
    {
        Loop,
        PingPong
    }
    public class DynamicData_Enemy : DynamicData_Unit
    {
        private Int2[] WayPoints;
        private int NextPointIndex = 1;
        private int moveDir = 1;
        private CycleFlag cycleFlag; 
        public bool isAutoMove = false;
        
        public DynamicData_Enemy() : base()
        {
            //测试用..
            MoveTimer = new Timer(0.2f, 0.4f);
        }

        //起点都是当前位置
        public void SetWayPoints(Int2[] points, CycleFlag flag)
        {
            isAutoMove = (points?.Length ?? 0) >= 2;
            cycleFlag = flag;
            WayPoints = points;
            NextPointIndex = 1;
        }

        private void MoveNextPointIndex()
        {
            NextPointIndex = (NextPointIndex + moveDir + WayPoints.Length) % WayPoints.Length;
        }

        public Int2 GetNextMovePoint()
        {
            if(!isAutoMove) return Pos;
            //TODO:可能需要检查路径的合法性
            var nextWayPoint = WayPoints[NextPointIndex];
            if(Pos == nextWayPoint)
            {
                bool isEndPoint = NextPointIndex == WayPoints.Length - 1 || NextPointIndex == 0;
                if(isEndPoint && cycleFlag == CycleFlag.PingPong)
                {
                    moveDir *= -1;
                }
                MoveNextPointIndex();
                nextWayPoint = WayPoints[NextPointIndex];
            }
            var offset = nextWayPoint - Pos;
            offset.X /= Math.Max(1, Math.Abs(offset.X));
            offset.Y /= Math.Max(1, Math.Abs(offset.Y));
            return Pos + offset;
        }

        public void ChangeMoveDir()
        {
            if(!isAutoMove) return;
            moveDir *= 1;
            MoveNextPointIndex();
        }

    }
}
