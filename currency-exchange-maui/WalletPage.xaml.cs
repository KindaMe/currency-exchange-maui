using System.Collections.ObjectModel;
using System.Windows.Input;
using api_access;
using api_access.DTOs;
using api_access.Models;

namespace currency_exchange_maui;

public partial class WalletPage : ContentPage
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

    private ObservableCollection<WalletModel> _wallets = new();

    public ObservableCollection<WalletModel> Wallets
    {
        get => _wallets;
        set
        {
            if (_wallets == value) return;

            _wallets = value;
            OnPropertyChanged();
        }
    }

    private decimal _totalBalanceInLocalCurrency;

    public decimal TotalBalanceInLocalCurrency
    {
        get => _totalBalanceInLocalCurrency;
        set
        {
            _totalBalanceInLocalCurrency = value;
            OnPropertyChanged();
        }
    }

    public ICommand RefreshCommand { get; }
    private readonly IApiService _apiService;

    public WalletPage(IApiService apiService)
    {
        _apiService = apiService;

        RefreshCommand = new Command(LoadPageContent);

        InitializeComponent();

        BindingContext = this;

        LoadPageContent();
    }

    private async void LoadPageContent()
    {
        Wallets.Clear();
        TotalBalanceInLocalCurrency = 0;

        IsPageContentLoading = true;
        IsButtonProcessing = true;

        var wallets = await _apiService.GetWalletsAsync();

        if (wallets == null) return;

        var rateTable = await _apiService.GetCurrentRatesAsync();

        foreach (var wallet in wallets)
        {
            wallet.InitWalletDetails(rateTable);
        }

        TotalBalanceInLocalCurrency = wallets.Sum(wallet => wallet.CurrentBalanceInLocalCurrency);

        Wallets = new ObservableCollection<WalletModel>(wallets);


        IsPageContentLoading = false;
        IsButtonProcessing = false;
    }


    private void OpenDetails(object sender, TappedEventArgs e)
    {
        if (sender is Grid { BindingContext: WalletModel wallet })
        {
            Navigation.PushAsync(new WalletDetailsPage(wallet));
        }
    }

    private void BuyButton_OnClicked(object sender, EventArgs e)
    {
        IsButtonProcessing = true;

        var page = new BuyPage(_apiService, Wallets);

        page.Disappearing += (o, args) => LoadPageContent();

        Navigation.PushAsync(page);
    }

    private void WithdrawButton_OnClicked(object sender, EventArgs e)
    {
        IsButtonProcessing = true;

        var page = new WithdrawPage(_apiService, Wallets);

        page.Disappearing += (o, args) => LoadPageContent();

        Navigation.PushAsync(page);
    }

    private async void ImageButton_OnClicked(object sender, EventArgs e)
    {
        if (sender is not ImageButton clickedButton) return;

        IsButtonProcessing = true;

        try
        {
            var ratesTable = await _apiService.GetCurrentRatesAsync();
            var codes = ratesTable.rates.Select(rate => rate.code).ToList();
            var codesWithoutOwnedCurrencies =
                codes.Where(code => !Wallets.Select(wallet => wallet.Currency).Contains(code)).ToList();

            var result = await DisplayActionSheet("Select currency for the new wallet:", "Cancel", null,
                codesWithoutOwnedCurrencies.ToArray());

            if (result is null or "Cancel") return;

            var isSuccessful = await _apiService.PostWalletAsync(new WalletDto { Code = result });
            if (isSuccessful)
            {
                await DisplayAlert("Wallet created!", $"You selected: {result}", "OK");
            }
            else
            {
                await DisplayAlert("Error!", "Something went wrong!", "OK");
            }
        }
        finally
        {
            LoadPageContent();
        }
    }
}