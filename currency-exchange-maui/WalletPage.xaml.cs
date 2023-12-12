using System.Collections.ObjectModel;
using api_access;
using api_access.Models;

namespace currency_exchange_maui;

public partial class WalletPage : ContentPage
{
    private ObservableCollection<WalletModel> _wallets = new();

    public ObservableCollection<WalletModel> Wallets
    {
        get => _wallets;
        set
        {
            if (_wallets != value)
            {
                _wallets = value;
                OnPropertyChanged(nameof(Wallets));
            }
        }
    }

    private bool _isPageContentLoading = true;

    public bool IsPageContentLoading
    {
        get => _isPageContentLoading;
        set
        {
            if (_isPageContentLoading != value)
            {
                _isPageContentLoading = value;
                OnPropertyChanged(nameof(IsPageContentLoading));
            }
        }
    }

    private double _totalBalance;

    public double TotalBalance
    {
        get => _totalBalance;
        set
        {
            if (Math.Abs(_totalBalance - value) > 0.01)
            {
                _totalBalance = Math.Round(value,2);
                OnPropertyChanged(nameof(TotalBalance));
            }
        }
    }

    public WalletPage()
    {
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
        Wallets.Clear();
        TotalBalance = 0;

        IsPageContentLoading = true;

        try
        {
            var userId = Preferences.Get(nameof(LoginPage.UserId), defaultValue: -1);

            var wallets = await CurrencyExchangeAPI.GetWallets(userId);

            if (wallets == null) return;

            Wallets = new ObservableCollection<WalletModel>(wallets);
            
            foreach (var wallet in Wallets)
            {
                TotalBalance += wallet.ConvertedBalance;
            }
            
            BindingContext = this;
        }
        finally
        {
            IsPageContentLoading = false;
        }
    }
}