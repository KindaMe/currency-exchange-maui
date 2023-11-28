using api_access;

namespace currency_exchange_maui;

public partial class LoginPage : ContentPage
{
    private int userId;

    public LoginPage()
    {
        InitializeComponent();

        // if (Preferences.ContainsKey(nameof(userId)))
        // {
        //     Application.Current.MainPage = new AppShell();
        // }

        NavigationPage.SetHasNavigationBar(this, false);
    }

    private async void OnLoginButtonClicked(object sender, EventArgs e)
    {
        string email = EmailEntry.Text;
        string password = PasswordEntry.Text;

        await testpog.TranslateTo(0, -2000, 1000, Easing.SpringIn);

        Indicator.IsRunning = true;
        userId = await CurrencyExchangeAPI.AuthenticateUser(email, password);

        if (userId != -1)
        {
            Preferences.Set(nameof(userId), userId);
            Application.Current.MainPage = new AppShell();
        }
        else
        {
            Indicator.IsRunning = false;
            await testpog.TranslateTo(0, 0, 1000, Easing.SpringOut);
        }
    }

    private void OnRegisterButtonClicked(object sender, EventArgs e)
    {
        //ph
    }

    private void OnFirstEntryCompleted(object sender, EventArgs e)
    {
        PasswordEntry.Focus();
    }
}