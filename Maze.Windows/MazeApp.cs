using Stride.Engine;

namespace Maze
{
    class MazeApp
    {
        static void Main(string[] args)
        {
            using (var game = new Game())
            {
                game.Run();
            }
        }
    }
}
