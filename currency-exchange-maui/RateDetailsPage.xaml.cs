using api_access;
using api_access.Models;
using Microcharts;
using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace currency_exchange_maui;

public partial class RateDetailsPage : ContentPage
{
    public Rate DetailedRate { get; }

    private List<ChartEntry> _entries;

    public RateDetailsPage(Rate rate)
    {
        InitializeComponent();

        DetailedRate = rate;

        FirstEntryConvertRateLabel.Text = string.Format("  (1 {0} = {1:F5} {2})", "PLN",
            1 / DetailedRate.mid, DetailedRate.code.ToUpper());

        SecondEntryConvertRateLabel.Text = string.Format("  (1 {0} = {1:F5} {2})", DetailedRate.code.ToUpper(),
            DetailedRate.mid, "PLN");

        BindingContext = this;

        RefreshEntries(DateTime.Now.AddDays(-30));
    }

    private async void RefreshEntries(DateTime startDate)
    {
        var pog = await CurrencyExchangeAPI.GetCurrencyRates(DetailedRate.code, startDate, DateTime.Now);

        Color textColor = Colors.Transparent;

        if (Application.Current != null)
        {
            textColor = Application.Current.RequestedTheme == AppTheme.Dark
                ? Color.FromArgb("#f0f0f0")
                : Color.FromArgb("#171514");
        }

        int divider = (int)Math.Ceiling(pog.rates.Count / 14.0);

        _entries = new List<ChartEntry>();

        for (var i = pog.rates.Count - 1; i >= 0; i -= divider)
        {
            _entries.Add(new ChartEntry((float)pog.rates[i].mid)
            {
                Color = textColor.ToSKColor(),
                Label = pog.rates[i].effectiveDate,
                ValueLabel = pog.rates[i].mid.ToString(),
                ValueLabelColor = textColor.ToSKColor()
            });
        }

        _entries.Reverse();

        RefreshChart();
    }


    private void RefreshChart()
    {
        Color bgColor = Colors.Transparent;

        if (Application.Current != null)
        {
            bgColor = Application.Current.RequestedTheme == AppTheme.Dark
                ? Color.FromArgb("#171514")
                : Color.FromArgb("#f0f0f0");
        }

        var maxRate = _entries.MaxBy(r => r.Value).Value;
        var minRate = _entries.MinBy(r => r.Value).Value;

        if (minRate != null && maxRate != null)
        {
            RateChartView.Chart = new LineChart()
            {
                Entries = _entries,
                MinValue = (float)minRate,
                MaxValue = (float)maxRate,
                LineMode = LineMode.Spline,
                LineSize = 7,
                PointMode = PointMode.Circle,
                PointSize = 32,
                BackgroundColor = bgColor.ToSKColor(),
                LabelTextSize = 36,
                ValueLabelTextSize = 36,
                IsAnimated = false
            };
        }
    }

    private void Button_OnClicked365(object sender, EventArgs e)
    {
        RefreshEntries(DateTime.Now.AddDays(-365));
    }

    private void Button_OnClicked90(object sender, EventArgs e)
    {
        RefreshEntries(DateTime.Now.AddDays(-90));
    }

    private void Button_OnClicked30(object sender, EventArgs e)
    {
        RefreshEntries(DateTime.Now.AddDays(-30));
    }

    private void Button_OnClicked7(object sender, EventArgs e)
    {
        RefreshEntries(DateTime.Now.AddDays(-7));
    }

    private void FirstCalcEntry_OnCompleted(object sender, EventArgs e)
    {
        SecondCalcEntry.Text = double.TryParse(FirstCalcEntry.Text, out var initValue)
            ? (initValue / DetailedRate.mid).ToString("F2")
            : string.Empty;
    }

    private void SecondCalcEntry_OnCompleted(object sender, EventArgs e)
    {
        FirstCalcEntry.Text = double.TryParse(SecondCalcEntry.Text, out var initValue)
            ? (initValue * DetailedRate.mid).ToString("F2")
            : string.Empty;
    }
}