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
        public static bool TryCreateSimpleMap(int[] grids, int start, int width, int height, int seed, out int[] disArray)
        {
            bool IsOutSide(int id)
            {
                return id < 0 || id >= width * height;
            }

            var random = new Random(seed);
            disArray = new int[width * height];
            var temp = new int[width * height];
            for (int i = 0; i < disArray.Length; i++)
            {
                disArray[i] = -1;
            }

   
            disArray[start] = 0;
            var checkList = new List<int>();
            checkList.Add(start);
            while (checkList.Count > 0)
            {
                var checkArray = checkList.ToArray();
                checkList.Clear();
                foreach(var id in checkArray)
                {
                    var currentDis = disArray[id];
                    var neighbors = new int[] { id - 1, id + 1, id - width, id + width };

                    for (int i = 0; i < neighbors.Length; i++)
                    {
                        var n = neighbors[i];
                        if (IsOutSide(n)) continue;
                        var dis = disArray[n];
                        var grid = grids[n];
                        if (dis < 0 && grid > 0)
                        {
                            disArray[n] = currentDis + 1;
                            checkList.Add(n);
                        }
                    }
                }

            }

            var isSuccsss = true;
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
                        if(sum == 0)
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
                        Array.Copy(disArray, temp, width * height);
                        var checkStack = new Stack<int>();
                        var waitSet = new HashSet<int>();
                        var isLegalBlock = true;

                        for (int i = 0; i < 4; i++)
                        {
                             var gridId = neighbors[i];
                            if((targetBlock & (1 << i)) == 0)
                            {
                                var dis = temp[gridId];
                                if(dis > 0)
                                {
                                    temp[gridId] = -1;
                                    var gridNeighbors = new int[]
                                    {
                                        gridId - 1, gridId + 1, gridId - width, gridId + width
                                    };
                                    Array.ForEach(gridNeighbors, id =>
                                    {
                                        if (!IsOutSide(id)) checkStack.Push(id);
                                    }); 
                                }                               
                            }
                            
                        }
                        int max = width * height;
                        
                        {
                            while (checkStack.Count > 0 && isLegalBlock)
                            {
                                var checkId = checkStack.Pop();
                                var midDis = temp[checkId];
                                if (midDis <= 0 || midDis == max) continue;
                                var checkNeighbors = new int[]
                                {
                                    checkId - 1, checkId + 1, checkId - width, checkId + width 
                                };
                                var hasLessNeighbor = false;
                                for (int i = 0; i < checkNeighbors.Length; i++)
                                {
                                    var checkNeighborId = checkNeighbors[i];
                                    if(IsOutSide(checkNeighborId)) continue;
                                    var checkDis = temp[checkNeighborId];
                                    if(checkDis >= 0 && checkDis < midDis)
                                    {
                                        hasLessNeighbor = true;
                                        break;
                                    }
                                }
                                if(hasLessNeighbor) continue;
                                temp[checkId] = max;
                                waitSet.Add(checkId);
                                bool hasNeighborNeedCheck = false;
                                for (int i = 0; i < checkNeighbors.Length; i++)
                                {
                                    var checkNeighborId = checkNeighbors[i];
                                    if(IsOutSide(checkNeighborId)) continue;
                                    var checkDis = temp[checkNeighborId];
                                    if(checkDis >= 0 && checkDis < max)
                                    {
                                        checkStack.Push(checkNeighborId);
                                        hasNeighborNeedCheck = true;
                                    }
                                    else if(checkDis == max)
                                    {
                                        hasNeighborNeedCheck = true;
                                    }
                                }
                                if(!hasNeighborNeedCheck)
                                {
                                    isLegalBlock = false;
                                }
                            }


                            int sideGridId = -1; 
                            foreach (var waitId in waitSet)
                            {
                                var waitsNeighbors = new int[]
                                {
                                    waitId - 1, waitId + 1, waitId - width, waitId + width
                                };
                                foreach (var nid in waitsNeighbors)
                                {
                                    if (IsOutSide(nid)) continue;
                                    if (temp[nid] >= 0 && temp[nid] < max)
                                    {
                                        sideGridId = waitId;
                                    }
                                }
                                if (sideGridId >= 0) break;
                            }
                            if(sideGridId == -1)
                            {
                                isLegalBlock = false;
                            }
                            else
                            {
                                var list = new List<int>(waitSet);
                                while (list.Count > 0 && isLegalBlock)
                                {
                                    isLegalBlock = false;
                                    for (int i = list.Count - 1; i >= 0 ; i--)
                                    {
                                        var sid = list[i];
                                        var sneighbors = new int[]
                                        {
                                            sid - 1, sid + 1, sid - width, sid + width
                                        };
                                        var min = max;
                                        foreach (var snid in sneighbors)
                                        {
                                            if(IsOutSide(snid)) continue;
                                            var v = temp[snid];
                                            if(v >= 0)
                                            {
                                                min = v < min ? v : min;
                                            }
                                        }
                                        if(min < max)
                                        {
                                            list.RemoveAt(i);
                                            temp[sid] = min + 1;
                                            isLegalBlock = true;
                                        }
                                    }
                                }
                            }
                        }
                        
                        if(isLegalBlock)
                        {
                            for (int i = 0; i < neighbors.Length; i++)
                            {
                                var neighborId = neighbors[i];
                                var value = (targetBlock & (1 << i)) >> i;
                                grids[neighborId] = value;
                            }
                            Array.Copy(temp, disArray, temp.Length);
                            blockList.Clear();
                        }
                        else
                        {
                            blockList.RemoveAt(targetId);
                            if(blockList.Count == 0) isSuccsss = false;                         
                        }
                    }
                    if (!isSuccsss) break;
                }
            }
            return isSuccsss;
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
