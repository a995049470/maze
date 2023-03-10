using Stride.Engine;

namespace Maze.Code.Map
{

    public class LevelManager : ScriptComponent
    {
        public Level CurrentLevel { get; private set; }

        public void LoadLevel(string url)
        {
            CurrentLevel = new Level();
            Script.Add(CurrentLevel);
            CurrentLevel.Load(url);
        }

    }
}
