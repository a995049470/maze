using Stride.UI;
using Stride.UI.Controls;
using Stride.UI.Events;

namespace Maze.Code.Game
{
    /// <summary>
    /// 初始界面
    /// </summary>
    public class UIWindow_Main : UIWindow
    {
        protected override string Url => "Prefab/UI/UI_Main";
        protected override UIWindowFlag Flag => UIWindowFlag.Main;
        private Button btn_enter;

        protected override void BindUIElements()
        {
            base.BindUIElements();
            btn_enter = root.FindVisualChildOfType<Button>(nameof(btn_enter));
            btn_enter.Click += OnEnterButtonDown;
        }

        private void OnEnterButtonDown(object sender, RoutedEventArgs args)
        {
            log.Info("i am click");
        }
    }
}
