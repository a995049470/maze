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
        public float CurrentTime { get; private set; }

        public StateFlag Flag;
        
        public UnitState(StateFlag _flag, float _protectTime, float _stateTime)
        {
            Flag = _flag;
            protectTime = _protectTime;
            stateTime = _stateTime;
            CurrentTime = 0;
        }

        public void Run(float time)
        {
            CurrentTime += time;
        }

        public bool IsCanSwtich()
        {
            return CurrentTime >= protectTime;
        }

        public bool IsFinsh()
        {
            return stateTime > 0 && CurrentTime > stateTime;
        }
    }

}
