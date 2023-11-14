using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace currency_exchange_maui;

public partial class WalletPage : ContentPage
{
    public ObservableCollection<TestContentModel> TestContent { get; set; }
    
    public WalletPage()
    {
        InitializeComponent();
        
        TestContent = new ObservableCollection<TestContentModel>();
        
        TestContent.Add(new TestContentModel("Test Wallet 1", 420.69, "USD"));
        TestContent.Add(new TestContentModel("Test Wallet 2", 420.69, "GBP"));
        TestContent.Add(new TestContentModel("Test Wallet 1", 420.69, "USD"));
        TestContent.Add(new TestContentModel("Test Wallet 2", 420.69, "GBP"));
        TestContent.Add(new TestContentModel("Test Wallet 1", 420.69, "USD"));
        TestContent.Add(new TestContentModel("Test Wallet 2", 420.69, "GBP"));
        TestContent.Add(new TestContentModel("Test Wallet 1", 420.69, "USD"));
        TestContent.Add(new TestContentModel("Test Wallet 2", 420.69, "GBP"));
        TestContent.Add(new TestContentModel("Test Wallet 1", 420.69, "USD"));
        TestContent.Add(new TestContentModel("Test Wallet 2", 420.69, "GBP"));
        TestContent.Add(new TestContentModel("Test Wallet 1", 420.69, "USD"));
        TestContent.Add(new TestContentModel("Test Wallet 2", 420.69, "GBP"));
        TestContent.Add(new TestContentModel("Test Wallet 1", 420.69, "USD"));
        TestContent.Add(new TestContentModel("Test Wallet 2", 420.69, "GBP"));
        TestContent.Add(new TestContentModel("Test Wallet 1", 420.69, "USD"));
        TestContent.Add(new TestContentModel("Test Wallet 2", 420.69, "GBP"));

        BindingContext = this;
    }
}

public class TestContentModel
{
    public TestContentModel(string name, double balance, string currency)
    {
        this.Name = name;
        this.Balance = balance;
        this.Currency = currency;
    }

    public string Name { get; set; }
    public double Balance { get; set; }
    public string Currency { get; set; }
}