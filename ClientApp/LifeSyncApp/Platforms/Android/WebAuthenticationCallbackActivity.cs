using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;

namespace LifeSyncApp;

[Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTop, Exported = true)]
[IntentFilter(new[] { Android.Content.Intent.ActionView },
              Categories = new[] { Android.Content.Intent.CategoryDefault, Android.Content.Intent.CategoryBrowsable },
              DataScheme = "com.lifesync.app")]
public class WebAuthenticationCallbackActivity : Microsoft.Maui.Authentication.WebAuthenticatorCallbackActivity
{
    private const string Tag = "LS.GoogleAuthCallback";

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        LogIntent("OnCreate", Intent);
        base.OnCreate(savedInstanceState);
    }

    protected override void OnNewIntent(Intent? intent)
    {
        LogIntent("OnNewIntent", intent);
        base.OnNewIntent(intent);
    }

    private static void LogIntent(string lifecycleEvent, Intent? intent)
    {
        var data = intent?.Data;
        Log.Info(Tag, "Event={Event} Action={Action} Scheme={Scheme} Host={Host} Path={Path}",
            lifecycleEvent,
            intent?.Action ?? "-",
            data?.Scheme ?? "-",
            data?.Host ?? "-",
            data?.Path ?? "-");
    }
}
