using System.Collections.ObjectModel;
using api_access;
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
            if (_isPageContentLoading != value)
            {
                _isPageContentLoading = value;
                OnPropertyChanged();
            }
        }
    }

    private ObservableCollection<WalletModel> _wallets = new();

    public ObservableCollection<WalletModel> Wallets
    {
        get => _wallets;
        set
        {
            if (_wallets != value)
            {
                _wallets = value;
                OnPropertyChanged();
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
                _totalBalance = Math.Round(value, 2);
                OnPropertyChanged();
            }
        }
    }

    private double _totalGain;

    public double TotalGain
    {
        get => _totalGain;
        set
        {
            if (Math.Abs(_totalGain - value) > 0.01)
            {
                _totalGain = Math.Round(value, 2);
                OnPropertyChanged();
            }
        }
    }

    private double _totalGainPercentage;

    public double TotalGainPercentage
    {
        get => _totalGainPercentage;
        set
        {
            if (Math.Abs(_totalGainPercentage - value) > 0.01)
            {
                _totalGainPercentage = Math.Round(value, 2);
                OnPropertyChanged();
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
        TotalGain = 0;
        TotalGainPercentage = 0;

        IsPageContentLoading = true;

        try
        {
            var userId = Preferences.Get(nameof(LoginPage.UserId), defaultValue: -1);

            var wallets = await CurrencyExchangeAPI.GetWallets(userId);

            if (wallets == null) return;

            double portfolioTotalValue = 0;
            double portfolioInitialTotalValue = 0;
            double portfolioGain = 0;

            foreach (var wallet in wallets)
            {
                double walletInitialTotalValue = 0;

                if (wallet.currency != "PLN")
                {
                    var rate = await CurrencyExchangeAPI.GetCurrentCurrencyRate(wallet.currency);
                    wallet.CurrentRate = rate.rates[0].mid;
                    
                    foreach (var transaction in wallet.Transactions)
                    {
                        var transactionOldRate =
                            await CurrencyExchangeAPI.GetCurrencyRates(wallet.currency, transaction.date.AddDays(-7),
                                transaction.date);

                        walletInitialTotalValue += (double)transaction.amount_in * transactionOldRate.rates[^1].mid;
                    }

                    var walletGain = wallet.ConvertedBalance - walletInitialTotalValue;

                    portfolioGain += walletGain;

                    wallet.GainPercentage = (walletGain / walletInitialTotalValue) * 100.0d;
                }
                else
                {
                    wallet.CurrentRate = 1;

                    foreach (var transaction in wallet.Transactions)
                    {
                        walletInitialTotalValue += (double)transaction.amount_in;
                    }
                    
                }

                portfolioInitialTotalValue += walletInitialTotalValue;
                portfolioTotalValue += wallet.ConvertedBalance;
            }

            Wallets = new ObservableCollection<WalletModel>(wallets);


            TotalBalance = portfolioTotalValue;

            TotalGain = portfolioGain;

            TotalGainPercentage = ((portfolioTotalValue - portfolioInitialTotalValue) / portfolioInitialTotalValue) *
                                  100;
        }
        finally
        {
            IsPageContentLoading = false;
        }
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
        Navigation.PushAsync(new BuyPage());
    }

    private void WithdrawButton_OnClicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new WithdrawPage());
    }
}