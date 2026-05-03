using LifeSyncApp.Services.ApiService.Implementation;
using LifeSyncApp.Services.ApiService.Interface;
using LifeSyncApp.Services.Auth;
using LifeSyncApp.Services.Financial;
using LifeSyncApp.Services.Nutrition;
using LifeSyncApp.Services.Profile;
using LifeSyncApp.Services.TaskManager;
using LifeSyncApp.Services.TaskManager.Implementation;
using LifeSyncApp.Services.UserSession;
using LifeSyncApp.ViewModels.Auth;
using LifeSyncApp.ViewModels.Financial;
using LifeSyncApp.ViewModels.Financial.Category;
using LifeSyncApp.ViewModels.Financial.Transaction;
using LifeSyncApp.ViewModels.Nutrition;
using LifeSyncApp.ViewModels.Profile;
using LifeSyncApp.ViewModels.TaskManager;
using LifeSyncApp.Views.Academic;
using LifeSyncApp.Views.Auth;
using LifeSyncApp.Views.Financial;
using LifeSyncApp.Views.Nutrition;
using LifeSyncApp.Views.Profile;
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
                    fonts.AddFont("Outfit-Regular.ttf", "OutfitRegular");
                    fonts.AddFont("Outfit-SemiBold.ttf", "OutfitSemiBold");
                });

            // Configure a URL base baseado na plataforma
            var baseUrl = "https://api.lifesync.tech";  // VPS produção

            // Para desenvolvimento local, descomente abaixo e comente a linha acima:
            // var baseUrl = DeviceInfo.Platform == DevicePlatform.Android && DeviceInfo.DeviceType == DeviceType.Virtual
            //     ? "http://10.0.2.2:6006"  // Emulador Android
            //     : "http://192.168.0.36:6006";  // Dispositivo físico

            // Auth DelegatingHandler - adds JWT token to every request
            builder.Services.AddTransient<AuthDelegatingHandler>();

            builder.Services.AddHttpClient("LifeSyncApi", client =>
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .AddHttpMessageHandler<AuthDelegatingHandler>()
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
#if ANDROID
                var handler = new Xamarin.Android.Net.AndroidMessageHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };
                return handler;
#else
                return new HttpClientHandler();
#endif
            });

            // JsonSerializerOptions
            builder.Services.AddSingleton(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            });

            // API Services
            builder.Services.AddSingleton(typeof(IApiService<>), typeof(ApiService<>));

            // Auth Service
            builder.Services.AddSingleton<IAuthService, AuthService>();

            // User Session
            builder.Services.AddSingleton<IUserSession, UserSession>();

            // Business Services
            builder.Services.AddSingleton<ITaskItemService, TaskItemService>();
            builder.Services.AddSingleton<ITaskLabelService, TaskLabelService>();

            // Financial Services
            builder.Services.AddSingleton<ITransactionService, TransactionService>();
            builder.Services.AddSingleton<ICategoryService, CategoryService>();

            // Nutrition Services
            builder.Services.AddSingleton<INutritionService, NutritionService>();

            // Profile Services
            builder.Services.AddSingleton<IUserProfileService, UserProfileService>();

            // Auth ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();

            // ViewModels - Singleton para manter estado entre navegações
            builder.Services.AddSingleton<TaskItemsViewModel>();
            builder.Services.AddSingleton<TaskLabelViewModel>();
            builder.Services.AddSingleton<FinancialViewModel>();
            builder.Services.AddSingleton<CategoriesViewModel>();
            builder.Services.AddSingleton<ManageTransactionViewModel>();
            builder.Services.AddSingleton<NutritionViewModel>();
            builder.Services.AddSingleton<ManageMealViewModel>();
            builder.Services.AddSingleton<MealDetailViewModel>();
            builder.Services.AddSingleton<ManageLiquidViewModel>();
            builder.Services.AddSingleton<DiaryDetailViewModel>();
            builder.Services.AddSingleton<FoodSearchViewModel>();
            builder.Services.AddSingleton<EditMealFoodViewModel>();
            builder.Services.AddSingleton<DailyProgressViewModel>();
            builder.Services.AddSingleton<DiaryHistoryViewModel>();
            builder.Services.AddSingleton<CreateDiaryViewModel>();
            builder.Services.AddSingleton<ManageCategoryViewModel>();
            builder.Services.AddSingleton<TransactionListViewModel>();
            builder.Services.AddTransient<TransactionDetailViewModel>();
            builder.Services.AddTransient<FilterTransactionViewModel>();

            // Profile ViewModels
            builder.Services.AddSingleton<ProfileViewModel>();
            builder.Services.AddSingleton<ChangeNameViewModel>();
            builder.Services.AddSingleton<ChangeEmailViewModel>();
            builder.Services.AddSingleton<ChangePasswordViewModel>();

            // Auth Views
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();

            // Views - Transient para criar nova instância sempre
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<TaskItemPage>();
            builder.Services.AddTransient<TaskItemDetailPage>();
            builder.Services.AddTransient<TaskLabelPage>();
            builder.Services.AddTransient<FinancialPage>();
            builder.Services.AddTransient<CategoriesPage>();
            builder.Services.AddTransient<ManageTransactionModal>();
            builder.Services.AddTransient<ManageCategoryModal>();
            builder.Services.AddTransient<TransactionListPage>();
            builder.Services.AddTransient<TransactionDetailModal>();
            builder.Services.AddTransient<FilterTransactionModal>();
            builder.Services.AddTransient<AcademicPage>();
            builder.Services.AddTransient<NutritionPage>();
            builder.Services.AddTransient<ManageMealModal>();
            builder.Services.AddTransient<MealDetailPage>();
            builder.Services.AddTransient<ManageLiquidModal>();
            builder.Services.AddTransient<DiaryDetailPage>();
            builder.Services.AddTransient<FoodSearchPage>();
            builder.Services.AddTransient<EditMealFoodModal>();
            builder.Services.AddTransient<DailyProgressPage>();
            builder.Services.AddTransient<DiaryHistoryPage>();
            builder.Services.AddTransient<CreateDiaryModal>();
            builder.Services.AddTransient<FilterTaskItemPopup>();
            builder.Services.AddTransient<ManageTaskItemModal>();
            builder.Services.AddTransient<ManageTaskLabelModal>();
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<ChangeNameModal>();
            builder.Services.AddTransient<ChangeEmailModal>();
            builder.Services.AddTransient<ChangePasswordModal>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
