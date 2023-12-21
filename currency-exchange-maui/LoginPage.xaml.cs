using api_access;
using api_access.Models;

namespace currency_exchange_maui;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();

        NavigationPage.SetHasNavigationBar(this, false);
    }

    private async void OnLoginButtonClicked(object sender, EventArgs e)
    {
        var  userCredentials = new UserCredentialsModel
        {
            Email = EmailEntry.Text,
            Password = PasswordEntry.Text
        };

        await testpog.TranslateTo(0, -2000, 1000, Easing.SpringIn);

        Indicator.IsRunning = true;
        var authToken = await CurrencyExchangeAPI.RequestToken(userCredentials);

        if (authToken != null)
        {
            Preferences.Set(nameof(CurrencyExchangeAPI.AuthToken), authToken);
            if (Application.Current != null)
            {
                Application.Current.MainPage = new AppShell();
            }
        }
        else
        {
            Indicator.IsRunning = false;
            await testpog.TranslateTo(0, 0, 1000, Easing.SpringOut);
        }
    }

    private void OnRegisterButtonClicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new RegisterPage());
    }

    private void OnFirstEntryCompleted(object sender, EventArgs e)
    {
        PasswordEntry.Focus();
    }

    protected override void OnAppearing()
    {
        if (CurrencyExchangeAPI.AuthToken != null)
        {
            if (Application.Current != null)
            {
                Application.Current.MainPage = new AppShell();
            }
        }
    }
}