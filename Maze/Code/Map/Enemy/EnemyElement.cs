using Stride.Core.Mathematics;

namespace Maze.Map
{

    public interface IEnemy
    {
        void AutoMove();
    }

    public class EnemyElement : UnitElement, IEnemy
    {
        private DynamicData_Enemy enemyData;

        public EnemyElement(StaticData_Unit staticData, DynamicData_Enemy dynamicData) : base(staticData, dynamicData)
        {   
            enemyData = dynamicData;
        }

        public void AutoMove()
        {
            bool isCanMove = enemyData.MoveTimer.Run(CurrentLevel.DeltaTime, CurrentLevel.FrameCount);
            if(isCanMove)
            {
                var targetPos = enemyData.GetNextMovePoint();
                var isCurrentPoint = targetPos == enemyData.Pos;
                var isWalkable = CurrentLevel.IsWalkable(targetPos);
                if(isWalkable)
                {
                    enemyData.Pos = targetPos;
                    Entity.Transform.Position = new Vector3(targetPos.X, targetPos.Y, GetPosZ());
                }
                else if(!isCurrentPoint && !isWalkable)
                {
                    enemyData.ChangeMoveDir();
                }
            }
        }
    }
}
