using api_access;
using CommunityToolkit.Maui;
using Microcharts.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;

namespace currency_exchange_maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMicrocharts()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<IApiService, ApiService>();

        builder.Services.AddTransient<WalletPage>();
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<RatesPage>();
        builder.Services.AddTransient<LoginPage>();
        
        builder.Services.AddSingleton<RegisterPage>();
        builder.Services.AddSingleton<WithdrawPage>();
        builder.Services.AddSingleton<RateDetailsPage>();
        builder.Services.AddSingleton<BuyPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}