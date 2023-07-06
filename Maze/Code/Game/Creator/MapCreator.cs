using Newtonsoft.Json.Linq;
using Stride.Core.Mathematics;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Windows.Forms.VisualStyles;

namespace Maze.Code.Game
{

    public class MapCreator
    {
        public const int Wall = 0;
        public const int Way = 1;
        public const int Unknow = 3;

        private const int invalidDistance = -1;
        private const int waitCheck = -2;
        private const int hasChecked = -1;
        private const int emptyUnit = 0;
        public const int BarrierUnit = 1;
        public const int MonsterUnit = 2;

        private static Dictionary<int, int> blockWeightDic;
        public static Dictionary<int, int> BlockWeightDic
        {
            get
            {
                if (blockWeightDic == null)
                {
                    blockWeightDic = GetBlockWeightDic();
                }
                return blockWeightDic;
            }
        }


        private static List<int> cacheList = new List<int>();
        private static int[] GetNeighbors(int id, int width, int height)
        {
            var x = id % width;
            var y = id / width;
            cacheList.Clear();
            if (x > 0) cacheList.Add(id - 1);
            if (x < width - 1) cacheList.Add(id + 1);
            if (y > 0) cacheList.Add(id - width);
            if (y < height - 1) cacheList.Add(id + width);
            return cacheList.ToArray();
        }




        #region 创建地图

        /// <summary>
        /// 创建地图
        /// 0:墙 1:路 3:墙或路
        /// </summary>
        /// <param name="grids"></param>
        /// <param name="start"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static bool TryCreateSimpleMap(int[] grids, int start, int width, int height, int seed)
        {

            var random = new Random(seed);
            //var temp = new int[width * height];

            var isSuccsss = true;
            var temp = new int[width * height];
            for (int y = 0; y < height - 1; y++)
            {
                if (!isSuccsss) break;
                for (int x = 0; x < width - 1; x++)
                {
                    var id = y * width + x;
                    var neighbors = new int[4]
                    {
                        id, id + 1, id + width, id + width + 1
                    };
                    var block = new int[]
                    {
                        grids[neighbors[0]],
                        grids[neighbors[1]],
                        grids[neighbors[2]],
                        grids[neighbors[3]],
                    };
                    isSuccsss = TryGetPossibleBlocks(block, out var blocks);
                    if (!isSuccsss) break;
                    var blockList = new List<Int2>(blocks);
                    while (blockList.Count > 0)
                    {
                        var sum = 0;
                        blockList.ForEach(b => sum += b.Y);
                        if (sum == 0)
                        {
                            isSuccsss = false;
                            break;
                        }
                        var r = random.Next(0, sum);
                        int targetId = 0;
                        for (targetId = 0; targetId < blockList.Count; targetId++)
                        {
                            r -= blockList[targetId].Y;
                            if (r < 0) break;
                        }
                        var targetBlock = blockList[targetId].X;


                        Array.Copy(grids, temp, width * height);
                        var isLegalBlock = true;
                        for (int i = 0; i < neighbors.Length; i++)
                        {
                            var neighborId = neighbors[i];
                            var value = (targetBlock & (1 << i)) >> i;
                            temp[neighborId] = value;
                        }

                        var sx_test = System.Math.Max(0, x - 1);
                        var sy_test = System.Math.Max(0, y - 1);
                        var ex_test = System.Math.Min(width, x + 3);
                        var ey_test = System.Math.Min(height, y + 3);
                        isLegalBlock &= BlockTest(sx_test, sy_test, ex_test, ey_test, temp, width);

                        if (isLegalBlock)
                            isLegalBlock &= PassTest(start, width, height, temp);


                        if (isLegalBlock)
                        {
                            for (int i = 0; i < neighbors.Length; i++)
                            {
                                var neighborId = neighbors[i];
                                var value = (targetBlock & (1 << i)) >> i;
                                grids[neighborId] = value;
                            }
                            blockList.Clear();
                        }
                        else
                        {
                            blockList.RemoveAt(targetId);
                            if (blockList.Count == 0) isSuccsss = false;
                        }
                    }
                    if (!isSuccsss) break;
                }
            }
            return isSuccsss;





        }
        /// <summary>
        /// 检查一篇区域的块是否合法
        /// </summary>
        static bool BlockTest(int sx, int sy, int ex, int ey, int[] temp, int width)
        {
            bool isSuccsss = true;
            int numX = ex - sx - 1;
            int numY = ey - sy - 1;
            int num = numX * numY;
            for (int i = 0; i < num; i++)
            {
                var id = sx + sy * width + i % numX + i / numX * width;
                var neighbors = new int[4]
                {
                    id, id + 1, id + width, id + width + 1
                };
                var block = new int[]
                {
                    temp[neighbors[0]],
                    temp[neighbors[1]],
                    temp[neighbors[2]],
                    temp[neighbors[3]],
                };
                isSuccsss = TryGetPossibleBlocks(block, out var res);
                if (!isSuccsss) break;
            }
            return isSuccsss;
        }
        static bool IsOutSide(int id, int total)
        {
            return id < 0 || id >= total;
        }

        static bool PassTest(int start, int width, int height, int[] temp)
        {
            var max = 8;
            bool isPass = true;
            var stack = new Stack<int>();
            stack.Push(start);
            temp[start] = max;
            while (stack.Count > 0)
            {
                var sid = stack.Pop();
                var sneighbors = GetNeighbors(sid, width, height);
                foreach (var nid in sneighbors)
                {
                    if (IsOutSide(nid, width * height)) continue;
                    var v = temp[nid];
                    if (v > 0 && v < max)
                    {
                        temp[nid] = max;
                        stack.Push(nid);
                    }
                }
            }


            foreach (var tempGrid in temp)
            {
                if (tempGrid > 0 && tempGrid < max)
                {
                    isPass = false;
                    break;
                }
            }

            return isPass;
        }





        private static bool TryGetPossibleBlocks(int[] inputBlock, out Int2[] possibleBlocks)
        {
            List<int> blocks = new List<int>();
            blocks.Add(0);
            for (int i = 0; i < inputBlock.Length; i++)
            {
                int grid = inputBlock[i];
                if (grid > 0)
                {
                    int gridNum = blocks.Count;
                    for (int j = 0; j < gridNum; j++)
                    {
                        if (grid == 1)
                        {
                            blocks[j] |= 1 << i;
                        }
                        else if (grid == 3)
                        {
                            blocks.Add(blocks[j] | (1 << i));
                        }
                    }
                }
            }
            int blockCount = blocks.Count;
            possibleBlocks = new Int2[blockCount];
            int sumWeight = 0;
            for (int i = 0; i < blockCount; i++)
            {
                int block = blocks[i];
                if (!BlockWeightDic.TryGetValue(block, out var weight))
                {
                    weight = 0;
                }
                sumWeight += weight;
                possibleBlocks[i] = new Int2(block, weight);
            }
            return sumWeight > 0;
        }

        //2x2 grid = block
        private static Dictionary<int, int> GetBlockWeightDic()
        {
            Dictionary<int, int> dic = new Dictionary<int, int>();
            dic[Convert.ToInt32("1110", 2)] = 100;
            dic[Convert.ToInt32("1101", 2)] = 100;
            dic[Convert.ToInt32("1011", 2)] = 100;
            dic[Convert.ToInt32("0111", 2)] = 100;

            dic[Convert.ToInt32("1100", 2)] = 100;
            dic[Convert.ToInt32("1010", 2)] = 100;
            dic[Convert.ToInt32("1001", 2)] = 0;
            dic[Convert.ToInt32("0110", 2)] = 0;
            dic[Convert.ToInt32("0101", 2)] = 100;
            dic[Convert.ToInt32("0011", 2)] = 100;

            dic[Convert.ToInt32("1000", 2)] = 40;
            dic[Convert.ToInt32("0100", 2)] = 40;
            dic[Convert.ToInt32("0010", 2)] = 40;
            dic[Convert.ToInt32("0001", 2)] = 40;


            //dic[Convert.ToInt32("1111", 2)] = 1;
            return dic;
        }

        public static int[] GetOriginGrids(int width, int height, int start)
        {
            int num = width * height;
            int[] grids = new int[num];
            for (int i = 0; i < num; i++)
            {
                bool isSide = i % width == 0 || i % width == width - 1 ||
                    i / width == 0 || i / width == height - 1;

                bool isStart = i == start;
                grids[i] = isStart ? Way : (isSide ? Wall : Unknow);
            }
            return grids;
        }

        #endregion

        #region 创建地图中的单位
        /// <summary>
        /// 创建地图中的单位
        /// </summary>
        /// <param name="grids">0:障碍 1:可行走</param>
        public static int[] CreateMapUnits(int[] grids, int width, int height, int start, int seed, int minAreaGridNum, int maxAreaGridNum, float monsterDensity, float minMonsterProbability)
        {
            var random = new Random(seed);
            //先计算所有位置到起点的位置
            var num = width * height;
            int[] distances = new int[num];
            int maxDis = 0;
            {
                for (int i = 0; i < num; i++)
                {
                    distances[i] = waitCheck;
                }
                var buffer0 = new List<int>();
                var buffer1 = new List<int>();
                buffer0.Add(start);
                int currentDis = 0;
                while (buffer0.Count > 0)
                {
                    foreach (var id in buffer0)
                    {
                        distances[id] = currentDis;
                    }                 
                    currentDis += 1;
                    foreach (var id in buffer0)
                    {
                        var neighbors = GetNeighbors(id, width, height);
                        foreach (var neighborId in neighbors)
                        {
                            if (grids[neighborId] == Way && distances[neighborId] == waitCheck)
                            {
                                distances[neighborId] = hasChecked;
                                buffer1.Add(neighborId);
                            }
                        }
                    }

                    //交换buffer
                    buffer0.Clear();
                    var temp = buffer0;
                    buffer0 = buffer1;
                    buffer1 = temp;

                }
                maxDis = Math.Max(currentDis - 1, 0);
            }


            //开始根据距离划分区域
            int[] units = new int[num];
            int[] gridAreaIndices = new int[num];
            var areaGridNums = new List<int>();
            //初步分割
            int segmentCount = 1;       
            {
                int t = maxDis / segmentCount;
                if (t * segmentCount < maxDis) t += segmentCount;
                
                for (int i = 0; i < num; i++)
                {
                    //重置区域Id
                    gridAreaIndices[i] = waitCheck;

                    //设置障碍点
                    var distance = distances[i];
                    int areaId = distance / t;
                    //设置障碍点
                    if (areaId > 0 && areaId < segmentCount && areaId * t == distance)
                    {
                        units[i] |= BarrierUnit;
                    };
                }

                int currentAreaIndex = 0;
                var stack = new Stack<int>();
                for (int i = 0; i < num; i++)
                {
                    bool isCheck = grids[i] == Way && units[i] == emptyUnit && gridAreaIndices[i] == waitCheck;
                    var gridNum = 0;
                    if(isCheck)
                    {
                        stack.Push(i);                       
                        while (stack.Count > 0)
                        {
                            var id = stack.Pop();
                            gridAreaIndices[id] = currentAreaIndex;
                            gridNum++;
                            var neighbors = GetNeighbors(id, width, height);
                            foreach (var neighbor in neighbors)
                            {
                                bool isCheckNeighbor = grids[neighbor] == Way && units[neighbor] == emptyUnit && gridAreaIndices[neighbor] == waitCheck;
                                if(isCheckNeighbor)
                                {
                                    gridAreaIndices[neighbor] = hasChecked;
                                    stack.Push(neighbor);
                                }
                            }
                        }
                        areaGridNums.Add(gridNum);
                        currentAreaIndex += 1;
                    }               
                }      
            }
         
            //第二次分割
            {
                var currentAreaMaxNum = 0;
                var gridIdCache = new List<int>();
                Dictionary<int, int> maxAreaGridNumDic = new Dictionary<int, int>();
                for (int i = 0; i < num; i++)
                {               
                    bool isEmptyGrid = grids[i] == Way && units[i] == emptyUnit;
                    if (!isEmptyGrid) continue;
                    var oldAreaId = gridAreaIndices[i];
                    if(!maxAreaGridNumDic.TryGetValue(oldAreaId, out currentAreaMaxNum))
                    {
                        currentAreaMaxNum = random.Next(minAreaGridNum, maxAreaGridNum);
                        maxAreaGridNumDic[oldAreaId] = currentAreaMaxNum;
                    }

                    bool isSuperArea = areaGridNums[oldAreaId] > currentAreaMaxNum;
                    if (!isSuperArea) continue;
                    //var emptyNeighborCount = 0;
                    //{
                    //    var neighbors = GetNeighbors(i, width, height);
                    //    foreach(var neighbor in neighbors)
                    //    {
                    //        var isEmptyNeighbor = grids[neighbor] == Way && units[neighbor] == emptyUnit;
                    //        emptyNeighborCount += isEmptyNeighbor ? 1 : 0;                      
                    //    }
                    //}
                    var isStartPoint = true;
                    if (!isStartPoint) continue;
                    var areaGridNum = currentAreaMaxNum;
                    //开始分割区域
                    gridIdCache.Add(i);
                    var queue = new Queue<int>();
                    queue.Enqueue(i);
                    while (queue.Count > 0)
                    {
                        var id = queue.Dequeue();
                        var neighbors = GetNeighbors(id, width, height);
                        foreach (var neighbor in neighbors)
                        {
                            var isEmptyNeighbor = grids[neighbor] == Way && units[neighbor] == emptyUnit;
                            var isNotContains = !gridIdCache.Contains(neighbor);
                            if(isEmptyNeighbor && isNotContains)
                            {
                                gridIdCache.Add(neighbor);
                                queue.Enqueue(neighbor);

                                if (gridIdCache.Count == areaGridNum) break;
                            }
                        }

                        if (gridIdCache.Count == areaGridNum)
                        {
                            break;
                        }
                    }
                    //分配新区域 生成新障碍
                    if(gridIdCache.Count == areaGridNum)
                    {
                        
                        //gridIdCache.ForEach(x => gridAreaIndices[x] = waitCheck);
                        for (int j = gridIdCache.Count - 1; j >= 0; j--)
                        {
                            var gridId = gridIdCache[j];
                            var neighbors = GetNeighbors(gridId, width, height);
                            foreach (var neighbor in neighbors)
                            {
                                var isEmptyNeighbor = grids[neighbor] == Way && units[neighbor] == emptyUnit;
                                var isOldAreaGrid = !gridIdCache.Contains(neighbor);
                                var isNotStartPoint = neighbor != start;
                                //设置为障碍
                                if (isEmptyNeighbor && isOldAreaGrid && isNotStartPoint)
                                {
                                    units[neighbor] |= BarrierUnit;
                                }
                            }
                        }
                        gridIdCache.Clear();
                        areaGridNums[oldAreaId] = 0;
                        //设置新区域
                        for (int j = 0; j < num; j++)
                        {
                            var isEmptyUnit = grids[j] == Way && units[j] == emptyUnit;
                            var isOldArea = gridAreaIndices[j] == oldAreaId;
                            if (isEmptyUnit && isOldArea)
                            {
                                var newAreaId = areaGridNums.Count;
                                var newAreaGridNum = 0;
                                var stack = new Stack<int>();
                                stack.Push(j);
                                while (stack.Count > 0)
                                {
                                    var id = stack.Pop();
                                    gridAreaIndices[id] = newAreaId;
                                    newAreaGridNum++;
                                    var neighbors = GetNeighbors(id, width, height);
                                    foreach (var neighbor in neighbors)
                                    {
                                        var isEmptyNeighbor = grids[neighbor] == Way && units[neighbor] == emptyUnit;
                                        var isNeighborOldArea = gridAreaIndices[neighbor] == oldAreaId;
                                        if (isEmptyNeighbor && isNeighborOldArea)
                                        {
                                            stack.Push(neighbor);
                                            gridAreaIndices[neighbor] = hasChecked;
                                        }
                                    }
                                }
                                areaGridNums.Add(newAreaGridNum);
                            }
                        }
                    }
                    
                }
            }
            var areaCount = areaGridNums.Count;
            var areaGridIdLists = new List<int>[areaCount];
            for (int i = 0; i < areaCount; i++)
            {
                areaGridIdLists[i] = new List<int>(areaGridNums[i]);
            }
            //TODO:放置宝物,放置怪物.....
            {
                var startPointAreaId = gridAreaIndices[start];
                for (int i = 0; i < num; i++)
                {
                    var isEmptyUnit = grids[i] == Way && units[i] == emptyUnit;
                    if (!isEmptyUnit) continue;
                    var areaId = gridAreaIndices[i];
                    areaGridIdLists[areaId].Add(i);
                }
                for (int i = 0; i < areaCount; i++)
                {
                    //初始区域没怪
                    if(i == startPointAreaId) continue;
                    var gridIdList = areaGridIdLists[i];
                    var areaGridNum = gridIdList.Count;
                    if (areaGridNum == 0) continue;
                    var p = (areaGridNum - minAreaGridNum) * monsterDensity;
                    p = Math.Max(minMonsterProbability, p);
                    var f = p - (int)p;
                    var monsterCount = (int)p + (f > random.NextSingle() ? 1 : 0);
                    for (int j = 0; j < monsterCount; j++)
                    {
                        var r = random.Next(j, areaGridNum);
                        var gridId = gridIdList[r];
                        units[gridId] |= MonsterUnit;

                        //将抽取过的Id移到列表头部
                        gridIdList[r] = gridIdList[j];
                        gridIdList[j] = gridId;
                    }
                    
                }
            }

            return units;
        }

        #endregion

    }
}
