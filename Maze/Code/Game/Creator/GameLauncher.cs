using Stride.Engine;

namespace Maze.Code.Game
{
    public class GameLauncher : StartupScript
    {
        public override void Start()
        {
            base.Start();
            Initialize();
        }

        /// <summary>
        /// 初始化一些管理类
        /// </summary>
        private void Initialize()
        {
            var uiManager = new UIManager(Services);
            uiManager.Initialize();
            Services.AddService(uiManager);
            
        }
    }
}
