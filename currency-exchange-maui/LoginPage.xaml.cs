using api_access;
using api_access.DTOs;
using CommunityToolkit.Maui.Core.Platform;

namespace currency_exchange_maui;

public partial class LoginPage : ContentPage
{
    private readonly IApiService _apiService;

    public LoginPage(IApiService apiService)
    {
        _apiService = apiService;

        InitializeComponent();

        NavigationPage.SetHasNavigationBar(this, false);
    }

    private async void OnLoginButtonClicked(object sender, EventArgs e)
    {
        await PasswordEntry.HideKeyboardAsync(CancellationToken.None);
        
        var userCredentials = new UserCredentialsDto
        {
            Email = EmailEntry.Text,
            Password = PasswordEntry.Text
        };

        await MainStackLayout.TranslateTo(0, -2000, 1000, Easing.SpringIn);

        Indicator.IsRunning = true;
        await _apiService.RequestAndSetTokenAsync(userCredentials);

        if (_apiService.GetAuthToken() != null)
        {
            if (Application.Current != null)
            {
                Application.Current.MainPage = new AppShell();
            }
        }
        else
        {
            Indicator.IsRunning = false;
            await MainStackLayout.TranslateTo(0, 0, 1000, Easing.SpringOut);
        }
    }

    private void OnRegisterButtonClicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new RegisterPage(_apiService));
    }

    private void OnFirstEntryCompleted(object sender, EventArgs e)
    {
        PasswordEntry.Focus();
    }
}