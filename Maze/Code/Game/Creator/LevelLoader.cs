
using LitJson;
using Stride.Core;
using Stride.Core.Mathematics;
using Stride.Core.Serialization.Contents;
using Stride.Engine;
using System;
using System.IO;

namespace Maze.Code.Game
{
    public class RandomUnit
    {
        public Prefab Prefab;
        private int weight;
        public int Weight { get => weight; set => weight = Math.Max(0, value); }
        public int WeightCut;
    }

    public class LevelLoader
    {   
        private IServiceRegistry service;
        private ContentManager content;
        protected SceneSystem sceneSystem;
        private string levelJsonUrl;
        private Int2 origin = Int2.Zero;

        public LevelLoader(IServiceRegistry _service, Int2 _origin, string jsonUrl)
        {
            sceneSystem = _service.GetSafeServiceAs<SceneSystem>();
            content = _service.GetSafeServiceAs<ContentManager>();

            origin = _origin;
            levelJsonUrl = jsonUrl;
        }
        

        public void LoadLevel()
        {
            var data = LoadJsonData(levelJsonUrl);
            CreateLevel(data);
        }


        private Vector3 IndexToPos(int id, int width, int height)
        {
            float x = origin.X + id % width;
            float y = 0;
            float z = origin.Y + id / width;
            Vector3 pos = new Vector3(x, y, z);
            return pos;
        }


        private RandomUnit[] GetUnitPrefabs(JsonData mapData, string key, JsonData configData)
        {
            var unitDatas = mapData[key];
            var count = unitDatas.Count;
            var res = new RandomUnit[count];
            for (int i = 0; i < count; i++)
            {
                var unitData = unitDatas[i];
                var weight = ((int)unitData[MapJsonKeys.weight]);
                var weightCut = ((int)unitData[MapJsonKeys.weightCut]);
                var url = configData[unitData[MapJsonKeys.name].ToString()][MapJsonKeys.url].ToString();
                var prefab = content.Load<Prefab>(url);
                res[i] = new RandomUnit()
                {
                    Prefab = prefab,
                    Weight = weight,
                    WeightCut = weightCut
                };
            }
            return res;
        }

        /// <summary>
        /// 创建随机单位
        private Entity InstantiateRandomUnit(RandomUnit[] units, Random random, Vector3 pos,  int weightCut = 0)
        {
            int total = 0;
            foreach (var unit in units) total += unit.Weight;
            var value = random.Next(0, total);
            Entity res = null;
            foreach (var unit in units)
            {
                value -= unit.Weight;
                if(value < 0)
                {
                    //降低自身权重
                    unit.Weight -= unit.WeightCut;
                    //static collider 不能在加入场景后在设置位置
                    res = unit.Prefab.Instantiate()[0];
                    res.Transform.Position = pos;
                    sceneSystem.SceneInstance.RootScene.Entities.Add(res);
                    break;
                }
            }
            return res;
        }


        private void CreateLevel(JsonData data)
        {
            
            var monsterData = LoadJsonData(data[MapJsonKeys.monsterJsonUrl].ToString());
            var barrierData = LoadJsonData(data[MapJsonKeys.barrierJsonUrl].ToString());
            var pickbleData = LoadJsonData(data[MapJsonKeys.pickableJsonUrl].ToString());
            int width, height, start, mapSeed, minAreaGridNum, maxAreaGridNum;
            double monsterDensity, pickableDensity;
            int unitSeed;
            try
            {
                width = ((int)data[MapJsonKeys.width]);
                height = ((int)data[MapJsonKeys.height]);
                start = ((int)data[MapJsonKeys.start]);
                mapSeed = ((int)data[MapJsonKeys.mapSeed]);
                minAreaGridNum = ((int)data[MapJsonKeys.minAreaGridNum]);
                maxAreaGridNum = ((int)data[MapJsonKeys.maxAreaGridNum]);
                monsterDensity = ((double)data[MapJsonKeys.monsterDensity]);   
                pickableDensity = ((double)data[MapJsonKeys.pickableDensity]);                  
            }
            catch (Exception)
            {
                throw new Exception("format error!");
            }
            unitSeed = mapSeed;
            int num = width * height;
            int[] grids = MapCreator.GetOriginGrids(width, height, start);
            var isSuccess = MapCreator.TryCreateSimpleMap(grids, start, width, height, mapSeed);
            

            if (isSuccess)
            {
                {
                    var mapUrl = data[MapJsonKeys.mapUrl].ToString();
                    //创建地图预制体
                    var mapPrefab = content.Load<Prefab>(mapUrl);
                    var map = mapPrefab.Instantiate()[0];
                    var s = MathF.Max(width, height) * Vector3.One;
                    //起点还得偏移半格才是左下角的位置
                    var center = new Vector3(origin.X - 0.5f, 0, origin.Y - 0.5f);
                    center.X += (width) * 0.5f;
                    center.Z += (height) * 0.5f;
                    map.Transform.Scale = s;
                    map.Transform.Position = center;
                    sceneSystem.SceneInstance.RootScene.Entities.Add(map);

                }

                {
                    var playerUrl = data[MapJsonKeys.playerUrl].ToString();
                    //创建玩家
                    if (playerUrl != null)
                    {
                        var playerPrefab = content.Load<Prefab>(playerUrl);
                        var player = playerPrefab.Instantiate()[0];
                        var pos = IndexToPos(start, width, height);
                        player.Transform.Position = pos;
                        player.Get<VelocityComponent>()?.UpdatePos(pos);
                        sceneSystem.SceneInstance.RootScene.Entities.Add(player);
                    }
                }

                //创建空气墙
                {
                    var airWallUrl = data[MapJsonKeys.airWallUrl].ToString();
                    var wallPrefab = content.Load<Prefab>(airWallUrl);
                    for (int i = 0; i < num; i++)
                    {
                        var grid = grids[i];
                        
                        if (grid == MapCreator.Wall)
                        {
                            var pos = IndexToPos(i, width, height);
                            var wall = wallPrefab.Instantiate()[0];
                            wall.Transform.Position = pos;
                            sceneSystem.SceneInstance.RootScene.Entities.Add(wall);
                        }
                    };
                    //往起点放个空气墙
                    {
                        var pos = IndexToPos(start, width, height);
                        var wall = wallPrefab.Instantiate()[0];
                        wall.Transform.Position = pos;
                        sceneSystem.SceneInstance.RootScene.Entities.Add(wall);

                    }
                }

                var units = MapCreator.CreateMapUnits(grids, width, height, start, unitSeed, minAreaGridNum, maxAreaGridNum, (float)monsterDensity, (float)pickableDensity);

                //创建地图单位
                {
                    var unitRandom = new Random(((int)data[MapJsonKeys.prefabSeed]));
                    var barrierPrefabs = GetUnitPrefabs(data, MapJsonKeys.barrier, barrierData);
                    var monsterPrefabs = GetUnitPrefabs(data, MapJsonKeys.monster, monsterData);
                    var pickablePrefabs = GetUnitPrefabs(data, MapJsonKeys.pickable, 
                    pickbleData);

                    var isCreateBarrier = barrierPrefabs.Length > 0;
                    var isCreateMonster = monsterPrefabs.Length > 0;
                    var isCreatePickable = pickablePrefabs.Length > 0;

                    for (int i = 0; i < num; i++)
                    {
                        var unit = units[i];
                        if (isCreateBarrier && (unit & MapCreator.BarrierUnit) > 0)
                        {
                            var pos = IndexToPos(i, width, height);
                            var barrier = InstantiateRandomUnit(barrierPrefabs, unitRandom, pos);
                           
                        }
                        if (isCreateMonster && (unit & MapCreator.MonsterUnit) > 0)
                        {
                            var pos = IndexToPos(i, width, height);
                            var monster = InstantiateRandomUnit(monsterPrefabs, unitRandom, pos);
                            monster.Get<VelocityComponent>()?.UpdatePos(pos);
                           
                        }
                        //道具的生成
                        if(isCreatePickable && (unit & MapCreator.PickableUnit) > 0)
                        {
                            var pos = IndexToPos(i, width, height);
                            InstantiateRandomUnit(pickablePrefabs, unitRandom, pos);
                        }
                    }
                }

            }
        }


        private JsonData LoadJsonData(string url)
        {
            JsonData jsonData = null;
            using (var stream = content.OpenAsStream(url, Stride.Core.IO.StreamFlags.Seekable))
            using (var streamReader = new StreamReader(stream))
            {
                jsonData = JsonMapper.ToObject(streamReader.ReadToEnd());
            }
            return jsonData;
        }
    }
}
