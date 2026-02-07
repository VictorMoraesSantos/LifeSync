using LifeSyncApp.Services.ApiService.Implementation;
using LifeSyncApp.Services.ApiService.Interface;
using LifeSyncApp.Services.TaskManager.Implementation;
using LifeSyncApp.ViewModels.TaskManager;
using LifeSyncApp.Views.TaskManager.TaskItem;
using LifeSyncApp.Views.TaskManager.TaskLabel;
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

            builder.Services.AddHttpClient("LifeSyncApi", client =>
            {
                client.BaseAddress = new Uri("http://10.0.2.2:6006");
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            // JsonSerializerOptions
            builder.Services.AddSingleton(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            });

            // API Services
            builder.Services.AddScoped(typeof(IApiService<>), typeof(ApiService<>));

            // Business Services
            builder.Services.AddScoped<TaskItemService>();
            builder.Services.AddScoped<TaskLabelService>();

            // ViewModels
            builder.Services.AddSingleton<TaskItemsViewModel>();
            builder.Services.AddSingleton<TaskLabelViewModel>();

            // Views
            builder.Services.AddTransient<TaskItemPage>();
            builder.Services.AddTransient<TaskItemDetailPage>();
            builder.Services.AddTransient<TaskLabelPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
