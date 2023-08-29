﻿namespace Maze.Code.Game
{
    public static class ProcessorOrder
    {
        /// <summary>
        /// 最早执行, 用以回收oneShot组件
        /// </summary>
        public const int OneShot = -1000;
        /// <summary>
        /// 计算好攻击数值
        /// </summary>
        public const int ComputeAtk = 100;
        /// <summary>
        /// 给予对手伤害数值(未计算防御值)(可能需要考虑伤害来源,伤害类型)
        /// </summary>
        public const int Attack = 200;
        /// <summary>
        /// 计算防御能力
        /// </summary>
        public const int ComputeDef = 800;
        /// <summary>
        /// 计算真正税后伤害
        /// </summary>
        public const int ComputeHurtValue = 900;
        /// <summary>
        /// 角色真正收到伤害
        /// </summary>
        public const int Hurt = 1000;
        /// <summary>
        /// 拾取物体
        /// </summary>
        public const int Pick = 1500;        
        /// <summary>
        /// 被捡到的物体生效
        /// </summary>
        public const int PickedItemTakeEffect = 2000;
        /// <summary>
        /// 捡到的物体进入销毁流程
        /// </summary>
        public const int PickedItemDestory = 3000;
    }
}
