
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Graphics;
using Stride.Rendering;
using Stride.Rendering.Sprites;
using LitJson;
using System.IO;
using System.Collections.Generic;
using Stride.Input;
using System.Text;
using System;
using Stride.Core.Collections;
using ServiceWire;

namespace Maze.Map
{
    public class Level : SyncScript
    {
        
        public CameraComponent Camera;
        public string JsonAssetUrl;     
        public string levelId;
        
        private Grid[,] grids;
        private IPlayer player;
        private FastCollection<IEnemy> enemies = new FastCollection<IEnemy>(4);
        private int mapNumX;
        private int mapNumY;
        private int mapGridSize;

        private Keys upKey = Keys.Up;
        private Keys downKey = Keys.Down;
        private Keys leftKey = Keys.Left;
        private Keys rightKey = Keys.Right;
        private float lastUpdateTime;
        public float DeltaTime { get=> (float)Game.UpdateTime.Elapsed.TotalSeconds; }
        public int FrameCount { get => Game.UpdateTime.FrameCount; }
        public void AddElement(int x, int y, IElement element)
        {
            //TODO:可能需要越界检查
            var grid = grids[x, y];
            if(grid == null)
            {
                grid = new Grid();
                grids[x, y] = grid;
            }
            grid.Add(element);
        }

        public void RemoveElement(int x, int y, IElement element)
        {
            var grid = grids[x, y];
            if(grid != null)
            {
                grid.Remove(element);
            }
        }

        public void ElementMove(Int2 originPos, Int2 targetPos, IElement element)
        {
            RemoveElement(originPos.X, originPos.Y, element);
            AddElement(targetPos.X, targetPos.Y, element);
        }

        private void CreateTile(string assetUrl, int layer, int frameIndex, Int2 pos, Int2 gridId, bool isWalkable = true)
        {
            var staticData = new StaticData_Tile();
            staticData.AssetUrl = assetUrl;
            staticData.Layer = layer;
            staticData.FrameIndex = frameIndex;
            staticData.IsWalkable = isWalkable;
            var dynamicData = new DynamicData_Tile();
            dynamicData.Pos = pos;
            var tile = new TileElement(staticData, dynamicData);
            tile.SetLevel(this);
            AddElement(gridId.X, gridId.Y, tile);
        }

        private void CreatePlayer(string assetUrl, int layer, int frameIndex, Int2 pos, Int2 gridId)
        {
            var staticData = new StaticData_Unit();
            staticData.AssetUrl = assetUrl;
            staticData.Layer = layer;
            staticData.FrameIndex = frameIndex;
            var dynamicData = new DynamicData_Unit();
            dynamicData.Pos = pos;
            var playerComponent = new PlayerElement(staticData, dynamicData);
            AddElement(gridId.X, gridId.Y, playerComponent);
            playerComponent.SetLevel(this);
            player = playerComponent;
        }

        private void CreateEnemy(string assetUrl, int layer, int frameIndex, Int2 pos, Int2 gridId, Int2[] wayPoints, CycleFlag flag)
        {
            var staticData = new StaticData_Unit();
            staticData.AssetUrl = assetUrl;
            staticData.Layer = layer;
            staticData.FrameIndex = frameIndex;
            var dynamicData = new DynamicData_Enemy();
            dynamicData.Pos = pos;
            dynamicData.SetWayPoints(wayPoints, flag);
            var unit = new EnemyElement(staticData, dynamicData);
            unit.SetLevel(this);
            AddElement(gridId.X, gridId.Y, unit);
            enemies.Add(unit);
        }

        public Int2 PxToPos(int x, int y)
        {
           var pos = new Int2(x / mapGridSize , mapNumY - 1 - y / mapGridSize);
           return pos;
        }

        public Int2 PosToGridId(Int2 pos)
        {
            var gridId = pos;
            //TODO:越界时报错
            return gridId;
        }

        
        public Grid GetGrid(int x, int y)
        {
            if(x <0 || x >= mapNumX || y < 0 || y >= mapNumY)
            {
                return null;
            }
            //TODO:可能会有越界
            return grids[x, y];
        }

        //TODO:考虑一个单位占多格的情况
        public void ConvertJsonToLevel(string json, int id)
        {
            var data = JsonMapper.ToObject(json);
            var defs = data[MapUtils.defs];
            mapGridSize = ((int)data[MapUtils.defaultGridSize]);
            var levels = data[MapUtils.levels];
            //加载所有的TileSet
            var tilesetDic = new Dictionary<int, Tileset>();
            {
                var tilesets = defs[MapUtils.tilesets];
                var tilesetCount = tilesets.Count;
                for (int i = 0; i < tilesetCount; i++)
                {
                    var tilesetData = tilesets[i];
                    var relPath = tilesetData[MapUtils.relPath];
                    var isEmptyPath = relPath == null;
                    //空地址代表是LDtk的内置资源 不处理
                    if(isEmptyPath) continue;
                    var pxWid = ((int)tilesetData[MapUtils.pxWid]);
                    var pxHei = ((int)tilesetData[MapUtils.pxHei]);
                    var tileGridSize = ((int)tilesetData[MapUtils.tileGridSize]);
                    var numX = pxWid / tileGridSize;
                    var numY = pxHei / tileGridSize;
                    //用identifier记录改图在项目中的url
                    var identifier = ((string)tilesetData[MapUtils.identifier]);
                    var url = identifier.Replace('_', '/');
                    var uid = ((int)tilesetData[MapUtils.uid]);
                    var customData = tilesetData[MapUtils.customData];
                    var dataCount = customData.Count;
                    var set = new Tileset();
                    for (int j = 0; j < dataCount; j++)
                    {
                        var tileData = customData[j];
                        var dataJson = ((string)tileData[MapUtils.data]);
                        var tileJsonData = JsonMapper.ToObject(dataJson);
                        //TODO:检测dataJson是否合法
                        var tileId = ((int)tileData[MapUtils.tileId]);
                        set.AddJsonData(tileId, tileJsonData);
                    }
                    set.AssetUrl = url;
                    set.Uid = uid;
                    set.NumX = numX;
                    set.NumY = numY;
                    set.TileGridSize = tileGridSize;
                    tilesetDic[set.Uid] = set;
                }
            }

            //加载对应关卡
            {
                var level = levels[id];
                var width = ((int)level[MapUtils.pxWid]);
                var hieght = ((int)level[MapUtils.pxHei]);
                mapNumX = width / mapGridSize;
                mapNumY = hieght / mapGridSize;
                
                grids = new Grid[mapNumX, mapNumY];

                var layerInstances = level[MapUtils.layerInstances];
                var layerNum = layerInstances.Count;
                for (int i = 0; i < layerNum; i++)
                {
                    var layerInstance = layerInstances[i];
                    var layer = layerNum - i;
                    var layerType = ((string)layerInstance[MapUtils.__type]);
                    var isLoadTile = layerType == MapUtils.layer_Tiles || layerType == MapUtils.layer_intGrid;
                    var isLoadEntities = layerType == MapUtils.layer_entities;
                    //填充tile
                    if (isLoadTile)
                    {
                        var __tilesetDefUid = ((int)layerInstance[MapUtils.__tilesetDefUid]);
                        if(!tilesetDic.TryGetValue(__tilesetDefUid, out var tileset))
                        {
                            continue;
                        }
                        var assetUrl = tileset.AssetUrl; 
                        var isTiles = layerType == MapUtils.layer_Tiles;
                        var key = isTiles ? MapUtils.gridTiles : MapUtils.autoLayerTiles;
                        var tiles = layerInstance[key];
                        //暂时默认所有tile都是1x1的
                        //TODO:处理Tile的可行走和视野遮罩
                        var tileCount = tiles.Count;
                        for (int j = 0; j < tileCount; j++)
                        {
                            var tile = tiles[j];
                            var t = ((int)tile[MapUtils.t]);
                            //var d = ((int)tile[MapUtils.d]);
                            var px = tile[MapUtils.px];
                            var pos = PxToPos(((int)px[0]), ((int)px[1]));
                            var frameIndex = t;
                            var isWalkable = tileset.IsWalkable(frameIndex);
                            CreateTile(assetUrl, layer, frameIndex, pos, pos, isWalkable);
                        }
                    }
                    else if(isLoadEntities)
                    {
                        var key = MapUtils.entityInstances;
                        var entityInstances = layerInstance[key];
                        var entityCount = entityInstances.Count;
                        for (int j = 0; j < entityCount; j++)
                        {
                            var entityInstance = entityInstances[j];
                            var __tile = entityInstance[MapUtils.__tile];
                            var tilesetUid = ((int)__tile[MapUtils.tilesetUid]);
                            if(!tilesetDic.TryGetValue(tilesetUid, out var tileset))
                            {
                                continue;
                            }
                            var assetUrl = tileset.AssetUrl; 
                            var __tags = entityInstance[MapUtils.__tags];
                            string tag = null;
                            if(__tags.Count > 0) tag = ((string)__tags[0]);
                            var px = entityInstance[MapUtils.px];
                            var pos = PxToPos(((int)px[0]), ((int)px[1]));
                            var tileX = ((int)__tile[MapUtils.x]);
                            var tileY = ((int)__tile[MapUtils.y]);
                            var frameIndex = tileset.GetTileId(tileX, tileY);
                            var gridId = pos;
                            if(tag == MapUtils.tag_enemy)
                            {
                                Int2[] wayPoints = null;
                                CycleFlag flag = CycleFlag.Loop;
                                var fieldInstances = entityInstance[MapUtils.fieldInstances];
                                var fieldCount = fieldInstances.Count;
                                for (int k = 0; k < fieldCount; k++)
                                {
                                    var fieldInstance = fieldInstances[k];
                                    var identifier = ((string)fieldInstance[MapUtils.__identifier]);
                                    if(identifier == MapUtils.filed_wayPoints)
                                    {
                                        var values = fieldInstance[MapUtils.__value];
                                        int valutCount = values.Count;
                                        wayPoints = new Int2[valutCount + 1];
                                        wayPoints[0] = pos;
                                        for (int n = 0; n < valutCount; n++)
                                        {
                                            var value = values[n];
                                            wayPoints[n + 1] = new Int2(((int)value[MapUtils.cx]), mapNumY - 1 - ((int)value[MapUtils.cy]));
                                        }
                                    }
                                    else if(identifier == MapUtils.filed_cycle)
                                    {
                                        var value = ((string)fieldInstance[MapUtils.__value]);
                                        flag = Enum.Parse<CycleFlag>(value);
                                    }
                                }
                                

                                CreateEnemy(assetUrl, layer, frameIndex, pos, pos, wayPoints, flag);
                            }
                            else if(tag == MapUtils.tag_player)
                            {
                                CreatePlayer(assetUrl, layer, frameIndex, pos, pos);
                            }
                            else
                            {
                                CreateTile(assetUrl, layer, frameIndex, pos, pos);
                            }
                            
                        }

                    }
                }
            }
        }

        
        

        public override void Start()
        {
            base.Start();


            using (var stream = Content.OpenAsStream(JsonAssetUrl, Stride.Core.IO.StreamFlags.Seekable))
            using (var streamReader = new StreamReader(stream))
            {
                var json = streamReader.ReadToEnd();
                ConvertJsonToLevel(json, 0);
                foreach (var grid in grids)
                {
                    if(grid == null) continue;
                    foreach (var element in grid.Elements)
                    {
                        element.Create();
                    }
                }
            }
        }

        public bool IsWalkable(Int2 pos)
        {
            var gridId = PosToGridId(pos);
            var grid = GetGrid(gridId.X, gridId.Y);
            return grid?.IsWalkable() ?? false;
        }

        
       
        private void PlayerUpdate(){
            var dir = Int2.Zero;
            if(Input.IsKeyPressed(upKey)) dir.Y = 1;
            else if(Input.IsKeyPressed(downKey)) dir.Y = -1;
            else if(Input.IsKeyPressed(leftKey)) dir.X = -1;
            else if(Input.IsKeyPressed(rightKey)) dir.X = 1;

            if(dir != Int2.Zero) player?.Move(dir.X, dir.Y);
        }

        private void EnemyUpdate()
        {
            foreach (var enemy in enemies)
            {
                enemy.AutoMove();
            }
        }

        public override void Update()
        {
            PlayerUpdate();
            EnemyUpdate();
        }


    }
}
