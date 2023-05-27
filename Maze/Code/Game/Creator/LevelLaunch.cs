
using Stride.Core;
using Stride.Core.Annotations;
using Stride.Core.Mathematics;
using Stride.Core.Serialization;
using Stride.Core.Threading;
using Stride.Engine;
using System;

namespace Maze.Code.Game
{
    public class LevelLaunch : StartupScript
    {
        [DataMember(10)]
        [DataMemberRange(1, 64, 1, 16, 0)]
        public int Width = 16;
        [DataMemberRange(1, 64, 1, 16, 0)]
        [DataMember(20)]
        public int Height = 16;
        [DataMember(30)]
        public int Seed = -1;
        [DataMember(40)]
        public Int2 Origin = Int2.Zero;
        [DataMember(50)]
        public UrlReference<Prefab> WallUrl;

        public override void Start()
        {
            base.Start();

            bool isSuccess = false;
            int num = Width * Height;
            int[] grids = new int[num];
            int tryCount = 5;
            int wall = 0;
            int way = 1;
            int unknow = 3;
            
            //while (isSuccess || tryCount <= 0)
            {
                //tryCount--;
                int start = Width / 2;
                for (int i = 0; i < num; i++)
                {
                    bool isSide = i % Width == 0 || i % Width == Width - 1 ||
                        i / Width == 0 || i / Width == Height - 1;
                    bool isStart = i == start;
                    grids[i] = isStart ? way : (isSide ? wall : unknow);
                }

                isSuccess = MapCreator.TryCreateSimpleMap(grids, start, Width, Height, Seed);
                Log.Info($"Width:{Width} Height:{Height} Start:({start % Width}, {start / Width}) Seed:{Seed} Success:{isSuccess}");
            }
            
            //if(isSuccess)
            {
                var wallPrefab = Content.Load(WallUrl);

                Dispatcher.For(0, num, i =>
                {
                    var grid = grids[i];
                    if(grid == wall)
                    {
                        float x = Origin.X + i % Width;
                        float y = 0;
                        float z = Origin.Y + i / Width;
                        Vector3 pos = new Vector3(x, y, z);
                        var wall = wallPrefab.Instantiate()[0];
                        wall.Transform.Position = pos;
                        SceneSystem.SceneInstance.RootScene.Entities.Add(wall);
                    }
                });
            }
        }



    }
}
