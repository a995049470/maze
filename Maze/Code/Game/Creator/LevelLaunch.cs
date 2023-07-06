
using LitJson;
using Stride.Core;
using Stride.Core.Annotations;
using Stride.Core.Mathematics;
using Stride.Core.Serialization;
using Stride.Core.Threading;
using Stride.Engine;
using Stride.Physics;
using Stride.Rendering;
using System;
using System.IO;
using System.Threading;

namespace Maze.Code.Game
{
    public class LevelLaunch : StartupScript
    {
        [DataMember(10)]
        public string LevelJsonUrl;
        private Int2 origin = Int2.Zero;
       
        private Vector3 IndexToPos(int id, int width, int height)
        {
            float x = origin.X + id % width;
            float y = 0;
            float z = origin.Y + id / width;
            Vector3 pos = new Vector3(x, y, z);
            return pos;
        }


        private (int, Prefab)[] GetUnitPrefabs(JsonData mapData, string key, JsonData configData)
        {
            var unitDatas = mapData[key];
            var count = unitDatas.Count;
            var res = new (int, Prefab)[count];
            for (int i = 0; i < count; i++)
            {
                var unitData = unitDatas[i];
                var weight = ((int)unitData[MapJsonKeys.weight]);
                var url = configData[unitData[MapJsonKeys.name].ToString()][MapJsonKeys.url].ToString();
                var prefab = Content.Load<Prefab>(url);
                res[i] = (weight, prefab);
            }
            return res;
        }

        private Entity InstantiateRandomUnit((int, Prefab)[] units, Random random, Vector3 pos
            )
        {
            int total = 0;
            foreach (var unit in units) total += unit.Item1;
            var value = random.Next(0, total);
            Entity res = null;
            foreach (var unit in units)
            {
                value -= unit.Item1;
                if(value < 0)
                {
                    //static collider 不能在加入场景后在设置位置
                    res = unit.Item2.Instantiate()[0];
                    res.Transform.Position = pos;
                    SceneSystem.SceneInstance.RootScene.Entities.Add(res);
                }
            }
            return res;
        }


        public void CreateLevel(JsonData data)
        {
            
            var monsterData = LoadJsonData(data[MapJsonKeys.monsterJsonUrl].ToString());
            var barrierData = LoadJsonData(data[MapJsonKeys.barrierJsonUrl].ToString());
            int width, height, start, mapSeed, minAreaGridNum, maxAreaGridNum;
            double monsterDensity, minMonsterProbability;
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
                minMonsterProbability = ((double)data[MapJsonKeys.minMonsterProbability]);                          
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
                    var mapPrefab = Content.Load<Prefab>(mapUrl);
                    var map = mapPrefab.Instantiate()[0];
                    var s = MathF.Max(width, height) * Vector3.One;
                    //起点还得偏移半格才是左下角的位置
                    var center = new Vector3(origin.X - 0.5f, 0, origin.Y - 0.5f);
                    center.X += (width) * 0.5f;
                    center.Z += (height) * 0.5f;
                    map.Transform.Scale = s;
                    map.Transform.Position = center;
                    SceneSystem.SceneInstance.RootScene.Entities.Add(map);

                }

                {
                    var playerUrl = data[MapJsonKeys.playerUrl].ToString();
                    //创建玩家
                    if (playerUrl != null)
                    {
                        var playerPrefab = Content.Load<Prefab>(playerUrl);
                        var player = playerPrefab.Instantiate()[0];
                        var pos = IndexToPos(start, width, height);
                        player.Transform.Position = pos;
                        player.Get<VelocityComponent>()?.UpdatePos(pos);
                        SceneSystem.SceneInstance.RootScene.Entities.Add(player);
                    }
                }

                //创建空气墙
                {
                    var airWallUrl = data[MapJsonKeys.airWallUrl].ToString();
                    var wallPrefab = Content.Load<Prefab>(airWallUrl);
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

                var units = MapCreator.CreateMapUnits(grids, width, height, start, unitSeed, minAreaGridNum, maxAreaGridNum, (float)monsterDensity, (float)minMonsterProbability);

                //创建地图单位
                {
                    var unitRandom = new Random(((int)data[MapJsonKeys.prefabSeed]));
                    var barrierPrefabs = GetUnitPrefabs(data, MapJsonKeys.barrier, barrierData);
                    var monsterPrefabs = GetUnitPrefabs(data, MapJsonKeys.monster, monsterData);

                    var isCreateBarrier = barrierPrefabs.Length > 0;
                    var isCreateMonster = monsterPrefabs.Length > 0;

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
                    }
                }

            }
        }

       

        public override void Start()
        {
            var data = LoadJsonData(LevelJsonUrl);
            CreateLevel(data);
        }

        private JsonData LoadJsonData(string url)
        {
            JsonData jsonData = null;
            using (var stream = Content.OpenAsStream(url, Stride.Core.IO.StreamFlags.Seekable))
            using (var streamReader = new StreamReader(stream))
            {
                jsonData = JsonMapper.ToObject(streamReader.ReadToEnd());
            }
            return jsonData;
        }



    }
}
