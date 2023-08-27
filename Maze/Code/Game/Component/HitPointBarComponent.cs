using System.Runtime.Serialization;
using Stride.Engine;
using Stride.Engine.Design;
using Stride.UI.Controls;


namespace Maze.Code.Game
{
    //控制血条显示的组件
    [DataContract]
    [DefaultEntityComponentProcessor(typeof(HitPointBarProcessor), ExecutionMode = ExecutionMode.Runtime)]
    public class HitPointBarComponent : ScriptComponent
    {
       
        private Slider slider;
        

        public bool TryBind()
        {
            //尝试寻找组件
            bool isSuccess = true;
            try
            {
                slider = Entity.FindChild(DeafultConfig.UI_HITPOINT).Get<UIComponent>().Page.RootElement.FindName(DeafultConfig.SLIDER_HITPOINT) as Slider;
            }
            catch (System.Exception e)
            {
                isSuccess = false;
            }
            
            if(!isSuccess)
            {
                Log.Info("Bind Fail!");
            }
            return isSuccess;
        }

        public void Refresh(float progress)
        {
            slider.Value = progress;
        }

    
    }

}
