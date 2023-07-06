using Maze.Code;
using System.Drawing;
using System.Drawing.Imaging;
using LitJson;
using Maze.Code.Game;

namespace MazeMapTool
{

    internal class Program
    {
        static void Main(string[] args)
        {
            string log = "";
            foreach (var file in args)
            {
                int width, height, mapSeed, tryCount, gridPixelSize, start;
                try
                {
                    var content = File.ReadAllText(file);
                    var data = JsonMapper.ToObject(content);
                    width = ((int)data[MapJsonKeys.width]);
                    height = ((int)data[MapJsonKeys.height]);
                    mapSeed = ((int)data[MapJsonKeys.mapSeed]);
                    gridPixelSize = ((int)data[MapJsonKeys.gridPixelSize]);
                    start = ((int)data[MapJsonKeys.start]);
                    tryCount = 64;

                }
                catch (Exception)
                {
                    log += $"{file} format error!\n";
                    continue;
                }
                var creator = new LevelMapCreator(width, height, start);
                bool isSuccess = creator.TryCraeteMapPicture(mapSeed, tryCount, gridPixelSize, out var currentSeed);

                if(isSuccess)
                {
                    if (currentSeed == mapSeed)
                        log += $"{file} pass!\n";
                    else
                        log += $"{file} newSeed:{currentSeed}";
                }
                else
                {
                    log += log += $"{file} fail!\n";
                }
            }

            Console.Write(log);
            Console.ReadKey();
        }
    }
}