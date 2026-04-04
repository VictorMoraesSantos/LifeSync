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

            // After the base class forwards the callback data to the
            // WebAuthenticatorIntermediateActivity, explicitly bring
            // MainActivity to the foreground so the Chrome Custom Tab
            // is no longer visible (it stays on top otherwise, showing
            // a blank white screen).
            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
            StartActivity(intent);
        }
    }
}
