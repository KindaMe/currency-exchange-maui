using api_access.Models;

namespace currency_exchange_maui;

public partial class WalletDetailsPage : ContentPage
{
    public WalletModel DetailedWallet { get; set; }

    public WalletDetailsPage(WalletModel wallet)
    {
        InitializeComponent();

        DetailedWallet = wallet;

        BindingContext = this;
    }
    
    
}