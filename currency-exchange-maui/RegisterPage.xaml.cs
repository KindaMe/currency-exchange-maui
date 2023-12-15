using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api_access;
using api_access.Models;

namespace currency_exchange_maui;

public partial class RegisterPage : ContentPage
{
    public UserModel NewUser { get; set; } = new();

    public RegisterPage()
    {
        InitializeComponent();

        BindingContext = this;
    }

    private void Button_OnClicked(object sender, EventArgs e)
    {
        CurrencyExchangeAPI.PostUser(NewUser);
    }
}