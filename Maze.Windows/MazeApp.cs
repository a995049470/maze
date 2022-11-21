using Stride.Engine;
using Stride.Graphics;

namespace Maze
{
    class MazeApp
    {
        static void Main(string[] args)
        {
            using (var game = new Game())
            {
                game.GraphicsDeviceManager.DeviceCreationFlags |= DeviceCreationFlags.Debug;
                game.Run();
            }
        }
    }
}
