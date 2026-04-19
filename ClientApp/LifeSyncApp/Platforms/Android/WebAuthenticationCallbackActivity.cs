using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

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
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Process intent on create
            if (Intent != null)
            {
                OnNewIntent(Intent);
            }
            // Finish immediately after processing to return to the app
            Finish();
        }
    }
}
