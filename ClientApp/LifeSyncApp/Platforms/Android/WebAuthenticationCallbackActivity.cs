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
        // Android.Util.Log usa semantica de String.Format ({0}, {1}), nao os placeholders
        // nomeados do ILogger. Passar "{Event}" fazia o String.Format tentar ler "Event"
        // como indice e lancar FormatException aqui no OnCreate, matando o processo antes
        // do callback do OAuth chegar ao WebAuthenticator. Interpolamos antes e mandamos
        // uma unica string, que resolve para a sobrecarga que nao formata.
        try
        {
            var data = intent?.Data;
            Log.Info(Tag,
                $"Event={lifecycleEvent} " +
                $"Action={intent?.Action ?? "-"} " +
                $"Scheme={data?.Scheme ?? "-"} " +
                $"Host={data?.Host ?? "-"} " +
                $"Path={data?.Path ?? "-"}");
        }
        catch (Exception ex)
        {
            // Log e diagnostico: nunca pode derrubar o fluxo de autenticacao.
            Log.Warn(Tag, $"Falha ao logar o intent de callback: {ex.Message}");
        }
    }
}
