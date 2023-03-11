using Stride.Engine;

namespace Maze.Code.Map
{
    public class LaunchTest : StartupScript
    {
        public string JsonAssetUrl;
        public override void Start()
        {
            base.Start();
            var levelManager = new LevelManager();
            Script.Add(levelManager);
            Services.AddService(levelManager);
            levelManager.LoadLevel(JsonAssetUrl);
        }
    }
}
