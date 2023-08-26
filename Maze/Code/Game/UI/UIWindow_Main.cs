using Stride.Core;
using Stride.Core.Mathematics;
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
        private LevelManager levelManager;


        private Button btn_enter;

        protected override void InitServices(IServiceRegistry _service)
        {
            base.InitServices(_service);
            levelManager = _service.GetSafeServiceAs<LevelManager>();
        }

        protected override void BindUIElements()
        {
            base.BindUIElements();
            btn_enter = root.FindVisualChildOfType<Button>(nameof(btn_enter));
            btn_enter.Click += OnEnterButtonDown;
        }

        private void OnEnterButtonDown(object sender, RoutedEventArgs args)
        {
            levelManager.LoadLevel("Config/Level/level_1", Int2.Zero);
            root.Visibility = Visibility.Hidden;
        }
    }
}
