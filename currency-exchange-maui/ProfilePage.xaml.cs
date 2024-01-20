using api_access;
using api_access.DTOs;
using api_access.Models;

namespace currency_exchange_maui;

public partial class ProfilePage : ContentPage
{
    private bool _isPageContentLoading = true;

    public bool IsPageContentLoading
    {
        get => _isPageContentLoading;
        set
        {
            if (_isPageContentLoading == value) return;

            _isPageContentLoading = value;
            OnPropertyChanged();
        }
    }

    private UserModel _userDetails;

    public UserModel UserDetails
    {
        get => _userDetails;
        set
        {
            _userDetails = value;
            OnPropertyChanged();
        }
    }

    private readonly IApiService _apiService;

    public ProfilePage(IApiService apiService)
    {
        _apiService = apiService;

        InitializeComponent();

        BindingContext = this;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        LoadPageContent();
    }

    private async void LoadPageContent()
    {
        IsPageContentLoading = true;

        UserDetails = await _apiService.GetUserDetailsAsync();

        IsPageContentLoading = false;
    }

    private async void ChangeEmailButton_OnClicked(object sender, EventArgs e)
    {
        var newEmail = await DisplayPromptAsync("Change Email", "Enter new email:", placeholder: UserDetails.Email,
            keyboard: Keyboard.Email);
        if (string.IsNullOrEmpty(newEmail)) return;
        var confirmPassword =
            await DisplayPromptAsync("Change Email", "Confirm your password:", keyboard: Keyboard.Text);
        if (string.IsNullOrEmpty(confirmPassword)) return;

        var isSuccess = await _apiService.PutEmailAsync(new UserUpdatedDetailsDto
            { ConfirmPassword = confirmPassword, NewEmail = newEmail });

        if (isSuccess)
        {
            await DisplayAlert("Change Email",
                "Email changed successfully. You will be logged out now.",
                "Ok");

            _apiService.SetAuthToken(null);

            if (Application.Current != null)
                Application.Current.MainPage = new LoginShell();
        }
        else
        {
            await DisplayAlert("Change Email",
                "Incorrect password. Please try again.",
                "Ok");
        }
    }

    private async void ChangePasswordButton_OnClicked(object sender, EventArgs e)
    {
        var currentPassword =
            await DisplayPromptAsync("Change Password", "Enter your current password:", keyboard: Keyboard.Text);
        if (string.IsNullOrEmpty(currentPassword)) return;
        var newPassword =
            await DisplayPromptAsync("Change Password", "Enter your new password:", keyboard: Keyboard.Text);
        if (string.IsNullOrEmpty(newPassword)) return;
        var confirmPassword =
            await DisplayPromptAsync("Change Password", "Confirm your new password:", keyboard: Keyboard.Text);

        if (newPassword != confirmPassword)
        {
            await DisplayAlert("Change Password", "Passwords do not match", "OK");
        }
        
        var isSuccess = await _apiService.PutPasswordAsync(new UserUpdatedDetailsDto
            { ConfirmPassword = currentPassword, NewPassword = newPassword });

        if (isSuccess)
        {
            await DisplayAlert("Change Password",
                "Password changed successfully. You will be logged out now.",
                "Ok");

            _apiService.SetAuthToken(null);

            if (Application.Current != null)
                Application.Current.MainPage = new LoginShell();
        }
        else
        {
            await DisplayAlert("Change Password",
                "Incorrect password. Please try again.",
                "Ok");
        }
    }


    private async void DeleteAccountButton_OnClicked(object sender, EventArgs e)
    {
        var isUserSure = await DisplayAlert("Delete Account",
            "Deleting your account will result in loss of all your data and money. Are you sure you want to proceed?",
            "Yes", "No");

        if (!isUserSure)
            return;

        var confirmPassword =
            await DisplayPromptAsync("Delete Account", "Confirm your password:", keyboard: Keyboard.Text);

        if (string.IsNullOrEmpty(confirmPassword)) return;

        var isSuccess = await _apiService.DeleteUserAsync(new UserUpdatedDetailsDto
            { ConfirmPassword = confirmPassword });

        if (isSuccess)
        {
            await DisplayAlert("Delete Account",
                "Account deleted successfully. You will be logged out now.",
                "Ok");

            _apiService.SetAuthToken(null);

            if (Application.Current != null)
                Application.Current.MainPage = new LoginShell();
        }
        else
        {
            await DisplayAlert("Delete Account",
                "Incorrect password. Please try again.",
                "Ok");
        }
    }

    private void OnLogoutButtonClicked(object sender, EventArgs e)
    {
        _apiService.SetAuthToken(null);
        if (Application.Current != null) Application.Current.MainPage = new LoginShell();
    }
}