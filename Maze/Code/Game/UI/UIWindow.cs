namespace Maze.Code.Game
{
    public abstract class UIWindow : UIModle
    {
        protected abstract UIWindowFlag Flag { get; }

        public virtual void Open() {}
        public virtual void Close(bool isDestory = false) {}
    }
}
