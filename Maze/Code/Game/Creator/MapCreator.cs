using Stride.Core.Mathematics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maze.Code.Game
{
    public class MapCreator 
    {
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
            dic[Convert.ToInt32("1110", 2)] = 2;
            dic[Convert.ToInt32("1101", 2)] = 2;
            dic[Convert.ToInt32("1011", 2)] = 2;
            dic[Convert.ToInt32("0111", 2)] = 2;

            dic[Convert.ToInt32("1100", 2)] = 2;
            dic[Convert.ToInt32("1010", 2)] = 2;
            dic[Convert.ToInt32("1001", 2)] = 2;
            dic[Convert.ToInt32("0110", 2)] = 2;
            dic[Convert.ToInt32("0101", 2)] = 2;
            dic[Convert.ToInt32("0011", 2)] = 2;

            dic[Convert.ToInt32("1000", 2)] = 3;
            dic[Convert.ToInt32("0100", 2)] = 3;
            dic[Convert.ToInt32("0010", 2)] = 3;
            dic[Convert.ToInt32("0001", 2)] = 3;


            //dic[Convert.ToInt32("1111", 2)] = 1;
            return dic;
        }

        //0:墙 1:路 3:墙或路
        public static bool TryCreateSimpleMap(int[] grids, int start, int width, int height, int seed)
        {
            bool IsOutSide(int id)
            {
                return id < 0 || id >= width * height;
            }
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
                    var blocks = GetPossibleBlocks(block);               
                    var blockList = new List<Int2>();
                    Array.ForEach(blocks, b =>
                    {
                        if (b.Y > 0) blockList.Add(b);
                    });
                    isSuccsss &= blockList.Count > 0;
                    if (!isSuccsss) break;
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

                        isLegalBlock = PassTest(start, width, height, temp);

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


            bool PassTest(int start, int width, int height, int[] temp)
            {
                var max = 8;
                var stack = new Stack<int>();
                stack.Push(start);
                temp[start] = max;
                while (stack.Count > 0)
                {
                    var sid = stack.Pop();
                    var sneighbors = GetNeighbors(sid, width, height);
                    foreach (var nid in sneighbors)
                    {
                        if (IsOutSide(nid)) continue;
                        var v = temp[nid];
                        if (v > 0 && v < max)
                        {
                            temp[nid] = max;
                            stack.Push(nid);
                        }
                    }
                }

                bool isPass = true;

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



        private static Int2[] GetPossibleBlocks(int[] inputBlock)
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
            Int2[] possibleBlocks = new Int2[blockCount];
            for (int i = 0; i < blockCount; i++)
            {
                int block = blocks[i];
                if (!BlockWeightDic.TryGetValue(block, out var weight))
                {
                    weight = 0;
                }
                possibleBlocks[i] = new Int2(block, weight);
            }
            return possibleBlocks;   
        }


    }
}
