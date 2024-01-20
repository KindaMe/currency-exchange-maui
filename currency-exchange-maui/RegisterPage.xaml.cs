using api_access;
using api_access.Models;
using CommunityToolkit.Maui.Core.Platform;

namespace currency_exchange_maui;

public partial class RegisterPage : ContentPage
{
    public UserModel NewUser { get; set; } = new();

    private readonly IApiService _apiService;

    public RegisterPage(IApiService apiService)
    {
        _apiService = apiService;

        InitializeComponent();

        BindingContext = this;
    }

    private async void Button_OnClicked(object sender, EventArgs e)
    {
        await PasswordEntry.HideKeyboardAsync(CancellationToken.None);
        
        var isSuccessful = await _apiService.PostUserAsync(NewUser);

        if (isSuccessful)
        {
            await DisplayAlert("Success", "User created", "OK");
        }
        else
        {
            await DisplayAlert("Error", "User creation failed", "OK");
        }
        
        await Navigation.PopAsync();
    }
}