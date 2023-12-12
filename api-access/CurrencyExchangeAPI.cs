using api_access.Models;
using RestSharp;

namespace api_access;

public static class CurrencyExchangeAPI
{
    private const string BaseUrl = "https://currency-exchange-api.azurewebsites.net/api";

    public static async Task<int> AuthenticateUser(string email, string password)
    {
        var client = new RestClient(BaseUrl);
        var request = new RestRequest("UsersAuthentication");
        request.AddParameter("email", email);
        request.AddParameter("password", password);

        var response = await client.ExecuteGetAsync<int>(request);

        return response.Data;
    }

    public static async Task<List<ExchangeRateTableModel>> GetCurrentRates()
    {
        var client = new RestClient(BaseUrl);
        var request = new RestRequest("RatesTable");

        var response = await client.ExecuteGetAsync<List<ExchangeRateTableModel>>(request);

        return response.Data;
    }

    public static async Task<ExchangeRateTableModel> GetCurrencyRates(string currency, DateTime startDate,
        DateTime endDate)
    {
        var client = new RestClient(BaseUrl);
        var request = new RestRequest("RatesTable");
        request.AddParameter("currency", currency);
        request.AddParameter("startDate", startDate.ToString("yyyy-MM-dd"));
        request.AddParameter("endDate", endDate.ToString("yyyy-MM-dd"));

        var response = await client.ExecuteGetAsync<ExchangeRateTableModel>(request);

        return response.Data;
    }

    public static async Task<List<WalletModel>> GetWallets(int userId)
    {
        var client = new RestClient(BaseUrl);
        var request = new RestRequest("Wallets");
        request.AddParameter("userId", userId);

        var response = await client.ExecuteGetAsync<List<WalletModel>>(request);

        return response.Data;
    }
}