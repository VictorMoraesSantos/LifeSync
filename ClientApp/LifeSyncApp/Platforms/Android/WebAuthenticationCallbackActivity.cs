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
            // Explicitly forward the intent to ensure the base class processes it.
            // This helps with the race condition on first authentication attempt
            // where the callback may arrive before the WebAuthenticator is fully initialized.
            if (Intent != null)
            {
                OnNewIntent(Intent);
            }
        }

        protected override void OnNewIntent(Intent? intent)
        {
            base.OnNewIntent(intent);
            // Base class WebAuthenticatorCallbackActivity handles the callback signaling
        }
    }
}
