using Maze.Code;
using System.Drawing;
using System.Drawing.Imaging;

namespace MazeMapTool
{

    internal class Program
    {
        static void Main(string[] args)
        {
            int width, height, seed, tryCount;
            try
            {
                width = int.Parse(args[0]);
                height = int.Parse(args[1]);
                seed = int.Parse(args[2]);
                tryCount = int.Parse(args[3]);
            }
            catch (Exception)
            {
                throw new Exception("Format Error!");
            }

            var creator = new LevelMapCreator(width, height);
            bool isSuccess = creator.TryCraeteMapPicture(seed, tryCount);
            if (isSuccess) Console.WriteLine($"Success!");
            else Console.WriteLine("Fail!");


        }
    }
}