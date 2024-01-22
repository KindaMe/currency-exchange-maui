using api_access;
using api_access.DTOs;
using CommunityToolkit.Maui.Core.Platform;

namespace currency_exchange_maui;

public partial class LoginPage : ContentPage
{
    private bool _isButtonProcessing = false;

    public bool IsButtonProcessing
    {
        get => _isButtonProcessing;
        set
        {
            if (_isButtonProcessing == value) return;

            _isButtonProcessing = value;
            OnPropertyChanged();
        }
    }
    
    
    private readonly IApiService _apiService;

    public LoginPage(IApiService apiService)
    {
        _apiService = apiService;

        InitializeComponent();
        
        BindingContext = this;

        NavigationPage.SetHasNavigationBar(this, false);
    }

    private async void OnLoginButtonClicked(object sender, EventArgs e)
    {
        IsButtonProcessing = true;
        
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
        
        IsButtonProcessing = false;
    }

    private async void OnRegisterButtonClicked(object sender, EventArgs e)
    {
        IsButtonProcessing = true;
        await Navigation.PushAsync(new RegisterPage(_apiService));
        IsButtonProcessing = false;
    }

    private void OnFirstEntryCompleted(object sender, EventArgs e)
    {
        PasswordEntry.Focus();
    }
}