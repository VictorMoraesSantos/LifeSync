using LifeSyncApp.Services.ApiService.Implementation;
using LifeSyncApp.Services.ApiService.Interface;
using LifeSyncApp.Services.TaskManager.Implementation;
using LifeSyncApp.ViewModels.TaskManager;
using LifeSyncApp.Views.TaskManager.TaskItem;
using Microsoft.Extensions.Logging;
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
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddHttpClient("LifeSyncApi", client =>
            {
                client.BaseAddress = new Uri("http://10.0.2.2:6006");
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            // JsonSerializerOptions
            builder.Services.AddSingleton(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = false,
                WriteIndented = true
            });

            // API Services
            builder.Services.AddScoped(typeof(IApiService<>), typeof(ApiService<>));

            // Business Services
            builder.Services.AddSingleton<TaskItemService>();
            builder.Services.AddSingleton<TaskLabelService>();

            // ViewModels
            builder.Services.AddTransient<TaskItemsViewModel>();

            // Views
            builder.Services.AddTransient<TaskItemPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
