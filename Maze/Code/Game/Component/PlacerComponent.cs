using Stride.Core;
using Stride.Core.Collections;
using Stride.Core.Mathematics;
using Stride.Core.Serialization;
using Stride.Engine;
using Stride.Engine.Design;
using Stride.Physics;
using System;

namespace Maze.Code.Game
{

    [DefaultEntityComponentProcessor(typeof(PlayerPlaceProcessor), ExecutionMode = ExecutionMode.Runtime)]
    [DefaultEntityComponentProcessor(typeof(PlacerProcessor), ExecutionMode = ExecutionMode.Runtime)]
    [DataContract]
    public class PlacerComponent : EntityComponent
    {
        [DataMemberIgnore]
        public bool IsReadyPlace = false;
        [DataMemberIgnore]
        public int CurrentPlaceItemCount = 0;

        private int maxPlaceItemCount = 1;
        [DataMember(10)]
        public int MaxPlaceItemCount
        {
            get => maxPlaceItemCount;
            set => maxPlaceItemCount = Math.Max(value, 0);
        }
        [DataMember(20)]
        public UrlReference<Prefab> ItemUrl;
        [DataMember(20)]
        public Vector3 LastPlacePos = Vector3.One * 65536;
    }

}
