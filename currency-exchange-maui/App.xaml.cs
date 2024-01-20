using api_access;

namespace currency_exchange_maui;

public partial class App : Application
{
    private readonly IApiService _apiService;

    public App(IApiService apiService)
    {
        _apiService = apiService;

        InitializeComponent();

        _apiService.UnauthorizedRequest += OnUnauthorizedRequest;

        if (_apiService.GetAuthToken() != null)
        {
            MainPage = new AppShell();
        }
        else
        {
            MainPage = new LoginShell();
        }
    }

    private void OnUnauthorizedRequest(object sender, EventArgs e)
    {
        MainPage = new LoginShell();
    }
}