using Maze.Code.Game;
using System.Drawing.Imaging;
using System.Drawing;

namespace MazeMapTool
{
    public class LevelMapCreator
    {
        private int width;
        private int height;
        private int start;
        private int[] grids;
        private string floder = "./output";

        public LevelMapCreator(int _width, int _height, int _start)
        {
            width = _width;
            height = _height;
            start = _start;
            int num = width * height;
            grids = new int[num];
        }

        public bool TryCraeteMapPicture(int startSeed, int tryCount, int gridPixelSize, out int seed)
        {
            var result = CreateMap(startSeed, tryCount);
            seed = result.Item2;
            if (result.Item1)
            {
                SavePNG(result.Item2, gridPixelSize);
            }
            return result.Item1;
        }

        public void SavePNG(int seed, int gridPixelSize)
        {
            if(!Directory.Exists(floder))
            {
                Directory.CreateDirectory(floder);
            }
            string path = $"{floder}/map_{width}x{height}_{seed}.png";
            Console.WriteLine(path);
            var bitmap = new Bitmap(width * gridPixelSize, height * gridPixelSize);
            var num = width * gridPixelSize * height * gridPixelSize;
           
            for (int i = 0; i < num; i++)
            {
                var x = i % (width * gridPixelSize);
                var y = i / (width * gridPixelSize);
                var v = (1 - grids[x / gridPixelSize + y / gridPixelSize * width]) * 255;
                var color = Color.FromArgb((255 << 24) + (v) + (v << 8) + (v << 16));
                bitmap.SetPixel(x, y, color);
            }
            bitmap.Save(path, ImageFormat.Png);
        }

        (bool, int) CreateMap(int startSeed, int tryCount)
        {
            bool isSuccess = false;
            int successSeed = 0;
            for (int seed = startSeed; seed < startSeed + tryCount; seed++)
            {
                grids = MapCreator.GetOriginGrids(width, height, start);
                successSeed = seed;
                isSuccess = MapCreator.TryCreateSimpleMap(grids, start, width, height, seed);
                if (isSuccess) break;
            }
            return (isSuccess, successSeed);
        }
    }
}