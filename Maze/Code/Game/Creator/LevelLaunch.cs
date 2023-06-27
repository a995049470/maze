
using Stride.Core;
using Stride.Core.Annotations;
using Stride.Core.Mathematics;
using Stride.Core.Serialization;
using Stride.Core.Threading;
using Stride.Engine;
using System;
using System.Threading;

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
        public UrlReference<Prefab> PlayerUrl;
        [DataMember(51)]
        public UrlReference<Prefab> WallUrl;
        [DataMember(52)]
        public UrlReference<Prefab> BarrierUrl;
        [DataMember(53)]
        public UrlReference<Prefab> MonsterUrl;
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

        private Vector3 IndexToPos(int id, int width, int height)
        {
            float x = Origin.X + id % width;
            float y = 0;
            float z = Origin.Y + id / width;
            Vector3 pos = new Vector3(x, y, z);
            return pos;
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
                //起点还得偏移半格才是左下角的位置
                var center = new Vector3(Origin.X - 0.5f, 0, Origin.Y - 0.5f);
                center.X += (width) * 0.5f;
                center.Z += (height) * 0.5f;
                map.Transform.Scale = s;
                map.Transform.Position = center;
                SceneSystem.SceneInstance.RootScene.Entities.Add(map);

                //创建玩家
                if (PlayerUrl != null)
                {
                    var playerPrefab = Content.Load(PlayerUrl);
                    var player = playerPrefab.Instantiate()[0];
                    var pos = IndexToPos(start, width, height);
                    player.Transform.Position = pos;
                    player.Get<VelocityComponent>()?.UpdatePos(pos);
                    SceneSystem.SceneInstance.RootScene.Entities.Add(player);
                }

                //创建墙
                {
                    var wallPrefab = Content.Load(WallUrl);
                    for (int i = 0; i < num; i++)
                    {
                        var grid = grids[i];
                        if (grid == MapCreator.Wall)
                        {
                            var pos = IndexToPos(i, width, height);
                            var wall = wallPrefab.Instantiate()[0];
                            wall.Transform.Position = pos;
                            SceneSystem.SceneInstance.RootScene.Entities.Add(wall);
                        }

                    };
                }
                var units = MapCreator.CreateMapUnit(grids, width, height, start, seed, 3, 8);
                //创建地图单位
                {
                    var isCreateBarrier = BarrierUrl != null;
                    var isCreateMonster = MonsterUrl != null;
                    Prefab barrierUnitPrefab = null;
                    Prefab monsterUnitPrefab = null;

                    if(isCreateBarrier) barrierUnitPrefab = Content.Load(BarrierUrl);
                    if(isCreateMonster) monsterUnitPrefab = Content.Load(MonsterUrl);

                    for (int i = 0; i < num; i++)
                    {
                        var unit = units[i];
                        if (isCreateBarrier && (unit & MapCreator.BarrierUnit) > 0)
                        {
                            var pos = IndexToPos(i, width, height);
                            var barrier = barrierUnitPrefab.Instantiate()[0];
                            barrier.Transform.Position = pos;
                            SceneSystem.SceneInstance.RootScene.Entities.Add(barrier);
                        }
                        if(isCreateMonster && (unit & MapCreator.MonsterUnit) > 0)
                        {
                            var pos = IndexToPos(i, width, height);
                            var monster = monsterUnitPrefab.Instantiate()[0];
                            monster.Transform.Position = pos;
                            monster.Get<VelocityComponent>()?.UpdatePos(pos);
                            SceneSystem.SceneInstance.RootScene.Entities.Add(monster);
                        }
                    }
                }

            }
        }



    }
}
