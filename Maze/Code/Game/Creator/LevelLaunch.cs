
using Stride.Core;
using Stride.Core.Annotations;
using Stride.Core.Mathematics;
using Stride.Core.Serialization;
using Stride.Core.Threading;
using Stride.Engine;
using System;

namespace Maze.Code.Game
{
    public class MapInfo
    {
        public int Width;
        public int Height;
        public int Seed;
    }

    public class LevelLaunch : StartupScript
    {

        [DataMember(30)]
        public UrlReference<Prefab> MapUrl;
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
        
        private MapInfo GetMapInfoFormUrl(string url)
        {
            var mapInfo = new MapInfo();
            try
            {
                var info = url.Split('_');
                var len = info.Length;
                mapInfo.Width = int.Parse(info[len - 3]);
                mapInfo.Height = int.Parse(info[len - 2]);
                mapInfo.Seed = int.Parse(info[len - 1]);
            }
            catch (System.Exception)
            {
                Log.Error("format error!");
                throw;
            }

            return mapInfo;
        }

        public override void Start()
        {
            
            if (!Run) return;
            base.Start();
            var mapInfo = GetMapInfoFormUrl(MapUrl?.Url);
            int width = mapInfo.Width;
            int height = mapInfo.Height;
            int seed = mapInfo.Seed;
            bool isSuccess = false;
            int num = width * height;
            int[] grids = MapCreator.GetOriginGrids(width, height, out var start);
            isSuccess = MapCreator.TryCreateSimpleMap(grids, start, width, height, seed);
            if (isSuccess)
            {
                //创建地图预制体
                var mapPrefab = Content.Load(MapUrl);
                var map = mapPrefab.Instantiate()[0];
                var s = MathF.Max(width, height) * Vector3.One;
                var center = new Vector3(Origin.X, 0, Origin.Y);
                center.X += width * 0.5f;
                center.Z += height * 0.5f;
                map.Transform.Scale = s;
                map.Transform.Position = center;
                SceneSystem.SceneInstance.RootScene.Entities.Add(map);

                //创建玩家
                if (CreatePlayer)
                {
                    var playerPrefab = Content.Load(PlayerUrl);
                    var player = playerPrefab.Instantiate()[0];
                    player.Transform.Position = Vector3.Zero;
                    SceneSystem.SceneInstance.RootScene.Entities.Add(player);
                }
                   
                //创建墙
                //{
                //    var wallPrefab = Content.Load(WallUrl);          
                //    Dispatcher.For(0, num, i =>
                //    {
                //        var grid = grids[i];
                //        if(grid == MapCreator.Wall)
                //        {
                //            float x = Origin.X + i % width;
                //            float y = 0;
                //            float z = Origin.Y + i / width;
                //            Vector3 pos = new Vector3(x, y, z);
                //            var wall = wallPrefab.Instantiate()[0];
                //            wall.Transform.Position = pos;
                //            SceneSystem.SceneInstance.RootScene.Entities.Add(wall);
                //        }
                    
                //    });
                //}

                
            }
        }



    }
}
