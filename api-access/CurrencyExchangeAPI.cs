using api_access.Models;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace api_access;

public static class CurrencyExchangeAPI
{
    private const string BaseUrl = "https://currency-exchange-api-core.azurewebsites.net/api";

    public static string AuthToken => Preferences.Get(nameof(AuthToken), defaultValue: null);

    public static async Task<string> GenerateToken(string email, string password)
    {
        var client = new RestClient(BaseUrl);
        var request = new RestRequest("Token");

        var userCredentials = new UserCredentialsModel
        {
            Email = email,
            Password = password
        };

        request.AddJsonBody(userCredentials);

        var response = await client.ExecutePostAsync(request);

        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            if (response.Content != null)
            {
                var jsonResponse = JObject.Parse(response.Content);
                var token = jsonResponse["token"]?.ToString();

                return token;
            }
        }

        return null;
    }

    public static async Task<ExchangeRateTableModel> GetCurrentRates()
    {
        var client = new RestClient(BaseUrl);
        var request = new RestRequest("Nbp/AllRates");

        request.AddHeader("Authorization", $"Bearer {AuthToken}");

        var response = await client.ExecuteGetAsync<ExchangeRateTableModel>(request);

        return response.Data;
    }

    public static async Task<ExchangeRateTableModel> GetCurrencyRates(string currency, DateTime startDate,
        DateTime endDate)
    {
        var client = new RestClient(BaseUrl);
        var request = new RestRequest("Nbp/CurrencyRatesWithDates");

        request.AddHeader("Authorization", $"Bearer {AuthToken}");

        request.AddParameter("currency", currency);
        request.AddParameter("startDate", startDate.ToString("yyyy-MM-dd"));
        request.AddParameter("endDate", endDate.ToString("yyyy-MM-dd"));

        var response = await client.ExecuteGetAsync<ExchangeRateTableModel>(request);

        return response.Data;
    }

    public static async Task<List<WalletModel>> GetWallets()
    {
        var client = new RestClient(BaseUrl);
        var request = new RestRequest("Wallets");

        request.AddHeader("Authorization", $"Bearer {AuthToken}");

        var response = await client.ExecuteAsync<List<WalletModel>>(request);

        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            return response.Data;
        }

        return null;
    }

    public static async Task<ExchangeRateTableModel> GetCurrentCurrencyRate(string currency)
    {
        var client = new RestClient(BaseUrl);
        var request = new RestRequest("Nbp/CurrentCurrencyRate");

        request.AddHeader("Authorization", $"Bearer {AuthToken}");

        request.AddParameter("currency", currency);

        var response = await client.ExecuteGetAsync<ExchangeRateTableModel>(request);

        return response.Data;
    }

    public static async void PostUser(UserModel user)
    {
        var client = new RestClient(BaseUrl);
        var request = new RestRequest("Users");
        request.AddBody(user);

        await client.ExecutePostAsync(request);
    }
}