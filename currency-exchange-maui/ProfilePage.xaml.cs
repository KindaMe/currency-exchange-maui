namespace currency_exchange_maui;

public partial class ProfilePage : ContentPage
{
    public ProfilePage()
    {
        InitializeComponent();
    }

    private void OnLogoutButtonClicked(object sender, EventArgs e)
    {
        if (Application.Current != null)
        {
            Preferences.Remove(nameof(LoginPage.UserId));
            Application.Current.MainPage = new NavigationPage(new LoginPage());
        }
    }
}