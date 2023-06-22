using Maze.Code;
using System.Drawing;
using System.Drawing.Imaging;

namespace MazeMapTool
{

    internal class Program
    {
        static void Main(string[] args)
        {
            int width, height, seed, tryCount, subCount;
            try
            {
                width = int.Parse(args[0]);
                height = int.Parse(args[1]);
                seed = int.Parse(args[2]);
                tryCount = int.Parse(args[3]);
                subCount = int.Parse(args[4]);
            }
            catch (Exception)
            {
                throw new Exception("Format Error!");
            }

            var creator = new LevelMapCreator(width, height);
            bool isSuccess = creator.TryCraeteMapPicture(seed, tryCount, subCount);
            if (isSuccess) Console.WriteLine($"Success!");
            else Console.WriteLine("Fail!");


        }
    }
}