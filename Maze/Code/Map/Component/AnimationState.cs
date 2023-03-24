namespace Maze.Code.Map
{
    public class UnitState
    {
        /// <summary>
        /// 受保护的状态时间
        /// </summary>
        private float protectTime;
        /// <summary>
        /// 状态持续时间
        /// </summary>
        private float stateTime;
        /// <summary>
        /// 当前时间
        /// </summary>
        private float currentTime;

        public StateFlag CurrentFlag;
        
        public UnitState(StateFlag _flag, float _protectTime, float _stateTime)
        {
            CurrentFlag = _flag;
            protectTime = _protectTime;
            stateTime = _stateTime;
            currentTime = 0;
        }

        public bool IsCanSwtich()
        {
            return currentTime >= protectTime;
        }

        public bool IsFinsh()
        {
            return stateTime > 0 && currentTime > stateTime;
        }
    }

}
