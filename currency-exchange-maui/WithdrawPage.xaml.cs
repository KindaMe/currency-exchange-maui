using System.Collections.ObjectModel;
using api_access;
using api_access.DTOs;
using api_access.Enums;
using api_access.Models;

namespace currency_exchange_maui;

public partial class WithdrawPage : ContentPage
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

    public WithdrawPage(IApiService apiService, ObservableCollection<WalletModel> wallets)
    {
        _apiService = apiService;

        InitializeComponent();

        Wallets = wallets;

        BindingContext = this;
    }

    private void SourceWalletPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        UpdateSummaryLabel();
    }

    private void AmountEntry_OnCompleted(object sender, EventArgs e)
    {
        UpdateSummaryLabel();
    }

    private void UpdateSummaryLabel()
    {
        if (SourceWalletPicker.SelectedItem != null && !string.IsNullOrEmpty(AmountEntry.Text))
        {
            var selectedWallet = (WalletModel)SourceWalletPicker.SelectedItem;
            _ = decimal.TryParse(AmountEntry.Text, out var result) ? result : 0;

            if (result > selectedWallet.CurrentBalance)
            {
                DisplayAlert("Error", "Amount exceeds wallet balance", "OK");
            }
            else
            {
                WithdrawalSummaryLabel.Text =
                    $"{AmountEntry.Text} {((WalletModel)SourceWalletPicker.SelectedItem).Currency}";

                BottomPanel.IsVisible = true;
                return;
            }
        }

        BottomPanel.IsVisible = false;
    }

    private async void Button_OnClicked(object sender, EventArgs e)
    {
        IsButtonProcessing = true;
        
        var sourceWallet = (WalletModel)SourceWalletPicker.SelectedItem;
        var amount = decimal.TryParse(AmountEntry.Text, out var result) ? result : 0;

        if (sourceWallet == null || result <= 0)
        {
            await DisplayAlert("Error", "Invalid data", "OK");
            IsButtonProcessing = false;
            return;
        }

        var data = new TransactionDto
        {
            Type = TransactionTypes.Withdrawal,
            FromWalletId = sourceWallet.Id,
            Amount = amount
        };

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
}