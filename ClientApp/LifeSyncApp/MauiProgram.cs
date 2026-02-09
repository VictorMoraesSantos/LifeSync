using LifeSyncApp.Services.ApiService.Implementation;
using LifeSyncApp.Services.ApiService.Interface;
using LifeSyncApp.Services.TaskManager.Implementation;
using LifeSyncApp.Services.Financial;
using LifeSyncApp.ViewModels.TaskManager;
using LifeSyncApp.ViewModels.Financial;
using LifeSyncApp.Views.TaskManager.TaskItem;
using LifeSyncApp.Views.TaskManager.TaskLabel;
using LifeSyncApp.Views.Financial;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;
using System.Text.Json;

namespace LifeSyncApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureMauiHandlers(handlers =>
                {
#if ANDROID
                    var transparentColorStateList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);

                    EntryHandler.Mapper.AppendToMapping("NoUnderline", (handler, view) =>
                    {
                        handler.PlatformView.BackgroundTintList = transparentColorStateList;
                        handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
                    });

                    EditorHandler.Mapper.AppendToMapping("NoUnderline", (handler, view) =>
                    {
                        handler.PlatformView.BackgroundTintList = transparentColorStateList;
                        handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
                    });
#endif
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Configure a URL base baseado na plataforma
            var baseUrl = DeviceInfo.Platform == DevicePlatform.Android && DeviceInfo.DeviceType == DeviceType.Virtual
                ? "http://10.0.2.2:6006"  // Emulador Android
                : "http://192.168.0.36:6006";  // Dispositivo físico

            // HttpClient com timeout otimizado
            builder.Services.AddHttpClient("LifeSyncApi", client =>
            {
                client.BaseAddress = new Uri(baseUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
#if ANDROID
                var handler = new Xamarin.Android.Net.AndroidMessageHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true,
                    AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
                };
                return handler;
#else
                return new HttpClientHandler
                {
                    AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
                };
#endif
            });

            // JsonSerializerOptions otimizado com conversores de enum
            builder.Services.AddSingleton(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                Converters =
                {
                    new System.Text.Json.Serialization.JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            });

            // API Services
            builder.Services.AddScoped(typeof(IApiService<>), typeof(ApiService<>));

            // Business Services
            builder.Services.AddScoped<TaskItemService>();
            builder.Services.AddScoped<TaskLabelService>();
            builder.Services.AddScoped<FinancialService>();

            // ViewModels - Scoped para evitar memory leaks
            builder.Services.AddScoped<TaskItemsViewModel>();
            builder.Services.AddScoped<TaskLabelViewModel>();
            builder.Services.AddScoped<TransactionsViewModel>();
            builder.Services.AddScoped<ManageTransactionViewModel>();

            // Views - Transient para criar nova instância sempre
            builder.Services.AddTransient<TaskItemPage>();
            builder.Services.AddTransient<TaskItemDetailPage>();
            builder.Services.AddTransient<TaskLabelPage>();
            builder.Services.AddTransient<TransactionsPage>();
            builder.Services.AddTransient<ManageTransactionPage>();

#if DEBUG
            builder.Logging.AddDebug();
            builder.Logging.SetMinimumLevel(LogLevel.Debug);
#else
            builder.Logging.SetMinimumLevel(LogLevel.Warning);
#endif

            return builder.Build();
        }
    }
}
