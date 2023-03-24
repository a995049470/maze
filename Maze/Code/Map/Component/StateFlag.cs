namespace Maze.Code.Map
{
    public enum StateFlag
    {
        Idle = 1 << 0,
        Walk = 1 << 1,
        Attack = 1 << 2,
        Run = 1 << 3,
    }

}
