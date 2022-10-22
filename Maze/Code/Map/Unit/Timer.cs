namespace Maze.Map
{
    public struct Timer
    {
        private float currentTime;
        private float targetTime;
        private int lastUpdateFrame;

        public Timer(float _currentTime, float _targetTime)
        {
            currentTime = _currentTime;
            targetTime = _targetTime;
            lastUpdateFrame = -1;
        }

        public bool Run(System.Single deltaTime, int frame)
        {    
            bool isArrive = false;
            if(frame != lastUpdateFrame)
            {
                lastUpdateFrame = frame;
                currentTime += targetTime;
                isArrive = currentTime >= targetTime;
                if(isArrive) currentTime -= targetTime;
            }
            return isArrive;
        }
    }
}
