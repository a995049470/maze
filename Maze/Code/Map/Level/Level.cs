
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Graphics;
using Stride.Rendering;
using Stride.Rendering.Sprites;
using LitJson;
using System.IO;
using System.Collections.Generic;
using Stride.Input;

namespace Maze.Map
{
    public class Level : SyncScript
    {
        
        public CameraComponent Camera;
        public string JsonAssetUrl;     
        public string levelId;
        
        private Grid[,] grids;
        private IPlayer player;

        private Keys upKey = Keys.Up;
        private Keys downKey = Keys.Down;
        private Keys leftKey = Keys.Left;
        private Keys rightKey = Keys.Right;

        public void AddElementComponent(int x, int y, IElement element)
        {
            //TODO:可能需要越界检查
            var grid = grids[x, y];
            if(grid == null)
            {
                grid = new Grid();
                grids[x, y] = grid;

            }
            element.SetLevel(this);
            grid.Add(element);
        }

        private void CreateUnit(string assetUrl, int layer, int frameIndex, Int2 pos, Int2 gridId)
        {
            var staticData = new StaticData_Tile();
            staticData.AssetUrl = assetUrl;
            staticData.Layer = layer;
            staticData.FrameIndex = frameIndex;
            var dynamicData = new DynamicData_Tile();
            dynamicData.Pos = pos;
            var tile = new TileComponent(staticData, dynamicData);
            AddElementComponent(gridId.X, gridId.Y, tile);
        }

        private void CreatePlayer(string assetUrl, int layer, int frameIndex, Int2 pos, Int2 gridId)
        {
            var staticData = new StaticData_Unit();
            staticData.AssetUrl = assetUrl;
            staticData.Layer = layer;
            staticData.FrameIndex = frameIndex;
            var dynamicData = new DynamicData_Unit();
            dynamicData.Pos = pos;
            var playerComponent = new PlayerComponent(staticData, dynamicData);
            AddElementComponent(gridId.X, gridId.Y, playerComponent);
            player = playerComponent;
        }

        private void CreateTile(string assetUrl, int layer, int frameIndex, Int2 pos, Int2 gridId)
        {
            var staticData = new StaticData_Unit();
            staticData.AssetUrl = assetUrl;
            staticData.Layer = layer;
            staticData.FrameIndex = frameIndex;
            var dynamicData = new DynamicData_Unit();
            dynamicData.Pos = pos;
            var unit = new UnitComponent(staticData, dynamicData);
            AddElementComponent(gridId.X, gridId.Y, unit);
        }

        public Int2 PxToPos(int x, int y, int gridsize, int numY)
        {
           var pos = new Int2(x / gridsize , numY - 1 - y / gridsize);
           return pos;
        }

        public Int2 PosToGridId(Int2 pos)
        {
            var gridId = pos;
            return gridId;
        }

        //TODO:考虑一个单位占多格的情况
        public void ConvertJsonToLevel(string json, int id)
        {
            var data = JsonMapper.ToObject(json);
            var defs = data[MapUtils.defs];
            var gridsize = ((int)data[MapUtils.defaultGridSize]);
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
                    var set = new Tileset();
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
                var numX = width / gridsize;
                var numY = hieght / gridsize;
                grids = new Grid[numX, numY];

                var layerInstances = level[MapUtils.layerInstances];
                var layerNum = layerInstances.Count;
                for (int i = 0; i < layerNum; i++)
                {
                    var layerInstance = layerInstances[i];
                    var layer = layerNum - i;
                    var layerType = ((string)layerInstance[MapUtils.__type]);
                    var isLoadTile = layerType == MapUtils.Tiles || layerType == MapUtils.IntGrid;
                    var isLoadEntities = layerType == MapUtils.Entities;
                    //填充tile
                    if (isLoadTile)
                    {
                        var __tilesetDefUid = ((int)layerInstance[MapUtils.__tilesetDefUid]);
                        if(!tilesetDic.TryGetValue(__tilesetDefUid, out var tileset))
                        {
                            continue;
                        }
                        var assetUrl = tileset.AssetUrl; 
                        var isTiles = layerType == MapUtils.Tiles;
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
                            var pos = PxToPos(((int)px[0]), ((int)px[1]), gridsize, numY);
                            var frameIndex = t;
                            CreateTile(assetUrl, layer, frameIndex, pos, pos);
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
                            var pos = PxToPos(((int)px[0]), ((int)px[1]), gridsize, numY);
                            var tileX = ((int)__tile[MapUtils.x]);
                            var tileY = ((int)__tile[MapUtils.y]);
                            var frameIndex = tileset.GetTileId(tileX, tileY);
                            var gridId = pos;
                            if(tag == MapUtils.tag_enemy)
                            {
                                CreateUnit(assetUrl, layer, frameIndex, pos, pos);
                            }
                            if(tag == MapUtils.tag_player)
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

                        var entity = new Entity();
                        SceneSystem.SceneInstance.RootScene.Entities.Add(entity);
                        entity.Add(element.GetComponent());
                        element.Create();
                    }
                }
            }
        }

        private void PlayerUpdate(){
            var dir = Int2.Zero;
            if(Input.IsKeyPressed(upKey)) dir.Y = 1;
            else if(Input.IsKeyPressed(downKey)) dir.Y = -1;
            else if(Input.IsKeyPressed(leftKey)) dir.X = -1;
            else if(Input.IsKeyPressed(rightKey)) dir.X = 1;

            if(dir != Int2.Zero) player?.Move(dir.X, dir.Y);
        }

        public override void Update()
        {
            PlayerUpdate();
        }
    }
}
