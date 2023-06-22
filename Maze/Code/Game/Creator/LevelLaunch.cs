
using Stride.Core;
using Stride.Core.Annotations;
using Stride.Core.Mathematics;
using Stride.Core.Serialization;
using Stride.Core.Threading;
using Stride.Engine;


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
        [DataMember(21)]
        public int StartSeed = 0;
        [DataMember(30)]
        public int TryCount = 1;
        [DataMember(40)]
        public Int2 Origin = Int2.Zero;
        [DataMember(50)]
        public UrlReference<Prefab> WallUrl;
        [DataMember(51)]
        public bool CreatePlayer;
        [DataMember(52)]
        public UrlReference<Prefab> PlayerUrl;
        [DataMember(60)]
        public bool Run;
        

        public override void Start()
        {
            if (!Run) return;
            base.Start();

            bool isSuccess = false;
            int num = Width * Height;
            int[] grids = new int[num];
            
            int wall = 0;
            int way = 1;
            int unknow = 3;
            
            for(int seed = StartSeed; seed < StartSeed + TryCount; seed++)
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

                isSuccess = MapCreator.TryCreateSimpleMap(grids, start, Width, Height, seed);
                Log.Info($"Width:{Width} Height:{Height} Start:({start % Width}, {start / Width}) Seed:{seed} Success:{isSuccess}");
                if (isSuccess) break;
            }
            
            //if(isSuccess)
            {
                var wallPrefab = Content.Load(WallUrl);
                if(CreatePlayer)
                {
                    var playerPrefab = Content.Load(PlayerUrl);
                    var player = playerPrefab.Instantiate()[0];
                    player.Transform.Position = Vector3.Zero;
                    SceneSystem.SceneInstance.RootScene.Entities.Add(player);

                }
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
