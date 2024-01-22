using System.Collections.ObjectModel;
using api_access;
using api_access.DTOs;
using api_access.Enums;
using api_access.Models;

namespace currency_exchange_maui;

public partial class BuyPage : ContentPage
{
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

    public BuyPage(IApiService apiService, ObservableCollection<WalletModel> wallets)
    {
        _apiService = apiService;

        InitializeComponent();

        Wallets = wallets;

        BindingContext = this;
    }

    private void TargetWalletPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var selectedTargetWallet = (WalletModel)TargetWalletPicker.SelectedItem;

        var filteredSourceWallets = Wallets.Where(wallet => wallet != selectedTargetWallet).ToList();

        SourceWalletPicker.ItemsSource = filteredSourceWallets;

        UpdateBottomPanel();
    }

    private void SourceWalletPicker_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        UpdateBottomPanel();
    }

    private async void BottomPanel_AnotherWallet()
    {
        var selectedTargetWallet = (WalletModel)TargetWalletPicker.SelectedItem;
        var selectedSourceWallet = (WalletModel)SourceWalletPicker.SelectedItem;

        var targetCurrency = selectedTargetWallet.Currency;
        var sourceCurrency = selectedSourceWallet.Currency;

        ExchangeRateTableModel targetCurrencyRate = null;
        ExchangeRateTableModel sourceCurrencyRate = null;

        if (targetCurrency != "PLN")
        {
            targetCurrencyRate = await _apiService.GetCurrentCurrencyRateAsync(targetCurrency);
        }

        if (sourceCurrency != "PLN")
        {
            sourceCurrencyRate = await _apiService.GetCurrentCurrencyRateAsync(sourceCurrency);
        }

        if (targetCurrencyRate == null && sourceCurrencyRate == null)
        {
            BottomPanel.IsVisible = false;
            return;
        }

        if (targetCurrencyRate == null)
        {
            ConversionSummaryLabel.Text = string.Format("{3} {1:N2} => {2} {0:N2} ", decimal.Parse(AmountEntry.Text),
                decimal.Parse(AmountEntry.Text) * (1 / sourceCurrencyRate.rates[0].bid), targetCurrency,
                sourceCurrency);

            if (decimal.Parse(AmountEntry.Text) * (1 / sourceCurrencyRate.rates[0].bid) >
                selectedSourceWallet.CurrentBalance)
            {
                BottomPanel.IsVisible = false;
                await DisplayAlert("Error", "Not enough money", "OK");
            }
        }
        else if (sourceCurrencyRate == null)
        {
            ConversionSummaryLabel.Text = string.Format("{3} {1:N2} => {2} {0:N2} ", decimal.Parse(AmountEntry.Text),
                decimal.Parse(AmountEntry.Text) * targetCurrencyRate.rates[0].ask, targetCurrency, sourceCurrency);

            if (decimal.Parse(AmountEntry.Text) * targetCurrencyRate.rates[0].ask > selectedSourceWallet.CurrentBalance)
            {
                BottomPanel.IsVisible = false;
                await DisplayAlert("Error", "Not enough money", "OK");
            }
        }
        else
        {
            var enteredAmount = decimal.Parse(AmountEntry.Text);

            var secondValue = enteredAmount * targetCurrencyRate.rates[0].ask;
            var firstValue = secondValue * (1 / sourceCurrencyRate.rates[0].bid);

            ConversionSummaryLabel.Text =
                string.Format(
                    $"{sourceCurrency} {firstValue:N2} => PLN {secondValue:N2} => {targetCurrency} {enteredAmount:N2}");

            if (firstValue > selectedSourceWallet.CurrentBalance)
            {
                BottomPanel.IsVisible = false;
                await DisplayAlert("Error", "Not enough money", "OK");
            }
        }
    }


    private void AmountEntry_OnCompleted(object sender, EventArgs e)
    {
        UpdateBottomPanel();
    }


    private async void Button_OnClicked(object sender, EventArgs e)
    {
        IsButtonProcessing = true;
        
        TransactionDto data;

        if (SourceCreditCard.IsChecked)
        {
            var targetWallet = (WalletModel)TargetWalletPicker.SelectedItem;
            var amount = decimal.Parse(AmountEntry.Text);

            if (targetWallet == null || amount <= 0)
            {
                await DisplayAlert("Error", "Invalid data", "OK");
                IsButtonProcessing = false;
                return;
            }

            data = new TransactionDto
            {
                Type = TransactionTypes.Deposit,
                ToWalletId = targetWallet.Id,
                Amount = amount
            };
        }
        else if (SourceAnotherWallet.IsChecked)
        {
            var targetWallet = (WalletModel)TargetWalletPicker.SelectedItem;
            var sourceWallet = (WalletModel)SourceWalletPicker.SelectedItem;
            var amount = decimal.Parse(AmountEntry.Text);

            if (targetWallet == null || sourceWallet == null || amount <= 0)
            {
                await DisplayAlert("Error", "Invalid data", "OK");
                IsButtonProcessing = false;
                return;
            }

            data = new TransactionDto
            {
                Type = TransactionTypes.Transfer,
                FromWalletId = sourceWallet.Id,
                ToWalletId = targetWallet.Id,
                Amount = amount
            };
        }
        else
        {
            await DisplayAlert("Error", "Invalid data", "OK");
            IsButtonProcessing = false;
            return;
        }


        var isSuccessful = await _apiService.PostTransactionAsync(data);

        if (isSuccessful)
        {
            await DisplayAlert("Success", "Transaction completed", "OK");
        }
        else
        {
            await DisplayAlert("Error", "Transaction failed", "OK");
        }

        await Navigation.PopAsync();
        
        IsButtonProcessing = false;
    }

    private void UpdateBottomPanel()
    {
        if (SourceCreditCard.IsChecked)
        {
            if (TargetWalletPicker.SelectedItem == null || string.IsNullOrEmpty(AmountEntry.Text))
            {
                BottomPanel.IsVisible = false;
                return;
            }

            ConversionSummaryLabel.Text =
                $"{AmountEntry.Text} {((WalletModel)TargetWalletPicker.SelectedItem).Currency}";
        }
        else if (SourceAnotherWallet.IsChecked)
        {
            if (TargetWalletPicker.SelectedItem == null || SourceWalletPicker.SelectedItem == null ||
                string.IsNullOrEmpty(AmountEntry.Text))
            {
                BottomPanel.IsVisible = false;
                return;
            }

            if (SourceAnotherWallet.IsChecked)
                BottomPanel_AnotherWallet();
        }

        BottomPanel.IsVisible = true;
    }

    private void SourceGroup_OnCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        UpdateBottomPanel();
    }
}