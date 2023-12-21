using api_access;

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
        string email = EmailEntry.Text;
        string password = PasswordEntry.Text;

        await testpog.TranslateTo(0, -2000, 1000, Easing.SpringIn);

        Indicator.IsRunning = true;
        var authToken = await CurrencyExchangeAPI.GenerateToken(email, password);

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