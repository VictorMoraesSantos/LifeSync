using Android.App;
using Android.Content;
using Android.Content.PM;

namespace LifeSyncApp
{
    [Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTop, Exported = true)]
    [IntentFilter(
        new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataScheme = "com.lifesync.app",
        DataHost = "callback")]
    public class WebAuthenticationCallbackActivity : WebAuthenticatorCallbackActivity
    {
    }
}
