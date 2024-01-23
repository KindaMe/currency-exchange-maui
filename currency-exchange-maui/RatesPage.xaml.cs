using System.Collections.ObjectModel;
using System.Windows.Input;
using api_access;
using api_access.Models;

namespace currency_exchange_maui;

public partial class RatesPage : ContentPage
{
    private ObservableCollection<Rate> _rates = new();

    public ObservableCollection<Rate> Rates
    {
        get => _rates;
        set
        {
            if (_rates == value) return;

            _rates = value;
            OnPropertyChanged();
        }
    }

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

    public ICommand RefreshCommand { get; }

    private readonly IApiService _apiService;

    public RatesPage(IApiService apiService)
    {
        _apiService = apiService;

        RefreshCommand = new Command(() => LoadPageContent());

        InitializeComponent();

        BindingContext = this;

        LoadPageContent();
    }

    private async void LoadPageContent(DateTime? date = null)
    {
        Rates.Clear();

        IsPageContentLoading = true;

        try
        {
            ExchangeRateTableModel ratesTable;

            if (date == null || DateOnly.FromDateTime(date.Value) == DateOnly.FromDateTime(DateTime.Now))
            {
                ratesTable = await _apiService.GetCurrentRatesAsync();
            }
            else
            {
                ratesTable = await _apiService.GetAllRatesWithDateAsync(date.Value);
            }

            if (ratesTable == null) return;

            Rates = new ObservableCollection<Rate>(ratesTable.rates);

            BindingContext = this;
        }
        finally
        {
            IsPageContentLoading = false;
        }
    }

    private void OpenDetails(object sender, TappedEventArgs e)
    {
        if (sender is Grid { BindingContext: Rate rate })
        {
            Navigation.PushAsync(new RateDetailsPage(_apiService, rate));
        }
    }

    private void DatePicker_OnDateSelected(object sender, DateChangedEventArgs e)
    {
        LoadPageContent(e.NewDate);
    }
}