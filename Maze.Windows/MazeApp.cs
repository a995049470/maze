using Maze.Code.Game;
using Microsoft.VisualBasic.Logging;
using Stride.Engine;
using Stride.Graphics;
using System.Drawing;
using System.Drawing.Imaging;

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
