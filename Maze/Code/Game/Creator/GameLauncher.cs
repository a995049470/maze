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
            var uiManager = new UIManager();
            var levelManager = new LevelManager();

            Services.AddService(uiManager);
            Services.AddService(levelManager);

            //先加服务, 再初始化服务
            uiManager.Initialize(Services);
            levelManager.Initialize(Services);
            
        }
    }
}
