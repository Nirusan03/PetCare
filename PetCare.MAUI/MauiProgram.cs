using Microsoft.Extensions.Logging;
using PetCare.MAUI.Services;

namespace PetCare.MAUI
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
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif
            string baseUrl;
            if(DeviceInfo.Platform == DevicePlatform.Android)
            {
                baseUrl = "http://10.0.2.2:5214";
            }
            else
            {
                baseUrl = "https://localhost:5214";
            }

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(baseUrl) });
            builder.Services.AddScoped<IAuthService, AuthService>();

            builder.Services.AddScoped<UserState>();
            return builder.Build();
        }
    }
}
