using Maze.Code.Game;
using System.Drawing.Imaging;
using System.Drawing;

namespace MazeMapTool
{
    public class LevelMapCreator
    {
        private int width;
        private int height;
        private int[] grids;
        private string floder = "./output";

        public LevelMapCreator(int _width, int _height)
        {
            width = _width;
            height = _height;
            int num = width * height;
            grids = new int[num];
        }

        public bool TryCraeteMapPicture(int startSeed, int tryCount, int subCount)
        {
            var result = CreateMap(startSeed, tryCount);
            if (result.Item1)
            {
                SavePNG(result.Item2, subCount);
            }
            return result.Item1;
        }

        public void SavePNG(int seed, int subCount)
        {
            if(!Directory.Exists(floder))
            {
                Directory.CreateDirectory(floder);
            }
            string path = $"{floder}/map_{width}x{height}_{seed}.png";
            Console.WriteLine(path);
            var bitmap = new Bitmap(width * subCount, height * subCount);
            var num = width * subCount * height * subCount;
           
            for (int i = 0; i < num; i++)
            {
                var x = i % (width * subCount);
                var y = i / (width * subCount);
                var v = (1 - grids[x / subCount + y / subCount * width]) * 255;
                var color = Color.FromArgb((255 << 24) + (v) + (v << 8) + (v << 16));
                bitmap.SetPixel(x, y, color);
            }
            bitmap.Save(path, ImageFormat.Png);
        }

        (bool, int) CreateMap(int startSeed, int tryCount)
        {
            bool isSuccess = false;
            int successSeed = 0;
            int num = width * height;
            int wall = 0;
            int way = 1;
            int unknow = 3;

            for (int seed = startSeed; seed < startSeed + tryCount; seed++)
            {
                //tryCount--;
                int start = width / 2;
                for (int i = 0; i < num; i++)
                {
                    bool isSide = i % width == 0 || i % width == width - 1 ||
                        i / width == 0 || i / width == height - 1;

                    bool isStart = i == start;
                    grids[i] = isStart ? way : (isSide ? wall : unknow);
                }
                successSeed = seed;
                isSuccess = MapCreator.TryCreateSimpleMap(grids, start, width, height, seed);
                if (isSuccess) break;
            }
            return (isSuccess, successSeed);
        }
    }
}