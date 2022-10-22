namespace Maze.Map
{
    public class UnitElement : BaseElement<StaticData_Unit, DynamicData_Unit>
    {
        public UnitElement(StaticData_Unit staticData, DynamicData_Unit dynamicData) : base(staticData, dynamicData)
        {

        }

        public override bool IsWalkable()
        {
            return false;
        }
    }
}
