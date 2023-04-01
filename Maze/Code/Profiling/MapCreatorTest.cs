using Maze.Code.Game;
using Stride.Core.Diagnostics;
using Stride.Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maze.Code.Profiling
{
    public class MapCreatorTest : StartupScript
    {
        public int Width;
        public int Height;
        public int StartId;
        public int Seed;
        public override void Start()
        {
            base.Start();
            var nums = new int[Width * Height];
            for (int i = 0; i < nums.Length; i++)
            {
                nums[i] = 3;
            }
            nums[StartId] = 1;
            
            var isSuccess =  MapCreator.TryCreateSimpleMap(nums, StartId, Width, Height, Seed);
            LogNums(nums, isSuccess);
            Log.Info(Test(nums).ToString());
        }

        bool Test(int[] nums)
        {
            int max = 4;
            var temp = nums;
            var start = StartId;
            temp[start] = max;
            var stack = new Stack<int>();
            var width = Width;
            var height = Height;
            stack.Push(start);
            while (stack.Count > 0)
            {
                var sid = stack.Pop();
                var sneighbors = new int[4]
                {
                     sid + 1, sid - 1, sid + width, sid - width
                };
                foreach (var nid in sneighbors)
                {
                    if (nid < 0 || nid >= width * height) continue;
                    var v = temp[nid];
                    if (v > 0 && v < max)
                    {
                        temp[nid] = max;
                        LogNums(nums, false);
                        stack.Push(nid);
                    }
                }
            }

            var isLegalBlock = true;
            foreach (var tempGrid in temp)
            {
                if (tempGrid > 0 && tempGrid < max)
                {
                    isLegalBlock = false;
                    break;
                }
            }
            return isLegalBlock;
        }

        void LogNums(int[] nums, bool isSuccess)
        {
            string log = $"isSuccess:{isSuccess}\n";
            for (int y = Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < Width; x++)
                {
                    var len = 3 - nums[x + y * Width].ToString().Length;
                    
                    log += nums[x + y * Width];
                    for (int i = 0; i < len; i++)
                    {
                        log += " ";
                    }
                }
                log += "\n";
            }
            Log.Info(log);
        }
    }
}
