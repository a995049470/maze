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

        public UnitAction CurrentAcction;
        
        public UnitState(UnitAction _action, float _protectTime, float _stateTime)
        {
            CurrentAcction = _action;
            protectTime = _protectTime;
            stateTime = _stateTime;
        }
    }

}
