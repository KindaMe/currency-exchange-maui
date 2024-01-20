using System.Net;
using api_access.DTOs;
using api_access.Models;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace api_access;

public interface IApiService
{
    event EventHandler UnauthorizedRequest;
    void OnUnauthorizedRequest();
    string GetAuthToken();
    void SetAuthToken(string token);

    Task RequestAndSetTokenAsync(UserCredentialsDto userCredentials);
    Task<UserModel> GetUserDetailsAsync();
    Task<bool> PutEmailAsync(UserUpdatedDetailsDto details);
    Task<bool> PutPasswordAsync(UserUpdatedDetailsDto details);
    Task<bool> DeleteUserAsync(UserUpdatedDetailsDto details);
    Task<ExchangeRateTableModel> GetCurrentRatesAsync();
    Task<ExchangeRateTableModel> GetCurrencyRatesAsync(string currency, DateOnly startDate, DateOnly endDate);
    Task<List<WalletModel>> GetWalletsAsync();
    Task<ExchangeRateTableModel> GetCurrentCurrencyRateAsync(string currency);
    Task<ExchangeRateTableModel> GetAllRatesWithDateAsync(DateTime date);
    Task<bool> PostUserAsync(UserModel user);
    Task<bool> PostTransactionAsync(TransactionDto data);
    Task<string> GetFlagUrlByCurrencyCodeAsync(string code);
    Task<bool> PostWalletAsync(WalletDto data);
}

public class ApiService : IApiService
{
    private const string BaseUrl = "https://currency-exchange-api-core.azurewebsites.net/api";
    private const string TokenEndpoint = "Token";
    private const string UsersEndpoint = "Users";
    private const string WalletsEndpoint = "Wallets";
    private const string TransactionsEndpoint = "Transactions";

    public event EventHandler UnauthorizedRequest;

    public void OnUnauthorizedRequest()
    {
        SetAuthToken(null);
        UnauthorizedRequest?.Invoke(this, EventArgs.Empty);
    }

    public string GetAuthToken()
    {
        return Preferences.Get("AuthToken", defaultValue: null);
    }

    public void SetAuthToken(string token)
    {
        Preferences.Set("AuthToken", token);
    }

    public async Task RequestAndSetTokenAsync(UserCredentialsDto userCredentials)
    {
        var client = new RestClient(BaseUrl);
        var request = new RestRequest(TokenEndpoint);

        try
        {
            request.AddJsonBody(userCredentials);

            var response = await client.ExecutePostAsync(request);

            if (response.IsSuccessful)
            {
                if (response.Content != null)
                {
                    var jsonResponse = JObject.Parse(response.Content);
                    var token = jsonResponse["token"]?.ToString();

                    SetAuthToken(token);
                }
            }
        }
        catch (Exception ex)
        {
            return;
        }
    }


    public async Task<UserModel> GetUserDetailsAsync()
    {
        var client = new RestClient(BaseUrl);
        var request = new RestRequest(UsersEndpoint);

        try
        {
            request.AddHeader("Authorization", $"Bearer {GetAuthToken()}");

            var response = await client.ExecuteGetAsync<UserModel>(request);

            if (response.IsSuccessful)
            {
                return response.Data;
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                OnUnauthorizedRequest();
            }
        }
        catch (Exception ex)
        {
            return null;
        }

        return null;
    }

    public async Task<bool> PutEmailAsync(UserUpdatedDetailsDto details)
    {
        var client = new RestClient(BaseUrl);
        var request = new RestRequest($"{UsersEndpoint}/Email");
        
        try
        {
            request.AddHeader("Authorization", $"Bearer {GetAuthToken()}");

            request.AddJsonBody(details);

            var response = await client.ExecutePutAsync(request);

            if (response.IsSuccessful)
            {
                return true;
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                OnUnauthorizedRequest();
            }
        }
        catch (Exception ex)
        {
            return false;
        }

        return false;
    }

    public async Task<bool> PutPasswordAsync(UserUpdatedDetailsDto details)
    {
        var client = new RestClient(BaseUrl);
        var request = new RestRequest($"{UsersEndpoint}/Password");
        
        try
        {
            request.AddHeader("Authorization", $"Bearer {GetAuthToken()}");

            request.AddJsonBody(details);

            var response = await client.ExecutePutAsync(request);

            if (response.IsSuccessful)
            {
                return true;
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                OnUnauthorizedRequest();
            }
        }
        catch (Exception ex)
        {
            return false;
        }

        return false;
    }

    public async Task<bool> DeleteUserAsync(UserUpdatedDetailsDto details)
    {
        var client = new RestClient(BaseUrl);
        var request = new RestRequest(UsersEndpoint);
        
        try
        {
            request.AddHeader("Authorization", $"Bearer {GetAuthToken()}");

            request.AddJsonBody(details);

            var response = await client.ExecuteDeleteAsync(request);

            if (response.IsSuccessful)
            {
                return true;
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                OnUnauthorizedRequest();
            }
        }
        catch (Exception ex)
        {
            return false;
        }

        return false;
    }


    public async Task<ExchangeRateTableModel> GetCurrentRatesAsync()
    {
        var client = new RestClient(BaseUrl);
        var request = new RestRequest("Nbp/AllRates");

        try
        {
            var response = await client.ExecuteGetAsync<ExchangeRateTableModel>(request);

            if (response.IsSuccessful)
            {
                return response.Data;
            }
        }
        catch (Exception ex)
        {
            return null;
        }

        return null;
    }


    public async Task<ExchangeRateTableModel> GetCurrencyRatesAsync(string currency, DateOnly startDate,
        DateOnly endDate)
    {
        var client = new RestClient(BaseUrl);
        var request = new RestRequest("Nbp/CurrencyRatesWithDates");

        try
        {
            request.AddParameter("currency", currency);
            request.AddParameter("startDate", startDate.ToString("yyyy-MM-dd"));
            request.AddParameter("endDate", endDate.ToString("yyyy-MM-dd"));

            var response = await client.ExecuteGetAsync<ExchangeRateTableModel>(request);

            if (response.IsSuccessful)
            {
                return response.Data;
            }
        }
        catch (Exception ex)
        {
            return null;
        }

        return null;
    }


    public async Task<List<WalletModel>> GetWalletsAsync()
    {
        var client = new RestClient(BaseUrl);
        var request = new RestRequest(WalletsEndpoint);

        try
        {
            request.AddHeader("Authorization", $"Bearer {GetAuthToken()}");

            var response = await client.ExecuteAsync<List<WalletModel>>(request);

            if (response.IsSuccessful)
            {
                return response.Data;
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                OnUnauthorizedRequest();
            }
        }
        catch (Exception ex)
        {
            return null;
        }

        return null;
    }


    public async Task<ExchangeRateTableModel> GetCurrentCurrencyRateAsync(string currency)
    {
        var client = new RestClient(BaseUrl);
        var request = new RestRequest("Nbp/CurrentCurrencyRate");

        try
        {
            request.AddParameter("currency", currency);

            var response = await client.ExecuteGetAsync<ExchangeRateTableModel>(request);

            if (response.IsSuccessful)
            {
                return response.Data;
            }
        }
        catch (Exception ex)
        {
            return null;
        }

        return null;
    }


    public async Task<ExchangeRateTableModel> GetAllRatesWithDateAsync(DateTime date)
    {
        var client = new RestClient(BaseUrl);
        var request = new RestRequest("Nbp/AllRatesWithDate");

        try
        {
            request.AddParameter("date", date.ToString("yyyy-MM-dd"));

            var response = await client.ExecuteGetAsync<ExchangeRateTableModel>(request);

            if (response.IsSuccessful)
            {
                return response.Data;
            }
        }
        catch (Exception ex)
        {
            return null;
        }

        return null;
    }


    public async Task<bool> PostUserAsync(UserModel user)
    {
        var client = new RestClient(BaseUrl);
        var request = new RestRequest(UsersEndpoint);

        try
        {
            request.AddJsonBody(user);

            var response = await client.ExecutePostAsync(request);

            if (response.IsSuccessful)
            {
                return true;
            }
        }
        catch (Exception ex)
        {
            return false;
        }

        return false;
    }


    public async Task<bool> PostTransactionAsync(TransactionDto data)
    {
        var client = new RestClient(BaseUrl);
        var request = new RestRequest(TransactionsEndpoint);

        try
        {
            request.AddHeader("Authorization", $"Bearer {GetAuthToken()}");
            request.AddJsonBody(data);

            var response = await client.ExecutePostAsync(request);

            if (response.IsSuccessful)
            {
                return true;
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                OnUnauthorizedRequest();
            }
        }
        catch (Exception ex)
        {
            return false;
        }

        return false;
    }


    public async Task<string> GetFlagUrlByCurrencyCodeAsync(string code)
    {
        var client = new RestClient(BaseUrl);
        var request = new RestRequest("Countries/FlagUrlByCurrencyCode");

        try
        {
            request.AddParameter("code", code);

            var response = await client.ExecuteGetAsync<string>(request);

            if (response.IsSuccessful)
            {
                return response.Data;
            }
        }
        catch (Exception ex)
        {
            return null;
        }

        return null;
    }


    public async Task<bool> PostWalletAsync(WalletDto data)
    {
        var client = new RestClient(BaseUrl);
        var request = new RestRequest(WalletsEndpoint);

        try
        {
            request.AddHeader("Authorization", $"Bearer {GetAuthToken()}");
            request.AddJsonBody(data);

            var response = await client.ExecutePostAsync(request);

            if (response.IsSuccessful)
            {
                return true;
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                OnUnauthorizedRequest();
            }
        }
        catch (Exception ex)
        {
            return false;
        }

        return false;
    }
}