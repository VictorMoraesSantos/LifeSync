using Foundation;
using UIKit;

namespace LifeSyncApp
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        // Entrega o callback do custom scheme (com.lifesync.app://callback) ao
        // WebAuthenticator, encerrando o fluxo de login com Google.
        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            if (Microsoft.Maui.Authentication.WebAuthenticator.Default.OpenUrl(app, url, options))
                return true;

            return base.OpenUrl(app, url, options);
        }

        public override bool ContinueUserActivity(UIApplication application, NSUserActivity userActivity, UIApplicationRestorationHandler completionHandler)
        {
            if (Microsoft.Maui.Authentication.WebAuthenticator.Default.ContinueUserActivity(application, userActivity, completionHandler))
                return true;

            return base.ContinueUserActivity(application, userActivity, completionHandler);
        }
    }
}
