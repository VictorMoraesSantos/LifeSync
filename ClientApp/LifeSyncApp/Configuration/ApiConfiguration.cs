namespace LifeSyncApp.Configuration
{
    public enum ApiEnvironment
    {
        /// <summary>Gateway rodando na maquina de desenvolvimento (docker compose, porta 6006).</summary>
        Local = 0,

        /// <summary>Gateway publicado no dominio de producao.</summary>
        Production = 1
    }

    /// <summary>
    /// Centraliza os enderecos da API. Troque <see cref="Current"/> para alternar
    /// entre o ambiente local e o dominio de producao.
    /// </summary>
    public static class ApiConfiguration
    {
        /// <summary>
        /// Ambiente ativo. Em build Release sempre usa producao; em Debug usa o
        /// valor definido em <see cref="DebugEnvironment"/>.
        /// </summary>
        public static ApiEnvironment Current =>
#if DEBUG
            DebugEnvironment;
#else
            ApiEnvironment.Production;
#endif

        /// <summary>
        /// Ambiente usado em builds de Debug. Mude para <see cref="ApiEnvironment.Production"/>
        /// para testar o app local contra o dominio publicado.
        /// </summary>
        public const ApiEnvironment DebugEnvironment = ApiEnvironment.Local;

        /// <summary>Dominio de producao (fallback padrao).</summary>
        public const string ProductionBaseUrl = "https://api.lifesync.tech";

        /// <summary>Porta em que o YarpApiGateway e exposto pelo docker compose local.</summary>
        public const int LocalGatewayPort = 6006;

        /// <summary>
        /// Endereco usado pelo HttpClient para as chamadas de API.
        /// </summary>
        /// <remarks>
        /// No Android o loopback do device nao e o loopback do host. Rode
        /// <c>adb reverse tcp:6006 tcp:6006</c> (funciona tanto no emulador quanto
        /// em device fisico via USB) para que <c>localhost:6006</c> alcance o
        /// gateway da sua maquina. Isso mantem o mesmo host das chamadas de API e
        /// do fluxo OAuth, que precisa obrigatoriamente ser localhost.
        /// </remarks>
        public static string BaseUrl => Current switch
        {
            ApiEnvironment.Production => ProductionBaseUrl,
            _ => $"http://localhost:{LocalGatewayPort}"
        };

        /// <summary>
        /// Endereco usado para iniciar o fluxo OAuth no navegador.
        /// </summary>
        /// <remarks>
        /// O Google recusa IPs de rede privada (10.0.2.2, 192.168.x.x) como
        /// redirect URI, aceitando apenas localhost/127.0.0.1 ou dominio publico.
        /// Por isso o fluxo local sempre passa por localhost, mesmo que as
        /// chamadas de API estejam apontando para outro host.
        /// </remarks>
        public static string OAuthBaseUrl => Current switch
        {
            ApiEnvironment.Production => ProductionBaseUrl,
            _ => $"http://localhost:{LocalGatewayPort}"
        };

        /// <summary>
        /// Custom scheme registrado no AndroidManifest e no Info.plist. Precisa ser
        /// igual ao <c>GoogleAuth:AppScheme</c> configurado na API.
        /// </summary>
        public const string AppScheme = "com.lifesync.app";

        /// <summary>URL de callback que encerra o fluxo do WebAuthenticator.</summary>
        public const string OAuthCallbackUrl = $"{AppScheme}://callback";

        /// <summary>Endpoint que redireciona para a tela de consentimento do Google.</summary>
        public static string GoogleLoginUrl(string state) =>
            $"{OAuthBaseUrl.TrimEnd('/')}/auth/google-login?state={Uri.EscapeDataString(state)}";
    }
}
