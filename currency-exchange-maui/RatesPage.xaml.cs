using System.Collections.ObjectModel;
using api_access;
using api_access.Models;

namespace currency_exchange_maui;

public partial class RatesPage : ContentPage
{
    private ObservableCollection<Rate> _rates = new ObservableCollection<Rate>();

    public ObservableCollection<Rate> Rates
    {
        get => _rates;
        set
        {
            if (_rates != value)
            {
                _rates = value;
                OnPropertyChanged(nameof(Rates));
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

    public RatesPage()
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
        Rates.Clear();

        IsPageContentLoading = true;

        try
        {
            var ratesTable = await CurrencyExchangeAPI.GetCurrentRates();

            if (ratesTable != null)
            {
                Rates = new ObservableCollection<Rate>(ratesTable[0].rates);
                BindingContext = this;
            }
        }
        finally
        {
            IsPageContentLoading = false;
        }
    }
}