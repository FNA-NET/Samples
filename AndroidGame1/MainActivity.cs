using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Microsoft.Xna.Framework;

namespace AndroidGame1
{
	[Activity(
		Label = "@string/app_name",
		MainLauncher = true,
		AlwaysRetainTaskState = true,
		LaunchMode = LaunchMode.SingleInstance,
		ScreenOrientation = ScreenOrientation.SensorLandscape,
		ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden |
								ConfigChanges.ScreenSize | ConfigChanges.ScreenLayout
	)]
	public class MainActivity : AndroidGameActivityEXT
	{
		protected override void SDLMain()
		{
            using (var game = new Game1())
                game.Run();
		}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			Window.AddFlags(WindowManagerFlags.KeepScreenOn);
			Window.AddFlags(WindowManagerFlags.TranslucentNavigation);
			Window.AddFlags(WindowManagerFlags.TranslucentStatus);
		}

		public override void OnWindowFocusChanged(bool hasFocus)
		{
			base.OnWindowFocusChanged(hasFocus);

			if (hasFocus)
				SetImmersive();
		}

		private void SetImmersive()
		{
			if (System.OperatingSystem.IsAndroidVersionAtLeast(30))
			{
				Window.SetDecorFitsSystemWindows(false);
				Window.InsetsController.SystemBarsBehavior = (int)WindowInsetsControllerBehavior.ShowTransientBarsBySwipe;
				//NO Color Type error.
				//Window.SetNavigationBarColor(Color.Transparent);
				Window.InsetsController.Hide(WindowInsets.Type.SystemBars());
			}
			else
			{
	#pragma warning disable CS0618
				this.Window.DecorView.SystemUiVisibility =
					(StatusBarVisibility) (SystemUiFlags.LayoutStable | SystemUiFlags.LayoutHideNavigation |
										SystemUiFlags.LayoutFullscreen | SystemUiFlags.HideNavigation |
										SystemUiFlags.Fullscreen | SystemUiFlags.ImmersiveSticky);
	#pragma warning restore CS0618
			}
		}
    }
}
