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
            nums[StartId] = 6;
            LogNums(nums, isSuccess);
     
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
