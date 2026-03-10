using System.Net.Http.Headers;

namespace LifeSyncApp.Services.Auth
{
    public class AuthDelegatingHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Skip auth header for login/register endpoints
            var path = request.RequestUri?.AbsolutePath ?? "";
            if (!path.Contains("/auth/login") && !path.Contains("/auth/register") && !path.Contains("/auth/external-login"))
            {
                try
                {
                    var token = await SecureStorage.GetAsync("access_token");
                    if (!string.IsNullOrEmpty(token))
                    {
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    }
                }
                catch
                {
                    // SecureStorage may fail on some platforms during startup
                }
            }

            var response = await base.SendAsync(request, cancellationToken);

            // If unauthorized, redirect to login
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    SecureStorage.RemoveAll();
                    if (Shell.Current != null)
                    {
                        await Shell.Current.GoToAsync("//LoginPage");
                    }
                });
            }

            return response;
        }
    }
}
