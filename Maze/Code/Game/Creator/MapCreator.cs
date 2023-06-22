using Stride.Core.Mathematics;
using System;
using System.Collections.Generic;


namespace Maze.Code.Game
{
    public class MapCreator 
    {
        public const int Wall = 0;
        public const int Way = 1;
        public const int Unknow = 3;
        private static Dictionary<int, int> blockWeightDic;
        public static Dictionary<int, int> BlockWeightDic
        {
            get
            {
                if(blockWeightDic == null)
                {
                    blockWeightDic = GetBlockWeightDic();
                }
                return blockWeightDic;
            }
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

        public static int[] GetOriginGrids(int widht, int height, out int start)
        {
            int num = widht * height;
            int[] grids = new int[num];
            start = widht / 2;
            for (int i = 0; i < num; i++)
            {
                bool isSide = i % widht == 0 || i % widht == widht - 1 ||
                    i / widht == 0 || i / widht == height - 1;

                bool isStart = i == start;
                grids[i] = isStart ? Way : (isSide ? Wall : Unknow);
            }
            return grids;
        }

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
                        
                        if(isLegalBlock)
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


        private static int[] GetNeighbors(int id, int width, int height)
        {
            var x = id % width;
            var y = id / width;
            var res = new List<int>();
            if (x > 0) res.Add(id - 1);
            if (x < width - 1) res.Add(id + 1);
            if (y > 0) res.Add(id - width);
            if (y < height - 1) res.Add(id + width);
            return res.ToArray();
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
            return sumWeight >0;   
        }


    }
}
