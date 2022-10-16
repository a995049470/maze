
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Graphics;
using Stride.Rendering;
using Stride.Rendering.Sprites;
using LitJson;
using System.IO;
using System.Collections.Generic;

namespace Maze.Map
{

    public class Level : SyncScript
    {
        public class Tileset
        {
            public int Uid;
            public int NumX;
            public int NumY;
            public string AssetUrl;

        }

        public string JsonAssetUrl;     
        public string levelId;
        
        private Grid[,] grids;

        public void AddElementComponent(int x, int y, EntityComponent component)
        {
            //TODO:可能需要越界检查
            var grid = grids[x, y];
            if(grid == null)
            {
                grid = new Grid();
                grids[x, y] = grid;
            }
            grid.Add(component);
        }

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
                            var x = ((int)px[0]);
                            var y = ((int)px[1]);
                            
                            var pos = new Int2(x / gridsize , numY - 1 - y / gridsize);
                            var frameIndex = t;
                            var staticData = new StaticData_Tile();
                            staticData.AssetUrl = assetUrl;
                            staticData.FrameIndex = frameIndex;
                            var dynamicData = new DynamicData_Tile();
                            dynamicData.Pos = pos;
                            var tileComponent = new TileComponent(staticData, dynamicData);
                            //暂时用pos代替gridId
                            var gridId = pos;
                            AddElementComponent(gridId.X, gridId.Y, tileComponent);
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
                    foreach (var element in grid.ElementComponents)
                    {

                        if (element is TileComponent tileComponent)
                        {
                            var entity = new Entity();
                            SceneSystem.SceneInstance.RootScene.Entities.Add(entity);
                            entity.Add(tileComponent);
                            tileComponent.Create();
                        }

                    }
                }
            }
        }

        public override void Update()
        {
            
        }
    }
}
